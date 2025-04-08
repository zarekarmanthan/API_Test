using System.Collections;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System;

public class API_Handler : MonoBehaviour
{
    const string baseUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";

   
    public event Action<JSONNode> OnUpdateWordData;
    public event Action<AudioClip> OnPlayWordAudio;
    public event Action<string> OnError;

   

    #region API Coroutine Functions
    IEnumerator FetchWordMeaning(string userInput)
    {
        string url = baseUrl + userInput.ToLower();

        UnityWebRequest wordDataRequest = UnityWebRequest.Get(url);

        yield return wordDataRequest.SendWebRequest();

        if (wordDataRequest.result == UnityWebRequest.Result.Success)
        {
            JSONNode data = JSON.Parse(wordDataRequest.downloadHandler.text);

            OnUpdateWordData?.Invoke(data);

            StartCoroutine(PlayAudio(data));
        }
        else
        {
            OnError?.Invoke(wordDataRequest.error);
        }

    }
    IEnumerator PlayAudio(JSONNode data)
    {
        string audioUrl = data[0]["phonetics"][0]["audio"];

        if (string.IsNullOrEmpty(audioUrl))
            audioUrl = data[0]["phonetics"][1]["audio"];


        UnityWebRequest wordAudioRequest = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG);

        yield return wordAudioRequest.SendWebRequest();

        if (wordAudioRequest.result == UnityWebRequest.Result.Success)
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(wordAudioRequest);

            OnPlayWordAudio?.Invoke(audioClip);
        }
    } 

    #endregion


    public void OnGetWordClick(string inputText)
    {
        StartCoroutine(FetchWordMeaning(inputText));
    }
}
