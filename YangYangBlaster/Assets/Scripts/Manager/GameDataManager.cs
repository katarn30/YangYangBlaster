using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct UserData
{
    public string nickName;
    public int stageNum;
    public int score;
    public int coin;
    public int ruby;
    public DateTime freeCoinGetTime;
    public DateTime freeCoinUpdateTime;
    public UpgradePlayer upgradePlayer;

    public MercenaryData leaderData;
    public List<MercenaryData> mercenaryDataList;
    public Dictionary<string, MercenaryData> getMercenaryDataDic;
}

[Serializable]
public struct UpgradePlayer
{
    public int powerLevel;
    public int powerIncrease;
    public int powerPrice;

    public int attackSpeedLevel;
    public int attackSpeedIncrease;
    public int attackSpeedPrice;

    public int criticalLevel;
    public int criticalIncrease;
    public int criticalPrice;

    public int skillDamageLevel;
    public int skillDamageIncrease;
    public int skillDamagePrice;

    public int freeCoinLevel;
    public int freeCoinIncrease;
    public int freeCoinPrice;
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
    public Sprite bulletImage;
    public RuntimeAnimatorController runtimeAnimator;
    public RuntimeAnimatorController uiRuntimeAnimator;
}

[System.Serializable]
public struct BossData
{
    public string name;
    public int pattenType;
    public int hp;
    public float attackTime;
    public Vector2 createPos;
    public Vector2 colliderOffset;
    public float radius;
    public Color deadColor;
    public RuntimeAnimatorController runtimeAnimator;
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
    public int freeCoin = 0;

    [Header("Mercenary Data")]
    [SerializeField]
    public List<MercenaryData> ReadMercenaryDataList = new List<MercenaryData>();    

    [Header("Stage Data")]
    public List<StageData> stageDataList = new List<StageData>();
    public List<BossData> BossDataList = new List<BossData>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetUserData()
    {
        userData.nickName = "멍뭉이는멍뭉";
        userData.coin = 0;
        userData.ruby = 0;
        userData.stageNum = 1;
        userData.score = 0;
        SetFreeCoinInfo();

        userData.leaderData = ReadMercenaryDataList[0];
        userData.mercenaryDataList = new List<MercenaryData>();
        userData.getMercenaryDataDic = new Dictionary<string, MercenaryData>();

        PlayerManager.Instance.ChangeLeaderCat(userData.leaderData.catImage);
    }

    public void SetFreeCoinInfo()
    {
        userData.freeCoinGetTime = DateTime.Now;
        userData.freeCoinUpdateTime = DateTime.Now.AddMinutes(1);
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
        int catPrice = 0;
        MercenaryData mercenaryData = ReadMercenaryDataList[_num];

        if (userData.getMercenaryDataDic.Count > 0 && isGetMercenaryCat(key) == true)
        {
            MercenaryData myMercenary = GetMyMercenaryData(key);
            myMercenary.level = myMercenary.level + 1;
            catPrice = myMercenary.price;

            SetMyMercenaryData(key, myMercenary);
        }
        else
        {
            mercenaryData.level = mercenaryData.level + 1;
            catPrice = mercenaryData.price;

            userData.getMercenaryDataDic.Add(key, mercenaryData);
        }

        userData.coin = userData.coin - catPrice;
    }

    public void SelectMercenary(MercenaryData _mercenaryData)
    {
        if (userData.mercenaryDataList.Count >= 2)
        {
            Debug.LogError("Mercenary Count is Big");
            return;
        }

        userData.mercenaryDataList.Add(_mercenaryData);
    }

    public void RemoveMercenary(MercenaryData _mercenaryData)
    {
        for (int i = 0; i < userData.mercenaryDataList.Count; i++)
        {
            if (userData.mercenaryDataList[i].name == _mercenaryData.name)
            {
                Debug.Log("Mercenary Remove : " + _mercenaryData.name);
                userData.mercenaryDataList.Remove(userData.mercenaryDataList[i]);
                break;
            }
        }        
    }

    public bool isDeckMercenary(MercenaryData _mercenaryData)
    {
        bool result = false;

        for (int i = 0; i < userData.mercenaryDataList.Count; i++)
        {
            if (userData.mercenaryDataList[i].name == _mercenaryData.name)
            {
                result = true;
                break;
            }            
        }

        return result;
    }

    //아이템 살 수 있는 지 여부 확인
    public bool isBuyItem(int _coin)
    {
        bool result = false;

        if (userData.coin >= _coin)
        {
            result = true;
        }

        return result;
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

    public float GetPlayerDamage()
    {
        float result = 0;

        result = 1 + ((float)(userData.upgradePlayer.powerLevel * userData.upgradePlayer.powerIncrease) / 100);        

        return result;
    }

    public int GetPlayerUpgradeDamage()
    {
        int result = 0;

        result = 100 + userData.upgradePlayer.powerLevel * userData.upgradePlayer.powerIncrease;

        return result;
    }

    public int GetPlayerUpgradeDamagePrice()
    {
        int result = 0;

        result = userData.upgradePlayer.powerLevel * userData.upgradePlayer.powerPrice;

        return result;
    }

    public int GetPlayerUpgradeAttackSpeed()
    {
        int result = 0;

        result = 100 + userData.upgradePlayer.attackSpeedLevel * userData.upgradePlayer.attackSpeedIncrease;

        return result;
    }

    public int GetPlayerUpgradeAttackSpeedPrice()
    {
        int result = 0;

        result = userData.upgradePlayer.attackSpeedLevel * userData.upgradePlayer.attackSpeedPrice;

        return result;
    }

    public int GetPlayerUpgradeCritical()
    {
        int result = 0;

        result = userData.upgradePlayer.criticalLevel * userData.upgradePlayer.criticalIncrease;

        return result;
    }

    public int GetPlayerUpgradeCriticalPrice()
    {
        int result = 0;

        result = userData.upgradePlayer.criticalLevel * userData.upgradePlayer.criticalPrice;

        return result;
    }

    public int GetPlayerUpgradeSkill()
    {
        int result = 0;

        result = 100 + userData.upgradePlayer.skillDamageLevel * userData.upgradePlayer.skillDamageIncrease;

        return result;
    }

    public int GetPlayerUpgradeSkillPrice()
    {
        int result = 0;

        result = userData.upgradePlayer.skillDamageLevel * userData.upgradePlayer.skillDamagePrice;

        return result;
    }

    public int GetPlayerUpgradeFreeCoin()
    {
        int result = 0;

        result = 100 + userData.upgradePlayer.freeCoinLevel * userData.upgradePlayer.freeCoinIncrease;

        return result;
    }

    public int GetPlayerUpgradeFreeCoinPrice()
    {
        int result = 0;

        result = userData.upgradePlayer.freeCoinLevel * userData.upgradePlayer.freeCoinPrice;

        return result;
    }

    public void SetUpgradeDamage(int _price)
    {
        userData.coin = userData.coin - _price;

        userData.upgradePlayer.powerLevel++;
    }

    public void SetUpgradeAttackSpeed(int _price)
    {
        userData.coin = userData.coin - _price;

        userData.upgradePlayer.attackSpeedLevel++;
    }

    public void SetUpgradeCritical(int _price)
    {
        userData.coin = userData.coin - _price;

        userData.upgradePlayer.criticalLevel++;
    }

    public void SetUpgradeSkillDamage(int _price)
    {
        userData.coin = userData.coin - _price;

        userData.upgradePlayer.skillDamageLevel++;
    }

    public void SetUpgradeFreeCoin(int _price)
    {
        userData.coin = userData.coin - _price;

        userData.upgradePlayer.freeCoinLevel++;
    }

    public bool isUpgrade(int _price)
    {
        bool result = false;

        if (userData.coin >= _price)
        {
            result = true;
        }

        return result;
    }
    #endregion
}
