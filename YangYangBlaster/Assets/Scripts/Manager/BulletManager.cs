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
    public List<Bullet> bulletList = new List<Bullet>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetInGameInit()
    {
        Destroy(bulletParent.gameObject);

        activeBullet = 0;
        bulletList.Clear();
    }

    public void CreateBullet(Vector2 _createPos)
    {
        if (bulletParent != null)
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.position = Vector2.zero;

            bulletParent = go.transform;
            bulletParent.name = "BulletParent";
        }

        if (bulletList.Count < bulletCount)
        {
            GameObject go = Instantiate(bullet.gameObject, bulletParent);            
            Bullet b = go.GetComponent<Bullet>();
            b.StartMove(_createPos);

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
}
