using System.Collections;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System;

public class API_Handler : MonoBehaviour
{
    const string baseUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";  // Base Url for the APi

   
    public event Action<JSONNode> OnUpdateWordData; // Event for fetching the data from the API response
    public event Action<AudioClip> OnPlayWordAudio; // Event fot fecthing the audio url from the APi response
    public event Action<string> OnError; // Event for showing the error message when the APi fails

   

    #region API Coroutine Functions

    /// <summary>
    /// Fetching the word data for word defintion and word example
    /// </summary>
    /// <param name="userInput"></param>
    /// <returns></returns>
    IEnumerator FetchWordMeaning(string userInput)
    {
        string url = baseUrl + userInput.ToLower(); // Adding the user input to the base url to get the required data

        UnityWebRequest wordDataRequest = UnityWebRequest.Get(url); // Unity Get Request

        yield return wordDataRequest.SendWebRequest(); 

        if (wordDataRequest.result == UnityWebRequest.Result.Success)
        {
            JSONNode data = JSON.Parse(wordDataRequest.downloadHandler.text); // Parsing the API response 

            OnUpdateWordData?.Invoke(data); // Triggering the event to Update the UI data
             
            StartCoroutine(PlayAudio(data)); // Coroutine to fetch the word audio from the url
        }
        else
        {
            OnError?.Invoke(wordDataRequest.error); // Triggring the Error event to display the erro message
        }

    }


    /// <summary>
    /// This funcitons gets the audio for the word from the API response
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    IEnumerator PlayAudio(JSONNode data)
    {
        string audioUrl = data[0]["phonetics"][0]["audio"]; // get the first audio url

        if (string.IsNullOrEmpty(audioUrl))
            audioUrl = data[0]["phonetics"][1]["audio"]; // get the second audio url if the 1stis empty 

        // Unity Web request to get the audio 
        UnityWebRequest wordAudioRequest = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG);

        yield return wordAudioRequest.SendWebRequest();

        if (wordAudioRequest.result == UnityWebRequest.Result.Success)
        {
            // save the downloaded data into an audio clip
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(wordAudioRequest);

            OnPlayWordAudio?.Invoke(audioClip); // Triggering the event to play the word audio
        }
        else
            yield return null;
    } 

    #endregion


    public void OnGetWordClick(string inputText)
    {
        StartCoroutine(FetchWordMeaning(inputText));
    }
}
