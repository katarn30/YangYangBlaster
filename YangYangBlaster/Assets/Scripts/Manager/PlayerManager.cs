using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Attack,
    Skill,
    Dead
}

public class PlayerManager : SingleTon<PlayerManager>
{
    public List<Transform> mercenaryPosList = new List<Transform>();
    public SpriteRenderer playerSprite;

    public Animator animator;

    public int playerHp = 0;
    public float attackSpeed = 0.0f;
    float bulletTime = 0.0f;
    public int originHp = 0;

    public GameObject hpBarUI;
    public Image hpBar;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    //로비 상태로 바뀔떄
    public void SetLobbyInit()
    {
        playerSprite.gameObject.SetActive(false);
        hpBarUI.gameObject.SetActive(false);
    }

    //인게임 상태로 바뀔때
    public void SetInGameInit()
    {        
        playerSprite.gameObject.SetActive(true);
        hpBarUI.gameObject.SetActive(true);

        if (originHp == 0)
        {
            originHp = playerHp;
        }
        else
        {
            playerHp = originHp;
        }

        transform.position = new Vector2(0, transform.position.y);

        UpdateHpBar(playerHp);
    }

    public void UpdateHpBar(float _playerHp)
    {
        if (hpBar == null)
        {
            Debug.LogError("PlayerManager HpBar UI Null");
            return;
        }

        hpBar.fillAmount = _playerHp / originHp;
    }

    // Update is called once per frame
    public void PlayerShot()
    {
        bulletTime += Time.deltaTime;

        if (bulletTime >= attackSpeed)
        {
            bulletTime = 0;

            BulletManager.Instance.CreateBullet(transform.position);               
        }
    }

    public void ChangeAniState(PlayerState _playerState)
    {
        switch (_playerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Attack:
                animator.SetTrigger("Attack");
                break;
            case PlayerState.Skill:
                break;
            case PlayerState.Dead:
                break;
        }
    }

    public void ChangeLeaderCat(Sprite _sprite)
    {
        playerSprite.sprite = _sprite;
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

            UpdateHpBar((float)playerHp);

            Handheld.Vibrate();
        }
    }
}
