using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InputHandler : MonoBehaviour
{

    public event Action OnButtonClickedEvent; // Event for button click

    [SerializeField] TMP_InputField inputField; // InputField reference for user to enter text
    [SerializeField] Button getMeaningBtn; // Refrence for the button to get the word data

    private void Awake()
    {
        getMeaningBtn.interactable = false; // No interaction at the start when the inputfield is empty

        getMeaningBtn.onClick.AddListener(OnClickGetMeaningBtn); 
        inputField.onValueChanged.AddListener(InputValueChanged);
    }

    /// <summary>
    /// this function will get called everytime the user makes changes in the input field
    /// </summary>
    /// <param name="input"></param>
    private void InputValueChanged(string input)
    {
        // If the input contains any spaces it will disable the interaction the button 
        if (string.IsNullOrWhiteSpace(input) || input.Contains(" "))
            getMeaningBtn.interactable = false;
        else
            getMeaningBtn.interactable = true;
    }

    void OnClickGetMeaningBtn()
    {
        // Triggering the event when the button is clicked to fetch the word data from API
        OnButtonClickedEvent?.Invoke(); 
    }
}
