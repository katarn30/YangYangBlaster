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
	// 向当前模块（game 模块）注册 Hello 消息的消息处理函数 handleHello
	handler(&msg.TosChat{}, handleTosChat)
	handler(&msg.LoadGameDataRequest{}, handleLoadGameDataRequest)
	handler(&msg.SaveGameDataRequest{}, handleSaveGameDataRequest)
}

func handler(m interface{}, h interface{}) {
	skeleton.RegisterChanRPC(reflect.TypeOf(m), h)
}
func handleTosChat(args []interface{}) {
	// 收到的 Hello 消息
	m := args[0].(*msg.TosChat)
	// 消息的发送者
	a := args[1].(gate.Agent)

	// 输出收到的消息的内容
	log.Debug("hello %v", m.Name)
	fmt.Println("hello %v", m.Name)
	var err gate.Agent = nil
	if a != err {
		fmt.Println(" != nil")
	}

	//给发送者回应一个 Hello 消息
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
		var msgMercenary = msg.Mercenary{
			MercenaryName:  mercenary.MercenaryName,
			MercenaryLevel: int32(mercenary.MercenaryLevel),
		}

		msgMercenaries = append(msgMercenaries, &msgMercenary)
	}

	msgStage = &msg.Stage{
		StageNum:   int32(stage.StageNum),
		StageScore: int64(stage.StageScore),
	}

	msgUpgradePlayer = &msg.UpgradePlayer{
		PowerLevel:        int32(upgradePlayer.PowerLevel),
		AttackSpeedLevel:  int32(upgradePlayer.AttackSpeedLevel),
		CriticalLevel:     int32(upgradePlayer.CriticalLevel),
		BuffDurationLevel: int32(upgradePlayer.BuffDurationLevel),
		FreeCoinLevel:     int32(upgradePlayer.FreeCoinLevel),
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
		var dbMercenary = Mercenary{
			Usn:            user.Usn,
			MercenaryName:  mercenary.MercenaryName,
			MercenaryLevel: uint(mercenary.MercenaryLevel),
		}

		dbMercenaries = append(dbMercenaries, dbMercenary)
	}

	if m.Stage != nil {
		dbStage = Stage{
			Usn:        user.Usn,
			StageNum:   uint(m.Stage.StageNum),
			StageScore: uint64(m.Stage.StageScore),
		}
	}

	if m.UpgradePlayer != nil {
		dbUpgradePlayer = UpgradePlayer{
			Usn:               user.Usn,
			PowerLevel:        uint(m.UpgradePlayer.PowerLevel),
			AttackSpeedLevel:  uint(m.UpgradePlayer.AttackSpeedLevel),
			CriticalLevel:     uint(m.UpgradePlayer.CriticalLevel),
			BuffDurationLevel: uint(m.UpgradePlayer.BuffDurationLevel),
			FreeCoinLevel:     uint(m.UpgradePlayer.FreeCoinLevel),
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
