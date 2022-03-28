using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RTLTMPro;

public class MenuManager : MonoBehaviour
{
    #region public and private fields

    public static MenuManager instance;

    [SerializeField] private Button
    optionsButton, soundMuteButton, quitButton, chatButton, soundOptionsButton, chatEnabledButton;

    [SerializeField] private GameObject
    raycastBlocker, loadingScreenObject;

    [SerializeField] public RectTransform
    leaveGamePopUp, optionsPopUp, loadingScreenIcon1, loadingScreenIcon2, chatPopUp;

    [SerializeField] private Image
    playerProfilePicMatchMaking, opponentProfilePicMatchMaking, loadingScreenIcon1Image, loadingScreenIcon1SubImage, playerProfilePicGame, opponentProfilePicGame;

    [SerializeField] private RTLTextMeshPro
    playerNameText, opponentNameText, loadingScreenText;

    [SerializeField] private Sprite[]
    soundButtonSprites, chatEnabledSprites;

    [SerializeField] private Button[]
    emojis;

    private Sequence loadingScreenAnimations;

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region get and set references, functions and animations for button clicks and pop ups, set player name and images for gameplay screen

    private void Awake()
    {
        instance = this;
        optionsButton.onClick.AddListener(() =>
        {
            AudioManager.instance.Play("ButtonClick1");
            optionsButton.transform.localScale = Vector3.one;
            optionsButton.transform.DOPunchScale(-Vector3.one * 0.2f, 0.2f, 1, 1f).SetUpdate(true);
            if (!optionsPopUp.gameObject.activeInHierarchy)
            {
                optionsPopUp.gameObject.SetActive(true);
                optionsPopUp.DOAnchorPosY(300f, 0.5f).SetEase(Ease.OutQuart).From(Vector3.up * (-Screen.height / 2f - optionsPopUp.sizeDelta.y)).SetUpdate(true).OnStart(() => optionsButton.interactable = false).OnComplete(() => optionsButton.interactable = true) ;

                //leaveGamePopUp.DOScaleX(1f, 0.35f).From(0.3f).SetEase(Ease.OutQuart).SetUpdate(true);
            }
            else
            {
                optionsPopUp.DOAnchorPosY(-Screen.height / 2f - optionsPopUp.sizeDelta.y, 0.2f).SetEase(Ease.OutQuart).SetUpdate(true).OnStart(() => optionsButton.interactable = false).OnComplete(() =>
                {
                    optionsPopUp.gameObject.SetActive(false);
                    optionsButton.interactable = true;
                }); 
            }
        });

        chatButton.onClick.AddListener(() =>
        {
            AudioManager.instance.Play("ButtonClick1");
            if (!chatPopUp.gameObject.activeSelf)
            {
                chatPopUp.DOAnchorPosY(300f, 0.5f).SetEase(Ease.OutQuart).OnStart(() =>
                {
                    chatPopUp.gameObject.SetActive(true);
                    chatButton.interactable = false;
                }).OnComplete(() =>
                {
                    chatButton.interactable = true;
                });
            }
            else
            {
                chatPopUp.DOAnchorPosY(-Screen.height / 2f - chatPopUp.sizeDelta.y, 0.2f).SetEase(Ease.InQuart).OnStart(() => chatButton.interactable = false).OnComplete(() =>
                {
                    chatButton.interactable = true;
                    chatPopUp.gameObject.SetActive(false);
                });
            }
        });

        quitButton.onClick.AddListener(() =>
        {
            AudioManager.instance.Play("ButtonClick2");
            StrikerController.instance.isOverlay = true;
            optionsPopUp.DOAnchorPosY(-Screen.height / 2f - optionsPopUp.sizeDelta.y, 0.3f).SetEase(Ease.OutQuart).SetUpdate(true).OnComplete(() => optionsPopUp.gameObject.SetActive(false));
            raycastBlocker.SetActive(true);
            optionsButton.interactable = false;
            leaveGamePopUp.gameObject.SetActive(true);
            leaveGamePopUp.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutQuart).From(Vector3.left * (Screen.width / 2f + leaveGamePopUp.sizeDelta.x)).SetUpdate(true);
            //leaveGamePopUp.DOScaleX(1f, 0.35f).From(0.3f).SetEase(Ease.OutQuart).SetUpdate(true);
        });

        soundMuteButton.onClick.AddListener(() =>
        {
            AudioManager.instance.Play("ButtonClick1");
            AudioManager.instance.Mute = !AudioManager.instance.Mute;
            soundMuteButton.image.sprite = soundButtonSprites[AudioManager.instance.Mute ? 1 : 0];
        });

        chatEnabledButton.onClick.AddListener(() =>
        {
            GameManager.instance.isChatEnabled = !GameManager.instance.isChatEnabled;
            optionsPopUp.gameObject.SetActive(false);
            chatEnabledButton.image.sprite = chatEnabledSprites[GameManager.instance.isChatEnabled ? 0 : 1];
        });

        loadingScreenAnimations = DOTween.Sequence();
        loadingScreenAnimations.Append(loadingScreenIcon1.DOLocalRotate(Vector3.forward * 720, 1.1f, RotateMode.FastBeyond360).SetEase(Ease.OutQuart).From(Vector3.zero));
        loadingScreenAnimations.Join(loadingScreenIcon1.DOScale(Vector3.one * 2f, 1.3f).SetEase(Ease.InOutSine).From(Vector3.one * 0.5f));
        loadingScreenAnimations.Join(loadingScreenIcon1Image.DOFade(0f, 1.3f).SetEase(Ease.InOutSine).From(1f));
        loadingScreenAnimations.Join(loadingScreenIcon1SubImage.DOFade(0f, 1.3f).SetEase(Ease.InOutSine).From(1f));
        loadingScreenAnimations.OnStepComplete(() =>
        {
            int textLength = loadingScreenText.text.Length;
            loadingScreenText.text = "";
            for (int i = 0; i < (textLength + 1) % 4; i++)
            {
                loadingScreenText.text += ".";
            }
        });
        loadingScreenAnimations.OnStart(() =>
        {
            loadingScreenObject.gameObject.SetActive(true);
            loadingScreenIcon2.gameObject.SetActive(false);
        });
        loadingScreenAnimations.OnPause(() => loadingScreenObject.SetActive(false));
        loadingScreenAnimations.SetLoops(-1);
        loadingScreenAnimations.SetAutoKill(false);
        loadingScreenAnimations.Pause();

        // Temporary player and opponent names
        // Set player name from the string provided by main app
        playerNameText.text = "Player";
        // Set opponent name from the string provided by node js server
        opponentNameText.text = "Opponent";

        // Do the same with player profile pic and opponent profile pic
    
    }

    public void KeepPlaying()
    {
        leaveGamePopUp.DOAnchorPosX(-Screen.width / 2f - leaveGamePopUp.sizeDelta.x, 0.2f).SetEase(Ease.OutQuart).From(Vector3.zero).SetUpdate(true).OnComplete(() =>
        {
            leaveGamePopUp.gameObject.SetActive(false);
            raycastBlocker.SetActive(false);
            optionsButton.interactable = true;
            StrikerController.instance.isOverlay = false;
        });
        //leaveGamePopUp.DOScaleX(0.5f, 0.1f).From(1f).SetEase(Ease.InQuart).SetUpdate(true);
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region send and receive emojis

    public void SendEmoji(int number)
    {
        NetworkClient.instance.SendEmoji((byte)number);
    }

    public void ReceiveEmojis(int emojiNumber)
    {

        /*emojiObjectsRects[emojiNumber].gameObject.SetActive(true);
        emojiObjectsRects[emojiNumber].anchoredPosition = emojiDisplayPositionRects[playerColour].anchoredPosition;
        // Add some animations*/
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region loading screen animations

    public void StartLoadingScreen()
    {
        Debug.Log("Starting loading screen");
        loadingScreenAnimations.Restart();
    }

    public void DisableLoadingScreen()
    {
        loadingScreenIcon2.gameObject.SetActive(true);
        loadingScreenAnimations.Pause();
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------


    public void LeaveGame()
    {
        NetworkClient.instance.LeaveMatch();
    }
}
