using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Yyb;

[System.Serializable]
public struct UserData
{
    public string nickName;
    public int stageNum;
    public int score;
    public UserCurrency userCurrency;
    public DateTime freeCoinGetTime;
    public DateTime freeCoinUpdateTime;
    public UpgradePlayer upgradePlayer;
    public List<MilkItem> milkItemList;   

    public MercenaryData leaderData;
    public List<MercenaryData> mercenaryDataList;
    public Dictionary<string, MercenaryData> getMercenaryDataDic;

    public LoginRequest.Types.LOGIN_TYPE loginType;
    public string loginKey;
    public string accessKey;
}

[Serializable]
public struct UpgradePlayer
{
    public int powerLevel;
    public int powerIncrease;
    public int powerPrice;

    public int attackSpeedLevel;
    public float attackSpeedIncrease;
    public int attackSpeedPrice;

    public int criticalLevel;
    public float criticalIncrease;
    public int criticalPrice;

    public int buffDurationLevel;
    public float buffDurationIncrease;
    public int buffDurationPrice;

    public int freeCoinLevel;
    public int freeCoinIncrease;
    public int freeCoinPrice;
}

public enum MilkType
{
    SLOW,
    FREZE,
    SPEED,
    POWER,
    MONEY,
    GIANT,
    SHIELD
}

[Serializable]
public struct MilkItem //
{
    public MilkType type;
    public Sprite milkSprite;
    public int milkLevel;
    public float milkDuration;
    public string milkInfo;
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

public struct UserCurrency
{
    public int userCoin;
    public int userRubby;

    public int knightPiece;
    public int piratePiece;
    public int starPiece;
    public int scientistPiece;
    public int studentPiece;

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

    public const string PREFIX_PREFS = "yyb_";

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    //protected override void OnApplicationQuit()
    //{
    //    SaveGameData();

    //    base.OnApplicationQuit();
    //}

    public void LoadGameData()
    {
        var request = new GameDataRequest();
        RpcServiceManager.Instance.LoadGameData(request, (GameDataReply reply)=>
        {
            foreach (var item in reply.Items)
            {
                switch (item.ItemType)
                {
                    case ITEM_TYPE.Gold:
                        userData.userCurrency.userCoin = item.ItemCount;
                        break;
                    case ITEM_TYPE.Ruby:
                        userData.userCurrency.userRubby = item.ItemCount;
                        break;
                    case ITEM_TYPE.PieceKnight:
                        userData.userCurrency.knightPiece = item.ItemCount;
                        break;
                    case ITEM_TYPE.PiecePirate:
                        userData.userCurrency.piratePiece = item.ItemCount;
                        break;
                    case ITEM_TYPE.PieceStar:
                        userData.userCurrency.starPiece = item.ItemCount;
                        break;
                    case ITEM_TYPE.PieceScientist:
                        userData.userCurrency.scientistPiece = item.ItemCount;
                        break;
                    case ITEM_TYPE.PieceStudent:
                        userData.userCurrency.studentPiece = item.ItemCount;
                        break;
                }
            }

            if (null != userData.getMercenaryDataDic)
            {
                userData.getMercenaryDataDic.Clear();

                foreach (var mercenary in reply.Mercenaries)
                {
                    MercenaryData mercenaryData = new MercenaryData();
                    mercenaryData.name = mercenary.MercenaryName;
                    mercenaryData.level = mercenary.MercenaryLevel;

                    userData.getMercenaryDataDic.Add(
                        mercenary.MercenaryName, mercenaryData);
                }
            }

            userData.stageNum = reply.Stage.StageNum;
            userData.score = (int)reply.Stage.StageScore;

            userData.upgradePlayer.powerLevel = reply.UpgradePlayer.PowerLevel;
            userData.upgradePlayer.attackSpeedLevel = reply.UpgradePlayer.AttackSpeedLevel;
            userData.upgradePlayer.criticalLevel = reply.UpgradePlayer.CriticalLevel;
            userData.upgradePlayer.buffDurationLevel = reply.UpgradePlayer.BuffDurationLevel;
            userData.upgradePlayer.freeCoinLevel = reply.UpgradePlayer.FreeCoinLevel;
        });
    }

    public void SaveGameDataItem()
    {
        var request = new GameDataRequest();

        if (0 < userData.userCurrency.userCoin)
        {
            Item item = new Item();
            item.ItemType = ITEM_TYPE.Gold;
            item.ItemName = "코인";
            item.ItemCount = userData.userCurrency.userCoin;

            request.Items.Add(item);
        }
        if (0 < userData.userCurrency.userRubby)
        {
            Item item = new Item();
            item.ItemType = ITEM_TYPE.Ruby;
            item.ItemName = "루비";
            item.ItemCount = userData.userCurrency.userRubby;

            request.Items.Add(item);
        }
        if (0 < userData.userCurrency.knightPiece)
        {
            Item item = new Item();
            item.ItemType = ITEM_TYPE.PieceKnight;
            item.ItemName = "기사 조각";
            item.ItemCount = userData.userCurrency.knightPiece;

            request.Items.Add(item);
        }
        if (0 < userData.userCurrency.piratePiece)
        {
            Item item = new Item();
            item.ItemType = ITEM_TYPE.PiecePirate;
            item.ItemName = "해적 조각";
            item.ItemCount = userData.userCurrency.piratePiece;

            request.Items.Add(item);
        }
        if (0 < userData.userCurrency.starPiece)
        {
            Item item = new Item();
            item.ItemType = ITEM_TYPE.PieceStar;
            item.ItemName = "스타 조각";
            item.ItemCount = userData.userCurrency.starPiece;

            request.Items.Add(item);
        }
        if (0 < userData.userCurrency.scientistPiece)
        {
            Item item = new Item();
            item.ItemType = ITEM_TYPE.PieceScientist;
            item.ItemName = "과학자 조각";
            item.ItemCount = userData.userCurrency.scientistPiece;

            request.Items.Add(item);
        }
        if (0 < userData.userCurrency.studentPiece)
        {
            Item item = new Item();
            item.ItemType = ITEM_TYPE.PieceStudent;
            item.ItemName = "학생 조각";
            item.ItemCount = userData.userCurrency.studentPiece;

            request.Items.Add(item);
        }

        RpcServiceManager.Instance.SaveGameData(request, (GameDataReply reply) =>
        {
            if (ERROR_CODE.Ok == reply.Error)
            {
                Debug.Log("Ok");
            }
            else
            {
                Debug.Log(reply.Error);
            }
        });
    }

    public void SaveGameDataMercenary()
    {
        var request = new GameDataRequest();

        if (null != userData.getMercenaryDataDic)
        {
            foreach (var pair in userData.getMercenaryDataDic)
            {
                Mercenary mercenary = new Mercenary();
                mercenary.MercenaryName = pair.Key;
                mercenary.MercenaryLevel = pair.Value.level;

                request.Mercenaries.Add(mercenary);
            }
        }

        RpcServiceManager.Instance.SaveGameData(request, (GameDataReply reply) =>
        {
            if (ERROR_CODE.Ok == reply.Error)
            {
                Debug.Log("Ok");
            }
            else
            {
                Debug.Log(reply.Error);
            }
        });
    }

    public void SaveGameDataStage()
    {
        var request = new GameDataRequest();

        request.Stage.StageNum = userData.stageNum;
        request.Stage.StageScore = userData.score;

        RpcServiceManager.Instance.SaveGameData(request, (GameDataReply reply) =>
        {
            if (ERROR_CODE.Ok == reply.Error)
            {
                Debug.Log("Ok");
            }
            else
            {
                Debug.Log(reply.Error);
            }
        });
    }

    public void SaveGameDataUpgradePlayer()
    {
        var request = new GameDataRequest();

        request.UpgradePlayer.PowerLevel = userData.upgradePlayer.powerLevel;
        request.UpgradePlayer.AttackSpeedLevel = userData.upgradePlayer.attackSpeedLevel;
        request.UpgradePlayer.CriticalLevel = userData.upgradePlayer.criticalLevel;
        request.UpgradePlayer.BuffDurationLevel = userData.upgradePlayer.buffDurationLevel;
        request.UpgradePlayer.FreeCoinLevel = userData.upgradePlayer.freeCoinLevel;

        RpcServiceManager.Instance.SaveGameData(request, (GameDataReply reply) =>
        {
            if (ERROR_CODE.Ok == reply.Error)
            {
                Debug.Log("Ok");
            }
            else
            {
                Debug.Log(reply.Error);
            }
        });
    }

    public void SetUserData()
    {
        //LoadUserDataLoginParts();

        //userData.nickName = "멍뭉이는멍뭉";
        if (userData.loginKey.Equals("") && userData.nickName.Equals(""))
        {
            userData.nickName = GenerateTempUserNickName("user");
        }

        userData.userCurrency.userCoin = 0;
        userData.userCurrency.userRubby = 0;
        userData.userCurrency.knightPiece = 0;
        userData.userCurrency.piratePiece = 0;
        userData.userCurrency.starPiece = 0;
        userData.userCurrency.scientistPiece = 0;
        userData.userCurrency.studentPiece = 0;
        userData.stageNum = 1;
        userData.score = 0;
        SetFreeCoinInfo();

        userData.leaderData = ReadMercenaryDataList[0];
        userData.mercenaryDataList = new List<MercenaryData>();
        userData.getMercenaryDataDic = new Dictionary<string, MercenaryData>();

        PlayerManager.Instance.ChangeLeaderCat(userData.leaderData.catImage);
    }

    public static string GenerateTempUserNickName(string prefix)
    {
        return string.Format("{0}#{1:N}", prefix, Guid.NewGuid());
    }

    public void LoadUserDataLoginParts()
    {
        userData.loginKey = PlayerPrefs.GetString(PREFIX_PREFS + "login_key", "");
        userData.loginType = (LoginRequest.Types.LOGIN_TYPE)PlayerPrefs.GetInt(
            PREFIX_PREFS + "login_type",
            (int)LoginRequest.Types.LOGIN_TYPE.NonCert);
        userData.nickName = PlayerPrefs.GetString(PREFIX_PREFS + "nick_name", "");
    }

    public void SaveUserDataLoginParts()
    {
        PlayerPrefs.SetString(PREFIX_PREFS + "login_key", userData.loginKey);
        PlayerPrefs.SetInt(PREFIX_PREFS + "login_type", (int)userData.loginType);
        PlayerPrefs.SetString(PREFIX_PREFS + "nick_name", userData.nickName);
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

        userData.userCurrency.userCoin = userData.userCurrency.userCoin - catPrice;
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

        if (userData.userCurrency.userCoin >= _coin)
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

    public float GetPlayerUpgradeAttackSpeed()
    {
        float result = 0;

        result = userData.upgradePlayer.attackSpeedLevel * userData.upgradePlayer.attackSpeedIncrease;

        return result;
    }

    public int GetPlayerUpgradeAttackSpeedPrice()
    {
        int result = 0;

        result = userData.upgradePlayer.attackSpeedLevel * userData.upgradePlayer.attackSpeedPrice;

        return result;
    }
    
    public float getPlayerCritical()
    {
        float result = 0.0f;

        result = (userData.upgradePlayer.criticalLevel * userData.upgradePlayer.criticalIncrease);

        return result;
    }

    public float GetPlayerUpgradeCritical()
    {
        float result = 0;

        result = userData.upgradePlayer.criticalLevel * userData.upgradePlayer.criticalIncrease;

        return result;
    }

    public int GetPlayerUpgradeCriticalPrice()
    {
        int result = 0;

        result = userData.upgradePlayer.criticalLevel * userData.upgradePlayer.criticalPrice;

        return result;
    }

    public float GetPlayerUpgradeBuffSKill()
    {
        float result = 0;

        result = (userData.upgradePlayer.buffDurationLevel * userData.upgradePlayer.buffDurationIncrease);

        return result;
    }

    public float GetPlayerUpgradeMilkSKill(MilkType _type)
    {
        float result = 0;

        result = userData.milkItemList[(int)_type].milkDuration + (userData.upgradePlayer.buffDurationLevel * userData.upgradePlayer.buffDurationIncrease);

        return result;
    }

    public int GetPlayerUpgradeMilkSkillPrice()
    {
        int result = 0;

        result = userData.upgradePlayer.buffDurationLevel * userData.upgradePlayer.buffDurationPrice;

        return result;
    }

    public int GetPlayerUpgradeFreeCoin()
    {
        int result = 0;

        result = freeCoin + (userData.upgradePlayer.freeCoinLevel * userData.upgradePlayer.freeCoinIncrease);

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
        userData.userCurrency.userCoin = userData.userCurrency.userCoin - _price;

        userData.upgradePlayer.powerLevel++;
    }

    public void SetUpgradeAttackSpeed(int _price)
    {
        userData.userCurrency.userCoin = userData.userCurrency.userCoin - _price;

        userData.upgradePlayer.attackSpeedLevel++;
    }

    public void SetUpgradeCritical(int _price)
    {
        userData.userCurrency.userCoin = userData.userCurrency.userCoin - _price;

        userData.upgradePlayer.criticalLevel++;
    }

    public void SetUpgradeSkillDamage(int _price)
    {
        userData.userCurrency.userCoin = userData.userCurrency.userCoin - _price;

        userData.upgradePlayer.buffDurationLevel++;
    }

    public void SetUpgradeFreeCoin(int _price)
    {
        userData.userCurrency.userCoin = userData.userCurrency.userCoin - _price;

        userData.upgradePlayer.freeCoinLevel++;
    }

    public bool isUpgrade(int _price)
    {
        bool result = false;

        if (userData.userCurrency.userCoin >= _price)
        {
            result = true;
        }

        return result;
    }
    #endregion
}
