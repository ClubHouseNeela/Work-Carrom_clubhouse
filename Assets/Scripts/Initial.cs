using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;
using UnityEngine.Networking;

public class Initial : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] float _gameFee;

    public string fetchAPIDomainURL;

    void Start()
    {
        SetGameFee();
        if (_gameFee == 0)
        {
            InitializeAds();
        }
        else
        {
            StartCoroutine(FetchDomains());
        }
    }

    void SetGameFee()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
        AndroidJavaObject extras = intent.Call<AndroidJavaObject>("getExtras");
        _gameFee = float.Parse(extras.Call<string>("getString", "game_fee"));
#endif
    }

    void InitializeAds()
    {
        Advertisement.Initialize((Application.platform == RuntimePlatform.IPhonePlayer)
            ? "5148278"
            : "5148279", false, this);
    }

    #region Interface Implementations
    public void OnInitializationComplete()
    {
        Debug.Log("Advertisments Init Success");
        Advertisement.Load((Application.platform == RuntimePlatform.IPhonePlayer)
            ? "Interstitial_iOS"
            : "Interstitial_Android", this);
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Init Failed: [{error}]: {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Load Success: {placementId}");
        Advertisement.Show((Application.platform == RuntimePlatform.IPhonePlayer)
            ? "Interstitial_iOS"
            : "Interstitial_Android", this);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Load Failed: [{error}:{placementId}] {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"OnUnityAdsShowFailure: [{error}]: {message}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log($"OnUnityAdsShowStart: {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"OnUnityAdsShowClick: {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"OnUnityAdsShowComplete: [{showCompletionState}]: {placementId}");
        StartCoroutine(FetchDomains());
    }
    #endregion

    private IEnumerator FetchDomains()
    {
        string singlePlayerURL = PlayerPrefs.GetString(Constants.FETCH_SERVER_URL, "");
        string adminURL = PlayerPrefs.GetString(Constants.FETCH_ADMIN_URL, "");
        if (singlePlayerURL == "" || adminURL == "")
        {
            UnityWebRequest request = UnityWebRequest.Get(fetchAPIDomainURL);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(fetchAPIDomainURL + " web request error in Get method with responce code : " + request.responseCode);
            }
            else
            {
                var response = request.downloadHandler.text;
                Debug.Log(fetchAPIDomainURL + "response" + response);
                SimpleJSON.JSONNode aPIDomains = SimpleJSON.JSONNode.Parse(SimpleJSON.JSONNode.Parse(response)["dev"].ToString());
                singlePlayerURL = aPIDomains["multiPalyerCarrom"];
                adminURL = aPIDomains["admin"] + "/";
                PlayerPrefs.SetString(Constants.FETCH_SERVER_URL, singlePlayerURL);
                PlayerPrefs.SetString(Constants.FETCH_ADMIN_URL, adminURL);
            }
            request.Dispose();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
