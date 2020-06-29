using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BuffInfoItem : MonoBehaviour
{
    public GameObject effectUI;

    public Image milkIcon;
    public Text milkNameText;
    public Image milkTimeGauge;

    private void OnEnable()
    {
        effectUI.transform.DOLocalMoveX(0, 0.4f);
    }

    private void OnDisable()
    {
        effectUI.transform.localPosition = new Vector2(282, 0);
    }

    public void SetBuffItem(MilkItem _item)
    {
        effectUI.transform.localPosition = new Vector2(282, 0);

        milkIcon.sprite = _item.milkSprite;
        milkNameText.text = _item.type.ToString();
        milkTimeGauge.fillAmount = 1;
    }

    public void UpdateMilkGauge(float _now, float _max)
    {
        milkTimeGauge.fillAmount = (_max - _now) / _max;       
        gameObject.SetActive(true);
    }
}
