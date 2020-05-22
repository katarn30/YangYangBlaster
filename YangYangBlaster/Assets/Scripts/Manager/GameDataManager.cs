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
    public Dictionary<string, MercenaryData> getMercenaryDataDic;
}

public enum MercenaryGetType
{
    Gold,
    Ruby,
    Video
}


[System.Serializable]
public struct MercenaryData
{
    public string name;
    public int damage;
    public float moveSpeed;
    public float attackSpeed;
    public Sprite catImage;
    public int level;
    public int price;
    public MercenaryGetType mercenaryGetType;
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

    [Header("Stage Data")]
    public List<StageData> stageDataList = new List<StageData>();

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

        userData.leaderData = ReadMercenaryDataList[0];
        userData.mercenaryDataList = new List<MercenaryData>();
        userData.getMercenaryDataDic = new Dictionary<string, MercenaryData>();

        PlayerManager.Instance.ChangeLeaderCat(userData.leaderData.catImage);
    }

    public void InsertMercenary(bool isInsert, MercenaryData _mercenaryData)
    {
        if (isInsert == true)
        {
            if (userData.mercenaryDataList.Count > 2)
            {
                return;
            }

            userData.mercenaryDataList.Add(_mercenaryData);
        }
        else
        {
            for (int i = 0; i < userData.mercenaryDataList.Count; i++)
            {
                if (userData.mercenaryDataList[i].name == _mercenaryData.name)
                {
                    userData.mercenaryDataList.Remove(userData.mercenaryDataList[i]);
                }
            }
        }                
    }

    public void BuyMercenary(int _num)
    {
        string key = ReadMercenaryDataList[_num].name;
        MercenaryData mercenaryData = ReadMercenaryDataList[_num];

        if (userData.getMercenaryDataDic.Count > 0 && isGetMercenaryCat(key) == true)
        {
            MercenaryData myMercenary = GetMyMercenaryData(key);
            myMercenary.level = myMercenary.level + 1;

            SetMyMercenaryData(key, myMercenary);
        }
        else
        {
            mercenaryData.level = mercenaryData.level + 1;

            userData.getMercenaryDataDic.Add(key, mercenaryData);
        }
    }

    #region Data Read
    public bool isGetMercenaryCat(string _name)
    {
        bool isResult = false;

        if (userData.getMercenaryDataDic.ContainsKey(_name) == true)
        {
            isResult = true;
        }

        return isResult;
    }

    public MercenaryData GetMyMercenaryData(string _name)
    {
        return userData.getMercenaryDataDic[_name];
    }

    public void SetMyMercenaryData(string _name, MercenaryData _mercenaryData)
    {
        userData.getMercenaryDataDic[_name] = _mercenaryData;
    }
    #endregion
}
