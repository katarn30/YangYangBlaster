﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckUIController : MonoBehaviour
{
    [Header("Deck UI")]
    public List<GameObject> plusButton = new List<GameObject>();
    public List<Image> deckMercenary = new List<Image>();

    [Header("Stage UI")]
    public GameObject[] stageGauge = new GameObject[5];

    [Header("Animator")]
    public List<Animator> mercenaryAnimatorList = new List<Animator>();    

    public void SetDeckUI()
    {
        if(GameDataManager.Instance.userData.mercenaryDataList.Count == 0)
        {
            plusButton[0].gameObject.SetActive(true);
            deckMercenary[0].gameObject.SetActive(false);

            plusButton[1].gameObject.SetActive(true);
            deckMercenary[1].gameObject.SetActive(false);
        }
        else if (GameDataManager.Instance.userData.mercenaryDataList.Count == 1)
        {
            plusButton[0].gameObject.SetActive(false);
            deckMercenary[0].gameObject.SetActive(true);
            deckMercenary[0].sprite = GameDataManager.Instance.userData.mercenaryDataList[0].catImage;
            mercenaryAnimatorList[0].runtimeAnimatorController = GameDataManager.Instance.userData.mercenaryDataList[0].uiRuntimeAnimator;

            plusButton[1].gameObject.SetActive(true);
            deckMercenary[1].gameObject.SetActive(false);
        }
        else if (GameDataManager.Instance.userData.mercenaryDataList.Count >= 2)
        {
            plusButton[0].gameObject.SetActive(false);
            deckMercenary[0].gameObject.SetActive(true);
            deckMercenary[0].sprite = GameDataManager.Instance.userData.mercenaryDataList[0].catImage;
            mercenaryAnimatorList[0].runtimeAnimatorController = GameDataManager.Instance.userData.mercenaryDataList[0].uiRuntimeAnimator;

            plusButton[1].gameObject.SetActive(false);
            deckMercenary[1].gameObject.SetActive(true);
            deckMercenary[1].sprite = GameDataManager.Instance.userData.mercenaryDataList[1].catImage;
            mercenaryAnimatorList[1].runtimeAnimatorController = GameDataManager.Instance.userData.mercenaryDataList[1].uiRuntimeAnimator;
        }     
    }

    public void SetStageGauge()
    {
        float num = ((float)GameDataManager.Instance.userData.stageNum % 6);

        if (num == 1)
        {
            StageActiveGaugeUI(0);
        }
        else if (num == 2)
        {
            StageActiveGaugeUI(1);
        }
        else if (num == 3)
        {
            StageActiveGaugeUI(2);
        }
        else if (num == 4)
        {
            StageActiveGaugeUI(3);
        }
        else if (num == 5)
        {
            StageActiveGaugeUI(4);
        }
    }

    void StageActiveGaugeUI(int _num)
    {
        for (int i = 0; i < stageGauge.Length; i++)
        {
            if (i <= _num)
            {
                stageGauge[i].gameObject.SetActive(true);
            }
            else
            {
                stageGauge[i].gameObject.SetActive(false);
            }
        }
    }

}
