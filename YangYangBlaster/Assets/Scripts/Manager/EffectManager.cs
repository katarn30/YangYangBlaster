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

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetLobbyInit()
    {
        if (parent != null)
        {
            Destroy(parent.gameObject);
            parent = null;
        }

        bubbleEffectList.Clear();
    }

    public void SetInGameInit()
    {
        if (parent == null)
        {
            parent = new GameObject().transform;
            parent.transform.parent = transform;
            parent.name = "Effect Parent";
        }

        activeBubbleNum = 0;
    }

    public void SetBubbleEffect(Vector2 _pos, Vector2 _scale, int _sortOrder, Color _color)
    {
        if (bubbleEffectList.Count < bubbleEffectMaxCount)
        {
            BubbleEffect bubble = Instantiate(bubblePrefab, parent);

            bubble.SetBubbleEffect(_pos, _scale, _sortOrder, _color);

            bubbleEffectList.Add(bubble);
        }
        else
        {
            activeBubbleNum = activeBubbleNum + 1;

            if (activeBubbleNum >= bubbleEffectList.Count)
            {
                activeBubbleNum = 0;
            }

            bubbleEffectList[activeBubbleNum].gameObject.SetActive(true);
            bubbleEffectList[activeBubbleNum].SetBubbleEffect(_pos, _scale, _sortOrder, _color);
        }
    }
}
