using DG.Tweening;
using RTLTMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardUIManager : MonoBehaviour
{
    #region public and private fields 

    public static LeaderboardUIManager instance;

    private string WalletInfoData;
    public bool canRestart = false;
    public GameObject NoBalPop, Footer_1, Footer;

    private string
    playerID, opponentID;

    public Text
    winPlayerName, loosePlayerName, winScore, looseScore, mainRank, mainScore, titleMsg, chancesLeft;

    private string walletUpdateData;
    private string walletCheckData;
    public WalletCheck walletCheck;
    public WalletCheckPost walletCheckPost;
    public SendData sendThisPlayerData;
    public WinningDetails winningDetails;
    public WalletInfo walletInfo;
    public bool isDataSend;

    public string sendDataURL = "http://52.66.182.199/api/gameplay";
    public string walletCheckURL = "https://livegamejoypro.com/api/checkWallet";

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region set data and init

    private void Awake()
    {
        instance = this;
    }

    public void SetLeaderboardData(bool isWon)
    {
        if(AndroidtoUnityJSON.instance.game_mode == "tour")
        {
            Footer.SetActive(false);
            Footer_1.SetActive(true);

            chancesLeft.text = GameManager.instance.attemptNo.ToString();
            chancesLeft.gameObject.SetActive(true);
        }
        else
        {
            Footer.SetActive(true);
            Footer_1.SetActive(false);
            chancesLeft.gameObject.SetActive(false);
        }

        var botindex = 0;

        if (NetworkClient.instance.matchDetails.playerName[0] == AndroidtoUnityJSON.instance.user_name)
        {
            botindex = 1;
        }

        winPlayerName.transform.parent.transform.GetChild(0).transform.gameObject.SetActive(false);
        loosePlayerName.transform.parent.transform.GetChild(0).transform.gameObject.SetActive(false);

        if (isWon)
        {
            winPlayerName.text = MatchMakingUIManager.instance.playerMobileNumberText.text;
            loosePlayerName.text = MatchMakingUIManager.instance.opponentMobileNumberText.text;

            winScore.text = GameManager.instance.GetScore(0).ToString();
            mainScore.text = GameManager.instance.GetScore(0).ToString();
            looseScore.text = GameManager.instance.GetScore(1).ToString();

            mainRank.text = "1";

            titleMsg.text = "YOU WON";

            sendThisPlayerData.game_status = "WIN";
            
            sendThisPlayerData.room_id = NetworkClient.instance.roomID;

            sendThisPlayerData.winning_details.thisplayerScore = GameManager.instance.GetScore(0);
            sendThisPlayerData.winning_details.winningPlayerScore = GameManager.instance.GetScore(0).ToString();
            sendThisPlayerData.winning_details.winningPlayerID = AndroidtoUnityJSON.instance.player_id;
            sendThisPlayerData.winning_details.lossingPlayerScore = GameManager.instance.GetScore(1).ToString();
            sendThisPlayerData.winning_details.lossingPlayerID = NetworkClient.instance.matchDetails.playerId[botindex].ToString();

            sendThisPlayerData.game_end_time = GetSystemTime();
        }
        else
        {
            winPlayerName.text = MatchMakingUIManager.instance.opponentMobileNumberText.text;
            loosePlayerName.text = MatchMakingUIManager.instance.playerMobileNumberText.text;

            winScore.text = GameManager.instance.GetScore(1).ToString();
            mainScore.text = GameManager.instance.GetScore(1).ToString();
            looseScore.text = GameManager.instance.GetScore(0).ToString();

            mainRank.text = "2";

            titleMsg.text = "YOU LOSE";

            sendThisPlayerData.game_status = "LOST";
                     
            sendThisPlayerData.room_id = NetworkClient.instance.roomID;

            sendThisPlayerData.winning_details.thisplayerScore = GameManager.instance.GetScore(0);
            sendThisPlayerData.winning_details.winningPlayerScore = GameManager.instance.GetScore(1).ToString();
            sendThisPlayerData.winning_details.winningPlayerID = NetworkClient.instance.matchDetails.playerId[botindex].ToString();
            sendThisPlayerData.winning_details.lossingPlayerScore = GameManager.instance.GetScore(0).ToString();
            sendThisPlayerData.winning_details.lossingPlayerID = AndroidtoUnityJSON.instance.player_id;

            sendThisPlayerData.game_end_time = GetSystemTime();
        }

        if(NetworkClient.instance.matchDetails.playerId[0] == int.Parse(AndroidtoUnityJSON.instance.player_id))
            sendThisPlayerData.player_id = NetworkClient.instance.matchDetails.playerId[1].ToString();
        else
            sendThisPlayerData.player_id = NetworkClient.instance.matchDetails.playerId[0].ToString();

        sendThisPlayerData.wallet_amt = AndroidtoUnityJSON.instance.game_fee.ToString();
        sendThisPlayerData.game_mode = AndroidtoUnityJSON.instance.game_mode;
        sendThisPlayerData.game_id = AndroidtoUnityJSON.instance.game_id;

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

    public void Reload()
    {
        //wallet check
        bool balance = false;

        walletCheckPost.game_id = AndroidtoUnityJSON.instance.game_id;
        walletCheckPost.type = AndroidtoUnityJSON.instance.game_mode;

        if (AndroidtoUnityJSON.instance.game_mode == "tour")
            walletCheckPost.tournament_battle_id = AndroidtoUnityJSON.instance.tour_id;
        else if (AndroidtoUnityJSON.instance.game_mode == "battle")
            walletCheckPost.tournament_battle_id = AndroidtoUnityJSON.instance.battle_id;

        string walletCheckPostData = JsonUtility.ToJson(walletCheckPost);
        WebRequestHandler.Instance.Post(walletCheckURL, walletCheckPostData, (response, status) =>
        {
            WalletCheck walletCheckResponse = JsonUtility.FromJson<WalletCheck>(response);
            balance = bool.Parse(walletCheckResponse.status);
            //Debug.Log(balance + " <= replay check balance");

            if (balance)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
            else
            {
                NoBalPop.SetActive(true);
            }
        });
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
