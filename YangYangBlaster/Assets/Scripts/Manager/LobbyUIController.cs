using System.Collections;
using System.Collections.Generic;
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

    public void OnInitialized()
    {
        deckUIController.SetDeckUI();

        UpdateScoreText();
        UpdateCoinText();
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

    public void FreeCoinButton()
    {

    }
}
