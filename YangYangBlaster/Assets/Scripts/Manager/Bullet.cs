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

    public void SetCriticalBullet()
    {
        bulletSprite.color = Color.red;
    }

    public void StartMove(Vector2 _createPos)
    {
        int rand = Random.Range(0, 101);

        if (rand <= GameDataManager.Instance.getPlayerCritical())
        {
            gameObject.tag = "CriticalBullet";
            bulletSprite.color = Color.red;
        }
        else
        {
            gameObject.tag = "Bullet";
            bulletSprite.color = Color.white;
        }

        gameObject.SetActive(true);
        transform.position = _createPos;

        transform
            .DOMove(new Vector2(_createPos.x, 7f), 1f)
            .OnComplete(()=> {
                gameObject.SetActive(false);
            });
    }
}
