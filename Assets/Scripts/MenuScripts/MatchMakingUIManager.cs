using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using DG.Tweening;
using TMPro;

public class MatchMakingUIManager : MonoBehaviour
{
    public static MatchMakingUIManager instance;

    [SerializeField] string walletUpdateURL;
    [SerializeField] RTLTextMeshPro searchingText;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More then 1 MatchmakingUIManager");
        }
        instance = this;
    }

    private void Start()
    {
        walletUpdateURL = NetworkClient.instance.adminURL + walletUpdateURL;
        AudioManager.instance.Play("Matchmaking");
        StartCoroutine(TextAnimation(searchingText));
    }

    IEnumerator TextAnimation(RTLTextMeshPro textToAnimate)
    {
        string _tempOpponentID = textToAnimate.text + "...";
        int size = textToAnimate.text.Length;
        Debug.Log(size);
        int temp = size;
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            textToAnimate.text = _tempOpponentID.Substring(0, temp);
            temp++;
            if (temp > size + 3)
            {
                temp = size;
            }
        }
    }

    public void Matched(bool firstTurn, bool initialStart, int randomSeed)
    {
        Random.InitState(randomSeed);
        if (initialStart)
        {
            if (!NetworkClient.instance.noPlayer)
                DeductWallet();
            GameManager.instance.StartFromServer(firstTurn);
            MenuManager.instance.SetOpponentDetail();
        }
        else
        {
            GameManager.instance.RejoinFromServer();
            MenuManager.instance.SetOpponentDetail();
        }
        AudioManager.instance.Stop("Matchmaking");
        gameObject.SetActive(false);
    }

    public void DeductWallet()
    {
        WallUpdate walletUpdate = new WallUpdate();
        if (AndroidtoUnityJSON.instance.game_mode == "tour")
            walletUpdate.game_id = AndroidtoUnityJSON.instance.tour_id;
        else if (AndroidtoUnityJSON.instance.game_mode == "battle")
            walletUpdate.game_id = AndroidtoUnityJSON.instance.battle_id;

        walletUpdate.user_id = AndroidtoUnityJSON.instance.player_id;
        walletUpdate.amount = AndroidtoUnityJSON.instance.game_fee;
        walletUpdate.type = AndroidtoUnityJSON.instance.game_mode;
        string mydata = JsonUtility.ToJson(walletUpdate);
        WebRequestHandler.Instance.Post(walletUpdateURL, mydata, (response, status) =>
        {
            Debug.Log(response + " sent wallet update");
        });
    }
}
