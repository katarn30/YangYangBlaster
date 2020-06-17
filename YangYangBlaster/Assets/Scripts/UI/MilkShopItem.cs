using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MilkShopItem : MonoBehaviour
{
    [Header("MilkShopItem")]
    public Image milkImage;
    public Text milkNameText;
    public Text milkLevelText;

    [Header("Info")]
    public GameObject infoGo;
    public Text infoNameText;
    public Text infoLevelText;
    public Text infoSummaryText;

    public void SetMilkShopItem(MilkItem _item)
    {
        milkImage.sprite = _item.milkSprite;
        milkNameText.text = string.Format("{0} MILK", _item.type);
        milkLevelText.text = string.Format("LEVEL.{0}", _item.milkLevel);
        
        infoNameText.text = string.Format("{0} MILK", _item.type);
        infoLevelText.text = string.Format("LEVEL.{0}", _item.milkLevel);
        infoSummaryText.text = _item.milkInfo;
    }

    public void InfoButton()
    {
        if (infoGo.activeInHierarchy == true)
        {
            infoGo.SetActive(false);
        }
        else
        {
            infoGo.SetActive(true);
        }
    }
}
