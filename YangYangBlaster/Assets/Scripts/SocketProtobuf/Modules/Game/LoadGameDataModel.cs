using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;

public class LoadGameDataModel : BaseModel<LoadGameDataModel>
{
    protected override void InitAddTocHandler()
    {
        AddTocHandler(typeof(LoadGameDataReply), STocLoadGameDataReply);
    }

    private void STocLoadGameDataReply(object data)
    {
        LoadGameDataReply reply = data as LoadGameDataReply;

        if (ERROR_CODE.Ok == reply.Error)
        {
            Debug.Log("Ok");

            foreach (var item in reply.Items)
            {
                switch (item.ItemType)
                {
                case ITEM_TYPE.Gold:
                    GameDataManager.Instance.userData.userCurrency.userCoin = (int)item.ItemCount;
                    break;
                case ITEM_TYPE.Ruby:
                    GameDataManager.Instance.userData.userCurrency.userRubby = (int)item.ItemCount;
                    break;
                case ITEM_TYPE.PieceKnight:
                    GameDataManager.Instance.userData.userCurrency.knightPiece = (int)item.ItemCount;
                    break;
                case ITEM_TYPE.PiecePirate:
                    GameDataManager.Instance.userData.userCurrency.piratePiece = (int)item.ItemCount;
                    break;
                case ITEM_TYPE.PieceStar:
                    GameDataManager.Instance.userData.userCurrency.starPiece = (int)item.ItemCount;
                    break;
                case ITEM_TYPE.PieceScientist:
                    GameDataManager.Instance.userData.userCurrency.scientistPiece = (int)item.ItemCount;
                    break;
                case ITEM_TYPE.PieceStudent:
                    GameDataManager.Instance.userData.userCurrency.studentPiece = (int)item.ItemCount;
                    break;
                }
            }

            if (null != GameDataManager.Instance.userData.getMercenaryDataDic)
            {
                GameDataManager.Instance.userData.getMercenaryDataDic.Clear();

                foreach (var mercenary in reply.Mercenaries)
                {
                    MercenaryData mercenaryData = new MercenaryData();
                    mercenaryData.name = mercenary.MercenaryName;
                    mercenaryData.level = mercenary.MercenaryLevel;

                    GameDataManager.Instance.userData.getMercenaryDataDic.Add(
                        mercenary.MercenaryName, mercenaryData);
                }
            }

            GameDataManager.Instance.userData.stageNum = reply.Stage.StageNum;
            GameDataManager.Instance.userData.score = (int)reply.Stage.StageScore;

            GameDataManager.Instance.userData.upgradePlayer.powerLevel = reply.UpgradePlayer.PowerLevel;
            GameDataManager.Instance.userData.upgradePlayer.attackSpeedLevel = reply.UpgradePlayer.AttackSpeedLevel;
            GameDataManager.Instance.userData.upgradePlayer.criticalLevel = reply.UpgradePlayer.CriticalLevel;
            GameDataManager.Instance.userData.upgradePlayer.buffDurationLevel = reply.UpgradePlayer.BuffDurationLevel;
            GameDataManager.Instance.userData.upgradePlayer.freeCoinLevel = reply.UpgradePlayer.FreeCoinLevel;

            GameObject gameObject = GameObject.Find("LobbyCanvas(Clone)");
            gameObject.SendMessage("UpdateCoinText");
            gameObject.SendMessage("UpdateScoreText");
        }
        else
        {
            Debug.Log(reply.Error);
        }
    }

    public void CTosLoadGameDataRequest()
    {
        LoadGameDataRequest request = new LoadGameDataRequest();

        SendTos(request);
    }
}
