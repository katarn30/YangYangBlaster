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
    public float bulletDamage = 0;

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
            bulletParent.gameObject.SetActive(false);
        }
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

            SetBulletSprite();

            CreateBullet();

            CreateMercenary1Bullet();
            CreateMercenary2Bullet();
        }
        else
        {
            bulletParent.gameObject.SetActive(true);

            SetBulletSprite();
        }        

        activeBullet = 0;
        mercenary1ActiveBullet = 0;
        mercenary2ActiveBullet = 0;

        bulletDamage = GameDataManager.Instance.GetPlayerDamage();
    }

    public void CreateBullet()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            Bullet b = Instantiate(bullet, bulletParent);
            b.gameObject.SetActive(false);
            b.SetBulletSprite(bulletSprite);

            bulletList.Add(b);
        }
    }

    public void SetBulletSprite()
    {
        bulletSprite = GameDataManager.Instance.userData.leaderData.bulletImage;
        if (GameDataManager.Instance.userData.mercenaryDataList.Count == 1)
        {
            mercenary1BulletSprite = GameDataManager.Instance.userData.mercenaryDataList[0].bulletImage;
            mercenary2BulletSprite = null;
        }
        else if (GameDataManager.Instance.userData.mercenaryDataList.Count >= 2)
        {
            mercenary1BulletSprite = GameDataManager.Instance.userData.mercenaryDataList[0].bulletImage;
            mercenary2BulletSprite = GameDataManager.Instance.userData.mercenaryDataList[1].bulletImage;
        }
    }

    public void ShotBullet(Vector2 _createPos)
    {
        if (bulletList[activeBullet].gameObject.activeInHierarchy == true)
        {
            bulletList[activeBullet].gameObject.SetActive(false);
        }

        bulletList[activeBullet].StartMove(_createPos);

        activeBullet++;
        if (activeBullet == bulletList.Count)
        {
            activeBullet = 0;
        }
    }

    public void CreateMercenary1Bullet()
    {
        for (int i = 0; i < mercenary1BulletCount; i++)
        {
            Bullet b = Instantiate(bullet, bulletParent);
            b.gameObject.SetActive(false);

            mercenary1BulletList.Add(b);
        }
    }

    public void ShotMercenary1Bullet(Vector2 _createPos)
    {
        mercenary1BulletList[mercenary1ActiveBullet].SetBulletSprite(mercenary1BulletSprite);
        mercenary1BulletList[mercenary1ActiveBullet].StartMove(_createPos);

        mercenary1ActiveBullet++;
        if (mercenary1ActiveBullet == mercenary1BulletList.Count)
        {
            mercenary1ActiveBullet = 0;
        }
    }

    public void CreateMercenary2Bullet()
    {
        for (int i = 0; i < mercenary2BulletCount; i++)
        {
            Bullet b = Instantiate(bullet, bulletParent);            
            b.gameObject.SetActive(false);

            mercenary2BulletList.Add(b);
        }       
    }

    public void ShotMercenary2Bullet(Vector2 _createPos)
    {
        mercenary2BulletList[mercenary2ActiveBullet].SetBulletSprite(mercenary2BulletSprite);
        mercenary2BulletList[mercenary2ActiveBullet].StartMove(_createPos);

        mercenary2ActiveBullet++;
        if (mercenary2ActiveBullet == mercenary2BulletList.Count)
        {
            mercenary2ActiveBullet = 0;
        }
    }
}
