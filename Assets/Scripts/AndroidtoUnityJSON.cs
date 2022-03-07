using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AndroidtoUnityJSON: MonoBehaviour
{
    public static AndroidtoUnityJSON instance;

    public Text data;

    public string player_id;
    public string token;
    public string game_fee;
    public string profile_image;
    public string user_name;
    public string battle_id;
    public string game_id;
    public string game_mode;
    public string multiplayer_game_mode;

    private void Awake()
    {
        instance = this;

#if (UNITY_EDITOR)

        player_id = "32323";
        token = "l9452bgQ3jYbKgXwnReONMvaK";
        game_fee = "20";
        profile_image = "https://picsum.photos/400";
        user_name = "John Doe";
        battle_id = "32";
        game_id = "32";
        game_mode = "battle";
        multiplayer_game_mode = "freestyle";
#endif

        getIntentData();
    }

    private bool getIntentData()
    {
#if (!UNITY_EDITOR && UNITY_ANDROID)
    return CreatePushClass (new AndroidJavaClass ("com.unity3d.player.UnityPlayer"));
#endif
        return false;
    }

    public bool CreatePushClass(AndroidJavaClass UnityPlayer)
    {
#if UNITY_ANDROID
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        AndroidJavaObject extras = GetExtras(intent);

        if (extras != null)
        {
            //{ 
            //    "player_id": '22',
            //    "token": 'skldnsklds',
            //    "user_name": 'JohnDoe',
            //    "game_id": '33',
            //    "battle_id": '555',
            //    "profile_image": 'https://picsum.photos/400',
            //    "wallet_amount": '333'
            //}

            string jsonString = GetProperty(extras, "data");
            
            data.text = "Base app data error!";

            player_id = GetProperty(extras, "player_id");
            token = GetProperty(extras, "token");
            user_name = GetProperty(extras, "user_name");
            game_id = GetProperty(extras, "game_id");
            battle_id = GetProperty(extras, "battle_id");
            profile_image = GetProperty(extras, "game_fee");
            game_fee = GetProperty(extras, "wallet_amount");
            game_mode = GetProperty(extras, "game_mode");
            multiplayer_game_mode = GetProperty(extras, "multiplayer_game_mode");

            data.text = "";            

            return true;
        }
#endif
        return false;
    }


    #region HELPER CLASSES FOR JAVA

    private AndroidJavaObject GetExtras(AndroidJavaObject intent)
    {
        AndroidJavaObject extras = null;

        try
        {
            extras = intent.Call<AndroidJavaObject>("getExtras");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        return extras;
    }

    private string GetProperty(AndroidJavaObject extras, string name)
    {
        string s = string.Empty;

        try
        {
            s = extras.Call<string>("getString", name);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        return s;
    }

    #endregion


}
