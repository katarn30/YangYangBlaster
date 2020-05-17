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

    public void OnInitialized()
    {
        text.gameObject.SetActive(false);                

        SetScoreUI();
        SetStageUI();
    }

    public void SetScoreUI()
    {
        scoreText.text = string.Format("Score : {0}", GameDataManager.Instance.userData.score);
    }

    public void SetStageUI()
    {
        stageText.text = string.Format("Stage {0}", GameDataManager.Instance.userData.stageNum);
    }

    public void GameOverUI()
    {
        text.text = "GAME OVER";
        text.gameObject.SetActive(true);
    }

    public void StageClearUI()
    {
        text.text = "STAGE CLEAR !!";
        text.gameObject.SetActive(true);
    }

    public void ContinueButton()
    {
        GameManager.Instance.SetInGame();
    }

    public void BuyMercenaryButton()
    {
        if (GameDataManager.Instance.userData.score >= 1200)
        {
            GameManager.Instance.UpdateScore(-1200);
            
        }        
    }

    public void ExitLobby()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.Lobby);
    }
}
