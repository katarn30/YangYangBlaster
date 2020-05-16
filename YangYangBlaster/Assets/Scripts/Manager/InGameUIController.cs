using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InGameUIController : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI stageText;

    public GameObject changeCatButton;
    public GameObject buyMercenaryButton;

    public void OnInitialized()
    {
        text.gameObject.SetActive(false);
        changeCatButton.SetActive(false);
        buyMercenaryButton.SetActive(false);

        SetScoreUI();
        SetStageUI();
    }

    public void SetScoreUI()
    {
        scoreText.text = string.Format("Score : {0}", GameManager.Instance.gameScore);
    }

    public void SetStageUI()
    {
        stageText.text = string.Format("Stage {0}", GameManager.Instance.gameStage);
    }

    public void GameOverUI()
    {
        text.text = "GAME OVER";
        text.gameObject.SetActive(true);
        changeCatButton.SetActive(true);
        buyMercenaryButton.SetActive(true);
    }

    public void StageClearUI()
    {
        text.text = "STAGE CLEAR !!";
        text.gameObject.SetActive(true);
        changeCatButton.SetActive(true);
        buyMercenaryButton.SetActive(true);
    }

    public void ContinueButton()
    {
        GameManager.Instance.SetInGame();
    }

    public void ChangeCatButton()
    {
        PlayerManager.Instance.ChangeCat();
    }

    public void BuyMercenaryButton()
    {
        if (GameManager.Instance.gameScore >= 1200)
        {
            GameManager.Instance.gameScore = GameManager.Instance.gameScore - 1200;
            PlayerManager.Instance.ActiveMercenary();
        }        
    }
}
