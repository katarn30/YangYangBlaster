using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Boss : MonoBehaviour
{
    public int bossPattenNum = 0;

    public SpriteRenderer spriteRenderer;
    public TextMeshPro hpText;
    public Animator animator;
    public CircleCollider2D circleCollider;
    public GameObject moveEffect;

    int originHp = 0;
    public int monsterHp = 0;

    public bool isLeft = false;
    public float xSpeed = 0.0f;
    public float xGap = 0.0f;

    public bool isUp = false;
    public float ySpeed = 0.0f;
    public float yGap = 0.0f;

    bool isPuchScaleEffect = false;
    public bool isDead = false;

    Vector3 originScale;

    public Color deadColor = Color.white;

    private void Update()
    {       
        if (isDead == true)
        {            
            return;
        }

        if (bossPattenNum == 0)
        {                      
            if (transform.position.x <= GameManager.Instance.minScreenPos.x + xGap)
            {
                isLeft = false;
            }
            else if (transform.position.x >= GameManager.Instance.maxScreenPos.x - xGap)
            {
                isLeft = true;
            }

            if (isLeft == true)
            {
                transform.position -= new Vector3(xSpeed * Time.deltaTime, 0);
                
            }
            else
            {
                transform.position += new Vector3(xSpeed * Time.deltaTime, 0);
            }

            if (transform.position.y <= GameManager.Instance.minScreenPos.y + yGap)
            {
                isUp = false;
            }
            else if (transform.position.y >= GameManager.Instance.maxScreenPos.y - yGap)
            {
                isUp = true;
            }

            if (isUp == true)
            {
                transform.position -= new Vector3(0, ySpeed * Time.deltaTime);

            }
            else
            {
                transform.position += new Vector3(0, ySpeed * Time.deltaTime);
            }
        }
        else if (bossPattenNum == 1)
        {

        }
    }

    public void SetBoss(BossData _data)
    {
        originHp = _data.hp;
        monsterHp = originHp;

        bossPattenNum = _data.pattenType;

        deadColor = _data.deadColor;
        animator.runtimeAnimatorController = _data.runtimeAnimator;

        circleCollider.offset = _data.colliderOffset;
        circleCollider.radius = _data.radius;

        isDead = false;
        isUp = false;
        isLeft = false;

        originScale = transform.localScale;
        transform.position = Vector3.zero;

        hpText.text = monsterHp.ToString();

        SetPattenInit(bossPattenNum);
    }

    public void SetPattenInit(int _type)
    {
        if (_type == 0)
        {
            if (moveEffect.gameObject.activeInHierarchy == false)
            {
                moveEffect.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (monsterHp > 0)
            {
                monsterHp = monsterHp - 1;
                
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
                
                EffectManager.Instance.SetBubbleEffect(transform.position, transform.localScale, deadColor);
                EffectManager.Instance.SetCoinEffect(transform.position);

                GameManager.Instance.UpdateScore(1000);
                GameManager.Instance.StageClear();

                transform.DOPause();

                gameObject.SetActive(false);
            }

            hpText.text = monsterHp.ToString();
            other.gameObject.SetActive(false);
        }
    }
}
