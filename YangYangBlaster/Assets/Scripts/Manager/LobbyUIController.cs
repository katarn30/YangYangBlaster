using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    public ShopController shopController;
    public DeckUIController deckUIController;

    public void OnInitialized()
    {
        deckUIController.SetDeckUI();
    }

    public void HeroButton()
    {

    }

    public void FriendButton()
    {
        if (shopController.gameObject.activeInHierarchy == false)
        {
            shopController.gameObject.SetActive(true);
        }

        shopController.SetCatShopList();
    }

    public void MilkSkillButton()
    {

    }

    public void ShopButton()
    {

    }

    public void LoginButton()
    {
        LoginManager.Instance.GoogleLogin();
    }

    public void GameStartButton()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.InGame);
    }
}
