using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkShopController : MonoBehaviour
{
    [Header("Create Prefab")]
    public MilkShopItem milkItemPrefab;

    public Transform prefabParent;
    public List<MilkShopItem> milkShopItemList = new List<MilkShopItem>();

    public void SetMilkShopList()
    {
        if (milkShopItemList.Count == 0)
        {
            for (int i = 0; i < GameDataManager.Instance.userData.milkItemList.Count; i++)
            {
                MilkShopItem ms = Instantiate(milkItemPrefab, prefabParent);

                ms.SetMilkShopItem(GameDataManager.Instance.userData.milkItemList[i]);

                milkShopItemList.Add(ms);
            }
        }
        else
        {
            RefreshMilkShopList();
        }
    }

    public void RefreshMilkShopList()
    {
        for (int i = 0; i < milkShopItemList.Count; i++)
        {
            milkShopItemList[i].SetMilkShopItem(GameDataManager.Instance.userData.milkItemList[i]);
        }
    }

    public void QuitButton()
    {
        gameObject.SetActive(false);
    }
}
