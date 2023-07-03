using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTLTMPro;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region private and public fields

    public static GameManager instance;
    public static event System.Action<bool> setKinematicForPieces = delegate { };

    //public string serverURL = "ws://3.7.19.73:5000/socket.io/?EIO=4&transport=websocket";
    public string sendDataURL;
    public string matchFoundURL;
    public string walletUpdateURL;
    public string getTournAttemptURL;


    // Logs
    public LogInfo logs = new LogInfo();

    [System.Serializable]
    public struct Logs
    {
        public string condition;
        public string stackTrace;
        public LogType type;

        public string dateTime;

        public Logs(string condition, string stackTrace, LogType type, string dateTime)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
            this.dateTime = dateTime;
        }
    }

    [System.Serializable]
    public class LogInfo
    {
        public List<Logs> logInfoList = new List<Logs>();
    }


    [Range(1, 20)] public int timerForTurn = 5;

    public Sprite[] pieceSprites;
    public float velocityThresholdForStoppingMovement = 0.1f;
    public float velocityThresholdForPieceFallInPocket = 2f;
    public CommonValues.GameMode gameMode;
    public bool flipped = false;
    public bool hasBot = false;
    public BotManager.BotType botType;
    public int maxPointsFreestyleMode;
    public int pieceTargetColour;
    public int playerNumberOnline;
    public bool gameStarted = false;
    public bool gameOver = false;
    public bool isChatEnabled;
    public bool coloursFlipped;
    public int attemptNo = 0;


    [SerializeField] private GameObject P1Chat;
    [SerializeField] private GameObject P2Chat;
    [SerializeField] private GameObject WarningMessage;
    [SerializeField] private GameObject LeaderboardScreen;
    [SerializeField] private List<PieceScript> piecesOnBoard = new List<PieceScript>();
    [SerializeField] private int numberOfMovingPieces;
    [SerializeField] private bool turn;
    [SerializeField] private RTLTextMeshPro[] scoreTexts;
    [SerializeField] private Sprite[] Result;
    [SerializeField] private PieceScript[] lastFallenPieces = new PieceScript[2];
    [SerializeField] private Vector2[] lastPocket = new Vector2[2] { Vector2.zero, Vector2.zero };
    [SerializeField] private RTLTextMeshPro gameEndText;
    [SerializeField] private int piecesFallen = 0;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject pieceColourSelectMenu;
    [SerializeField] private GameObject[] piecePoints;
    [SerializeField] private GameObject targetPiece;
    [SerializeField] private GameObject oppTargetPiece;
    [SerializeField] private GameObject pieceInPocketAnimObject;
    [SerializeField] private Animator pieceInPocketAnim;
    [SerializeField] private RTLTextMeshPro promptText;
    [SerializeField] private RectTransform promptBackgroundRect;

    private List<Vector2> piecePos = new List<Vector2>();
    private List<float> pieceRot = new List<float>();
    private bool oneMoreChance;
    private bool isLocalPlayerWon;
    private bool redPieceFallenWithoutAdditionalPiece = false;
    private bool strikerInPocket = false;
    private bool queenHasFallen = false;
    private ContactFilter2D contactFilter = new ContactFilter2D();


    public NetworkingPlayer thisPlayer;
    private NetworkingPlayer otherPlayer;
    public SendData sendThisPlayerData;
    public WinningDetails winning_details;
    public WalletInfo walletInfo;
    public WallUpdate walletUpdate;
    public bool foundOtherPlayer = false;
    public bool canStartGame;
    public string sendWinningDetailsData;
    public string sendNewData1;
    private string walletInfoData;
    private string walletUpdateData;
    // public bool canRemoveTouchBlock { get; private set; }

    public bool foundWinner;
    public bool isDataSend;
    string myRoomId;
    //public static GameManager instance;
    //matchmaking Variable
    [SerializeField] GameObject ReplayBtn;

    //[SerializeField] public Image blocker;
    public bool isGameOver;

    private bool isReEntryPaid;
    private bool isSingleEntry;


    //Testing variables
    private bool pieceHasFallen = false;

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region references and initialization

    private void Awake()
    {
        sendDataURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL, "") + sendDataURL;
        matchFoundURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL, "") + matchFoundURL;
        walletUpdateURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL, "") + walletUpdateURL;
        getTournAttemptURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL, "") + getTournAttemptURL;
        if (AndroidtoUnityJSON.instance.multiplayer_game_mode == "free")
            gameMode = CommonValues.GameMode.FREESTYLE;
        if (AndroidtoUnityJSON.instance.multiplayer_game_mode == "pro")
            gameMode = CommonValues.GameMode.BLACK_AND_WHITE;

        if (AndroidtoUnityJSON.instance.game_mode == "tour")
        {
            if (AndroidtoUnityJSON.instance.mm_player == "2")
            {
                //P2.SetActive(true);

                if (AndroidtoUnityJSON.instance.entry_type == "re entry paid")
                {
                    isReEntryPaid = true;
                }
                else if (AndroidtoUnityJSON.instance.entry_type == "re entry")
                {
                    isReEntryPaid = false;
                }
                else if (AndroidtoUnityJSON.instance.entry_type == "single entry")
                {
                    isSingleEntry = true;
                }

                //StartCoroutine(StartOnlinePlay());
            }
            else if (AndroidtoUnityJSON.instance.mm_player == "1")
            {
                //VS.SetActive(false);
                //P2.SetActive(false);

                if (AndroidtoUnityJSON.instance.entry_type == "re entry paid")
                {
                    isReEntryPaid = true;
                }
                else if (AndroidtoUnityJSON.instance.entry_type == "re entry")
                {
                    isReEntryPaid = false;
                }
                else if (AndroidtoUnityJSON.instance.entry_type == "single entry")
                {
                    isSingleEntry = true;
                }

                //StartCoroutine(StartOfflinePlay());
            }
        }

        Debug.Log("Game mode: " + gameMode);

        Shader.WarmupAllShaders();

        instance = this;

        PieceScript.pieceIsMoving += SetMovingPieces;
        PieceScript.pieceInPocket += PieceInPocket;

        StrikerController.strikerIsMoving += SetMovingPieces;
        StrikerController.shoot += StrikerShot;
        StrikerController.strikerInPocket += PieceInPocket;

        TimerScript.timerOver += EndTurn;

        Application.logMessageReceived += LogCallback;


        contactFilter.useTriggers = true;


        // Don't allow mobile screen to turn off
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Unlimited framerate
        Application.targetFrameRate = -1;

        // No Vsync
        QualitySettings.vSyncCount = 0;


        Random.InitState(42);
        this.enabled = false;
        isChatEnabled = true;
    }

    private void Start()
    {

    }

    public void OnEnable()
    {
        if (hasBot)
        {
            TimerScript.timerOver -= EndTurn;
            TimerScript.timerOver += EndTurn;
        }
        if (gameMode == CommonValues.GameMode.FREESTYLE || gameMode == CommonValues.GameMode.PRACTICE)
        {
            startGameButton.SetActive(false);
            PieceGenerator.instance.GeneratePieces(false);
        }
        else if (gameMode == CommonValues.GameMode.BLACK_AND_WHITE)
        {
            for (int i = 0; i < piecePoints.Length; i++)
            {
                piecePoints[i].SetActive(false);
            }
        }

    }

    void LogCallback(string condition, string stackTrace, LogType type)
    {
        //Create new Log
        Logs logInfo = new Logs(condition, stackTrace, type, System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz"));

        //Add it to the List
        logs.logInfoList.Add(logInfo);
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region getters and setters, make and destroy pieces, get player and match details from main app

    public bool Turn
    {
        get
        {
            return turn;
        }
        set
        {
            pieceHasFallen = false;
            if (value != turn)
            {
                redPieceFallenWithoutAdditionalPiece = false;
            }
            turn = value;
            oneMoreChance = false;
            strikerInPocket = false;
            if (gameStarted)
            {
                if (hasBot)
                {
                    NetworkClient.instance.SendScoresBot((byte)GetScore(0), (byte)GetScore(1));
                }
                else if (value)
                {
                    NetworkClient.instance.SendScoresNoBot((byte)GetScore(0));
                }
            }
        }

    }

    private uint GetPlayerNumber()
    {
        if (Turn)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    public int GetScore(int playerNumber)
    {
        return int.Parse(scoreTexts[playerNumber].text.Split('/')[0]);
    }

    public void SetScoreFromServer(int player1Score, int player2Score)
    {
        if (gameMode == CommonValues.GameMode.BLACK_AND_WHITE)
        {
            if (playerNumberOnline == 0)
            {
                scoreTexts[0].text = player1Score.ToString() + "/9";
                scoreTexts[1].text = player2Score.ToString() + "/9";
            }
            else
            {
                scoreTexts[0].text = player2Score.ToString() + "/9";
                scoreTexts[1].text = player1Score.ToString() + "/9";
            }
        }
        else
        {
            if (playerNumberOnline == 0)
            {
                scoreTexts[0].text = player1Score.ToString() + "/" + maxPointsFreestyleMode.ToString();
                scoreTexts[1].text = player2Score.ToString() + "/" + maxPointsFreestyleMode.ToString();
            }
            else
            {
                scoreTexts[0].text = player2Score.ToString() + "/" + maxPointsFreestyleMode.ToString();
                scoreTexts[1].text = player1Score.ToString() + "/" + maxPointsFreestyleMode.ToString();
            }

        }

    }

    public void ChangeScore(uint playerNumber, int value)
    {
        if (gameMode == CommonValues.GameMode.BLACK_AND_WHITE)
        {
            if (value == 0)
            {
                scoreTexts[playerNumber].text = "0/9";
                return;
            }
            if (Mathf.Abs(value) == 50)
            {
                return;
            }
            int score = 0;

            if (Mathf.Abs(value) == (pieceTargetColour * 10 + 10))
            {
                score = int.Parse(scoreTexts[0].text.Split('/')[0]) + ((int)Mathf.Sign(value));
                playerNumber = 0;
            }
            else
            {
                score = int.Parse(scoreTexts[1].text.Split('/')[0]) + ((int)Mathf.Sign(value));
                playerNumber = 1;
            }
            scoreTexts[playerNumber].text = score.ToString() + "/9";
        }
        else
        {
            if (value == 0)
            {
                scoreTexts[playerNumber].text = "0/" + maxPointsFreestyleMode.ToString();
                return;
            }
            int score = int.Parse(scoreTexts[playerNumber].text.Split('/')[0]) + value;
            scoreTexts[playerNumber].text = score.ToString() + "/" + maxPointsFreestyleMode.ToString();
        }
    }

    public void SetPiecesOnBoard(List<PieceScript> pieces)
    {
        if (piecesOnBoard.Count == 0)
        {
            piecesOnBoard = new List<PieceScript>(pieces);
            if (hasBot)
            {
                StrikerController.instance.SetBotValues(botType);
            }
            Debug.Log("Setting pieces on board, " + piecesOnBoard.Count + " pieces");
        }
        else
        {
            Debug.LogError("pieces on board already set");
        }
    }

    public void DestroyPiecesOnBoard()
    {
        while (piecesOnBoard.Count > 0)
        {
            Destroy(piecesOnBoard[0].gameObject);
            piecesOnBoard.RemoveAt(0);
        }
    }
    public void SetMatchData(List<Vector2> piecePos, List<float> pieceRot, bool queenHasFallen, byte numPiecesFallen, bool[] piecesEnabled, byte[] lastPiecesFallen, Vector2[] lastPocketPos, bool redPieceFallenWithoutOtherPieces)
    {
        Debug.Log("Setting Match data");
        SetPieceMag(piecePos, pieceRot, piecesEnabled);
        this.queenHasFallen = queenHasFallen;
        this.piecesFallen = (int)numPiecesFallen;
        this.redPieceFallenWithoutAdditionalPiece = redPieceFallenWithoutOtherPieces;
        for (int i = 0; i < 2; i++)
        {
            if (lastPiecesFallen[i] != 250)
            {
                lastFallenPieces[i] = piecesOnBoard[(int)lastPiecesFallen[i]];
            }
            else
            {
                lastFallenPieces[i] = null;
            }
            lastPocket[i] = lastPocketPos[i];
        }
    }

    public void SetPieceMag(List<Vector2> piecePos, List<float> pieceRot, bool[] piecesEnabled)
    {
        for (int i = 0; i < piecesOnBoard.Count; i++)
        {
            piecesOnBoard[i].rigidbody.position = piecePos[i];
            piecesOnBoard[i].rigidbody.rotation = pieceRot[i];
            piecesOnBoard[i].gameObject.SetActive(piecesEnabled[i]);
        }
    }

    public void GetPlayerAndMatchDetailsFromMainApp()
    {
        // set game mode
        // set player ID, name and picture in MatchMakingUIManager
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region start game, send match details to server, rejoin game, resume game

    public void SetGameModeAndStart(int gameMode)
    {
        this.gameMode = (CommonValues.GameMode)gameMode;
        startGameButton.SetActive(false);
        gameEndText.gameObject.SetActive(false);
        PieceGenerator.instance.GeneratePieces(false);
        ChangeScore(0, 0);
        ChangeScore(1, 0);
        StrikerController.instance.enabled = true;
        if (flipped)
        {
            StrikerController.instance.FlipScreenMaxMinPoints();
        }
        ActivatePieceSimulation();
        Turn = true;
        gameStarted = true;
        StrikerController.instance.DisableStriker();
        SetTurn(Turn);
    }

    public void StartFromServer(bool value)
    {
        AudioManager.instance.BackgroundMusicStart();
        if (!value)
        {
            Camera.main.transform.eulerAngles = new Vector3(0, 0, 180f);
            flipped = true;
            playerNumberOnline = 1;
        }
        else
        {
            playerNumberOnline = 0;
        }
        startGameButton.SetActive(false);
        ChangeScore(0, 0);
        ChangeScore(1, 0);
        if (gameMode == CommonValues.GameMode.FREESTYLE || gameMode == CommonValues.GameMode.PRACTICE)
        {

            StrikerController.instance.enabled = true;
            StrikerController.instance.PlayerSliderAddListenerAndCorrecGlowCirclePosition(value);
            if (flipped)
            {
                StrikerController.instance.FlipScreenMaxMinPoints();
            }
            ActivatePieceSimulation();
            Turn = value;
            gameStarted = true;
            StrikerController.instance.DisableStriker();
            SetTurn(value);
        }
        else
        {
            //pieceColourSelectMenu.SetActive(true);

            //int pick = Random.Range(0, 1);
            PieceColourSelected(playerNumberOnline);
        }
    }

    public void RejoinFromServer()
    {
        playerNumberOnline = NetworkClient.instance.rejoinRoom.playerNumber;
        AudioManager.instance.BackgroundMusicStart();
        if (playerNumberOnline == 1)
        {
            Camera.main.transform.eulerAngles = new Vector3(0, 0, 180f);
            flipped = true;
        }


        startGameButton.SetActive(false);
        ChangeScore(0, 0);
        ChangeScore(1, 0);
        StrikerController.instance.enabled = true;

        StrikerController.instance.PlayerSliderAddListenerAndCorrecGlowCirclePosition(playerNumberOnline == 0);

        if (flipped)
        {
            StrikerController.instance.FlipScreenMaxMinPoints();
        }
        if (gameMode == CommonValues.GameMode.BLACK_AND_WHITE)
        {
            int colour = (PlayerPrefs.GetInt(CommonValues.PlayerPrefKeys.PLAYER_COLOUR, 0));
            pieceColourSelectMenu.SetActive(false);

            var pieceCol = 0;

            if (colour == 1)
                pieceCol = 0;
            else if (colour == 0)
                pieceCol = 1;

            targetPiece.GetComponent<Image>().sprite = pieceSprites[colour];
            targetPiece.SetActive(true);

            oppTargetPiece.GetComponent<Image>().sprite = pieceSprites[pieceCol];
            oppTargetPiece.SetActive(true);

            pieceTargetColour = colour;
            this.coloursFlipped = PlayerPrefs.GetInt(CommonValues.PlayerPrefKeys.COLOURS_FLIPPED, -1) == 1;
            GeneratePiecesForBlackAndWhiteMode(coloursFlipped);
        }

        ActivatePieceSimulation();
        setKinematicForPieces(true);
    }

    public void CheckColorFlip()
    {
        if (coloursFlipped)
        {
            foreach (var piece in piecesOnBoard)
            {
                if (piece.Colour != CommonValues.Colour.RED)
                {
                    if (piece.Colour == CommonValues.Colour.BLACK)
                    {
                        piece.Colour = CommonValues.Colour.WHITE;
                    }
                    else if (piece.Colour == CommonValues.Colour.WHITE)
                    {
                        piece.Colour = CommonValues.Colour.BLACK;
                    }
                }
            }
        }
    }
    public void ResumeGame(bool value, bool setTurn)
    {
        gameStarted = true;
        StrikerController.instance.DisableStriker();
        if (setTurn)
        {
            Turn = !Turn;
            Debug.Log("Resetting values and conitnuing");
            SetTurn(Turn);
        }
    }

    private void ActivatePieceSimulation()
    {
        for (int i = 0; i < piecesOnBoard.Count; i++)
        {
            piecesOnBoard[i].IsSimulated = true;
        }
    }

    public void GeneratePiecesForBlackAndWhiteMode(bool flipColour)
    {
        PieceGenerator.instance.GeneratePieces(flipColour);
    }

    public void StartGameForBlackAndWhiteMode(bool coloursFlipped)
    {
        AudioManager.instance.BackgroundMusicStart();
        StrikerController.instance.enabled = true;
        StrikerController.instance.PlayerSliderAddListenerAndCorrecGlowCirclePosition(!flipped);
        if (flipped)
        {
            StrikerController.instance.FlipScreenMaxMinPoints();
        }
        this.coloursFlipped = coloursFlipped;
        ActivatePieceSimulation();
        Turn = !flipped;
        gameStarted = true;
        StrikerController.instance.DisableStriker();
        SetTurn(Turn);
    }

    public void PieceColourSelected(int colour)
    {
        pieceColourSelectMenu.SetActive(false);

        var pieceCol = 0;

        if (colour == 1)
            pieceCol = 0;
        else if (colour == 0)
            pieceCol = 1;

        targetPiece.GetComponent<Image>().sprite = pieceSprites[colour];
        targetPiece.SetActive(true);

        oppTargetPiece.GetComponent<Image>().sprite = pieceSprites[pieceCol];
        oppTargetPiece.SetActive(true);

        pieceTargetColour = colour;
        NetworkClient.instance.SendPlayerPieceColour(colour);
    }

    public void SetTurn(bool value)
    {
        StrikerController.instance.PlayerTurn = value;
        if (value)
        {
            uint player = GetPlayerNumber();
            Debug.Log("Turn for player" + player);
            if (gameMode == CommonValues.GameMode.BLACK_AND_WHITE)
            {

                int fallenPieces = GetScore((int)player);
                if (fallenPieces == 8 && !queenHasFallen)
                {
                    LastColourPieceOnBoardTextAnimation();
                }
            }
        }
        if (value)
        {
            NetworkClient.instance.SetTurnOnServer();
        }
        numberOfMovingPieces = 0;
        if (((value && gameMode != CommonValues.GameMode.LOCAL_MULTIPLAYER) || hasBot) && gameStarted)
        {
            SendMatchDataToServer();
        }
    }
    public void PlayerRejoinSetTurn(bool value)
    {
        Turn = value;
    }

    private void SendMatchDataToServer()
    {
        piecePos.Clear();
        pieceRot.Clear();
        bool[] _piecesFallen = new bool[piecesOnBoard.Count];
        for (int i = 0; i < piecesOnBoard.Count; i++)
        {
            piecePos.Add(piecesOnBoard[i].rigidbody.position);
            pieceRot.Add(piecesOnBoard[i].rigidbody.rotation);
            _piecesFallen[i] = piecesOnBoard[i].gameObject.activeSelf;
        }
        byte[] _lastPiecesFallen = new byte[2];
        if (lastFallenPieces[0] != null)
        {
            _lastPiecesFallen[0] = (byte)lastFallenPieces[0].pieceIndex;
        }
        else
        {
            _lastPiecesFallen[0] = 250;
        }

        if (lastFallenPieces[1] != null)
        {
            _lastPiecesFallen[1] = (byte)lastFallenPieces[1].pieceIndex;
        }
        else
        {
            _lastPiecesFallen[1] = 250;
        }

        NetworkClient.instance.SendMatchData(piecePos, pieceRot, queenHasFallen, (byte)piecesFallen, _piecesFallen, _lastPiecesFallen, lastPocket, (playerNumberOnline == 0) ? true : false, redPieceFallenWithoutAdditionalPiece);
    }

    private void SetMovingPieces(bool value)
    {
        if (value)
        {
            numberOfMovingPieces++;
        }
        else
        {
            numberOfMovingPieces--;
            if (numberOfMovingPieces <= 0)
            {
                EndTurn();
            }
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region game over logic

    private bool GameOverCheck()
    {
        if (redPieceFallenWithoutAdditionalPiece && gameMode == CommonValues.GameMode.BLACK_AND_WHITE)
        {
            return false;
        }
        int p1Score = GetScore(0);
        int p2Score = GetScore(1);
        if (gameMode != CommonValues.GameMode.BLACK_AND_WHITE)
        {
            if (p1Score >= maxPointsFreestyleMode)
            {
                // Game Over you win
                Debug.Log("You win");
                gameEndText.text = "";
                gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[0];
                AudioManager.instance.Play("Win");
                NetworkClient.instance.SendPlayerWin();
                GameOver(true);
                return true;
            }
            else if (p2Score >= maxPointsFreestyleMode)
            {
                // Game Over opponent won
                Debug.Log("Opponent won");
                AudioManager.instance.Play("Lose");
                gameEndText.text = "";
                gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[1];
                NetworkClient.instance.SendPlayerLose();
                GameOver(false);
                return true; ;
            }
            else if (piecesFallen == 19)
            {
                if (p1Score == p2Score)
                {
                    // Game Over draw
                    AudioManager.instance.Play("Lose");
                    gameEndText.text = "DRAW";
                    NetworkClient.instance.SendPlayerDraw();
                    GameOver(false);
                    return true; ;
                }
                else if (p1Score > p2Score)
                {
                    // Game Over you win
                    Debug.Log("You win");
                    gameEndText.text = "";
                    gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[0];
                    AudioManager.instance.Play("Win");
                    NetworkClient.instance.SendPlayerWin();
                    GameOver(true);
                    return true; ;
                }
                else
                {
                    // Game Over opponent won
                    Debug.Log("Opponent won");
                    AudioManager.instance.Play("Lose");
                    gameEndText.text = "";
                    gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[1];
                    NetworkClient.instance.SendPlayerLose();
                    GameOver(true);
                    return true; ;
                }
            }
        }
        else
        {
            if (p1Score >= 9 && !queenHasFallen && Turn)
            {
                Debug.Log("You lose");
                gameEndText.text = "";
                gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[1];
                AudioManager.instance.Play("Lose");
                GameOver(false);
                return true;
            }

            if (p2Score >= 9 && !queenHasFallen)
            {
                Debug.Log("You win");
                gameEndText.text = "";
                gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[0];
                AudioManager.instance.Play("Win");
                NetworkClient.instance.SendPlayerWin();
                GameOver(true);
                return true;
            }

            if (p1Score >= 9 || (p1Score >= 9 && !queenHasFallen && !Turn))
            {
                Debug.Log("You win");
                gameEndText.text = "";
                gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[0];
                AudioManager.instance.Play("Win");
                NetworkClient.instance.SendPlayerWin();
                GameOver(true);
                return true;
            }
            else if (p2Score >= 9)
            {
                Debug.Log("Opponent Won");
                AudioManager.instance.Play("Lose");
                gameEndText.text = "";
                gameEndText.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Result[1];
                GameOver(false);
                return true;
            }
        }
        return false;
    }

    public void GameOver(bool isWon)
    {
        gameOver = true;
        gameStarted = false;
        BotManager.instance.StopAllCoroutines();
        this.StopAllCoroutines(); ;
        StrikerController.instance.StopAllCoroutines();
        StrikerController.instance.MakeSlidersUninteractable();
        TimerScript.instance.StopTimer();
        gameEndText.gameObject.SetActive(true);
        gameEndText.transform.DOScale(1, 1f).From(0.3f).SetEase(Ease.OutBack).easeOvershootOrAmplitude = 2f;
        if (gameMode == CommonValues.GameMode.LOCAL_MULTIPLAYER)
        {
            StartCoroutine(GameOverPause());
            while (piecesOnBoard.Count > 0)
            {
                Destroy(piecesOnBoard[0].gameObject);
                piecesOnBoard.RemoveAt(0);
            }
        }
        else
        {
            //Time.timeScale = 0f;
        }

        //if (AndroidtoUnityJSON.instance.game_mode == "tour" && AndroidtoUnityJSON.instance.entry_type == "single entry")
        //{
        //    ChanceLeft.gameObject.SetActive(false);
        //    ReplayBtn.transform.parent.gameObject.SetActive(false);
        //    Footer_1.SetActive(true);
        //}
        //else if (AndroidtoUnityJSON.instance.game_mode == "tour")
        //{
        //    if (attemptNo <= 0)
        //    {
        //        ReplayBtn.transform.parent.gameObject.SetActive(false);
        //        Footer_1.SetActive(true);
        //    }

            //    ChanceLeft.gameObject.SetActive(true);
            //    ChanceLeft.text = "Chances Left: " + attemptNo;
            //}
        //else if (AndroidtoUnityJSON.instance.game_mode == "battle")
        //{
        //    ChanceLeft.gameObject.SetActive(false);
        //}



        //if (AndroidtoUnityJSON.instance.mm_player == "1")
        //{
        //    LosserNameText.transform.parent.gameObject.SetActive(false);
        //}



        //if (float.Parse(AndroidtoUnityJSON.instance.game_fee) <= 0 || AndroidtoUnityJSON.instance.entry_type == "re entry" && AndroidtoUnityJSON.instance.game_mode == "tour")
        //{
        //    ReloadPrice.text = "FREE";
        //}
        //else
        //{
        //    ReloadPrice.text = /* "?" +*/ AndroidtoUnityJSON.instance.game_fee;
        //}

        LeaderboardScreen.SetActive(true);

        LeaderboardUIManager.instance.SetLeaderboardData(isWon);
    }
    private IEnumerator GameOverPause()
    {
        yield return new WaitForSeconds(3f);
        gameEndText.text = "PAUSED";
        Time.timeScale = 0f;
    }
    public void OpponentLeftMatchGameOver()
    {
        if (!gameEndText.gameObject.activeInHierarchy)
        {
            AudioManager.instance.Play("Win");
            gameEndText.text = "You win";
            GameOver(true);
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region turn end logic

    public void EndTurn()
    {
        numberOfMovingPieces = 0;
        setKinematicForPieces(true);
        StrikerController.instance.DisableStriker();
        if (strikerInPocket || !GameOverCheck())
        {
            if (strikerInPocket && pieceHasFallen)
            {
                strikerInPocket = false;
                StartCoroutine(Penalty());
                return;
            }

            if (oneMoreChance)
            {
                // correct piece in pocket so another chance
                if (!pieceHasFallen)
                {
                    Debug.Log("No piece has fallen but still giving another turn");
                }
                Debug.Log("One more chance");
                Turn = Turn;
            }
            else
            {
                if (redPieceFallenWithoutAdditionalPiece)
                {
                    // red piece in pocket but no extra piece in pocket after that
                    Debug.Log("Starting red piece unable to fall coroutine");
                    StartCoroutine(RedPieceUnableToFall());
                    return;
                }
                else
                {
                    // turn goes to opponent player
                    Debug.Log("Opponent Turn");
                    Turn = !Turn;
                }
            }
            if (gameMode != CommonValues.GameMode.LOCAL_MULTIPLAYER || !hasBot)
            {
                if (Turn && gameStarted)
                {
                    SendMatchDataToServer();
                }
                if (playerNumberOnline == 0)
                {
                    NetworkClient.instance.SendEndTurnSignal((byte)(Turn ? 0 : 1));
                }
                else
                {
                    NetworkClient.instance.SendEndTurnSignal((byte)(Turn ? 1 : 0));
                }
            }
            else if (hasBot)
            {
                if (gameStarted)
                {
                    SendMatchDataToServer();
                }
                SetTurn(Turn);
            }
        }

    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region piece in pocket animation, penalty animation, points animation and red piece fall logic

    private IEnumerator PointScoredAnimation(int points, Vector2 pocketPos, float time, int pieceIndex)
    {
        yield return new WaitForSeconds(time);
        yield return new WaitForSeconds(0.5f);
        if (points > 0)
        {
            Vector2 textPos;
            piecesOnBoard[pieceIndex].transform.localScale = Vector3.one * piecesOnBoard[pieceIndex].scale;
            piecesOnBoard[pieceIndex].rigidbody.rotation = 0f;

            // storing the position of pocket in which the piece fell and also the piece script of the fallen piece to be used later
            if (gameMode != CommonValues.GameMode.BLACK_AND_WHITE)
            {
                oneMoreChance = true;
                if (Turn)
                {
                    textPos = scoreTexts[0].transform.position;
                    lastFallenPieces[0] = piecesOnBoard[pieceIndex];
                    lastPocket[0] = pocketPos;
                }
                else
                {
                    textPos = scoreTexts[1].transform.position;
                    lastFallenPieces[1] = piecesOnBoard[pieceIndex];
                    lastPocket[1] = pocketPos;
                }
            }
            else
            {
                if (points == 50)
                {
                    oneMoreChance = true;
                    uint playerNumber = GetPlayerNumber();
                    textPos = scoreTexts[playerNumber].transform.position;
                    lastFallenPieces[playerNumber] = piecesOnBoard[pieceIndex];
                    lastPocket[playerNumber] = pocketPos;
                }
                else if (points == (10 * pieceTargetColour) + 10)
                {

                    oneMoreChance = Turn;

                    textPos = scoreTexts[0].transform.position;
                    lastFallenPieces[0] = piecesOnBoard[pieceIndex];
                    lastPocket[0] = pocketPos;
                }
                else
                {
                    oneMoreChance = !Turn;

                    textPos = scoreTexts[1].transform.position;
                    lastFallenPieces[1] = piecesOnBoard[pieceIndex];
                    lastPocket[1] = pocketPos;
                }
            }

            // piece animation from pocket to correct player score text
            piecesOnBoard[pieceIndex].rigidbody.DOKill();
            piecesOnBoard[pieceIndex].transform.DOKill();
            piecesOnBoard[pieceIndex].pieceColourSprite.DOKill();
            piecesOnBoard[pieceIndex].transform.DOMove(textPos, 0.3f);
            piecesOnBoard[pieceIndex].transform.DOScale(piecesOnBoard[pieceIndex].transform.localScale / 3f, 0.3f);
            piecesOnBoard[pieceIndex].pieceColourSprite.DOFade(0.5f, 0.3f).SetEase(Ease.OutQuart).From(1f);

            AudioManager.instance.Play("PointScored");
            yield return new WaitForSeconds(0.3f);
            piecesOnBoard[pieceIndex].gameObject.SetActive(false);
            if (strikerInPocket)
            {
                oneMoreChance = false;
            }

        }
        if (points == 50)
        {
            AudioManager.instance.Play("QueenPocket");
        }
        else if (points == 20 || points == 10)
        {
            AudioManager.instance.Play("BlackOrWhitePocket" + Random.Range(1, 3));
        }
        // increment points
        if (Turn)
        {
            ChangeScore(0, points);
        }
        else
        {
            ChangeScore(1, points);
        }
        SetMovingPieces(false);
    }

    private IEnumerator Penalty()
    {
        // when striker in pocket, last piece pocketed by that player should come on board and player gets another chance
        yield return new WaitForSeconds(0.5f);
        uint player = GetPlayerNumber();

        if (lastFallenPieces[player] != null)
        {
            if (redPieceFallenWithoutAdditionalPiece)
            {
                redPieceFallenWithoutAdditionalPiece = false;
            }
            int points = PieceReturnAnimation(player);
            yield return new WaitForSeconds(0.5f);
            ChangeScore(player, -points);
            //oneMoreChance = true;
        }
        EndTurn();
    }


    private IEnumerator RedPieceUnableToFall()
    {
        // red piece comes on board from the pocket and turn goes to other player
        uint player = GetPlayerNumber();
        int points = PieceReturnAnimation(player);
        yield return new WaitForSeconds(0.5f);
        ChangeScore(player, -points);
        redPieceFallenWithoutAdditionalPiece = false;
        Debug.Log("Red Piece unable to fall");
        EndTurn();
    }

    private int PieceReturnAnimation(uint player)
    {
        // piece returns from pocket to center of board
        PieceScript lastPieceFallen = null;
        Vector2 lastPocketPos = Vector2.zero;
        lastPieceFallen = lastFallenPieces[player];
        lastPocketPos = lastPocket[player];
        lastPieceFallen.gameObject.SetActive(true);
        Vector2 targetPos = Vector2.zero;
        List<Collider2D> temp = new List<Collider2D>();
        float range = 0.5f;
        int tries = 0;

        // check if piece that returns to center of board collides with any other piece or not
        if (Physics2D.OverlapCircle(targetPos, lastPieceFallen.collider.radius, contactFilter, temp) > 0)
        {
            do
            {
                targetPos = Random.insideUnitCircle * range;
                tries++;
                if (tries == 5)
                {
                    range += 0.3f * lastPieceFallen.transform.localScale.x;
                }
            } while (Physics2D.OverlapCircle(targetPos, lastPieceFallen.collider.radius, contactFilter, temp) > 0);
        }
        lastPieceFallen.rigidbody.DOMove(targetPos, 0.5f).From(lastPocketPos);
        lastFallenPieces[player] = null;
        piecesFallen--;
        return lastPieceFallen.points;
    }

    private void PieceInPocket(Collider2D pocket, Vector2 velocity, CircleCollider2D piece, SpriteRenderer pieceColourSprite, Transform pieceTransform, int pieceIndex)
    {
        // move piece to pocket center based on velocity of piece
        float time = Vector3.Distance(pocket.transform.position, pieceTransform.position) / velocity.magnitude;
        time *= 5f;
        time = Mathf.Clamp(time, 0.1f, 0.8f);
        pieceTransform.DOMove(pocket.transform.position, time).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            pieceColourSprite.DOFade(0, 0.5f).From(1f).OnComplete(() =>
            {
                pieceInPocketAnim.enabled = false;
                pieceInPocketAnimObject.SetActive(false);
            });
            pieceInPocketAnim.enabled = true;
            pieceInPocketAnimObject.transform.position = pocket.transform.position;
            pieceInPocketAnimObject.SetActive(true);
        });

        // check if piece is striker or not
        int points;
        if (pieceIndex > -1)
        {
            // piece is not striker

            AudioManager.instance.Play("PieceInPocket");
            pieceHasFallen = true;
            piecesFallen++;
            points = piecesOnBoard[pieceIndex].points;
            if (redPieceFallenWithoutAdditionalPiece)
            {

                if (gameMode == CommonValues.GameMode.BLACK_AND_WHITE)
                {
                    if (points == (10 * pieceTargetColour) + 10 && Turn)
                    {
                        QueenFallenTextAnimation();
                        redPieceFallenWithoutAdditionalPiece = false;
                        queenHasFallen = true;
                    }
                    else if (points != (10 * pieceTargetColour) + 10 && !Turn)
                    {
                        QueenFallenTextAnimation();
                        redPieceFallenWithoutAdditionalPiece = false;
                        queenHasFallen = true;
                    }
                }
                else
                {
                    QueenFallenTextAnimation();
                    redPieceFallenWithoutAdditionalPiece = false;
                    queenHasFallen = true;
                }
            }
            if (points == 50)
            {
                Debug.Log("Red Piece fallen");
                redPieceFallenWithoutAdditionalPiece = true;
            }
            StartCoroutine(PointScoredAnimation(points, pocket.transform.position, time, pieceIndex));
        }
        else
        {
            // piece is striker
            AudioManager.instance.Play("StrikerInPocket");
            AudioManager.instance.Play("StrickerPocket");
            strikerInPocket = true;
            StartCoroutine(StrikerInPocketWaitForTurnEnd(time));
        }
    }

    private IEnumerator StrikerInPocketWaitForTurnEnd(float time)
    {
        yield return new WaitForSeconds(time + 0.5f);

        SetMovingPieces(false);
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region striker shot check

    private void StrikerShot()
    {
        setKinematicForPieces(false);
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region bots

    public List<Vector2> GetEnabledPiecePos()
    {
        List<Vector2> piecePos = new List<Vector2>();
        for (int i = 0; i < piecesOnBoard.Count; i++)
        {
            if (piecesOnBoard[i].gameObject.activeSelf)
            {
                piecePos.Add(piecesOnBoard[i].transform.position);
            }
        }
        return piecePos;
    }

    public List<(Vector2, int)> GetEnabledPiecePosAndColour()
    {
        List<(Vector2, int)> piecePos = new List<(Vector2, int)>();
        for (int i = 0; i < piecesOnBoard.Count; i++)
        {
            if (piecesOnBoard[i].gameObject.activeSelf)
            {
                piecePos.Add((piecesOnBoard[i].transform.position, (int)piecesOnBoard[i].Colour));
            }
        }
        return piecePos;
    }

    public float GetPieceRadius()
    {
        return piecesOnBoard[0].collider.radius;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region prompt text animations

    private void QueenFallenTextAnimation()
    {
        AudioManager.instance.Play("QueenCovered");
        promptText.fontSize = 60;
        promptText.text = "Queen Has Fallen";
        PromptTextAnimation(2.2f);
    }

    private void PromptTextAnimation(float delay)
    {
        promptBackgroundRect.gameObject.SetActive(true);
        promptText.rectTransform.anchoredPosition = Vector2.zero;
        promptBackgroundRect.DOAnchorPosX(0f, 0.3f).From(Vector3.right * (-Screen.width / 2f - promptBackgroundRect.sizeDelta.x)).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            promptText.gameObject.SetActive(true);
            promptText.transform.DOScale(1f, 0.5f).From(0.3f).SetEase(Ease.OutBack);
        });
        promptBackgroundRect.DOAnchorPosX(-Screen.width / 2f - promptBackgroundRect.sizeDelta.x, 0.2f).SetEase(Ease.InQuart).SetDelay(delay).OnComplete(() => promptBackgroundRect.gameObject.SetActive(false));
        promptText.rectTransform.DOAnchorPosX(-Screen.width / 2f - promptText.rectTransform.sizeDelta.x, 0.2f).SetEase(Ease.InQuart).SetDelay(delay).OnComplete(() => promptText.gameObject.SetActive(false));
    }

    private void LastColourPieceOnBoardTextAnimation()
    {
        promptText.fontSize = 47;
        promptText.text = "Last " + ((CommonValues.Colour)pieceTargetColour).ToString() + " piece on board, pocket the <b>QUEEN</b> first!";
        PromptTextAnimation(5f);
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region chat 

    public void ChatType(int type)
    {
        string msg = "";

        switch (type)
        {
            case 1:
                msg = "Nice Shot";
                break;

            case 2:
                msg = "Victory is mine";
                break;

            case 3:
                msg = "The Clock is TICKING!";
                break;

            case 4:
                msg = "What a strike!";
                break;

            case 5:
                msg = "Thanks";
                break;

            case 6:
                msg = "OMG";
                break;

            case 7:
                msg = "Awesome!";
                break;

            case 8:
                msg = "LOL";
                break;

            case 9:
                msg = "You are good";
                break;

            case 10:
                msg = "Sorry";
                break;

            case 11:
                msg = "Lucky!";
                break;

            case 12:
                msg = "Take your time";
                break;

            case 13:
                msg = "hehehehe";
                break;

            case 14:
                msg = "Nice try";
                break;

            case 15:
                msg = "Good Game";
                break;

            case 16:
                msg = "Well Played";
                break;

            case 17:
                msg = "OOPS!";
                break;

            case 18:
                msg = "Ouch";
                break;

            case 19:
                msg = "Close One!";
                break;

            case 20:
                msg = "Whoa!";
                break;
        }

        OnSendChat(msg);
    }

    public void OnSendChat(string msg)
    {
        //INPUTFIELD
        //if(Inputfield.text.Length > 0)
        //{
        //    NetworkClient.instance.SendChatMsg((int)playerColour, Inputfield.text);
        //    StartCoroutine(ChatDisplay((int)playerColour, Inputfield.text));
        //    Inputfield.text = "";
        //}

        //PRESET
        if (isChatEnabled == false)
        {
            MenuManager.instance.chatPopUp.gameObject.SetActive(false);
            StartCoroutine(EnableCheck());

        }
        if (isChatEnabled)
        {

            NetworkClient.instance.SendChatMsg((int)playerNumberOnline, msg);
            StartCoroutine(ChatDisplay((int)playerNumberOnline, msg));
            MenuManager.instance.chatPopUp.gameObject.SetActive(false);
        }

        IEnumerator EnableCheck()
        {
            WarningMessage.SetActive(true);
            yield return new WaitForSeconds(4f);
            WarningMessage.SetActive(false);
        }
    }

    public void ReceiveChatAndShow(int playerId, string msg)
    {
        StartCoroutine(ChatDisplay(playerId, msg));
    }

    IEnumerator ChatDisplay(int id, string msg)
    {
        GameObject bubble = null;

        if (id == 0)
            bubble = P1Chat;
        if (id == 1)
            bubble = P2Chat;

        if (bubble != null)
        {
            bubble.transform.GetChild(0).GetComponent<Text>().text = msg;
            bubble.SetActive(true);
            yield return new WaitForSeconds(5.0f);
            bubble.SetActive(false);
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------
    private void OnDestroy()
    {
        PieceScript.pieceIsMoving -= SetMovingPieces;
        PieceScript.pieceInPocket -= PieceInPocket;

        StrikerController.strikerIsMoving -= SetMovingPieces;
        StrikerController.shoot -= StrikerShot;
        StrikerController.strikerInPocket -= PieceInPocket;

        TimerScript.timerOver -= EndTurn;

        Application.logMessageReceived -= LogCallback;
    }

    public void DeductWallet()
    {


        walletUpdate.amount = AndroidtoUnityJSON.instance.game_fee;
        //  GlobalWalletBalance += int.Parse(AndroidtoUnityJSON.instance.game_fee);

        //else//-
        //{
        //    walletUpdate.amount = AndroidtoUnityJSON.instance.game_fee;
        //    GlobalWalletBalance += int.Parse(AndroidtoUnityJSON.instance.game_fee);
        //}

        walletUpdate.user_id = AndroidtoUnityJSON.instance.player_id;
        walletUpdate.game_id = AndroidtoUnityJSON.instance.game_id;
        walletUpdate.type = AndroidtoUnityJSON.instance.game_mode;

        string mydata = JsonUtility.ToJson(walletUpdate);
        WebRequestHandler.Instance.Post(walletUpdateURL, mydata, (response, status) =>
        {
            Debug.Log(response + " sent wallet update");
        });

        //check balance
        //WebRequestHandler.Instance.Post(walletInfoURL, walletInfoData, (response, status) =>
        //{
        //    WalletInfo walletInfoResponse = JsonUtility.FromJson<WalletInfo>(response);
        //    GlobalWalletBalance = int.Parse(walletInfoResponse.data.cash_balance);
        //    Debug.Log(GlobalWalletBalance + " <= updated balance");
        //});
    }
}
