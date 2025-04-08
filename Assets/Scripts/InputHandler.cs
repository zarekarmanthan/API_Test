using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class InputHandler : MonoBehaviour
{

    public event Action OnButtonClickedEvent;

    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button getMeaningBtn;

    private void Awake()
    {
        getMeaningBtn.interactable = false;

        getMeaningBtn.onClick.AddListener(OnClickGetMeaningBtn);
        inputField.onValueChanged.AddListener(InputValueChanged);
    }

    private void InputValueChanged(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Contains(" "))
            getMeaningBtn.interactable = false;
        else
            getMeaningBtn.interactable = true;
    }

    void OnClickGetMeaningBtn()
    {
        OnButtonClickedEvent?.Invoke();
    }
}
