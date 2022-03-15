using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private Button quitButton;

    private void Start()
    {
        AddListeners();
    }
    private void AddListeners()
    {
        quitButton.onClick.AddListener(() => { OnQuitButtonClicked(); });
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
