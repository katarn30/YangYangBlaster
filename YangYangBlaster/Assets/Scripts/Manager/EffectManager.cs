using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingleTon<EffectManager>
{
    public Transform parent;

    [Header("Bubble Effect")]
    public BubbleEffect bubblePrefab;
    public List<BubbleEffect> bubbleEffectList = new List<BubbleEffect>();
    public int bubbleEffectMaxCount = 0;
    public int activeBubbleNum = 0;

    [Header("Coin Effect")]
    public CoinEffect coinPrefab;
    public List<CoinEffect> coinEffectList = new List<CoinEffect>();
    public int coinEffectMaxCount = 0;
    public int activeCoinNum = 0;

    [Header("Milk Effect")]
    public MilkEffect milkPrefab;
    public List<MilkEffect> milkEffectList = new List<MilkEffect>();
    public int milkEffectMaxCount = 0;
    public int activeMilkNum = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetLobbyInit()
    {
        if (parent != null)
        {
            parent.gameObject.SetActive(false);
        }
    }

    public void SetInGameInit()
    {
        if (parent == null)
        {
            parent = new GameObject().transform;
            parent.transform.parent = transform;
            parent.name = "Effect Parent";

            CreateBubbleEffect();
            CreateCoinEffect();
            CreateMilkEffect();
        }
        else
        {
            parent.gameObject.SetActive(true);
        }

        activeBubbleNum = 0;
        activeCoinNum = 0;
    }

    #region Bubble
    public void CreateBubbleEffect()
    {
        for (int i = 0; i < bubbleEffectMaxCount; i++)
        {
            BubbleEffect bubble = Instantiate(bubblePrefab, parent);

            bubble.gameObject.SetActive(false);

            bubbleEffectList.Add(bubble);
        }
    }

    public void SetBubbleEffect(Vector2 _pos, Vector2 _scale, Color _color)
    {
        activeBubbleNum = activeBubbleNum + 1;

        if (activeBubbleNum >= bubbleEffectList.Count)
        {
            activeBubbleNum = 0;
        }

        bubbleEffectList[activeBubbleNum].gameObject.SetActive(true);
        bubbleEffectList[activeBubbleNum].SetBubbleEffect(_pos, _scale, _color);
    }
    #endregion

    #region Coin

    public void CreateCoinEffect()
    {
        for (int i = 0; i < coinEffectMaxCount; i++)
        {
            CoinEffect coin = Instantiate(coinPrefab, parent);

            coin.gameObject.SetActive(false);

            coinEffectList.Add(coin);
        }
    }

    public void SetCoinEffect(Vector2 _pos)
    {
        int ran = Random.Range(1, 6);

        for (int i = 0; i < ran; i ++)
        {
            activeCoinNum = activeCoinNum + 1;

            if (activeCoinNum >= coinEffectList.Count)
            {
                activeCoinNum = 0;
            }
            if (coinEffectList[activeCoinNum].gameObject.activeInHierarchy == true)
            {
                GameManager.Instance.GetCoin(100);
            }
            else
            {
                coinEffectList[activeCoinNum].gameObject.SetActive(true);
            }
            
            coinEffectList[activeCoinNum].SetCoinEffect(_pos);
        }        
    }
    #endregion

    #region Milk
    public void CreateMilkEffect()
    {
        for (int i = 0; i < coinEffectMaxCount; i++)
        {
            MilkEffect milk = Instantiate(milkPrefab, parent);

            milk.gameObject.SetActive(false);

            milkEffectList.Add(milk);
        }
    }

    public void SetMilkEffect(Vector2 _pos)
    {
        int dropRan = Random.Range(0,10);

        if (dropRan <= 0)
        {
            int ran = Random.Range(0, GameDataManager.Instance.userData.milkItemList.Count);

            Debug.Log(ran);

            MilkItem item = GameDataManager.Instance.userData.milkItemList[ran];

            activeMilkNum = activeMilkNum + 1;

            if (activeMilkNum >= milkEffectList.Count)
            {
                activeMilkNum = 0;
            }
            if (milkEffectList[activeMilkNum].gameObject.activeInHierarchy == false)
            {
                milkEffectList[activeMilkNum].gameObject.SetActive(true);
                milkEffectList[activeMilkNum].SetMilkEffect(item, _pos);
            }
        }        
    }
    #endregion
}
