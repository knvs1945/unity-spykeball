using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

/// <summary>
/// Panel that displays after finishing a game
/// </summary>
public class RestartPanel : Panel
{

    [SerializeField]
    protected GameObject submitSubpanel;
    [SerializeField]
    protected GameObject returnSubpanel;
    [SerializeField]
    protected InputField tbName;

    protected bool isInSubmitMode = false, isSubmittingScore = false;

    // do on awake 
    protected override void OnEnable()
    {
        base.OnEnable();
        preventNavigation = true;
        isInSubmitMode = true;
        submitSubpanel.SetActive(true);
        returnSubpanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(tbName.gameObject);
    }

    // we check if the text field is currently selected and prevent panel button press detection when so
    protected override void Update() {
        if (isInSubmitMode) {

            // cancel high score submission if escape is pressed
            if (Input.GetKeyDown(KeyCode.Escape)) {
                preventNavigation = false;
                btCancelSubmit();
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                btSubmitScore();
            }
        }
        else if (!preventNavigation) highlightNextButton();
    }

    // gather game info then connect to server to record entry
    // form fields: {"name":"KJC", "targets":"5", "score":"5", "time":"3.5", "date":"3/5"}
    protected void createNewRecord(){
        string name = tbName.text;
        string date = System.DateTime.Today.ToString("MM/dd/yy");
        ScoreSet record = new ScoreSet();

        // Use JSON format for the records instead using ScoreSet class;
        record.name = name;
        record.score = UIHandler.roundData.score;
        record.targets = UIHandler.roundData.targets;
        record.time = UIHandler.roundData.time;
        record.date = date;
        
        UIHandler.createModal("confirm", "Submitting record... ", 2f);
        StartCoroutine(submitNewRecord(record));
    }

    // send the gathered game info to the server
    private IEnumerator submitNewRecord(ScoreSet hsrecord) {

        string gamemodeURL = GAMEMODEUL;
        if (Panel.gameMode == "Time Attack") gamemodeURL = GAMEMODETA;

        // string UpdateURL = LOCAL_UPDATEHS + gamemodeURL;
        string UpdateURL = LIVE_UPDATEHS + gamemodeURL;
        
        // convert the received scoreset into JSON data here
        string jsondata = JsonUtility.ToJson(hsrecord);
        // then convert the JSON data into a byte array for sending
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsondata);

        // setup the JSON webrequest here
        UnityWebRequest req = new UnityWebRequest(UpdateURL, "POST");        
        req.SetRequestHeader("Content-Type", "application/json");

        // use an UploadHandlerRaw to send the json bytes
        req.uploadHandler = new UploadHandlerRaw(jsonBytes);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.timeout = 10;

        // send web request and check if it was successful
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success) {
            UIHandler.createModal("warning", "Connection Error:\r\n" + req.error, 3);
            isSubmittingScore = false;
        }
        else {
            string rankResult = req.downloadHandler.text;
            rankResult = rankResult.Trim('\"');
            UIHandler.createModal("confirm", "Your score has been added! \r\n\r\n Score Ranking: " + rankResult);
            // display the return subpanel if successful, otherwise let the player retry as many times as they want
            isInSubmitMode = false;
            isSubmittingScore = false;
            submitSubpanel.SetActive(false);
            returnSubpanel.SetActive(true);
            preventNavigation = false;
        }
    }

    // button behaviors here
    // button cancel submit behavior
    public void btCancelSubmit() {
        isInSubmitMode = false;
        isSubmittingScore = false;
        submitSubpanel.SetActive(false);
        returnSubpanel.SetActive(true);
    }

    // button submit record behavior
    public void btSubmitScore() {
        if (!isSubmittingScore) {  // prevent sending score multiple times with multiple keypresses
            isSubmittingScore = true;
            createNewRecord();
        }
    }
}
