using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : SingleTon<BulletManager>
{
    [Header("Create Prefab")]
    public Bullet bullet;

    public int bulletCount = 0;
    int activeBullet = 0;
    public Transform bulletParent;
    public Sprite bulletSprite;
    public List<Bullet> bulletList = new List<Bullet>();

    [Header("Mercenary 1")]
    public int mercenary1BulletCount = 0;
    int mercenary1ActiveBullet = 0;
    public Sprite mercenary1BulletSprite;
    public List<Bullet> mercenary1BulletList = new List<Bullet>();

    [Header("Mercenary 2")]
    public int mercenary2BulletCount = 0;
    int mercenary2ActiveBullet = 0;
    public Sprite mercenary2BulletSprite;
    public List<Bullet> mercenary2BulletList = new List<Bullet>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetLobbyInit()
    {
        if (bulletParent != null)
        {
            Destroy(bulletParent.gameObject);
            bulletParent = null;
        }

        bulletList.Clear();
        mercenary1BulletList.Clear();
        mercenary2BulletList.Clear();
    }

    public void SetInGameInit()
    {
        if (bulletParent == null)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.position = Vector2.zero;

            bulletParent = go.transform;
            bulletParent.name = "BulletParent";
        }

        bulletSprite = GameDataManager.Instance.userData.leaderData.bulletImage;
        if (GameDataManager.Instance.userData.mercenaryDataList.Count == 1)
        {
            mercenary1BulletSprite = GameDataManager.Instance.userData.mercenaryDataList[0].bulletImage;
        }
        else if (GameDataManager.Instance.userData.mercenaryDataList.Count >= 2)
        {
            mercenary1BulletSprite = GameDataManager.Instance.userData.mercenaryDataList[0].bulletImage;
            mercenary2BulletSprite = GameDataManager.Instance.userData.mercenaryDataList[1].bulletImage;
        }

        activeBullet = 0;
        mercenary1ActiveBullet = 0;
        mercenary2ActiveBullet = 0;
    }

    public void CreateBullet(Vector2 _createPos)
    {        
        if (bulletList.Count < bulletCount)
        {
            GameObject go = Instantiate(bullet.gameObject, bulletParent);            
            Bullet b = go.GetComponent<Bullet>();            
            b.StartMove(_createPos, bulletSprite);

            bulletList.Add(b);
        }
        else
        {            
            bulletList[activeBullet].StartMove(_createPos);

            activeBullet++;
            if (activeBullet == bulletList.Count)
            {
                activeBullet = 0;
            }
        }
    }

    public void CreateMercenary1Bullet(Vector2 _createPos)
    {
        if (mercenary1BulletList.Count < mercenary1BulletCount)
        {
            GameObject go = Instantiate(bullet.gameObject, bulletParent);
            Bullet b = go.GetComponent<Bullet>();
            b.StartMove(_createPos, mercenary1BulletSprite);

            mercenary1BulletList.Add(b);
        }
        else
        {
            mercenary1BulletList[mercenary1ActiveBullet].StartMove(_createPos);

            mercenary1ActiveBullet++;
            if (mercenary1ActiveBullet == mercenary1BulletList.Count)
            {
                mercenary1ActiveBullet = 0;
            }
        }
    }

    public void CreateMercenary2Bullet(Vector2 _createPos)
    {
        if (mercenary2BulletList.Count < mercenary2BulletCount)
        {
            GameObject go = Instantiate(bullet.gameObject, bulletParent);
            Bullet b = go.GetComponent<Bullet>();
            b.StartMove(_createPos, mercenary2BulletSprite);

            mercenary2BulletList.Add(b);
        }
        else
        {
            mercenary2BulletList[mercenary2ActiveBullet].StartMove(_createPos);

            mercenary2ActiveBullet++;
            if (mercenary2ActiveBullet == mercenary2BulletList.Count)
            {
                mercenary2ActiveBullet = 0;
            }
        }
    }
}
