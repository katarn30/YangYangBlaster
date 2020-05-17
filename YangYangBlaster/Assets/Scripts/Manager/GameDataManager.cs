using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserData
{
    public string nickName;
    public int stageNum;
    public int score;
    public int gold;
    public int ruby;

    public MercenaryData leaderData;
    public List<MercenaryData> mercenaryDataList;
}

[System.Serializable]
public struct MercenaryData
{
    public string name;
    public int damage;
    public float moveSpeed;
    public float attackSpeed;
    public Sprite catImage;
}

[System.Serializable]
public struct StageData
{
    public int stageNum;
    public int minHp;
    public int maxHp;
    public int divideCount;
    public int spawnTime;
    public int spawnCount;
}

public class GameDataManager : SingleTon<GameDataManager>
{
    [Header("User Data")]
    [SerializeField]
    public UserData userData;

    [Header("Mercenary Data")]
    [SerializeField]
    public List<MercenaryData> ReadMercenaryDataList = new List<MercenaryData>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetUserData()
    {
        userData.nickName = "멍뭉이는멍뭉";
        userData.gold = 0;
        userData.ruby = 0;
        userData.stageNum = 1;
        userData.score = 0;

        int ran = Random.Range(0, ReadMercenaryDataList.Count);
        userData.leaderData = ReadMercenaryDataList[ran];

        PlayerManager.Instance.ChangeLeaderCat(userData.leaderData.catImage);
    }

    public void BuyMercenary()
    {
        int ran = Random.Range(0, ReadMercenaryDataList.Count);
        userData.mercenaryDataList.Add(ReadMercenaryDataList[ran]);
    }
}
