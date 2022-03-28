using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using DG.Tweening;

public class MatchMakingUIManager : MonoBehaviour
{
    #region public and private fields 

    public static MatchMakingUIManager instance;

    [SerializeField] public string
    playerID, opponentID;

    [SerializeField] private Image
    playerImage, opponentImage, loadingIconImage, loadingCircleImage, playerGameImg, oppgameImg;

    [SerializeField] public RTLTextMeshPro
    playerMobileNumberText, opponentMobileNumberText, PlayerGameName, OpponentGameName;

    private Color[] colourList = new Color[] { Color.cyan, Color.red, Color.magenta, Color.yellow, Color.green, Color.blue, Color.gray, Color.white };

    private Sequence loadingIconSequence;

    private const string glyphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string getTournAttemptURL = "https://admin.gamejoypro.com/api/getattempts";
    public WallUpdate walletUpdate;

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region get and set references, set animation sequences, set player names and image from main app and opponent name and image from node js server

    private void Awake()
    {
        instance = this;
        PlayerPrefs.SetString(CommonValues.PlayerPrefKeys.PLAYER_MOBILE_NUMBER, playerID);
    }

    private void Start()
    {
        if (GameManager.instance.gameMode == CommonValues.GameMode.LOCAL_MULTIPLAYER)
        {
            StartCoroutine(FakePause(Random.Range(3, 6)));
        }
        if (gameObject.activeInHierarchy)
        {
            // Assign profile images for player and opponent here

            // Loading icon animation
            loadingIconSequence = DOTween.Sequence();
            float _fadeAmount = 1f;
            loadingIconSequence.Append(loadingIconImage.DOColor(colourList[0], 1f).SetEase(Ease.InOutSine).From(colourList[colourList.Length - 1]));
            loadingIconSequence.Join(loadingCircleImage.DOFade(_fadeAmount, 1f).SetEase(Ease.Linear).From(0));
            for (int i = 1; i < colourList.Length; i++)
            {
                _fadeAmount = (i % 2 == 0) ? 1f : 0f;
                loadingIconSequence.Append(loadingIconImage.DOColor(colourList[i], 1f).SetEase(Ease.InOutSine));
                loadingIconSequence.Join(loadingCircleImage.DOFade(_fadeAmount, 1f).SetEase(Ease.Linear));
            }
            loadingIconSequence.SetDelay(0f).OnComplete(() => loadingIconSequence.Restart());

            // Opponent mobile number generation
            opponentID = "";
            for (int i = 0; i < 10; i++)
            {
                opponentID += glyphs[Random.Range(0, glyphs.Length)];
            }
            PlayerPrefs.SetString(CommonValues.PlayerPrefKeys.OPPONENT_MOBILE_NUMBER, opponentID);
            playerMobileNumberText.text = AndroidtoUnityJSON.instance.user_name;//playerID;
            PlayerGameName.text = AndroidtoUnityJSON.instance.user_name;//playerID;
            WebRequestHandler.Instance.DownloadSprite(AndroidtoUnityJSON.instance.profile_image, (sprite) =>
            {
                playerImage.sprite = sprite;
                playerGameImg.sprite = sprite;
            });
            StartCoroutine(OpponentMobileNumberSearchCoroutine());
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region testing for matchmaking screen

    private IEnumerator FakePause(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        FakeMatchmakingForLocalMultiplayer();
    }

    private void FakeMatchmakingForLocalMultiplayer()
    {
        SetOpponentNumberText();
        StartCoroutine(LocalMultiplayerStart());
    }

    private IEnumerator LocalMultiplayerStart()
    {
        yield return new WaitForSeconds(1f);
        GameStart();
        GameManager.instance.SetGameModeAndStart(0);
    }


    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region matchmaking screen animations

    private IEnumerator OpponentMobileNumberSearchCoroutine()
    {
        yield return null;
        opponentMobileNumberText.rectTransform.DOScale(Vector3.one * 1.5f, 0.5f).SetEase(Ease.OutQuart);
        string _tempOpponentID;
        while (true)
        {
            _tempOpponentID = "";
            for (int i = 0; i < 10; i++)
            {
                _tempOpponentID += glyphs[Random.Range(0, glyphs.Length)];
            }
            opponentMobileNumberText.text = _tempOpponentID;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region matched with opponent and game start

    public void SetOpponentNumberText()
    {
        StopAllCoroutines();
        opponentMobileNumberText.text = opponentID;

        if (NetworkClient.instance.matchDetails.playerId[1] != 0)
        {
            OpponentGameName.text = NetworkClient.instance.matchDetails.playerName[GameManager.instance.playerNumberOnline];
            opponentMobileNumberText.text = NetworkClient.instance.matchDetails.playerName[GameManager.instance.playerNumberOnline];
            WebRequestHandler.Instance.DownloadSprite(NetworkClient.instance.matchDetails.playerDp[GameManager.instance.playerNumberOnline], (sprite) =>
            {
                oppgameImg.sprite = sprite;
                opponentImage.sprite = sprite;
            });
        }
        else
        {
            //if bot
            OpponentGameName.text = NetworkClient.instance.oppPlayerName;
            opponentMobileNumberText.text = NetworkClient.instance.oppPlayerName;
            WebRequestHandler.Instance.DownloadSprite(NetworkClient.instance.oppPlayerDp, (sprite) => 
            { 
                oppgameImg.sprite = sprite;
                opponentImage.sprite = sprite;
            });
        }

        opponentMobileNumberText.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutQuart);

        var attemptNo = GameManager.instance.attemptNo;
        //attempt check
        WebRequestHandler.Instance.Get(getTournAttemptURL + "/" + AndroidtoUnityJSON.instance.tour_id, (response, status) =>
        {
            GetAttempt attempt = JsonUtility.FromJson<GetAttempt>(response);

            Debug.Log("Attempt(s) remaining: " + attempt.data.remainAttemp);

            attemptNo = int.Parse(attempt.data.remainAttemp);
            GameManager.instance.attemptNo = attemptNo;

            if (attempt.data.remainAttemp == AndroidtoUnityJSON.instance.no_of_attempts)
            {
                AndroidtoUnityJSON.instance.isFirst = true;
            }
            else
            {
                AndroidtoUnityJSON.instance.isFirst = false;
            }
        });

        if (AndroidtoUnityJSON.instance.game_mode == "tour")
        {
            if (AndroidtoUnityJSON.instance.entry_type == "re entry" && AndroidtoUnityJSON.instance.isFirst ||
                AndroidtoUnityJSON.instance.entry_type == "re entry paid" || AndroidtoUnityJSON.instance.entry_type == "single entry")
            {
                DeductWallet();
            }
        }
        else
        {
            DeductWallet();
        }
    }

    public void DeductWallet()
    {
        if (AndroidtoUnityJSON.instance.game_mode == "tour")
            walletUpdate.game_id = AndroidtoUnityJSON.instance.tour_id;
        else if (AndroidtoUnityJSON.instance.game_mode == "battle")
            walletUpdate.game_id = AndroidtoUnityJSON.instance.battle_id;

        walletUpdate.amount = AndroidtoUnityJSON.instance.game_fee;
        walletUpdate.type = AndroidtoUnityJSON.instance.game_mode;
        string mydata = JsonUtility.ToJson(walletUpdate);
        WebRequestHandler.Instance.Post(GameManager.instance.walletUpdateURL, mydata, (response, status) =>
        {
            Debug.Log(response + " sent wallet update");
        });
    }

    public void GameStart()
    {
        if (gameObject.activeInHierarchy)
        {
            loadingIconSequence.Kill();
            gameObject.SetActive(false);
        }

        //Destroy(gameObject);
    }

    public void Matched(bool firstTurn, bool initialStart, int randomSeed)
    {
        Random.InitState(randomSeed);
        if (initialStart)
        {
            SetOpponentNumberText();
            StartCoroutine(LoadingOver(firstTurn));
        }
        else
        {
            GameStart();
            GameManager.instance.RejoinFromServer();
        }
    }

    private IEnumerator LoadingOver(bool firstTurn)
    {
        yield return new WaitForSeconds(1f);
        GameStart();
        GameManager.instance.StartFromServer(firstTurn);
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------
}
