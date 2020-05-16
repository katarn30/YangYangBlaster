using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{    
    public GameObject lobbyUI;
    public InGameUIController inGameUI;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetLobbyUI()
    {
        lobbyUI.SetActive(true);
        inGameUI.gameObject.SetActive(false);
    }

    public void SetInGameUI()
    {        
        lobbyUI.SetActive(false);
        inGameUI.gameObject.SetActive(true);
        inGameUI.OnInitialized();
    }    
}
