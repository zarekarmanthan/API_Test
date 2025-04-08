using SimpleJSON;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Variables 

    [Header("UI References")]
    [SerializeField] TMP_InputField textInput; // Inputfield for user input
    [SerializeField] TextMeshProUGUI definition; // UI for displaying word definition
    [SerializeField] TextMeshProUGUI example; // UI for displaying word example

    [Space]
    [SerializeField] GameObject errorPanel; // UI panel to show API error
    [SerializeField] TextMeshProUGUI errorMsgText; // UI to display error message

    [Space]
    [SerializeField] GameObject loadingPanel; // UI panel to show loading screen

    [Space]
    [Header("Script References")]
    [SerializeField] API_Handler apiHandler; // Script refrence for API_handler
    [SerializeField] InputHandler inputHandler; // script reference for InputHandler

    [Space]
    AudioSource audioSource; // AudioSource refrence to play the audio

    [Space]
    [SerializeField] Animator animator; // Animator reference for playing the animation

    #endregion


    private void Awake()
    {
        // Subscribe to the API events
        apiHandler.OnUpdateWordData += UpdateWordUI;
        apiHandler.OnPlayWordAudio += PlayAudio;
        apiHandler.OnError += DisplayError;

        // Subscribe to the Input Handler events
        inputHandler.OnButtonClickedEvent += GetwordMeaning;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // getting the audio source compenent of this gameobject
    }


    #region Event Action Functions
    /// <summary>
    /// This function updates the data in the UI received from the API response
    /// </summary>
    /// <param name="data"></param>
    private void UpdateWordUI(JSONNode data)
    {
        definition.text = "<b>Definition : </b>" + data[0]["meanings"][0]["definitions"][0]["definition"];
        example.text = "<b>Example : </b>" + GetExample(data);

        loadingPanel.SetActive(false); // disabling the loading panel

    }

    /// <summary>
    /// Displays the error message from the API response
    /// </summary>
    /// <param name="error"></param>
    private void DisplayError(string error)
    {
        loadingPanel.SetActive(false); // disabling the loading panel

        errorPanel.SetActive(true); // Enabling the error panel 
        errorMsgText.text = error;
    }

    /// <summary>
    /// Plays the audio clip downloaded from the Api
    /// </summary>
    /// <param name="clip"></param>
    private void PlayAudio(AudioClip clip)
    {
        audioSource.volume = 0.8f; // sets the volume of the audio
        audioSource.clip = clip; // add the audio clip in the audio source component

        StartCoroutine(PlayAnimation(clip)); 
    }

    // This function is called when the user click the Get Meaning Button
    private void GetwordMeaning()
    {
        loadingPanel.SetActive(true); // Enabling the loading panel
        apiHandler.OnGetWordClick(textInput.text);
    }

    #endregion

    /// <summary>
    /// Handling the audio and animation play
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    private IEnumerator PlayAnimation(AudioClip clip)
    {
        animator.SetBool("isTalking", true); // starting the talking animation

        yield return null; // wait for next frame

        audioSource.Play(); // plays the audio

        // wait until the audio is playing 
        while (audioSource.isPlaying)
        {
            yield return null; 
        }

        animator.SetBool("isTalking", false); // stop the talking animation 
    }


    /// <summary>
    /// Checks through the Api data and returns the example string value which is not null
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    string GetExample(JSONNode node)
    {
        var meanings = node[0]["meanings"];

        foreach (JSONNode meaning in meanings)
        {
            var definitions = meaning["definitions"].AsArray;

            // Loop through all definitions in the current meaning
            foreach (JSONNode definition in definitions.AsArray)
            {
                string example = definition["example"];
                if (!string.IsNullOrEmpty(example))  // if the example value is not null then return
                {
                    return example;
                }
            }
        }

        return "No available example."; // No example values present in the API response data
    }

    private void OnDestroy()
    {
        // Un-subscribing to all the events which are subscibed
        apiHandler.OnUpdateWordData -= UpdateWordUI;
        apiHandler.OnPlayWordAudio -= PlayAudio;
        apiHandler.OnError -= DisplayError;

        inputHandler.OnButtonClickedEvent -= GetwordMeaning;
    }

   
}
