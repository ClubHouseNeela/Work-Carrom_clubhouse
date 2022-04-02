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
    public GameObject NoBalPop;

    private string
    playerID, opponentID;

    public Text
    winPlayerName, loosePlayerName, winScore, looseScore, mainRank, mainScore, titleMsg;

    public SendData sendThisPlayerData;
    public WinningDetails winning_details;
    public WalletInfo walletInfo;
    bool isDataSend;

    public string sendDataURL = "http://52.66.182.199/api/gameplay";

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region set data and init

    private void Awake()
    {
        instance = this;
    }

    public void SetLeaderboardData(bool isWon)
    {
        if(isWon)
        {
            winPlayerName.text = MatchMakingUIManager.instance.playerMobileNumberText.text;
            loosePlayerName.text = MatchMakingUIManager.instance.opponentMobileNumberText.text;

            winScore.text = GameManager.instance.GetScore(0).ToString();
            mainScore.text = GameManager.instance.GetScore(0).ToString();
            looseScore.text = GameManager.instance.GetScore(1).ToString();

            mainRank.text = "1";

            titleMsg.text = "You Win!";

            sendThisPlayerData.game_status = "WIN";
            
            sendThisPlayerData.room_id = NetworkClient.instance.roomID;

            sendThisPlayerData.winning_details.thisplayerScore = GameManager.instance.GetScore(0);
            sendThisPlayerData.winning_details.winningPlayerScore = GameManager.instance.GetScore(0).ToString();
            sendThisPlayerData.winning_details.winningPlayerID = NetworkClient.instance.matchDetails.playerId[1].ToString();
            sendThisPlayerData.winning_details.lossingPlayerScore = GameManager.instance.GetScore(1).ToString();
            sendThisPlayerData.winning_details.lossingPlayerID = NetworkClient.instance.oppPlayerId.ToString();

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

            titleMsg.text = "You Loose!";

            sendThisPlayerData.game_status = "LOST";
                     
            sendThisPlayerData.room_id = NetworkClient.instance.roomID;

            sendThisPlayerData.winning_details.thisplayerScore = GameManager.instance.GetScore(0);
            sendThisPlayerData.winning_details.winningPlayerScore = GameManager.instance.GetScore(1).ToString();
            sendThisPlayerData.winning_details.winningPlayerID = NetworkClient.instance.oppPlayerId.ToString();
            sendThisPlayerData.winning_details.lossingPlayerScore = GameManager.instance.GetScore(0).ToString();
            sendThisPlayerData.winning_details.lossingPlayerID = NetworkClient.instance.matchDetails.playerId[1].ToString();

            sendThisPlayerData.game_end_time = GetSystemTime();
        }

        if (NetworkClient.instance.matchDetails.playerId[1] == 0)
            sendThisPlayerData.player_id = NetworkClient.instance.oppPlayerId.ToString();
        else
            sendThisPlayerData.player_id = NetworkClient.instance.matchDetails.playerId[1].ToString();

        sendThisPlayerData.wallet_amt = AndroidtoUnityJSON.instance.game_fee.ToString();
        sendThisPlayerData.game_mode = AndroidtoUnityJSON.instance.game_mode;
        sendThisPlayerData.game_id = AndroidtoUnityJSON.instance.game_id;

        if (AndroidtoUnityJSON.instance.game_mode == "tour")
            sendThisPlayerData.battle_tournament_id = AndroidtoUnityJSON.instance.tour_id;
        else if (AndroidtoUnityJSON.instance.game_mode == "battle")
            sendThisPlayerData.battle_tournament_id = AndroidtoUnityJSON.instance.battle_id;

        string sendWinningDetailsData = JsonUtility.ToJson(winning_details);
        string sendNewData = JsonUtility.ToJson(sendThisPlayerData);

        //Debug.Log(sendNewData + " <= sendNewData");
        WebRequestHandler.Instance.Post(sendDataURL, sendNewData, (response, status) =>
        {
            Debug.Log(response + " <- HitNewApi");
        });

        isDataSend = true;
    }

    //public class WalletInfoData
    //{
    //    public string cash_balance;
    //    public string winning_balance;
    //    public string bonus_amount;
    //    public string coin_balance;
    //}

    public void Reload()
    {
        //wallet check
        float balance = 0f;

        WalletInfoData = JsonUtility.ToJson(walletInfo);
        WebRequestHandler.Instance.Post(GameManager.instance.walletInfoURL, WalletInfoData, (response, status) =>
        {
            WalletInfo walletInfoResponse = JsonUtility.FromJson<WalletInfo>(response);
            balance = float.Parse(walletInfoResponse.data.cash_balance);
            Debug.Log(balance + " <= replay check balance");

            if (balance >= float.Parse(AndroidtoUnityJSON.instance.game_fee))
                canRestart = true;
            else
                canRestart = false;

            if (canRestart)
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            else
                NoBalPop.SetActive(true);

            //Debug.Log("CANT START, NOT ENOUGH BALANCE!"); //show no bal popup
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
