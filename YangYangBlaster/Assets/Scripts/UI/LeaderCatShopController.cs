using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderCatShopController : MonoBehaviour
{
    public Text powerLevelText;
    public Text powerText;
    public Text powerCoinText;

    public Text attackSpeedLevelText;
    public Text attackSpeedText;
    public Text attackSpeedCoinText;

    public Text criticalLevelText;
    public Text criticalText;
    public Text criticalCoinText;

    public Text skillLevelText;
    public Text skillText;
    public Text skillCoinText;

    public Text freeCoinLevelText;
    public Text freeCoinText;
    public Text freeCoinCoinText;

    public void SetLeaderCatShop()
    {
        powerLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.powerLevel.ToString());
        powerText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeDamage().ToString());
        powerCoinText.text = GameDataManager.Instance.GetPlayerUpgradeDamagePrice().ToString();

        attackSpeedLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.attackSpeedLevel.ToString());
        attackSpeedText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeAttackSpeed().ToString());
        attackSpeedCoinText.text = GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice().ToString();

        criticalLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.criticalLevel.ToString());
        criticalText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeCritical().ToString());
        criticalCoinText.text = GameDataManager.Instance.GetPlayerUpgradeCriticalPrice().ToString();

        skillLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.buffDurationLevel.ToString());
        skillText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeBuffSKill().ToString());
        skillCoinText.text = GameDataManager.Instance.GetPlayerUpgradeMilkSkillPrice().ToString();

        freeCoinLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.freeCoinLevel.ToString());
        freeCoinText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeFreeCoin().ToString());
        freeCoinCoinText.text = GameDataManager.Instance.GetPlayerUpgradeFreeCoinPrice().ToString();
    }

    public void QuitButton()
    {
        gameObject.SetActive(false);
    }

    public void PowerButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeDamagePrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeDamage(GameDataManager.Instance.GetPlayerUpgradeDamagePrice());

        powerLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.powerLevel.ToString());
        powerText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeDamage().ToString());
        powerCoinText.text = GameDataManager.Instance.GetPlayerUpgradeDamagePrice().ToString();

        UIManager.Instance.lobbyUI.UpdateCoinText();
    }

    public void AttackSpeedButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeAttackSpeed(GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice());

        attackSpeedLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.attackSpeedLevel.ToString());
        attackSpeedText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeAttackSpeed().ToString());
        attackSpeedCoinText.text = GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice().ToString();

        UIManager.Instance.lobbyUI.UpdateCoinText();
    }

    public void CriticalButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeCriticalPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeCritical(GameDataManager.Instance.GetPlayerUpgradeCriticalPrice());

        criticalLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.criticalLevel.ToString());
        criticalText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeCritical().ToString());
        criticalCoinText.text = GameDataManager.Instance.GetPlayerUpgradeCriticalPrice().ToString();

        UIManager.Instance.lobbyUI.UpdateCoinText();
    }

    public void SkillButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeMilkSkillPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeSkillDamage(GameDataManager.Instance.GetPlayerUpgradeMilkSkillPrice());

        skillLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.buffDurationLevel.ToString());
        skillText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeBuffSKill().ToString());
        skillCoinText.text = GameDataManager.Instance.GetPlayerUpgradeMilkSkillPrice().ToString();

        UIManager.Instance.lobbyUI.UpdateCoinText();
    }

    public void FreeCoinButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeFreeCoinPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeFreeCoin(GameDataManager.Instance.GetPlayerUpgradeFreeCoinPrice());

        freeCoinLevelText.text = string.Format("Lv.{0}", GameDataManager.Instance.userData.upgradePlayer.freeCoinLevel.ToString());
        freeCoinText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeFreeCoin().ToString());
        freeCoinCoinText.text = GameDataManager.Instance.GetPlayerUpgradeFreeCoinPrice().ToString();

        UIManager.Instance.lobbyUI.UpdateCoinText();
    }
}
