using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackendConnector : MonoBehaviour
{
    private string URI = "http://172.20.128.59:5000";

    public void AddPlayer(string playerName, string deviceId)
    {
        StartCoroutine(CreateNewPlayer(playerName, deviceId));
    }
    
    public void setHighscore(string deviceId, string highscore)
    {
        StartCoroutine(setNewHighscore(deviceId, highscore));
    }

    private IEnumerator CreateNewPlayer(string playerName, string deviceId)
    {
        Debug.Log("SetUP");
        using (UnityWebRequest webRequest =
               UnityWebRequest.Get(URI + "/createNewPlayer/" + playerName + "/" + deviceId))
        {
            yield return webRequest.SendWebRequest();


            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
    
    private IEnumerator setNewHighscore(string deviceId, string highscore)
    {
        Debug.Log("SetUP");
        using (UnityWebRequest webRequest =
               UnityWebRequest.Get(URI + "/setNewHighscore/" + deviceId + "/" + highscore))
        {
            yield return webRequest.SendWebRequest();


            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}