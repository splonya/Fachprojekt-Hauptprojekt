using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreenText : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text nameInputField = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";

    public void HandleDisplayNameChanged(string oldValue, string newValue) 
    {
        nameInputField.text = $"The winner is {newValue}";
    }
}
