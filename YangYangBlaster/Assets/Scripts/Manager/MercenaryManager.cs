using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MercenaryManager : SingleTon<MercenaryManager>
{
    public SpriteRenderer mercenary;
    public float speed;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        MercenaryMovePoint();
    }
    public void MercenaryMovePoint()
    {
        mercenary.transform.position = Vector3.Lerp(mercenary.transform.position, PlayerManager.Instance.mercenaryPosList[1].position, Time.deltaTime * speed);
    }
}
