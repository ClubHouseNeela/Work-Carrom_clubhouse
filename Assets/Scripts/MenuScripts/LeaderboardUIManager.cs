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

    private string
    playerID, opponentID;

    public Text
    winPlayerName, loosePlayerName, winScore, looseScore, mainRank, mainScore, titleMsg;

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
        }

        //hit wallet api
        //send data
    }

    public void Reload()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void UpdateWallet()
    {

    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------
}
