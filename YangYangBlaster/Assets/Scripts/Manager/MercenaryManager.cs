using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryManager : SingleTon<MercenaryManager>
{
    public SpriteRenderer mercenary1;
    public SpriteRenderer mercenary2;
    public float speed;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetLobbyInit()
    {
        mercenary1.gameObject.SetActive(false);
        mercenary2.gameObject.SetActive(false);
    }

    public void SetInGameInit()
    {
        mercenary1.gameObject.SetActive(true);
        mercenary2.gameObject.SetActive(true);

        InitMercenaryPosition();
    }

    public void InitMercenaryPosition()
    {
        mercenary1.transform.position = PlayerManager.Instance.mercenaryPosList[0].position;
        mercenary2.transform.position = PlayerManager.Instance.mercenaryPosList[1].position;
    }

    public void MercenaryMovePoint()
    {
        if (mercenary1.gameObject.activeInHierarchy == true)
        {
            mercenary1.transform.position = Vector3.Lerp(mercenary1.transform.position, PlayerManager.Instance.mercenaryPosList[0].position, Time.deltaTime * speed);
        }

        if (mercenary2.gameObject.activeInHierarchy == true)
        {
            mercenary2.transform.position = Vector3.Lerp(mercenary2.transform.position, PlayerManager.Instance.mercenaryPosList[1].position, Time.deltaTime * speed);
        }
    }
}
