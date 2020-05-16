using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class UIManager : SingleTon<UIManager>
{
    [Header("Create Prefab")]
    public GameObject createlobbyUI;
    public GameObject createIngameUI;

    public GameObject lobbyUI;
    public InGameUIController inGameUI;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetLobbyUI()
    {
        if (createlobbyUI == null)
        {
            Debug.LogError("UIManager CreateIngameUI is Null !!");
            return;
        }

        if (lobbyUI == null)
        {
            GameObject go = Instantiate(createIngameUI, transform);
            lobbyUI = go;
        }

        if (inGameUI != null)
        {
            inGameUI.gameObject.SetActive(false);
        }

        lobbyUI.SetActive(true);
        
    }

    public void SetInGameUI()
    {        
        if (createIngameUI == null)
        {
            Debug.LogError("UIManager CreateIngameUI is Null !!");
            return;
        }

        if (inGameUI == null)
        {
            GameObject go = Instantiate(createIngameUI, transform);
            inGameUI = go.GetComponent<InGameUIController>();
        }

        if (lobbyUI != null)
        {
            lobbyUI.SetActive(false);
        }

        inGameUI.gameObject.SetActive(true);
        inGameUI.OnInitialized();
    }    
}
