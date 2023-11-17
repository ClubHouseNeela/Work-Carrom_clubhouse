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
    [SerializeField] string chanceDeductURL;
    [SerializeField] RTLTextMeshPro searchingText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] float timeLeft;

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
        chanceDeductURL = NetworkClient.instance.adminURL + chanceDeductURL;
        AudioManager.instance.Play("Matchmaking");
        StartCoroutine(TextAnimation(searchingText));
        StartCoroutine(StartTimerCoroutine());
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
        if (AndroidtoUnityJSON.instance.game_mode == "tour")
        {
            WebRequestHandler.Instance.Post
            (
                chanceDeductURL,
                "{\"user_id\":\"" + AndroidtoUnityJSON.instance.player_id + "\",\"tournament_id\":\"" + AndroidtoUnityJSON.instance.tour_id + "\"}",
                (response, status) => { }
            );
            if (AndroidtoUnityJSON.instance.entry_type != "re entry paid")
            {
                return;
            }
        }
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

    private IEnumerator StartTimerCoroutine()
    {
        while (timeLeft > 0)
        {
            timerText.text = string.Format("{0:00} : {1: 00}", timeLeft / 60, timeLeft % 60);
            yield return new WaitForSecondsRealtime(1f);
            timeLeft -= 1f;
        }
        GameManager.instance.TimeOver();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}
