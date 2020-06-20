using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    public ParticleSystem deadParticle;
    public ParticleSystem getItemParticle;
    public ParticleSystem buffParticle;
    public GameObject shieldParticle;

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
        deadParticle.gameObject.SetActive(false);
        buffParticle.gameObject.SetActive(false);
        shieldParticle.SetActive(false);
    }

    //인게임 상태로 바뀔때
    public void SetInGameInit()
    {
        playerSprite.gameObject.SetActive(true);

        bulleParticle.gameObject.SetActive(true);
        getItemParticle.gameObject.SetActive(true);
        deadParticle.gameObject.SetActive(true);
        buffParticle.gameObject.SetActive(true);
        //shieldParticle.SetActive(true);

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
    public void PlayerUpdate()
    {        
        if (GameManager.Instance.isShieldMode == true || GameManager.Instance.isGiantMode == true)
        {
            shieldParticle.SetActive(true);
        }
        else if (GameManager.Instance.isShieldMode == false && GameManager.Instance.isGiantMode == false)
        {
            shieldParticle.SetActive(false);
        }

        if (GameManager.Instance.isGiantMode == true)
        {
            //-0.21
            playerSprite.transform.DOLocalMoveY(0.84f, 0.4f);
            playerSprite.transform.DOScale(new Vector3(2, 2, 0), 0.4f);
        }
        else
        {
            playerSprite.transform.DOLocalMoveY(-0.21f, 0.4f);
            playerSprite.transform.DOScale(new Vector3(0.7f, 0.7f, 0), 0.4f);
        }
    }

    public void PlayerShot()
    {
        if (GameManager.Instance.isSpeedMode == true)
        {
            attackSpeed = GameDataManager.Instance.userData.leaderData.attackSpeed - 0.04f;
        }
        else
        {
            attackSpeed = GameDataManager.Instance.userData.leaderData.attackSpeed;
        }

        bulletTime += Time.deltaTime;

        if (bulletTime >= attackSpeed)
        {
            bulletTime = 0;

            BulletManager.Instance.ShotBullet(transform.position);
            SoundManager.Instance.PlayerBulletSound();
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
                deadParticle.Play();
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

            if (GameManager.Instance.isGameOver == true)
                return;

            if (GameManager.Instance.isShieldMode == true || GameManager.Instance.isGiantMode == true)
                return;

            playerHp = playerHp - 1;

            if (playerHp <= 0)
            {
                playerHp = 0;
                ChangeAniState(PlayerState.Dead);
                GameManager.Instance.GameOver();
            }
            
            Handheld.Vibrate();
        }
        else if (other.gameObject.CompareTag("Milk"))
        {
            SoundManager.Instance.CoinSound();
            buffParticle.Stop();
            buffParticle.Play();
        }
        else if (other.gameObject.CompareTag("Coin"))
        {
            SoundManager.Instance.CoinSound();
            getItemParticle.Stop();
            getItemParticle.Play();
        }
    }
}
