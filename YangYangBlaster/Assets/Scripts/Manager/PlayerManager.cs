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
    public PlayerState state = PlayerState.Idle;

    public List<Transform> mercenaryPosList = new List<Transform>();
    public SpriteRenderer playerSprite;

    public Animator animator;

    public int playerHp = 0;
    public float attackSpeed = 0.0f;
    float bulletTime = 0.0f;
    public int originHp = 0;

    public ParticleSystem bulleParticle;
    public ParticleSystem getItemParticle;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    //로비 상태로 바뀔떄
    public void SetLobbyInit()
    {
        playerSprite.gameObject.SetActive(false);
        bulleParticle.gameObject.SetActive(false);
        getItemParticle.gameObject.SetActive(false);
    }

    //인게임 상태로 바뀔때
    public void SetInGameInit()
    {        
        playerSprite.gameObject.SetActive(true);
        bulleParticle.gameObject.SetActive(true);
        getItemParticle.gameObject.SetActive(true);

        if (originHp == 0)
        {
            originHp = playerHp;
        }
        else
        {
            playerHp = originHp;
        }

        transform.position = new Vector2(0, transform.position.y);
        attackSpeed = GameDataManager.Instance.userData.leaderData.attackSpeed;

        ChangeAniState(PlayerState.Idle);
    }

    // Update is called once per frame
    public void PlayerShot()
    {
        bulletTime += Time.deltaTime;

        if (bulletTime >= attackSpeed)
        {
            bulletTime = 0;

            BulletManager.Instance.ShotBullet(transform.position);               
        }

        ChangeAniState(PlayerState.Attack);        
    }

    public void ChangeAniState(PlayerState _playerState)
    {
        if (state == _playerState)
            return;

        state = _playerState;

        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Attack");

        switch (state)
        {
            case PlayerState.Idle:
                animator.SetTrigger("Idle");
                bulleParticle.Stop();
                break;
            case PlayerState.Attack:
                animator.SetTrigger("Attack");
                bulleParticle.Play();       
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
            if (GameManager.Instance.isStageClear == true)
                return;

            playerHp = playerHp - 1;

            if (playerHp <= 0)
            {
                playerHp = 0;
                GameManager.Instance.GameOver();
            }
            
            Handheld.Vibrate();
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            getItemParticle.Stop();
            getItemParticle.Play();
        }
    }
}
