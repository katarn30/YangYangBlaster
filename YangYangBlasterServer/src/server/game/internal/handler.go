package internal

import (
	"fmt"
	"reflect"
	"server/common"
	"server/msg"
	"server/mysql"

	"github.com/jinzhu/gorm"
	"github.com/name5566/leaf/gate"
	"github.com/name5566/leaf/log"
)

func init() {
	// 현재 모듈(게임 모듈)에 Hello 소식을 등록하는 소식 처리 함수 handleHello
	handler(&msg.TosChat{}, handleTosChat)
	handler(&msg.LoadGameDataRequest{}, handleLoadGameDataRequest)
	handler(&msg.SaveGameDataRequest{}, handleSaveGameDataRequest)
}

func handler(m interface{}, h interface{}) {
	skeleton.RegisterChanRPC(reflect.TypeOf(m), h)
}
func handleTosChat(args []interface{}) {
	// 받은 Hello 소식
	m := args[0].(*msg.TosChat)
	// 소식의 발송자
	a := args[1].(gate.Agent)

	// 받은 소식의 내용을 출력하다
	log.Debug("hello %v", m.Name)
	fmt.Println("hello %v", m.Name)
	var err gate.Agent = nil
	if a != err {
		fmt.Println(" != nil")
	}

	//보낸 사람에게 하나의 Hello 메시지 응답
	a.WriteMsg(&msg.TocChat{
		Name:    m.Name,
		Content: m.Content,
	})
}

func handleLoadGameDataRequest(args []interface{}) {
	m := args[0].(*msg.LoadGameDataRequest)
	a := args[1].(gate.Agent)

	log.Debug("%v", m)
	log.Debug("%v", a)

	user := a.UserData().(*common.User)

	var usn = user.Usn
	var items []Item
	var mercenaries []Mercenary
	var stage Stage
	var upgradePlayer UpgradePlayer

	db := mysql.MysqlDB()

	if err := GetItems(db, uint(usn), &items); err != nil {

		a.WriteMsg(&msg.LoadGameDataReply{Error: msg.ERROR_CODE_FAILED_TO_LOAD_ITEM})
		log.Debug("Login failed ERROR_CODE_FAILED_TO_LOAD_ITEM")
		return
	}

	if err := GetMercenaries(db, uint(usn), &mercenaries); err != nil {
		a.WriteMsg(&msg.LoadGameDataReply{Error: msg.ERROR_CODE_FAILED_TO_LOAD_MERCENARY})
		log.Debug("Login failed ERROR_CODE_FAILED_TO_LOAD_MERCENARY")
		return
	}

	if err := GetStage(db, uint(usn), &stage); err != nil {
		a.WriteMsg(&msg.LoadGameDataReply{Error: msg.ERROR_CODE_FAILED_TO_LOAD_STAGE})
		log.Debug("Login failed ERROR_CODE_FAILED_TO_LOAD_STAGE")
		return
	}

	if err := GetUpgradePlayer(db, uint(usn), &upgradePlayer); err != nil {
		a.WriteMsg(&msg.LoadGameDataReply{
			Error: msg.ERROR_CODE_FAILED_TO_LOAD_UPGRADE_PLAYER})
		log.Debug("Login failed ERROR_CODE_FAILED_TO_LOAD_UPGRADE_PLAYER")
		return
	}

	var msgItems []*msg.Item
	var msgMercenaries []*msg.Mercenary
	var msgStage *msg.Stage
	var msgUpgradePlayer *msg.UpgradePlayer

	for _, item := range items {
		var msgItem = msg.Item{
			ItemName:     item.ItemName,
			ItemType:     item.ItemType,
			ItemCategory: item.ItemCategory,
			ItemCount:    int64(item.ItemCount),
		}

		msgItems = append(msgItems, &msgItem)
	}

	for _, mercenary := range mercenaries {
		var fixedMercenaryLevel int32 = int32(mercenary.MercenaryLevel)
		if 0 == fixedMercenaryLevel {
			fixedMercenaryLevel = 1
		}

		var msgMercenary = msg.Mercenary{
			MercenaryName:  mercenary.MercenaryName,
			MercenaryLevel: fixedMercenaryLevel,
		}

		msgMercenaries = append(msgMercenaries, &msgMercenary)
	}

	var fixedStageNum int32 = int32(stage.StageNum)
	if 0 == fixedStageNum {
		fixedStageNum = 1
	}

	msgStage = &msg.Stage{
		StageNum:   fixedStageNum,
		StageScore: int64(stage.StageScore),
	}

	var fixedPowerLevel int32 = int32(upgradePlayer.PowerLevel)
	var fixedAttackSpeedLevel int32 = int32(upgradePlayer.AttackSpeedLevel)
	var fixedCriticalLevel int32 = int32(upgradePlayer.CriticalLevel)
	var fixedBuffDurationLevel int32 = int32(upgradePlayer.BuffDurationLevel)
	var fixedFreeCoinLevel int32 = int32(upgradePlayer.FreeCoinLevel)

	if 0 == fixedPowerLevel {
		fixedPowerLevel = 1
	}
	if 0 == fixedAttackSpeedLevel {
		fixedAttackSpeedLevel = 1
	}
	if 0 == fixedCriticalLevel {
		fixedCriticalLevel = 1
	}
	if 0 == fixedBuffDurationLevel {
		fixedBuffDurationLevel = 1
	}
	if 0 == fixedFreeCoinLevel {
		fixedFreeCoinLevel = 1
	}

	msgUpgradePlayer = &msg.UpgradePlayer{
		PowerLevel:        fixedPowerLevel,
		AttackSpeedLevel:  fixedAttackSpeedLevel,
		CriticalLevel:     fixedCriticalLevel,
		BuffDurationLevel: fixedBuffDurationLevel,
		FreeCoinLevel:     fixedFreeCoinLevel,
	}

	var reply = msg.LoadGameDataReply{
		Error:         msg.ERROR_CODE_OK,
		Items:         msgItems,
		Mercenaries:   msgMercenaries,
		Stage:         msgStage,
		UpgradePlayer: msgUpgradePlayer,
	}

	a.WriteMsg(&reply)
	log.Debug("Loaded gmae data")
	return
}

func handleSaveGameDataRequest(args []interface{}) {
	m := args[0].(*msg.SaveGameDataRequest)
	a := args[1].(gate.Agent)

	log.Debug("%v", m)
	log.Debug("%v", a)

	user := a.UserData().(*common.User)

	var dbItems []Item
	var dbMercenaries []Mercenary
	var dbStage Stage
	var dbUpgradePlayer UpgradePlayer

	for _, item := range m.Items {
		var dbItem = Item{
			Usn:          user.Usn,
			ItemName:     item.ItemName,
			ItemType:     item.ItemType,
			ItemCategory: item.ItemCategory,
			ItemCount:    uint64(item.ItemCount),
		}

		dbItems = append(dbItems, dbItem)
	}

	for _, mercenary := range m.Mercenaries {
		var fixedMercenaryLevel uint = uint(mercenary.MercenaryLevel)
		if 0 == fixedMercenaryLevel {
			fixedMercenaryLevel = 1
		}

		var dbMercenary = Mercenary{
			Usn:            user.Usn,
			MercenaryName:  mercenary.MercenaryName,
			MercenaryLevel: fixedMercenaryLevel,
		}

		dbMercenaries = append(dbMercenaries, dbMercenary)
	}

	if m.Stage != nil {
		var fixedStageNum uint = uint(m.Stage.StageNum)
		if 0 == fixedStageNum {
			fixedStageNum = 1
		}

		dbStage = Stage{
			Usn:        user.Usn,
			StageNum:   fixedStageNum,
			StageScore: uint64(m.Stage.StageScore),
		}
	}

	if m.UpgradePlayer != nil {
		var fixedPowerLevel uint = uint(m.UpgradePlayer.PowerLevel)
		var fixedAttackSpeedLevel uint = uint(m.UpgradePlayer.AttackSpeedLevel)
		var fixedCriticalLevel uint = uint(m.UpgradePlayer.CriticalLevel)
		var fixedBuffDurationLevel uint = uint(m.UpgradePlayer.BuffDurationLevel)
		var fixedFreeCoinLevel uint = uint(m.UpgradePlayer.FreeCoinLevel)

		if 0 == fixedPowerLevel {
			fixedPowerLevel = 1
		}
		if 0 == fixedAttackSpeedLevel {
			fixedAttackSpeedLevel = 1
		}
		if 0 == fixedCriticalLevel {
			fixedCriticalLevel = 1
		}
		if 0 == fixedBuffDurationLevel {
			fixedBuffDurationLevel = 1
		}
		if 0 == fixedFreeCoinLevel {
			fixedFreeCoinLevel = 1
		}

		dbUpgradePlayer = UpgradePlayer{
			Usn:               user.Usn,
			PowerLevel:        fixedPowerLevel,
			AttackSpeedLevel:  fixedAttackSpeedLevel,
			CriticalLevel:     fixedCriticalLevel,
			BuffDurationLevel: fixedBuffDurationLevel,
			FreeCoinLevel:     fixedFreeCoinLevel,
		}
	}

	db := mysql.MysqlDB()

	err := db.Transaction(func(tx *gorm.DB) error {
		// do some database operations in the transaction (use 'tx' from this point, not 'db')
		if err := UpdateItems(tx, &dbItems); err != nil {
			// return any error will rollback
			return err
		}

		if err := UpdateMercenaries(tx, &dbMercenaries); err != nil {
			// return any error will rollback
			return err
		}

		if m.Stage != nil {
			if err := UpdateStage(tx, &dbStage); err != nil {
				// return any error will rollback
				return err
			}
		}

		if m.UpgradePlayer != nil {
			if err := UpdateUpgradePlayer(tx, &dbUpgradePlayer); err != nil {
				// return any error will rollback
				return err
			}
		}

		// return nil will commit
		return nil
	})

	if err != nil {
		log.Debug("%v", err)
	}
}

//func handleHello(args []interface{}) {
//	// 收到的 Hello 消息
//	m := args[0].(*msg.Hello)
//	// 消息的发送者
//	a := args[1].(gate.Agent)
//
//	// 输出收到的消息的内容
//	log.Debug("hello %v", m.Name)
//	fmt.Println("hello %v", m.Name)
//	// 给发送者回应一个 Hello 消息
//	a.WriteMsg(&msg.Hello{
//		Name: "XXXXXXXXXXXXXXXXXXX",
//	})
//}
