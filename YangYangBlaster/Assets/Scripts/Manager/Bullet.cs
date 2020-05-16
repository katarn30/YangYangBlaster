using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public void StartMove(Vector2 _createPos)
    {
        transform
            .DOMove(new Vector2(_createPos.x, 8f), 1f)
            .OnComplete(()=> {
                Destroy(gameObject);
            });
    }
}
