using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;


/// <summary>
///  Panel that handles displaying the scoreboard
/// </summary>
public class ScorePanel : Panel
{
    protected const int pagelimit = 10;
    protected string READHS_PARAMS = "?sort=score&order=asc";

    public delegate void closeScoreboard(bool close);
    public static event closeScoreboard doOnCloseScoreboard;
    public Text errorText, pageText;

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
    protected string currentMode = "ul", currentSort = "score", currentOrder = "desc";
    protected float loadTextTimer = 0;
    protected bool isCheckingConn = true, isConnected = false;
    private int loadTextSequence = 1, currentPage = 1;

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
        currentPage = 1; // reset the page every time the window is reopened
        pageText.text = currentPage.ToString();
        readFromServer();
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
    protected void readFromServer() {
        StartCoroutine(readHighScores());
    }

    private IEnumerator readHighScores() {
        
        //set parameters here
        int limit = pagelimit;
        string searchMode = "&mode=" + currentMode;
        string searchSort = "&sort=" + currentSort;
        string searchOrder = "&order=" + currentOrder;
        string searchPages = "&page=" + currentPage + "&limit=" + limit;

        string ReadURL = LOCAL_READHS + searchMode + searchSort + searchOrder + searchPages;
        // string ReadURL = LIVE_READHS + searchMode + searchSort + searchOrder + searchPages;
        
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
        }
    }

    // read highscores from server

    // write highscores into rank texts
    protected void writeHSList() {
        if (highscores.highscores.Length <= 0) return; // we have highscores to write
        Debug.Log("rewriting highscore list at page: " + currentPage);
        int rank = currentPage;
        rankText.text = nameText.text = scoreText.text = targetsText.text = timeText.text = dateText.text = "";
        for (int i = 0; i < highscores.highscores.Length; i++) {

            // use the current page as the rank label's base count
            rank = ((currentPage - 1) * pagelimit);

            // reverse the order of the rank if it's ascending
            if (currentOrder == "desc") rankText.text += (rank + (i+1)) + "\r\n";
            else if (currentOrder == "asc") rankText.text += ((highscores.highscores.Length + rank)-i) + "\r\n";
            
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
        readFromServer();
    }

    // button change sort & order
    public void btChangeSort(string sort) {
        // check first if the current sort is just the same with the passed value, then just switch the order
        if (currentSort == sort) {
            currentOrder = (currentOrder == "desc") ? "asc" : "desc";
            // reset the current page back to 1 to prevent complications
            currentPage = 1;
        }
        // default the order to desc and change the sort value
        else {
            currentSort = sort;
            currentOrder = "desc";
            currentPage = 1;
        }
        
        pageText.text = currentPage.ToString();
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        readFromServer();
    }

    // button change page
    public void btMovePage(string order) {
        if (order == "back") {
            currentPage = (currentPage > 1) ? currentPage - 1 : currentPage;
        }
        else if (order == "next") {
            currentPage++;
        }

        pageText.text = currentPage.ToString();
        SoundHandler.Instance.playSFX(SFXType.ButtonClick);
        readFromServer();
    }
}
