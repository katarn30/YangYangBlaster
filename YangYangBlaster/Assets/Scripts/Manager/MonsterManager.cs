using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingleTon<MonsterManager>
{
    public List<Sprite> monsterSpriteList = new List<Sprite>();

    [Header("Monster Prefab")]
    public int monsterMaxSpawnCount = 0;
    public List<Monster> monsterList = new List<Monster>();
    public int activeMonster = 0;

    [Header("Monster Spawn")]
    public int monsterHp = 0;
    public int monsterStageCount = 0;
    public int nowMonsterCount = 0;

    [Header("Monster JudgeMent")]
    public int allSpawnCount = 0;
    public int deadCount = 0;

    [Header("Boos Prefab")]
    public int bossMaxSpawnCount = 0;
    public List<Boss> bossList = new List<Boss>();
    public int activeBoss = 0;

    public Monster monster;
    public Boss boss;
    Transform MonsterParent = null;
    public bool isBossStage = false;

    public float regenTime = 0.0f;
    float nowTime = 0.0f;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public bool intToBool(int Number)
    {
        return (Number == 0 ? false : true);
    }

    public void SetLobbyInit()
    {
        if (MonsterParent != null)
        {            
            MonsterParent.gameObject.SetActive(false);               
        }
    }

    public void SetInGameInit()
    {        
        if (MonsterParent == null)
        {
            MonsterParent = new GameObject().transform;
            MonsterParent.parent = transform;
            MonsterParent.name = "MonsterParent";

            MonsterParent.position = Vector3.zero;
            CreateMonster();
            CreateBoss();
        }
        else
        {
            MonsterParent.gameObject.SetActive(true);
        }
                
        initMonster();
        initBoss();        
    }

    #region Boss
    public void initBoss()
    {
        isBossStage = GameManager.Instance.isBossStage();

        activeBoss = 0;

        for (int i = 0; i < bossList.Count; i++)
        {
            bossList[i].gameObject.SetActive(false);
        }

        if (isBossStage == true)
        {
            SetBoss();            
        }
    }

    public void CreateBoss()
    {
        for (int i = 0; i < bossMaxSpawnCount; i++)
        {
            Boss b = Instantiate(boss, MonsterParent);
            b.gameObject.SetActive(false);
            bossList.Add(b);
        }
    }

    public void SetBoss()
    {
        bossList[activeBoss].gameObject.SetActive(true);

        bossList[activeBoss].SetBoss(GameDataManager.Instance.BossDataList[activeBoss]);

        activeBoss = activeBoss + 1;

        if (activeBoss >= bossList.Count)
        {
            activeBoss = 0;
        }


    }
    #endregion


    #region Monster
    public void initMonster()
    {
        activeMonster = 0;
        deadCount = 0;
        allSpawnCount = 0;
        nowMonsterCount = 0;

        for (int i = 0; i < monsterList.Count; i++)
        {
            monsterList[i].gameObject.SetActive(false);
        }
    }

    public void CreateMonster()
    {
        for (int i = 0; i < monsterMaxSpawnCount; i++)
        {
            Monster mon = Instantiate(monster, MonsterParent);
            mon.gameObject.SetActive(false);
            monsterList.Add(mon);
        }
    }

    public void MonsterManagerUpdate()
    {
        if (GameManager.Instance.isGameOver == true || GameManager.Instance.isStageClear == true)
            return;

        if (isBossStage == true)
            return;

        if (monsterStageCount <= nowMonsterCount)
        {
            if (allSpawnCount <= deadCount)
            {
                GameManager.Instance.StageClear();
            }

            return;
        }            

        nowTime += Time.deltaTime;

        if (nowTime >= regenTime)
        {
            nowTime = 0f;

            float x = 0.0f;
            x = Random.Range(-2.38f, 2.34f);
            float y = 1.74f;

            int rnd = Random.Range(0, monsterSpriteList.Count);
            int rnds = Random.Range(0, 2);
            int rndCount = Random.Range(1, 3);
            int stage = GameDataManager.Instance.userData.stageNum;

            ActiveMonsterInit(true, rnd, new Vector2(x, y), intToBool(rnds), false, rndCount, monsterSpriteList[rnd], Random.Range(stage + 4, stage + 6));

            UIManager.Instance.inGameUI.SetStageGaugeUI();
        }
    }

    public void SetSubMonster(Vector2 _createPos, bool _isUp, int _spawnCount, int _hp)
    {
        int rnd = Random.Range(0, monsterSpriteList.Count);
        int minHp = _hp - 2;
        if (minHp < 1)
        {
            minHp = 1;
        }

        int maxHp = _hp - 1;
        if (maxHp < minHp)
        {
            maxHp = minHp + 1;
        }

        int createHp = Random.Range(minHp, maxHp);
                
        ActiveMonsterInit(false, rnd, new Vector2(_createPos.x + 0.5f, _createPos.y), false, _isUp, _spawnCount, monsterSpriteList[rnd], createHp);
        ActiveMonsterInit(false, rnd, new Vector2(_createPos.x - 0.5f, _createPos.y), true, _isUp, _spawnCount, monsterSpriteList[rnd], createHp);
    }

    public void ActiveMonsterInit(bool isLeader, int _spriteNum, Vector2 _pos, bool _isLeft, bool _isUp, int _spawnCount, Sprite _sprite, int _monsterHp)
    {
        if (monsterList[activeMonster].gameObject.activeInHierarchy == true)
        {
            return;
        }

        if (isLeader == true)
        {
            nowMonsterCount = nowMonsterCount + 1;
        }
        
        allSpawnCount = allSpawnCount + 1;
        monsterList[activeMonster].gameObject.SetActive(true);

        monsterList[activeMonster].transform.position = _pos;
        
        monsterList[activeMonster].CreateMonster(_spriteNum, _isLeft, _isUp, _spawnCount, _sprite, activeMonster + 1, _monsterHp);

        activeMonster = activeMonster + 1;

        if (activeMonster >= monsterList.Count)
        {
            activeMonster = 0;
        }
    }
    #endregion
}
