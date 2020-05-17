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

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        GameDataManager.Instance.SetUserData();

        ChangeGameState(state);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.InGame)
        {
            if (isGameOver == true || isStageClear == true)
                return;

            MonsterManager.Instance.MonsterManagerUpdate();

            if (Input.GetMouseButton(0))
            {
                Vector2 target = new Vector2(Input.mousePosition.x, 0);
                target = Camera.main.ScreenToWorldPoint(target);
                target.x = Mathf.Clamp(target.x, minScreenPos.x, maxScreenPos.x);
                PlayerManager.Instance.transform.DOMove(new Vector2(target.x, -3.89f), 0.2f);

                PlayerManager.Instance.PlayerShot();
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
        UIManager.Instance.SetLobbyUI();

        PlayerManager.Instance.SetLobbyInit();
        MonsterManager.Instance.SetLobbyInit();
        BulletManager.Instance.SetLobbyInit();
    }
    #endregion

    #region INGAME
    public void SetInGame()
    {
        Debug.Log("Set InGame");
        isGameOver = false;

        if (isStageClear == true)
        {
            GameDataManager.Instance.userData.stageNum = GameDataManager.Instance.userData.stageNum + 1;       
            
            MonsterManager.Instance.monsterStageCount = MonsterManager.Instance.monsterStageCount + 5;
            MonsterManager.Instance.regenTime = MonsterManager.Instance.regenTime - 0.01f;
            MonsterManager.Instance.monsterHp = MonsterManager.Instance.monsterHp + 2;            
        }

        isStageClear = false;
        

        minScreenPos = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        maxScreenPos = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        UIManager.Instance.SetInGameUI();
        MonsterManager.Instance.SetInGameInit();
        PlayerManager.Instance.SetInGameInit();
        BulletManager.Instance.SetInGameInit();
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

        UIManager.Instance.inGameUI.SetScoreUI();
    }
    #endregion
}
