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
    void OnEnable()
    {
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

        int score = 0, targets = 0, time = 0;
        string name = tbName.text;
        string date = System.DateTime.Today.ToString("MM/dd/yy");
        ScoreSet record = new ScoreSet();

        // Use JSON format for the records instead using ScoreSet class;
        record.name = name;
        record.score = UIHandler.roundData.score;
        record.targets = UIHandler.roundData.targets;
        record.time = UIHandler.roundData.time;
        record.date = date;
        
        StartCoroutine(submitNewRecord(record));
    }

    // send the gathered game info to the server
    private IEnumerator submitNewRecord(ScoreSet hsrecord) {

        string gamemodeURL = GAMEMODEUL;
        if (Panel.gameMode == "Time Attack") gamemodeURL = GAMEMODETA;

        // string UpdateURL = LOCAL_UPDATEHS + gamemodeURL;
        string UpdateURL = LIVE_UPDATEHS + gamemodeURL;
        Debug.Log("Connecting to DB using URL: " + UpdateURL);
        
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
            Debug.Log("Connection Error: " + req.error);
            UIHandler.createModal("Warning", "Connection Error:\r\n" + req.error);
        }
        else {
            Debug.Log("Record successfully added!");
            UIHandler.createModal("confirm", "Your score has been added! \r\n\r\n Your Rank: ");
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
