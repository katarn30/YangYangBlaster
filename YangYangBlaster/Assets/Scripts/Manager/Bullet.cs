using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour
{
    public SpriteRenderer bulletSprite;

    public void SetBulletSprite(Sprite _bulletSprite)
    {
        if (_bulletSprite == null)
        {
            _bulletSprite = bulletSprite.sprite;
        }

        bulletSprite.sprite = _bulletSprite;
    }

    public void StartMove(Vector2 _createPos)
    {        
        gameObject.SetActive(true);
        transform.position = _createPos;

        transform
            .DOMove(new Vector2(_createPos.x, 8f), 1f)
            .OnComplete(()=> {
                gameObject.SetActive(false);
            });
    }
}
