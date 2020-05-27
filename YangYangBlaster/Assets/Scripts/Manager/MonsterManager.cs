using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingleTon<MonsterManager>
{
    public List<Sprite> monsterSpriteList = new List<Sprite>();

    [Header("Monster Prefab")]
    public int monsterMaxSpawnCount = 0;
    List<Monster> monsterList = new List<Monster>();
    int activeMonster = 0;

    [Header("Monster Spawn")]
    public int monsterHp = 0;
    public int monsterStageCount = 0;
    public int nowMonsterCount = 0;

    [Header("Monster JudgeMent")]
    public int allSpawnCount = 0;
    public int deadCount = 0;

    public Monster monster;
    Transform MonsterParent = null;

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
            Destroy(MonsterParent.gameObject);
            MonsterParent = null;
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
        }

        activeMonster = 0;
        deadCount = 0;
        allSpawnCount = 0;
        nowMonsterCount = 0;
    }

    public void MonsterManagerUpdate()
    {
        if (GameManager.Instance.isGameOver == true)
            return;

        if (monsterStageCount == nowMonsterCount)
        {
            if (allSpawnCount == deadCount)
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

            nowMonsterCount = nowMonsterCount + 1;
            UIManager.Instance.inGameUI.SetStageGaugeUI();

            int rnd = Random.Range(0, monsterSpriteList.Count);
            int rnds = Random.Range(0, 2);
            int rndCount = Random.Range(1, 3);
            int stage = GameDataManager.Instance.userData.stageNum;

            allSpawnCount = allSpawnCount + 1;

            if (monsterMaxSpawnCount > monsterList.Count)
            {
                GameObject go = Instantiate(monster.gameObject, MonsterParent);
                go.transform.position = new Vector2(x, y);
                Monster mon = go.GetComponent<Monster>();
                mon.CreateMonster(intToBool(rnds), false, rndCount, monsterSpriteList[rnd], monsterList.Count + 1, Random.Range(stage + 6, stage + 10));

                monsterList.Add(monster);
            }
            else
            {
                ActiveMonsterInit(new Vector2(x, y), intToBool(rnds), false, rndCount, monsterSpriteList[rnd], monsterList.Count, Random.Range(stage + 6, stage + 10));
            }
        }
    }

    public void CreateMonster(Vector2 _createPos, bool _isUp, int _spawnCount, int _hp)
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
        
        allSpawnCount = allSpawnCount + 2;

        if (monsterMaxSpawnCount > monsterList.Count)
        {
            GameObject go = Instantiate(monster.gameObject, MonsterParent);
            go.transform.position = new Vector2(_createPos.x + 0.5f, _createPos.y);
            go.GetComponent<Monster>().CreateMonster(false, _isUp, _spawnCount, monsterSpriteList[rnd], MonsterParent.childCount, createHp);
        }
        else
        {
            ActiveMonsterInit(new Vector2(_createPos.x + 0.5f, _createPos.y), false, _isUp, _spawnCount, monsterSpriteList[rnd], MonsterParent.childCount, createHp);
        }

        if (monsterMaxSpawnCount > monsterList.Count)
        {
            GameObject go1 = Instantiate(monster.gameObject, MonsterParent);
            go1.transform.position = new Vector2(_createPos.x - 0.5f, _createPos.y);
            go1.GetComponent<Monster>().CreateMonster(true, _isUp, _spawnCount, monsterSpriteList[rnd], MonsterParent.childCount, createHp);
        }
        else
        {
            ActiveMonsterInit(new Vector2(_createPos.x - 0.5f, _createPos.y), true, _isUp, _spawnCount, monsterSpriteList[rnd], MonsterParent.childCount, createHp);
        }
            
    }

    public void ActiveMonsterInit(Vector2 _pos, bool _isLeft, bool _isUp, int _spawnCount, Sprite _sprite, int _sortOrder, int _monsterHp)
    {
        for (int i = activeMonster; i < monsterList.Count; i++)
        {
            if (monsterList[i].gameObject.activeInHierarchy == false)
            {
                activeMonster = i;

                if (activeMonster > monsterList.Count - 1)
                {
                    activeMonster = 0;
                }

                monsterList[i].transform.position = _pos;
                monsterList[i].CreateMonster(_isLeft, _isUp, _spawnCount, _sprite, _sortOrder, _monsterHp);

                break;
            }
        }
    }
}
