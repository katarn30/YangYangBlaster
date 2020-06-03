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
        }
        else
        {
            parent.gameObject.SetActive(true);
        }

        activeBubbleNum = 0;
        activeCoinNum = 0;
    }

    public void CreateBubbleEffect()
    {
        for (int i = 0; i < bubbleEffectMaxCount; i++)
        {
            BubbleEffect bubble = Instantiate(bubblePrefab, parent);

            bubble.gameObject.SetActive(false);

            bubbleEffectList.Add(bubble);
        }
    }

    public void SetBubbleEffect(Vector2 _pos, Vector2 _scale, int _sortOrder, Color _color)
    {
        activeBubbleNum = activeBubbleNum + 1;

        if (activeBubbleNum >= bubbleEffectList.Count)
        {
            activeBubbleNum = 0;
        }

        bubbleEffectList[activeBubbleNum].gameObject.SetActive(true);
        bubbleEffectList[activeBubbleNum].SetBubbleEffect(_pos, _scale, _sortOrder, _color);
    }

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
}
