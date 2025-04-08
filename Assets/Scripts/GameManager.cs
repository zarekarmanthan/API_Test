using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region Variables 

    [Header("UI References")]
    [SerializeField] TMP_InputField textInput;
    [SerializeField] TextMeshProUGUI definition;
    [SerializeField] TextMeshProUGUI example;

    [Space]
    [SerializeField] GameObject errorPanel;
    [SerializeField] TextMeshProUGUI errorMsgText;

    [Space]
    [Header("Script References")]
    [SerializeField] API_Handler apiHandler;
    [SerializeField] InputHandler inputHandler;

    [Space]
    AudioSource audioSource;

    [Space]
    [SerializeField] Animator animator; 

    #endregion


    private void Awake()
    {
        apiHandler.OnUpdateWordData += UpdateWordUI;
        apiHandler.OnPlayWordAudio += PlayAudio;
        apiHandler.OnError += DisplayError;

        inputHandler.OnButtonClickedEvent += GetwordMeaning;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    #region Event Action Functions
    private void UpdateWordUI(JSONNode data)
    {
        definition.text = "<b>Definition : </b>" + data[0]["meanings"][0]["definitions"][0]["definition"];
        example.text = "<b>Example : </b>" + data[0]["meanings"][0]["definitions"][0]["example"];
    }
    private void DisplayError(string error)
    {
        errorPanel.SetActive(true);
        errorMsgText.text = error;
    }
    private void PlayAudio(AudioClip clip)
    {
        audioSource.volume = 0.8f;
        audioSource.clip = clip;
        StartCoroutine(PlayAnimation(clip));
    }
    private void GetwordMeaning()
    {
        apiHandler.OnGetWordClick(textInput.text);
    }

    #endregion


    private IEnumerator PlayAnimation(AudioClip clip)
    {
        animator.SetBool("isTalking", true);

        yield return null;

        audioSource.Play();

        while (audioSource.isPlaying)
        {
            yield return null; 
        }

        animator.SetBool("isTalking", false);
    }


    private void OnDestroy()
    {
        apiHandler.OnUpdateWordData -= UpdateWordUI;
        apiHandler.OnPlayWordAudio -= PlayAudio;
        apiHandler.OnError -= DisplayError;

        inputHandler.OnButtonClickedEvent -= GetwordMeaning;
    }


}
