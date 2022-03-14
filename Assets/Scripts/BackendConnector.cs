using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class BackendConnector : MonoBehaviour
{
    private string URI = "http://172.20.128.71:5000";

    public GameObject HighscorePanel;

    public void AddPlayer(string playerName, string deviceId)
    {
        StartCoroutine(CreateNewPlayer(playerName, deviceId));
    }

    public void setHighscore(string deviceId, string highscore)
    {
        StartCoroutine(setNewHighscore(deviceId, highscore));
    }

    public void getHighscores(GameObject parentObject)
    {
        StartCoroutine(getHighscoresIE(parentObject));
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

    private IEnumerator getHighscoresIE(GameObject parentObject)
    {
        Debug.Log("SetUP");
        using (UnityWebRequest webRequest =
               UnityWebRequest.Get(URI + "/getHighscores"))
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
                    String[] highscoreInfo = webRequest.downloadHandler.text.Split(',');
                    Highscore[] highscores = new Highscore[highscoreInfo.Length / 2];
                    int currentHighscore = 0;
                    for (int i = 0; i < highscoreInfo.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            highscoreInfo[i] = highscoreInfo[i].Replace("[", "");
                            highscores[currentHighscore] = new Highscore();
                            highscores[currentHighscore].playerName = highscoreInfo[i];
                        }
                        else
                        {
                            highscoreInfo[i] = highscoreInfo[i].Replace("]", "");
                            Debug.Log(highscoreInfo[i]);
                            highscores[currentHighscore].highscore = int.Parse(highscoreInfo[i]);
                            currentHighscore++;
                        }
                    }

                    Debug.Log(highscores[0].highscore);
                    foreach (var x in highscores)
                    {
                        GameObject currentPanel = Instantiate(HighscorePanel, parentObject.transform.position,
                            parentObject.transform.rotation, parentObject.transform);
                        GameObject text = currentPanel.transform.GetChild(0).gameObject;
                        text.GetComponent<TextMeshProUGUI>().text  = " " + x.playerName + "      highscore: " + x.highscore;
                        text.SetActive(false);
                    }

                    break;
            }
        }
    }
}