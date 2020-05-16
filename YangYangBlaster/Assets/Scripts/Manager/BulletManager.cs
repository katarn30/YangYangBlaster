using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : SingleTon<BulletManager>
{
    public Bullet bullet;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void CreateBullet(Vector2 _createPos)
    {
        GameObject go = Instantiate(bullet.gameObject, transform);
        go.transform.position = _createPos;
        Bullet b = go.GetComponent<Bullet>();
        b.StartMove(_createPos);
    }
}
