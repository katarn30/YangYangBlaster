using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Monster : MonoBehaviour
{
    public enum MonsterType
    {
        
    }

    public SpriteRenderer spriteRender;
    public Rigidbody2D rigidbody2D;

    int originHp = 0;
    public int monsterHp = 0;

    public bool isLeft = false;
    public float xSpeed = 0.0f;

    public bool isUp = false;
    public float yForce = 0.0f;
    public float createAddForce = 0.0f;

    public TextMeshPro hpText;
    public int spawnCount = 0;

    bool isPuchScaleEffect = false;
    public bool isDead = false;
    bool isRotLeft = false;
    bool isRotRight = false;

    Vector3 originScale;
    public float rotEndTime = 0.0f;
    public int spriteNum = 0;

    public List<Color> colorList = new List<Color>();

    private void Update()
    {       
        if (isDead == true)
        {
            
            return;
        }

        if (transform.position.x <= GameManager.Instance.minScreenPos.x)
        {
            isLeft = false;
        }
        else if (transform.position.x >= GameManager.Instance.maxScreenPos.x)
        {
            isLeft = true;
        }        

        if (isLeft == true)
        {
            transform.position -= new Vector3(xSpeed * Time.deltaTime, 0);
            if (isRotLeft == false)
            {
                isRotLeft = true;
                isRotRight = false;                
                transform.DORotate(new Vector3(0, 0, 45), rotEndTime);
            }            
        }
        else
        {
            transform.position += new Vector3(xSpeed * Time.deltaTime, 0);
            if (isRotRight == false)
            {
                isRotRight = true;
                isRotLeft = false;
                transform.DORotate(new Vector3(0, 0, -45), rotEndTime);
            }            
        }        

        if (transform.position.y <= -4.0f)
        {
            isUp = true;            
        }

        if (isUp == true)
        {
            isUp = false;
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.angularVelocity = 0f;
            
            rigidbody2D.AddForce(Vector2.up * yForce, ForceMode2D.Impulse);            
        }
    }

    private Vector3 AngleToDirection(float angle)
    {
        Vector3 direction = transform.forward;
        // 정면을 기준으로 한다면 transform.forward; 를 입렵하면 된다.

        var quaternion = Quaternion.Euler(0, 0, angle);
        Vector3 newDirection = quaternion * direction;

        return newDirection;
    }


    public void CreateMonster(int _spriteNum, bool _isLeft, bool _isUp, int _spwanCount, Sprite _sprite, int _sortOrder, int _monsterHp) 
    {
        spriteNum = _spriteNum;

        isRotLeft = false;
        isRotRight = false;

        isLeft = _isLeft;
        isUp = _isUp;
        spawnCount = _spwanCount;
        spriteRender.sprite = _sprite;
        isDead = false;

        originHp = _monsterHp;
        monsterHp = _monsterHp;
        hpText.text = monsterHp.ToString();

        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.AddForce(Vector2.up * createAddForce, ForceMode2D.Impulse);

        spriteRender.sortingOrder = _sortOrder;
        hpText.sortingOrder = _sortOrder;

        if (spawnCount != 0)
        {
            if (spawnCount <= 2)
            {
                transform.localScale = Vector3.one;
            }
            else if (spawnCount > 2 && spawnCount <= 5)
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            else
            {
                transform.localScale = new Vector3(2f, 2f, 2f);
            }
        }
        else
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }

        transform.DOPlay();
        transform.rotation = Quaternion.Euler(Vector2.zero);
        originScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (monsterHp > 0)
            {
                monsterHp = monsterHp - 1;
                hpText.text = monsterHp.ToString();

                if (isPuchScaleEffect == false)
                {
                    isPuchScaleEffect = true;
                    transform.DOPunchScale(new Vector2(0.2f, 0.2f), 0.1f).OnComplete(
                        () => 
                        {
                            isPuchScaleEffect = false;
                            transform.localScale = originScale; 
                        });
                }                
            }
            
            if (monsterHp <= 0)
            {
                isDead = true;
                monsterHp = 0;
                MonsterManager.Instance.deadCount = MonsterManager.Instance.deadCount + 1;

                if (spawnCount > 0)
                {
                    MonsterManager.Instance.SetSubMonster(transform.position, isUp, spawnCount - 1, originHp);
                }

                GameManager.Instance.UpdateScore(100);

                EffectManager.Instance.SetBubbleEffect(transform.position, transform.localScale, colorList[spriteNum]);
                EffectManager.Instance.SetCoinEffect(transform.position);

                transform.DOPause();

                gameObject.SetActive(false);
            }

            other.gameObject.SetActive(false);
        }
    }
}
