using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class Initial : MonoBehaviour
{
    public string fetchAPIDomainURL;

    void Start()
    {
        StartCoroutine(FetchDomains());
    }

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
                adminURL = aPIDomains["admin"];
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
