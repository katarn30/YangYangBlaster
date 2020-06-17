using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : SingleTon<GameManager>
{
    public enum GameState
    {
        Lobby,
        InGame
    }
    [Header("Game State")]
    public GameState state = GameState.Lobby;

    [Header("InGame")]
    [HideInInspector]
    public Vector2 minScreenPos;
    [HideInInspector]
    public Vector2 maxScreenPos;

    public bool isGameOver = false;
    public bool isStageClear = false;

    [Header("InGameMode")]
    public float slowTime = 0.0f;
    public float slowDurationTime = 0.0f;
    public bool isSlowMode = false;

    public float frezeTime = 0.0f;
    public float frezeDurationTime = 0.0f;
    public bool isFrezeMode = false;

    public float speedTime = 0.0f;
    public float speedDurationTime = 0.0f;
    public bool isSpeedMode = false;

    public float powerTime = 0.0f;
    public float powerDurationTime = 0.0f;
    public bool isPowerMode = false;

    public float moneyTime = 0.0f;
    public float moneyDurationTime = 0.0f;
    public bool isMoneyMode = false;

    public float shieldTime = 0.0f;
    public float shieldDurationTime = 0.0f;
    public bool isShieldMode = false;

    public float giantTime = 0.0f;
    public float giantDurationTime = 0.0f;
    public bool isGiantMode = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        GameDataManager.Instance.SetUserData();
        LoginManager.Instance.DoAutoLogin();

        ChangeGameState(state);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.InGame)
        {
            if (isGameOver == true || isStageClear == true)
            {
                PlayerManager.Instance.ChangeAniState(PlayerState.Idle);
            }
            else
            {
                MonsterManager.Instance.MonsterManagerUpdate();

                if (Input.GetMouseButton(0))
                {
                    Vector2 target = new Vector2(Input.mousePosition.x, 0);
                    target = Camera.main.ScreenToWorldPoint(target);
                    target.x = Mathf.Clamp(target.x, minScreenPos.x, maxScreenPos.x);
                    PlayerManager.Instance.transform.DOMove(new Vector2(target.x, -3.89f), 0.2f);

                    PlayerManager.Instance.PlayerShot();
                    MercenaryManager.Instance.MercenaryShot();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    PlayerManager.Instance.ChangeAniState(PlayerState.Idle);
                }

                MercenaryManager.Instance.MercenaryMovePoint();

                UIManager.Instance.InGameUIUpdate();

                SetMilkMode();
            }            
        }
        else if (state == GameState.Lobby)
        {
            UIManager.Instance.LobbyUIUpdate();
        }
    }

    public void SetMilkMode()
    {
        if (isSlowMode == true)
        {
            slowTime = slowTime + Time.deltaTime;

            if (slowTime >= slowDurationTime)
            {
                Debug.Log("Slow End");
                isSlowMode = false;
                slowTime = 0;
            }
        }

        if (isFrezeMode == true)
        {
            frezeTime = frezeTime + Time.deltaTime;

            if (frezeTime >= frezeDurationTime)
            {
                Debug.Log("Freze End");
                isFrezeMode = false;
                frezeTime = 0;
            }
        }

        if (isSpeedMode == true)
        {
            speedTime = speedTime + Time.deltaTime;

            if (speedTime >= speedDurationTime)
            {
                Debug.Log("Speed End");
                isSpeedMode = false;
                speedTime = 0;
            }
        }

        if (isShieldMode == true)
        {
            shieldTime = shieldTime + Time.deltaTime;

            if (shieldTime >= shieldDurationTime)
            {
                Debug.Log("Shield End");
                isShieldMode = false;
                shieldTime = 0;
            }
        }

        if (isMoneyMode == true)
        {
            moneyTime = moneyTime + Time.deltaTime;

            if (moneyTime >= moneyDurationTime)
            {
                Debug.Log("Money End");
                isMoneyMode = false;
                moneyTime = 0;
            }
        }

        if (isGiantMode == true)
        {
            giantTime = giantTime + Time.deltaTime;

            if (giantTime >= giantDurationTime)
            {
                Debug.Log("Giant End");
                isGiantMode = false;
                giantTime = 0;
            }
        }

        if (isPowerMode == true)
        {
            powerTime = powerTime + Time.deltaTime;

            if (powerTime >= powerDurationTime)
            {
                Debug.Log("Power End");
                isPowerMode = false;
                powerTime = 0;
            }
        }
    }

    public void ChangeGameState(GameState _state)
    {
        state = _state;

        switch (state)
        {
            case GameState.Lobby:
                SetLobby();
                break;
            case GameState.InGame:
                SetInGame();
                break;
        }
    }

    #region Lobby
    public void SetLobby()
    {        
        PlayerManager.Instance.SetLobbyInit();
        MonsterManager.Instance.SetLobbyInit();
        BulletManager.Instance.SetLobbyInit();
        RpcServiceManager.Instance.SetLobbyInit();
        MercenaryManager.Instance.SetLobbyInit();
        EffectManager.Instance.SetLobbyInit();

        UIManager.Instance.SetLobbyUI();
        SoundManager.Instance.LobbyBGMSound();
    }

    public void BuyMercenary(int _num)
    {
        Debug.Log("BuyMercenary");
        GameDataManager.Instance.BuyMercenary(_num);
        UIManager.Instance.lobbyUI.shopController.RefreshCatShopList();
        UIManager.Instance.lobbyUI.deckUIController.SetDeckUI();
        UIManager.Instance.lobbyUI.UpdateCoinText();
    }

    public void SelectMercenary(string _name)
    {
        GameDataManager.Instance.SelectMercenary(GameDataManager.Instance.GetMyMercenaryData(_name));

        UIManager.Instance.lobbyUI.deckUIController.SetDeckUI();
    }

    public void UnSelectMercenary(string _name)
    {
        GameDataManager.Instance.RemoveMercenary(GameDataManager.Instance.GetMyMercenaryData(_name));

        UIManager.Instance.lobbyUI.deckUIController.SetDeckUI();
    }

    #endregion

    #region INGAME
    public void SetInGame()
    {
        Debug.Log("Set InGame");
        
        if (isStageClear == true)
        {
            GameDataManager.Instance.userData.stageNum = GameDataManager.Instance.userData.stageNum + 1;       
            
            MonsterManager.Instance.monsterStageCount = MonsterManager.Instance.monsterStageCount + 5;
            MonsterManager.Instance.regenTime = MonsterManager.Instance.regenTime - 0.01f;
            MonsterManager.Instance.monsterHp = MonsterManager.Instance.monsterHp + 2;            
        }

        isGameOver = false;
        isStageClear = false;

        slowTime = 0.0f;
        slowDurationTime = 0.0f;
        isSlowMode = false;

        frezeTime = 0.0f;
        frezeDurationTime = 0.0f;
        isFrezeMode = false;

        minScreenPos = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxScreenPos = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        
        PlayerManager.Instance.SetInGameInit();
        MonsterManager.Instance.SetInGameInit();        
        BulletManager.Instance.SetInGameInit();
        MercenaryManager.Instance.SetInGameInit();        
        EffectManager.Instance.SetInGameInit();
        UIManager.Instance.SetInGameUI();

        if (isBossStage() == true)
        {
            SoundManager.Instance.StageBossBGMSound();
        }
        else
        {
            SoundManager.Instance.StageBGMSound();
        }                
    }

    public void GameOver()
    {
        isGameOver = true;

        UIManager.Instance.inGameUI.GameOverUI();        
    }

    public void StageClear()
    {
        isStageClear = true;

        UIManager.Instance.inGameUI.StageClearUI();        
    }

    public void UpdateScore(int _score)
    {
        GameDataManager.Instance.userData.score = GameDataManager.Instance.userData.score + _score;        
    }

    public void GetCoin(int _coin)
    {
        GameDataManager.Instance.userData.userCurrency.userCoin += _coin;

        UIManager.Instance.inGameUI.SetCoinUI();
    }

    public bool isBossStage()
    {
        bool result = false;
        float num = ((float)GameDataManager.Instance.userData.stageNum % 6);
        if (num == 3 || num == 5)
        {
            result = true;
        }

        return result;
    }

    public void SetMilkItem(MilkItem _item)
    {
        if (_item.type == MilkType.SLOW)
        {
            isSlowMode = true;
            slowTime = 0;
            slowDurationTime = _item.milkDuration;            
        }
        else if (_item.type == MilkType.FREZE)
        {
            isFrezeMode = true;
            frezeTime = 0;
            frezeDurationTime = _item.milkDuration;
        }
        else if (_item.type == MilkType.GIANT)
        {
            isGiantMode = true;
            giantTime = 0;
            giantDurationTime = _item.milkDuration;
        }
        else if (_item.type == MilkType.MONEY)
        {
            isMoneyMode = true;
            moneyTime = 0;
            moneyDurationTime = _item.milkDuration;
        }
        else if (_item.type == MilkType.POWER)
        {
            isPowerMode = true;
            powerTime = 0;
            powerDurationTime = _item.milkDuration;
        }
        else if (_item.type == MilkType.SHIELD)
        {
            isShieldMode = true;
            shieldTime = 0;
            shieldDurationTime = _item.milkDuration;
        }
        else if (_item.type == MilkType.SPEED)
        {
            isSpeedMode = true;
            speedTime = 0;
            speedDurationTime = _item.milkDuration;
        }
    }
    #endregion
}
