using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingleTon<MonsterManager>
{
    public List<Sprite> monsterSpriteList = new List<Sprite>();

    public int monsterHp = 0;
    public int monsterStageCount = 0;
    int nowMonsterCount = 0;

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

    public void SetInGameInit()
    {
        if (MonsterParent != null)
        {
            Destroy(MonsterParent.gameObject);
            MonsterParent = null;
        }

        if (MonsterParent == null)
        {
            MonsterParent = new GameObject().transform;
            MonsterParent.parent = transform;
            MonsterParent.name = "MonsterParent";

            MonsterParent.position = Vector3.zero;

            nowMonsterCount = 0;
        }               
    }

    public void MonsterManagerUpdate()
    {
        if (GameManager.Instance.isGameOver == true)
            return;

        if (monsterStageCount == nowMonsterCount)
        {
            if (MonsterParent.childCount == 0)
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

            GameObject go = Instantiate(monster.gameObject, MonsterParent);
            go.transform.position = new Vector2(x, y);

            int rnd = Random.Range(0, monsterSpriteList.Count);
            int rnds = Random.Range(0, 2);
            int rndCount = Random.Range(1, 3);

            go.GetComponent<Monster>().CreateMonster(intToBool(rnds), false, rndCount, monsterSpriteList[rnd], MonsterParent.childCount, Random.Range(monsterHp, monsterHp + 5));
        }
    }

    public void CreateMonster(Vector2 _createPos, bool _isUp, int _spawnCount, int _hp)
    {
        int rnd = Random.Range(0, monsterSpriteList.Count);
        int createHp = Random.Range(_hp - 4, _hp);

        GameObject go = Instantiate(monster.gameObject, MonsterParent);
        go.transform.position = new Vector2(_createPos.x + 0.5f, _createPos.y);
        go.GetComponent<Monster>().CreateMonster(false, _isUp, _spawnCount, monsterSpriteList[rnd], MonsterParent.childCount, createHp);        


        GameObject go1 = Instantiate(monster.gameObject, MonsterParent);
        go1.transform.position = new Vector2(_createPos.x - 0.5f, _createPos.y);
        go1.GetComponent<Monster>().CreateMonster(true, _isUp, _spawnCount, monsterSpriteList[rnd], MonsterParent.childCount, createHp);        
    }
}
