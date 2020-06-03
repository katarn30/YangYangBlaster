using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [Header("Center Popup")]
    public LeaderCatShopController leaderCatShopController;
    public ShopController shopController;
    public DeckUIController deckUIController;

    [Header("TOP UI")]
    public Text scoreText;
    public Text coinText;

    [Header("CENTER UI")]
    public Image freeCoinGauge;
    public Text freeCoinText;
    public Text stageText;

    public void OnInitialized()
    {
        deckUIController.SetDeckUI();
        deckUIController.SetStageGauge();

        UpdateScoreText();
        UpdateCoinText();
        UpdateFreeCoinText();
        UpdateStageText();
    }

    public void LobbyUIUpdate()
    {
        TimeSpan sp = GameDataManager.Instance.userData.freeCoinUpdateTime - GameDataManager.Instance.userData.freeCoinGetTime;

        TimeSpan nsp = GameDataManager.Instance.userData.freeCoinUpdateTime - DateTime.Now;

        freeCoinGauge.fillAmount = (float)(nsp.TotalSeconds / sp.TotalSeconds);
    }

    public void AllClosePopup()
    {
        leaderCatShopController.QuitButton();
        shopController.QuitButton();
    }

    public void HeroButton()
    {
        AllClosePopup();

        if (leaderCatShopController.gameObject.activeInHierarchy == false)
        {
            leaderCatShopController.gameObject.SetActive(true);
        }
    }

    public void FriendButton()
    {
        AllClosePopup();

        if (shopController.gameObject.activeInHierarchy == false)
        {
            shopController.gameObject.SetActive(true);
        }

        shopController.SetCatShopList();
    }

    public void MilkSkillButton()
    {
        AllClosePopup();
    }

    public void ShopButton()
    {
        AllClosePopup();
    }

    public void LoginButton()
    {
        LoginManager.Instance.GoogleLogin();
    }

    public void GameStartButton()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.InGame);
    }

    public void UpdateScoreText()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score Text is Null");
            return;
        }

        scoreText.text = GameDataManager.Instance.userData.score.ToString();
    }

    public void UpdateCoinText()
    {
        if (coinText == null)
        {
            Debug.LogError("Coint Text is Null");
            return;
        }

        coinText.text = GameDataManager.Instance.userData.coin.ToString();
    }

    public void UpdateFreeCoinText()
    {
        freeCoinText.text = GameDataManager.Instance.freeCoin.ToString();
    }

    public void UpdateStageText()
    {
        stageText.text = string.Format("STAGE {0}", GameDataManager.Instance.userData.stageNum.ToString());
    }

    public void FreeCoinButton()
    {
        TimeSpan sp = GameDataManager.Instance.userData.freeCoinUpdateTime - DateTime.Now;

        if (sp.TotalSeconds <= 0)
        {
            GameDataManager.Instance.userData.coin = GameDataManager.Instance.userData.coin + GameDataManager.Instance.freeCoin;
            GameDataManager.Instance.SetFreeCoinInfo();

            UpdateFreeCoinText();
            UpdateCoinText();
        }
    }

    public void ShowRewardButton()
    {
        GoogleAdmobManager.Instance.ShowReward();
    }
}
