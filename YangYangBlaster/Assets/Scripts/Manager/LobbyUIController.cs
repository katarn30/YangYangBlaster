using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    public ShopController shopController;

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

    public void GameStartButton()
    {
        GameManager.Instance.ChangeGameState(GameManager.GameState.InGame);
    }
}
