using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class ScoreSet
{
    public string name, date;
    public int score, targets;
    public float time;
}

[System.Serializable]
public class HSList
{
    public ScoreSet[] highscores;
}

/// <summary>
///  Panel that handles displaying the scoreboard
/// </summary>
public class ScorePanel : Panel
{
    const string LOCAL_READHS = "http://localhost:5000/sbreadhs";
    const string LIVE_READHS = "https://bitknvs-30e00398cef5.herokuapp.com/sbreadhs";

    protected string READHS_PARAMS = "?sort=targets&order=asc";

    public delegate void closeScoreboard(bool close);
    public static event closeScoreboard doOnCloseScoreboard;
    public Text errorText;

    [SerializeField]
    protected GameObject scoreFields;
    [SerializeField]
    protected Text rankText;
    [SerializeField]
    protected Text nameText;
    [SerializeField]
    protected Text scoreText;
    [SerializeField]
    protected Text targetsText;
    [SerializeField]
    protected Text timeText;
    [SerializeField]
    protected Text dateText;

    [SerializeField]
    protected Button btModeUL;
    [SerializeField]
    protected Button btModeTA;
    
    protected HSList highscores;
    protected string currentMode = "ul";
    protected float loadTextTimer = 0;
    protected bool isCheckingConn = true, isConnected = false;
    private int loadTextSequence = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // start connecting to server here
    void Awake()
    {
    }

    void OnEnable() {
        isCheckingConn = true;
        connectToServer();
    }

    void OnDisable() {
        rankText.text = nameText.text = scoreText.text = targetsText.text = timeText.text = dateText.text = "";
        scoreFields.SetActive(false);
        isCheckingConn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCheckingConn) {
            updateLoadingText();
        }
    }

    protected void updateLoadingText() {
        string ellipses = ".";
        if (Time.time > loadTextTimer) {
            loadTextSequence = (loadTextSequence >= 3) ? 1 : loadTextSequence + 1;
            
            if (loadTextSequence == 1) ellipses = ".";
            if (loadTextSequence == 2) ellipses = "..";
            if (loadTextSequence == 3) ellipses = "...";
            errorText.text = "Connecting to Server" + ellipses;

            loadTextTimer = Time.time + 1f; // add one second
        }
    }

    // server connection functions here
    protected void connectToServer() {
        StartCoroutine(readHighScores());
    }

    private IEnumerator readHighScores() {
        
        string searchMode = "&mode=" + currentMode;
        //string ReadURL = LOCAL_READHS + READHS_PARAMS;
        string ReadURL = LIVE_READHS + READHS_PARAMS + searchMode;
        
        Debug.Log("Connecting to server: " + ReadURL);

        // first, create a web request using "Get"
        UnityWebRequest req = UnityWebRequest.Get(ReadURL);
        req.timeout = 10;
        yield return req.SendWebRequest();
        if (req.isNetworkError || req.isHttpError) {
            Debug.Log("Connection Error: " + req.error);
            errorText.text = req.downloadHandler.text;
            isConnected = false;
        }
        else {
            isCheckingConn = false;
            scoreFields.SetActive(true);
            highscores = JsonUtility.FromJson<HSList>(req.downloadHandler.text);
            errorText.text = "";
            writeHSList();
            Debug.Log(highscores);
        }
    }

    // read highscores from server

    // write highscores into rank texts
    protected void writeHSList() {
        if (highscores.highscores.Length <= 0) return; // we have highscores to write

        rankText.text = nameText.text = scoreText.text = targetsText.text = timeText.text = dateText.text = "";

        for (int i = 0; i < highscores.highscores.Length; i++) {
            rankText.text += (i+1) + "\r\n";
            nameText.text += highscores.highscores[i].name + "\r\n";
            scoreText.text += highscores.highscores[i].score + "\r\n";
            targetsText.text += highscores.highscores[i].targets + "\r\n";
            timeText.text += highscores.highscores[i].time + "\r\n";
            dateText.text += highscores.highscores[i].date + "\r\n";
        }

    }

    // Button behaviors here
    // button close panel
    public void btCloseScoreboard(bool close) {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnCloseScoreboard(false);
    }

    // button change mode to use
    public void btChangeMode(string mode) {
        // change mode from unlimited to time attack
        if (currentMode == "ul" && mode == "ta") {
            btModeUL.gameObject.SetActive(false);
            btModeTA.gameObject.SetActive(true);
        }
        else if (currentMode == "ta" && mode == "ul") {
            btModeUL.gameObject.SetActive(true);
            btModeTA.gameObject.SetActive(false);
        }
        
        currentMode = mode;
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        connectToServer();
    }
}
