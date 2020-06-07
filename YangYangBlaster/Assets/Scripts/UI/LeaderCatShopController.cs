using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderCatShopController : MonoBehaviour
{
    public Text powerText;
    public Text powerCoinText;

    public Text attackSpeedText;
    public Text attackSpeedCoinText;

    public Text criticalText;
    public Text criticalCoinText;

    public Text skillText;
    public Text skillCoinText;

    public Text freeCoinText;
    public Text freeCoinCoinText;

    public void SetLeaderCatShop()
    {
        powerText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeDamage().ToString());
        powerCoinText.text = GameDataManager.Instance.GetPlayerUpgradeDamagePrice().ToString();

        attackSpeedText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeAttackSpeed().ToString());
        attackSpeedCoinText.text = GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice().ToString();

        criticalText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeCritical().ToString());
        criticalCoinText.text = GameDataManager.Instance.GetPlayerUpgradeCriticalPrice().ToString();

        skillText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeSkill().ToString());
        skillCoinText.text = GameDataManager.Instance.GetPlayerUpgradeSkillPrice().ToString();

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

        powerText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeDamage().ToString());
        powerCoinText.text = GameDataManager.Instance.GetPlayerUpgradeDamagePrice().ToString();
    }

    public void AttackSpeedButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeAttackSpeed(GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice());

        attackSpeedText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeAttackSpeed().ToString());
        attackSpeedCoinText.text = GameDataManager.Instance.GetPlayerUpgradeAttackSpeedPrice().ToString();
    }

    public void CriticalButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeCriticalPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeCritical(GameDataManager.Instance.GetPlayerUpgradeCriticalPrice());

        criticalText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeCritical().ToString());
        criticalCoinText.text = GameDataManager.Instance.GetPlayerUpgradeCriticalPrice().ToString();
    }

    public void SkillButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeSkillPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeSkillDamage(GameDataManager.Instance.GetPlayerUpgradeSkillPrice());

        skillText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeSkill().ToString());
        skillCoinText.text = GameDataManager.Instance.GetPlayerUpgradeSkillPrice().ToString();
    }

    public void FreeCoinButton()
    {
        if (GameDataManager.Instance.isUpgrade(GameDataManager.Instance.GetPlayerUpgradeFreeCoinPrice()) == false)
            return;

        GameDataManager.Instance.SetUpgradeFreeCoin(GameDataManager.Instance.GetPlayerUpgradeFreeCoinPrice());

        freeCoinText.text = string.Format("{0}%", GameDataManager.Instance.GetPlayerUpgradeFreeCoin().ToString());
        freeCoinCoinText.text = GameDataManager.Instance.GetPlayerUpgradeFreeCoinPrice().ToString();
    }
}
