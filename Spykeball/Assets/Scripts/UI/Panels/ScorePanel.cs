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

    protected string READHS_PARAMS = "?mode=ul&sort=targets&order=asc";

    public delegate void closeScoreboard(bool close);
    public static event closeScoreboard doOnCloseScoreboard;
    public Text errorText;
    
    private int loadTextSequence = 1;

    protected HSList highscores;
    protected float loadTextTimer = 0;
    protected bool isCheckingConn = true, isConnected = false;

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
        StartCoroutine(checkConnectionToServer());
    }

    private IEnumerator checkConnectionToServer() {
        Debug.Log("Connecting to server: " + LOCAL_READHS + READHS_PARAMS);

        // first, create a web request using "Get"
        UnityWebRequest req = UnityWebRequest.Get(LOCAL_READHS + READHS_PARAMS);
        req.timeout = 10;
        yield return req.SendWebRequest();
        if (req.isNetworkError || req.isHttpError) {
            Debug.Log("Connection Error: " + req.error);
            isConnected = false;
        }
        else {
            isCheckingConn = false;
            highscores = JsonUtility.FromJson<HSList>(req.downloadHandler.text);
            Debug.Log(highscores);
            errorText.text = req.downloadHandler.text;
        }
    }

    // read highscores from server
    protected void readHighScores() {


    }

    
    // button close panel
    public void btCloseScoreboard(bool close) {
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        doOnCloseScoreboard(false);
    }
}
