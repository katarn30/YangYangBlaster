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

    public LobbyUIController lobbyUI;
    public InGameUIController inGameUI;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void LobbyUIUpdate()
    {
        lobbyUI.LobbyUIUpdate();
    }

    public void InGameUIUpdate()
    {
        
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
            GameObject go = Instantiate(createlobbyUI, transform);
            lobbyUI = go.GetComponent<LobbyUIController>();
        }

        if (inGameUI != null)
        {
            inGameUI.gameObject.SetActive(false);
        }

        lobbyUI.gameObject.SetActive(true);
        lobbyUI.OnInitialized();

        SetMainCanvasScale(lobbyUI.GetComponent<CanvasScaler>());
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
            lobbyUI.gameObject.SetActive(false);
        }

        inGameUI.gameObject.SetActive(true);
        inGameUI.OnInitialized();

        SetMainCanvasScale(inGameUI.GetComponent<CanvasScaler>());
    }

    public void SetMainCanvasScale(CanvasScaler _canvasScaler)
    {
        float mobile_w = Screen.width - 720;
        float mobile_h = Screen.height - 1280;

        Debug.Log(mobile_w + " : " + mobile_h);

        if (mobile_w <= 0 && mobile_h <= 0)
        {
            Debug.Log("Normal");
            _canvasScaler.matchWidthOrHeight = 0.5f;          
        }
        else
        {
            if (mobile_w > mobile_h)
            {
                Debug.Log("hight");
                //가로가 길어짐
                _canvasScaler.matchWidthOrHeight = 1;
            }
            else
            {
                Debug.Log("width");
                //세로가 길어짐
                _canvasScaler.matchWidthOrHeight = 0f;
            }
        }
    }
}
