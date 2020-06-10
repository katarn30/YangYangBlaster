using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InGameUIController : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Text coinText;
    public Text stageText;
    public Text nextStageText;
    public Image stageGauge;

    public void OnInitialized()
    {
        text.gameObject.SetActive(false);

        SetCoinUI();
        StageUI();
        
        if (GameManager.Instance.isBossStage() == true)
        {
            SetBossStageGaugeUI();
        }
        else
        {
            SetStageGaugeUI();
        }
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

    public void SetCoinUI()
    {
        coinText.text = GameDataManager.Instance.userData.coin.ToString();
    }

    public void StageUI()
    {
        stageText.text = GameDataManager.Instance.userData.stageNum.ToString();
        nextStageText.text = (GameDataManager.Instance.userData.stageNum + 1).ToString();
    }

    public void SetStageGaugeUI()
    {
        stageGauge.fillAmount = (float)MonsterManager.Instance.nowMonsterCount / (float)MonsterManager.Instance.monsterStageCount;
    }

    public void SetBossStageGaugeUI()
    {
        stageGauge.fillAmount = 1;
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
