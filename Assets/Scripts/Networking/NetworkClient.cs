using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.Linq;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class NetworkClient : SocketIOComponent
{
    #region public and private fields

    public static NetworkClient instance;
    public RejoinRoomInfo rejoinRoom = new RejoinRoomInfo();
    public MatchStartDetails matchDetails = new MatchStartDetails();
    public GameObject EndScreen;

    private StrikerInfo shootForce = new StrikerInfo();
    private MatchData matchData = new MatchData();
    private ChatBody chatBody = new ChatBody();
    private ChatBody chatBodyReceived = new ChatBody();
    private GameModeDetails mode = new GameModeDetails();
    private ScoresWithBot scores = new ScoresWithBot();
    private JSONObject json;
    private Vector2 force;
    private Vector2 position;
    private float rotation;
    private List<Vector2> piecePos = new List<Vector2>();
    private List<float> pieceRot = new List<float>();
    private bool joinedRoom = false;
    private bool needToRejoinRoom = false;
    private bool reconnecting = false;
    private bool disconnected = true;
    private bool gamePaused = false;
    public string roomID;
    private WaitForSecondsRealtime oneSec = new WaitForSecondsRealtime(1f);
    private bool isLeft = false;
    public string gameID;


    public string MatchFoundURL;
    public string botListURL;
    public BotList botList;
    public List<int> botDetailsList = new List<int>();

    public int oppPlayerId;
    public string oppPlayerName;
    public string oppPlayerDp;

    public bool noPlayer = false;

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region awake, start, connection in start, check room rejoin in start

    public override void Awake()
    {
        base.Awake();
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);*/
    }

    IEnumerator StartQuit(float sec)
    {
        yield return new WaitForSeconds(sec);
        Application.Quit();
    }

    public override void Start()
    {
        MatchFoundURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL) + MatchFoundURL;
        botListURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL) + botListURL;
        Debug.Log("android data => " +
            AndroidtoUnityJSON.instance.player_id + ", " + AndroidtoUnityJSON.instance.token + ", " + AndroidtoUnityJSON.instance.user_name + ", " +
            AndroidtoUnityJSON.instance.game_id + ", " + AndroidtoUnityJSON.instance.profile_image + ", " + AndroidtoUnityJSON.instance.game_fee + ", " +
            AndroidtoUnityJSON.instance.game_mode + ", " + AndroidtoUnityJSON.instance.battle_id + ", " + AndroidtoUnityJSON.instance.tour_id + ", " +
            AndroidtoUnityJSON.instance.tour_mode + ", " + AndroidtoUnityJSON.instance.tour_name + ", " + AndroidtoUnityJSON.instance.no_of_attempts + ", " +
            AndroidtoUnityJSON.instance.mm_player + ", " + AndroidtoUnityJSON.instance.entry_type + ", " + AndroidtoUnityJSON.instance.multiplayer_game_mode);

        WebRequestHandler.Instance.Get(botListURL, (response, status) =>
        {
            botList = JsonUtility.FromJson<BotList>(response);

            if (botList.data == null)
            {
                StartCoroutine(StartQuit(2.0f));
                noPlayer = true;
            }
            else
            {
                for (int z = 0; z < botList.data.Length; z++)
                {
                    botDetailsList.Add(z);
                }

                System.Random random = new System.Random();
                botList.data = botList.data.OrderBy(x => random.Next()).ToArray();

                Debug.Log("Bot data recvd");

                oppPlayerId = int.Parse(botList.data[0].id);
                oppPlayerName = botList.data[0].first_name;
                oppPlayerDp = botList.data[0].image;
                noPlayer = false;
            }
        });

        if (GameManager.instance.gameMode != CommonValues.GameMode.PRACTICE && GameManager.instance.gameMode != CommonValues.GameMode.LOCAL_MULTIPLAYER)
        {
            // Check if game quit or disconnected abruptly last time, in that case rejoin previous room
            int reconnect = PlayerPrefs.GetInt(CommonValues.PlayerPrefKeys.NEED_TO_RECONNECT, 0);
            needToRejoinRoom = reconnect == 1;

            // Get previously saved roomID
            roomID = PlayerPrefs.GetString(CommonValues.PlayerPrefKeys.ROOM_ID, "");
            matchDetails.roomID = roomID;
            Debug.Log("Got room ID to rejoin - " + matchDetails.roomID);
            if (matchDetails.roomID != "")
            {
                rejoinRoom.playerNumber = PlayerPrefs.GetInt(CommonValues.PlayerPrefKeys.PLAYER_NUMBER, -1);
                rejoinRoom.playerColour = PlayerPrefs.GetInt(CommonValues.PlayerPrefKeys.PLAYER_COLOUR, -1);
                if (rejoinRoom.playerColour > -1)
                {
                    GameManager.instance.pieceTargetColour = rejoinRoom.playerColour;
                }
                int gameMode = PlayerPrefs.GetInt(CommonValues.PlayerPrefKeys.GAME_MODE, -1);
                if (gameMode > -1)
                {
                    if ((CommonValues.GameMode)gameMode != GameManager.instance.gameMode)
                    {
                        needToRejoinRoom = false;
                        matchDetails.roomID = "";
                        roomID = "";
                        ResetPlayerPrefs();
                    }
                }
                if (needToRejoinRoom)
                {
                    // Show loading screen while socket rejoins to old room and game syncs to other players
                    MenuManager.instance.StartLoadingScreen();
                    joinedRoom = true;
                }
            }

            base.Start();
            SetupEvents();
        }
    }

    public void SetRoomDetailsFromMainApp()
    {
        // private room ID goes here
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region socket io receive events

    private void SetupEvents()
    {
        On("open", (E) =>
        {
            // Socket connected to node js

            // Join random room when no room is given from main app
            disconnected = false;
            reconnecting = false;
            Debug.Log("Connected to server");
            if (!joinedRoom && !disconnected)
            {
                // Join a room for respective game mode in node js server
                joinedRoom = true;
                Debug.Log("Joining Room");
                mode.Mode = (int)GameManager.instance.gameMode - 1;
                mode.Bot = GameManager.instance.hasBot;
                mode.BotType = (int)GameManager.instance.botType;
                mode.playerDp = AndroidtoUnityJSON.instance.profile_image;
                mode.playerName = AndroidtoUnityJSON.instance.user_name;
                mode.playerId = int.Parse(AndroidtoUnityJSON.instance.player_id);
                json = new JSONObject(JsonUtility.ToJson(this.mode));
                Emit("JoinRoom", json);
            }
            else
            {
                Debug.Log("Reconnected = " + reconnecting);
                CheckRoomRejoin();
            }

            // Join particular room here in case of private room when room is provided by main app
            
        });

        On("OpponentStrikerPositionChanged", (E) =>
        {
            StrikerController.instance.sliders[1].value=(1f - (float.Parse(E.data.list[0].ToString())/10000f));
        });

        On("ReceiveEmoji", (E) =>
        {
            Debug.Log("Emoji received - " + E.data.list[0].ToString());
            MenuManager.instance.ReceiveEmojis((int)byte.Parse(E.data.list[0].ToString()));
        });

        On("StrikerShoot", (E) =>
        {
            // Make striker position, rotation, force same as opponent
            Debug.Log("Got opponent striker force from server " + E.data.list);
            force.x = float.Parse(E.data["xMagnitude"].ToString());
            force.y = float.Parse(E.data["yMagnitude"].ToString());
            position.x = float.Parse(E.data["xPos"].ToString());
            position.y = float.Parse(E.data["yPos"].ToString());
            rotation = float.Parse(E.data["zRot"].ToString());
            Debug.Log(force + "," + position);

            StrikerController.instance.StrikerShootFromServer(force, position, rotation);
        });

        On("OnChat", (E) =>
        {
            //Receive chat msg text and sender player id

            Debug.Log("Chat recieved from server " + E.data);

            chatBodyReceived = JsonUtility.FromJson<ChatBody>(E.data.ToString());
            GameManager.instance.ReceiveChatAndShow(chatBodyReceived.playerColorId, chatBodyReceived.message);
        });

        On("StartGame", (E) =>
        {
            Debug.Log("Server signal to start game " + E.data);

            matchDetails = JsonUtility.FromJson<MatchStartDetails>(E.data.ToString());
            matchDetails.firstTurn = bool.Parse(E.data["turn"].ToString());
            matchDetails.initialStart = bool.Parse(E.data["initialStart"].ToString());
            matchDetails.randomSeed = byte.Parse(E.data["randomSeed"].ToString());
            roomID = E.data["room"].ToString();
            roomID = roomID.Remove(0, 1);
            roomID = roomID.Remove(roomID.Length - 1, 1);
            matchDetails.roomID = roomID;

            var botindex = 0;

            if (matchDetails.playerName[0] == AndroidtoUnityJSON.instance.user_name)
            {
                botindex = 1;
            }

            if (matchDetails.playerId[botindex] == 0)
            {
                matchDetails.playerId[botindex] = oppPlayerId;
                matchDetails.playerName[botindex] = oppPlayerName;
                matchDetails.playerDp[botindex] = oppPlayerDp;
            }

            GameManager.instance.hasBot = bool.Parse(E.data["hasBot"].ToString());
            GameManager.instance.enabled = true;
            
            MatchMakingUIManager.instance.Matched(matchDetails.firstTurn,matchDetails.initialStart, (int)matchDetails.randomSeed);
            StartCoroutine(MatchFound());
        });

        /*On("UpdateTimer", (E) =>
        {
            Debug.Log("Server signal to update Timer " + E.data);

            TimerScript.instance.TimerUpdateFromServer(int.Parse(E.data.list[0].ToString()));
        });*/

       
        On("BlackAndWhiteModeStartGame", (E) =>
        {
            Debug.Log("Server signal to start game for Black and white mode  " + E.data);
            GameManager.instance.StartGameForBlackAndWhiteMode(bool.Parse(E.data.list[0].ToString()));
            //TimerScript.instance.TimerEndedFromServer();
        });

        On("GeneratePieces", (E) =>
        {
            Debug.Log("Server signal to generate pieces for Black and white mode " + E.data);
            GameManager.instance.GeneratePiecesForBlackAndWhiteMode(bool.Parse(E.data.list[0].ToString()));
        });

        On("TurnEnded", (E) =>
        {
            Debug.Log("Server signal to end Turn ");

            GameManager.instance.SetTurn(GameManager.instance.Turn);
        });

        On("RoomNotFound", (E) =>
        {
            // Previous room that the player wants to rejoin is deleted in the server and no longer present

            Debug.Log("Room " + roomID + " not found");
            ResetPlayerPrefs();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });

        On("ResumeGame", (E) =>
        {
            // Resume game for rejoined player

            Debug.Log("Player can resume game");

            // Set turn
            GameManager.instance.PlayerRejoinSetTurn(int.Parse(E.data.list[0].ToString()) == GameManager.instance.playerNumberOnline);
            Debug.Log("Current turn = " + E.data.list[0].ToString());

            MenuManager.instance.DisableLoadingScreen();

            // Increment turn and continue game
            GameManager.instance.ResumeGame(GameManager.instance.Turn, bool.Parse(E.data.list[1].ToString()));
            
        });

        On("SetScore", (E) =>
        {
            Debug.Log("Scores from server - " + E.data.ToString());
            GameManager.instance.SetScoreFromServer(int.Parse(E.data.list[0].ToString()), int.Parse(E.data.list[1].ToString()));
        });

        On("PieceInfo", (E) =>
        {
            Debug.Log("Server send piece positions and rotations" + E.data);
            piecePos.Clear();
            pieceRot.Clear();
            List<bool> _piecesEnabled = new List<bool>();
            List<Vector2> _lastPocketPos = new List<Vector2>();
            for (int i = 0; i < E.data.list[0].Count; i++)
            {
                piecePos.Add(new Vector2(float.Parse(E.data.list[0].list[i][0].ToString()), float.Parse(E.data.list[0].list[i][1].ToString())));
                pieceRot.Add(float.Parse(E.data.list[0].list[i][2].ToString()));
                _piecesEnabled.Add(bool.Parse(E.data.list[0].list[i][3].ToString()));
            }
            matchData.queenHasFallen = bool.Parse(E.data.list[1].ToString());
            matchData.numPiecesFallen = byte.Parse(E.data.list[2].ToString());
            matchData.redPieceFallenWithoutAdditionalPieces = bool.Parse(E.data.list[6].ToString());
            matchData.playerNumber = bool.Parse(E.data.list[4].ToString());
            if ((matchData.playerNumber && GameManager.instance.playerNumberOnline == 0) || (!matchData.playerNumber && GameManager.instance.playerNumberOnline == 1))
            {
                matchData.lastPiecesFallen[0] = byte.Parse(E.data.list[3].list[0].ToString());
                matchData.lastPiecesFallen[1] = byte.Parse(E.data.list[3].list[1].ToString());
            }
            else
            {
                matchData.lastPiecesFallen[0] = byte.Parse(E.data.list[3].list[1].ToString());
                matchData.lastPiecesFallen[1] = byte.Parse(E.data.list[3].list[0].ToString());
            }

            for (int i = 0; i < 2; i++)
            {
                matchData.lastPocketPos[i].xPos = float.Parse(E.data.list[5].list[i][0].ToString());
                matchData.lastPocketPos[i].yPos = float.Parse(E.data.list[5].list[i][1].ToString());
                _lastPocketPos.Add(new Vector2(matchData.lastPocketPos[i].xPos, matchData.lastPocketPos[i].yPos));
            }
            GameManager.instance.SetMatchData(piecePos, pieceRot, matchData.queenHasFallen, matchData.numPiecesFallen, _piecesEnabled.ToArray(), matchData.lastPiecesFallen, _lastPocketPos.ToArray(), matchData.redPieceFallenWithoutAdditionalPieces);
        });

        On("PlayerDisconnected", (E) =>
        {
            Debug.Log("Player won because other player Disconnected ");

            //GameManager.instance.PlayerDisconnectedGameOver();
        });

        On("OpponentLeftMatch", (E) =>
        {
            Debug.Log("Player won because other player left match ");

            GameManager.instance.OpponentLeftMatchGameOver();
        });

        On("SetSeed", (E) =>
        {
            Debug.Log("Setting seed from Server -> " + E.data["randomSeed"].ToString());
            matchDetails.randomSeed = byte.Parse(E.data["randomSeed"].ToString());
            UnityEngine.Random.InitState(matchDetails.randomSeed);
        });

        On("disconnect", (E) =>
        {
            // Player disconnected from the server due to connection loss/ quit game/ leave match

            Debug.Log("You are disconnected");
            disconnected = true;
            if (!gamePaused && matchDetails.roomID != "")
            {
                if (!GameManager.instance.gameOver)
                {
                    // If game is ongoing save room and player details
                    if (matchDetails.roomID != "")
                    {
                        SetPlayerPrefs();
                        MenuManager.instance.StartLoadingScreen();
                        needToRejoinRoom = true;
                        reconnecting = true;
                        Debug.Log("Reconnecting.........");

                        // Try to reconnect to server
                        StartCoroutine(ReconnectingCoroutine());
                    }
                }
                else
                {
                    // If game not started yet or ended, close game
                    if (Time.realtimeSinceStartup >= 1f)
                    {
                        ResetPlayerPrefs();
                    }
#if UNITY_EDITOR

                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
            }
        });
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region emit data/events to server

    private void CheckRoomRejoin()
    {
        if (needToRejoinRoom && !disconnected)
        {
            // Rejoin the previous room

            needToRejoinRoom = false;
            joinedRoom = true;
            Debug.Log("Trying to rejoin previous room...");
            rejoinRoom.roomID = matchDetails.roomID;
            rejoinRoom.Mode = (int)GameManager.instance.gameMode - 1;
            //rejoinRoom.playerId = int.Parse(AndroidtoUnityJSON.instance.player_id);
            //rejoinRoom.playerName = AndroidtoUnityJSON.instance.user_name;
            //rejoinRoom.playerDP = AndroidtoUnityJSON.instance.profile_image;
            json = new JSONObject(JsonUtility.ToJson(this.rejoinRoom));
            Emit("RejoinRoom", json);
        }
    }

    public void SendEmoji(byte emojiNumber)
    {
        json = new JSONObject(emojiNumber);
        Emit("SendEmoji", json);
    }
    private IEnumerator ReconnectingCoroutine()
    {
        while (reconnecting)
        {
            Connect();
            yield return oneSec;
        }
    }

    public void SendStrikerPositionChangeSignal(float sliderValue)
    {
        json = new JSONObject((ushort)(sliderValue*10000f));
        Emit("PlayerStrikerPositionChanged", json);
    }

    public void SendStrikerShootSignal(Vector2 force, Vector2 position, float rotation)
    {
        Debug.Log("Sending player striker shoot force to server");
        this.shootForce.xMagnitude = (force.x);// * 1000) / 1000;
        this.shootForce.yMagnitude = (force.y);// * 1000) / 1000;
        this.shootForce.xPos = (position.x);// * 1000) / 1000;  
        this.shootForce.yPos = (position.y);// * 1000) / 1000;
        this.shootForce.zRot = (rotation);// * 1000) / 1000;

        var mag = new Vector2(this.shootForce.xMagnitude, this.shootForce.yMagnitude);
        var pos = new Vector2(this.shootForce.xPos, this.shootForce.yPos);

        //StrikerController.instance.StrikerShootFromServer(mag, pos, this.shootForce.zRot);

        json = new JSONObject(JsonUtility.ToJson(this.shootForce));
        Emit("PlayerStrikerShoot", json);
    }

    public void SetTurnOnServer()
    {
        Emit("SetTurn");
    }
    public void SendMatchData(List<Vector2> pos, List<float> rot,bool queenHasFallen, byte numPiecesFallen,bool[] piecesFallen, byte[] lastPiecesFallen, Vector2[] lastPocketPos,bool playerNumber, bool redPieceFallenWithoutOtherPiece)
    {
        List<Vector2> vectors = new List<Vector2>();
        List<float> rotations = new List<float>();

        matchData.pieces.Clear();
        for(int i = 0; i < pos.Count;i++)
        {
            matchData.pieces.Add(new PieceInfo());
            matchData.pieces[i].xPos = (pos[i].x);// * 1000) / 1000;
            matchData.pieces[i].yPos = (pos[i].y);// * 1000) / 1000;
            matchData.pieces[i].zRot = (rot[i]);// * 1000) / 1000;

            vectors.Add(new Vector2(matchData.pieces[i].xPos, matchData.pieces[i].yPos));
            rotations.Add(matchData.pieces[i].zRot);

            matchData.pieces[i].enabled = piecesFallen[i];
        }
        matchData.queenHasFallen = queenHasFallen;
        matchData.numPiecesFallen = numPiecesFallen;
        matchData.lastPiecesFallen = lastPiecesFallen;
        for(int i = 0; i < 2; i++)
        {
            matchData.lastPocketPos[i].xPos = lastPocketPos[i].x;
            matchData.lastPocketPos[i].yPos = lastPocketPos[i].y;
        }
        matchData.playerNumber = playerNumber;
        matchData.redPieceFallenWithoutAdditionalPieces = redPieceFallenWithoutOtherPiece;
        json = new JSONObject(JsonUtility.ToJson(this.matchData));
        GameManager.instance.SetPieceMag(vectors, rotations, piecesFallen);
        Emit("PieceInfo", json);
    }

    public void SendScoresNoBot(byte player1score)
    {
        json = new JSONObject(player1score);
        Emit("SetScoresNoBot", json);
    }

    public void SendScoresBot(byte player1score, byte botscore)
    {
        scores.playerScore = player1score;
        scores.botScore = botscore;
        json = new JSONObject(JsonUtility.ToJson(this.scores));
        Emit("SetScoresBot", json);
    }

    public void SendPlayerWin()
    {
        Emit("PlayerWon");
    }

    public void SendPlayerLose()
    {
        Emit("PlayerLost");
    }

    public void SendPlayerDraw()
    {
        Emit("PlayerDraw");
    }

    public void SendPlayerPieceColour(int colour)
    {
        json = new JSONObject(colour);
        Emit("PlayerSelectedPieceColour", json);
    }

    public void SendEndTurnSignal(byte turn)
    {
        Debug.Log("Sending signal to end turn");
        json = new JSONObject(turn);
        Emit("TurnEnd", json);
    }

    public void SendEmojiSignal(byte emojiNumber)
    {
        json = new JSONObject(emojiNumber);
        Emit("SendEmoji", json);
    }

    public void SendChatMsg(int playerId, string msg)
    {
        if (!disconnected)
        {
            Debug.Log("Sent chat msg: " + msg);
            chatBody.playerColorId = playerId;
            chatBody.message = msg;
            json = new JSONObject(JsonUtility.ToJson(chatBody));
            Emit("OnChat", json);
        }
    }

    public void LeaveMatch()
    {
        ResetPlayerPrefs();
        isLeft = true;
        if (!disconnected)
        {
            // Player clicked on leave match button

            Emit("LeaveMatch");
        }
        needToRejoinRoom = false;

#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator MatchFound()
    {
        WWWForm ww = new WWWForm();
        if (NetworkClient.instance.matchDetails.playerId[0] == int.Parse(AndroidtoUnityJSON.instance.player_id))
            ww.AddField("player_id", matchDetails.playerId[1].ToString());
        else
            ww.AddField("player_id", matchDetails.playerId[0].ToString());
        ww.AddField("room_id", roomID);
        ww.AddField("game_mode", AndroidtoUnityJSON.instance.game_mode);
        ww.AddField("winning_details", GameManager.instance.GetScore(0));
        ww.AddField("winning_score", GameManager.instance.GetScore(0));
        ww.AddField("game_end_time", GetSystemTime());
        ww.AddField("wallet_amt", AndroidtoUnityJSON.instance.game_fee);
        ww.AddField("game_id", AndroidtoUnityJSON.instance.game_id);
        if (AndroidtoUnityJSON.instance.game_mode == "tour")
            ww.AddField("battle_tournament_id", AndroidtoUnityJSON.instance.tour_id);
        else if (AndroidtoUnityJSON.instance.game_mode == "battle")
            ww.AddField("battle_tournament_id", AndroidtoUnityJSON.instance.battle_id);
        using (UnityWebRequest updateUserHistory = UnityWebRequest.Post(MatchFoundURL, ww))
        {
            updateUserHistory.SetRequestHeader("token", AndroidtoUnityJSON.instance.token);
            updateUserHistory.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return updateUserHistory.SendWebRequest();

            if (updateUserHistory.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(updateUserHistory.error);
            }
            else
            {
                JSONObject response = new JSONObject(updateUserHistory.downloadHandler.text);

                var myJsonString = (response["data"]);
                var id = myJsonString["id"].ToString();
                gameID = id;
                Debug.Log("Match URL data upload complete! " );
            }
        }
    }

    public string GetSystemTime()
    {
        int hr = DateTime.Now.Hour;
        int min = DateTime.Now.Minute;
        int sec = DateTime.Now.Second;

        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;

        string format = string.Format("{0}:{1}:{2} {3}:{4}:{5}", year, month, day, hr, min, sec);

        return format;
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region set and reset player prefs, application quit/pause

    private void OnApplicationPause(bool pause)
    {
#if !UNITY_EDITOR
        if (pause)
        {   
            DataSaver.saveData(GameManager.instance.logs, "BountyBunchLudo_savelog");
            if(!gamePaused)
            {
                Debug.Log("You are disconnected");
                gamePaused = true;
                
                base.OnApplicationQuit();
                if (!GameManager.instance.gameOver)
                {
                    if (matchDetails.roomID != "")
                    {
                        //GameManager.instance.DestroyPiecesOnBoard();
                        //StrikerController.instance.DisableStriker();
                        //TimerScript.instance.StopTimer();
                        SetPlayerPrefs();
                        MenuManager.instance.StartLoadingScreen();
                        needToRejoinRoom = true;                        
                        Application.Quit();
                    }
                }
            }
            else
            {
                ResetPlayerPrefs();
                Application.Quit();
            }
        }
        else
        {
            if(gamePaused)
            {
                gamePaused = false;
            }
        }
#endif
    }

    private new void OnApplicationQuit()
    {
        //Hit API
        EndScreen.SetActive(true);
        if (!LeaderboardUIManager.instance.isDataSend && joinedRoom)
        {
            if (matchDetails.playerId[0] == int.Parse(AndroidtoUnityJSON.instance.player_id))
                LeaderboardUIManager.instance.sendThisPlayerData.player_id = matchDetails.playerId[1].ToString();
            else
                LeaderboardUIManager.instance.sendThisPlayerData.player_id = matchDetails.playerId[0].ToString();

            LeaderboardUIManager.instance.sendThisPlayerData.winning_details.thisplayerScore = 0;
            LeaderboardUIManager.instance.sendThisPlayerData.wallet_amt = AndroidtoUnityJSON.instance.game_fee;
            LeaderboardUIManager.instance.sendThisPlayerData.game_mode = AndroidtoUnityJSON.instance.game_mode;
            LeaderboardUIManager.instance.sendThisPlayerData.game_id = AndroidtoUnityJSON.instance.game_id;

            if (AndroidtoUnityJSON.instance.game_mode == "tour")
                LeaderboardUIManager.instance.sendThisPlayerData.battle_tournament_id = AndroidtoUnityJSON.instance.tour_id;
            else if (AndroidtoUnityJSON.instance.game_mode == "battle")
                LeaderboardUIManager.instance.sendThisPlayerData.battle_tournament_id = AndroidtoUnityJSON.instance.battle_id;

            LeaderboardUIManager.instance.sendThisPlayerData.id = gameID;
            LeaderboardUIManager.instance.sendThisPlayerData.game_end_time = LeaderboardUIManager.instance.GetSystemTime();
            LeaderboardUIManager.instance.sendThisPlayerData.game_status = "LEFT";

            string sendWinningDetailsData = JsonUtility.ToJson(LeaderboardUIManager.instance.winningDetails);
            string sendNewData1 = JsonUtility.ToJson(LeaderboardUIManager.instance.sendThisPlayerData);
            WebRequestHandler.Instance.Post(LeaderboardUIManager.instance.sendDataURL, sendNewData1, (response, status) =>
            {
                Debug.Log(response + "HitNewApi");
            });

            LeaderboardUIManager.instance.isDataSend = true;
        }
    

        // Save logs at persistent data path in android
        DataSaver.saveData(GameManager.instance.logs, "BountyBunchCarrom_savelog");

        if (!disconnected && GameManager.instance)
        {
            // For socket disconnecting
            base.OnApplicationQuit();
        }

        // Save room and player details
        if(!isLeft)
            SetPlayerPrefs();
    }

    private void SetPlayerPrefs()
    {
        if (GameManager.instance.gameStarted && !GameManager.instance.gameOver && GameManager.instance.gameMode != CommonValues.GameMode.LOCAL_MULTIPLAYER)
        {
            Debug.Log("Setting room ID to rejoin -" + matchDetails.roomID);
            PlayerPrefs.SetString(CommonValues.PlayerPrefKeys.ROOM_ID, roomID);
            PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.GAME_MODE, (int)GameManager.instance.gameMode);
            PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.NEED_TO_RECONNECT, 1);
            PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.PLAYER_COLOUR, (int)GameManager.instance.pieceTargetColour);
            PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.PLAYER_NUMBER, (int)GameManager.instance.playerNumberOnline);
            PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.COLOURS_FLIPPED, GameManager.instance.coloursFlipped? 1:0);
            PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.HAS_BOT, GameManager.instance.hasBot ? 1 : 0);
            //PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.OPPONENT_MOBILE_NUMBER,MatchMakingUIManager.instance.opponentID );
            PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.RANDOM_SEED, matchDetails.randomSeed);
        }
        else
        {
            if (Time.realtimeSinceStartup >= 2f)
            {
                Debug.Log("Real time since startup = " + Time.realtimeSinceStartup);
                ResetPlayerPrefs();
            }
        }
    }

    public void ResetPlayerPrefs()
    {
        Debug.Log("Resetting player prefs");
        PlayerPrefs.SetString(CommonValues.PlayerPrefKeys.ROOM_ID, "");
        PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.GAME_MODE, -1);
        PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.NEED_TO_RECONNECT, 0);
        PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.PLAYER_COLOUR, -1);
        PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.PLAYER_NUMBER, -1);
        PlayerPrefs.SetInt(CommonValues.PlayerPrefKeys.RANDOM_SEED, -1);
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------
}

#region serialized classes used to send data to and fro nodejs server via socketio

[System.Serializable]
public class ChatBody
{
    public int playerColorId;
    public string message;
}

[System.Serializable]
public class RejoinRoomInfo
{
    public string roomID = "";
    public int Mode;
    public int playerColour;
    public int playerNumber;
    public int playerId;
    public string playerName;
    public string playerDP;
}

[System.Serializable]
public class StrikerInfo
{
    public float xMagnitude;
    public float yMagnitude;
    public float xPos;
    public float yPos;
    public float zRot;
}

[System.Serializable]
public class PieceInfo
{
    public float xPos;
    public float yPos;
    public float zRot;
    public bool enabled;
}

[System.Serializable]
public class PocketPos
{
    public float xPos;
    public float yPos;
}

[System.Serializable]
public class MatchData
{
    public List<PieceInfo> pieces = new List<PieceInfo>();
    public bool queenHasFallen;
    public byte numPiecesFallen;
    public byte[] lastPiecesFallen = new byte[2];
    public bool playerNumber;
    public PocketPos[] lastPocketPos = new PocketPos[2] { new PocketPos(), new PocketPos() };
    public bool redPieceFallenWithoutAdditionalPieces;
}


[System.Serializable]
public class GameModeDetails
{
    public int Mode;
    public bool Bot;
    public int BotType;
    public int playerId;
    public string playerName;
    public string playerDp;
}

[System.Serializable]
public class MatchStartDetails
{
    public bool firstTurn;
    public bool initialStart;
    public byte randomSeed;
    public string roomID;
    public int[] playerId;
    public string[] playerName;
    public string[] playerDp;
}

[System.Serializable]
public class ScoresWithBot
{
    public byte playerScore = 0;
    public byte botScore = 0;
}


#endregion

//--------------------------------------------------------------------------------------------------------------------------------------