using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : SingleTon<PlayerManager>
{
    public List<SpriteRenderer> mercenaryList = new List<SpriteRenderer>();
    public List<Sprite> catSpriteList = new List<Sprite>();
    int catSpriteNum = 0;

    public SpriteRenderer playerSprite;    

    public int playerHp = 0;
    public float attackSpeed = 0.0f;
    float bulletTime = 0.0f;
    public int originHp = 0;

    public Slider hpBar;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetInGameInit()
    {        
        playerSprite.gameObject.SetActive(true);

        if (originHp == 0)
        {
            originHp = playerHp;
        }
        else
        {
            playerHp = originHp;
        }

        transform.position = new Vector2(0, transform.position.y);

        hpBar.value = playerHp / originHp;
    }

    // Update is called once per frame
    public void PlayerShot()
    {
        bulletTime += Time.deltaTime;

        if (bulletTime >= attackSpeed)
        {
            bulletTime = 0;

            BulletManager.Instance.CreateBullet(transform.position);

            for (int i = 0; i < mercenaryList.Count; i++)
            {
                if (mercenaryList[i].gameObject.activeInHierarchy == true)
                {
                    BulletManager.Instance.CreateBullet(mercenaryList[i].transform.position);
                }                
            }                        
        }
    }

    public void ChangeCat()
    {
        catSpriteNum++;
        if (catSpriteNum >= catSpriteList.Count)
        {
            catSpriteNum = 0;
        }

        playerSprite.sprite = catSpriteList[catSpriteNum];
    }

    public void ActiveMercenary()
    {
        int rnd = Random.Range(0, catSpriteList.Count);

        if (mercenaryList[0].gameObject.activeInHierarchy == false)
        {            
            mercenaryList[0].sprite = catSpriteList[rnd];
            mercenaryList[0].gameObject.SetActive(true);
        }
        else
        {
            mercenaryList[1].sprite = catSpriteList[rnd];
            mercenaryList[1].gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            if (playerHp > 0)
            {
                playerHp = playerHp - 1;
            }
            else
            {
                playerHp = 0;
                GameManager.Instance.GameOver();
            }

            hpBar.value = (float)playerHp / (float)originHp;

            Handheld.Vibrate();
        }
    }
}
