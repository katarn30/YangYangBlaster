using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCatItem : MonoBehaviour
{
    public int index;
    public Text catName;
    public Image catImage;
    public Text levelText;
    public Text priceText;
    public GameObject selectEffect;
    public List<GameObject> buttonList = new List<GameObject>();

    MercenaryData mercenaryData;

    public void SetCatItem(int _index, MercenaryData _mercenaryData)
    {
        index = _index;
        mercenaryData = _mercenaryData;

        catName.text = mercenaryData.name;
        catImage.sprite = mercenaryData.catImage;
        levelText.text = string.Format("Lv.{0}", mercenaryData.level);
        priceText.text = string.Format("{0} {1}", mercenaryData.price, "K");

        if (_mercenaryData.mercenaryGetType == MercenaryGetType.Video)
        {
            buttonList[0].gameObject.SetActive(false);
            buttonList[1].gameObject.SetActive(false);
            buttonList[2].gameObject.SetActive(true);
        }
        else
        {
            buttonList[0].gameObject.SetActive(true);
            buttonList[1].gameObject.SetActive(false);
            buttonList[2].gameObject.SetActive(false);
        }
    }

    public void RefreshCatItem()
    {
        SetCatItem(index, mercenaryData);
    }

    public void SelectButton()
    {
        selectEffect.gameObject.SetActive(!selectEffect.gameObject.activeSelf);
    }

    public void UpgrageButton()
    {
        GameManager.Instance.BuyMercenary(index);
    }

    public void VideoButton()
    {

    }
}
