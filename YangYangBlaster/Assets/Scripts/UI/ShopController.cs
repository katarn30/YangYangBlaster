using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [Header("Create Prefab")]
    public GameObject catItemPrefab;

    public Transform prefabParent;
    public List<ShopCatItem> shopCatItemList = new List<ShopCatItem>();

    public void SetCatShopList()
    {
        if (shopCatItemList.Count == 0)
        {
            for (int i = 0; i < GameDataManager.Instance.ReadMercenaryDataList.Count; i++)
            {
                GameObject go = Instantiate(catItemPrefab.gameObject, prefabParent);

                ShopCatItem sc = go.GetComponent<ShopCatItem>();

                if (GameDataManager.Instance.isGetMercenaryCat(GameDataManager.Instance.ReadMercenaryDataList[i].name) == true)
                {                    
                    sc.SetCatItem(i, GameDataManager.Instance.GetMyMercenaryData(GameDataManager.Instance.ReadMercenaryDataList[i].name));
                }
                else
                {
                    sc.SetCatItem(i, GameDataManager.Instance.ReadMercenaryDataList[i]);
                }

                shopCatItemList.Add(sc);
            }
        }
        else
        {
            RefreshCatShopList();
        }
    }

    public void RefreshCatShopList()
    {
        for (int i = 0; i < shopCatItemList.Count; i++)
        {
            if (GameDataManager.Instance.isGetMercenaryCat(shopCatItemList[i].catName.text) == true)
            {                
                shopCatItemList[i].SetCatItem(i, GameDataManager.Instance.GetMyMercenaryData(shopCatItemList[i].catName.text));
            }
            else
            {
                shopCatItemList[i].RefreshCatItem();
            }
        }

    }

    public void QuitButton()
    {
        gameObject.SetActive(false);
    }
}
