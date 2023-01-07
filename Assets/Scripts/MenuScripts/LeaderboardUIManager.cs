using DG.Tweening;
using RTLTMPro;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUIManager : MonoBehaviour
{
    #region public and private fields 

    public static LeaderboardUIManager instance;
    public Transform myScoreParent;
    public Transform opponentScoreParent;
    public Text myScoreOnScoreboard;
    public Text opponentScoreOnScroeboard;
    public Text myRankOnScoreboard;
    public Text opponentRankOnScoreboard;
    public TMP_Text gameOverLable;
    public Color winnerScoreColor;
    public GameObject winPanel;
    public GameObject losePanel;
    private Vector3 winnerScoreParentPosition;
    private Vector3 loserScoreParentPosition;

    public SendData sendThisPlayerData;
    public WinningDetails winningDetails;
    public bool isDataSend;

    public string sendDataURL;

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region set data and init

    private void Awake()
    {
        instance = this;
        sendDataURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL, "") + sendDataURL;
    }

    public void SetLeaderboardData(bool isWon)
    {

        var botindex = 0;

        if (NetworkClient.instance.matchDetails.playerName[0] == AndroidtoUnityJSON.instance.user_name)
        {
            botindex = 1;
        }

        myScoreOnScoreboard.text = GameManager.instance.GetScore(0).ToString();
        opponentScoreOnScroeboard.text = GameManager.instance.GetScore(1).ToString();
        winnerScoreParentPosition = myScoreParent.localPosition;
        loserScoreParentPosition = opponentScoreParent.localPosition;

        if (isWon)
        {

            //winScore.text = GameManager.instance.GetScore(0).ToString();
            //mainScore.text = GameManager.instance.GetScore(0).ToString();
            //looseScore.text = GameManager.instance.GetScore(1).ToString();
            //gameoverlable
            AudioManager.instance.Play("Cheering", 1f);
            sendThisPlayerData.game_status = "WIN";
            
            sendThisPlayerData.room_id = NetworkClient.instance.roomID;

            sendThisPlayerData.winning_details.thisplayerScore = GameManager.instance.GetScore(0);
            sendThisPlayerData.winning_details.winningPlayerScore = GameManager.instance.GetScore(0).ToString();
            sendThisPlayerData.winning_details.winningPlayerID = AndroidtoUnityJSON.instance.player_id;
            sendThisPlayerData.winning_details.lossingPlayerScore = GameManager.instance.GetScore(1).ToString();
            sendThisPlayerData.winning_details.lossingPlayerID = NetworkClient.instance.matchDetails.playerId[botindex].ToString();

            gameOverLable.text = "You Won";
            myScoreParent.GetComponent<Image>().color = winnerScoreColor;
            myRankOnScoreboard.text = "1";
            opponentRankOnScoreboard.text = "2";
            winPanel.SetActive(true);

            sendThisPlayerData.game_end_time = GetSystemTime();
        }
        else
        {

            //winScore.text = GameManager.instance.GetScore(1).ToString();
            //mainScore.text = GameManager.instance.GetScore(1).ToString();
            //looseScore.text = GameManager.instance.GetScore(0).ToString();

            //mainRank.text = "2";

            //titleMsg.text = "YOU LOSE";

            sendThisPlayerData.game_status = "LOST";
                     
            sendThisPlayerData.room_id = NetworkClient.instance.roomID;

            sendThisPlayerData.winning_details.thisplayerScore = GameManager.instance.GetScore(0);
            sendThisPlayerData.winning_details.winningPlayerScore = GameManager.instance.GetScore(1).ToString();
            sendThisPlayerData.winning_details.winningPlayerID = NetworkClient.instance.matchDetails.playerId[botindex].ToString();
            sendThisPlayerData.winning_details.lossingPlayerScore = GameManager.instance.GetScore(0).ToString();
            sendThisPlayerData.winning_details.lossingPlayerID = AndroidtoUnityJSON.instance.player_id;

            gameOverLable.text = "Try Again";
            myScoreParent.localPosition = loserScoreParentPosition;
            opponentScoreParent.localPosition = winnerScoreParentPosition;
            myScoreParent.GetComponent<Image>().color = Color.white;
            opponentScoreParent.GetComponent<Image>().color = winnerScoreColor;
            myRankOnScoreboard.text = "2";
            opponentRankOnScoreboard.text = "1";
            losePanel.SetActive(true);

            sendThisPlayerData.game_end_time = GetSystemTime();
        }

        if (NetworkClient.instance.matchDetails.playerId[0] == int.Parse(AndroidtoUnityJSON.instance.player_id))
            sendThisPlayerData.player_id = NetworkClient.instance.matchDetails.playerId[1].ToString();
        else
            sendThisPlayerData.player_id = NetworkClient.instance.matchDetails.playerId[0].ToString();
        sendThisPlayerData.wallet_amt = AndroidtoUnityJSON.instance.game_fee.ToString();
        sendThisPlayerData.game_mode = AndroidtoUnityJSON.instance.game_mode;
        sendThisPlayerData.game_id = AndroidtoUnityJSON.instance.game_id;
        sendThisPlayerData.id = NetworkClient.instance.gameID;

        if (AndroidtoUnityJSON.instance.game_mode == "tour")
            sendThisPlayerData.battle_tournament_id = AndroidtoUnityJSON.instance.tour_id;
        else if (AndroidtoUnityJSON.instance.game_mode == "battle")
            sendThisPlayerData.battle_tournament_id = AndroidtoUnityJSON.instance.battle_id;

        string sendWinningDetailsData = JsonUtility.ToJson(winningDetails);
        string sendNewData = JsonUtility.ToJson(sendThisPlayerData);
        WebRequestHandler.Instance.Post(sendDataURL, sendNewData, (response, status) =>
        {
            Debug.Log(response + " <- HitNewApi");
        });

        isDataSend = true;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public string GetSystemTime()
    {
        int hr = System.DateTime.Now.Hour;
        int min = System.DateTime.Now.Minute;
        int sec = System.DateTime.Now.Second;

        int year = System.DateTime.Now.Year;
        int month = System.DateTime.Now.Month;
        int day = System.DateTime.Now.Day;

        string format = string.Format("{0}:{1}:{2} {3}:{4}:{5}", year, month, day, hr, min, sec);

        return format;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------
}
