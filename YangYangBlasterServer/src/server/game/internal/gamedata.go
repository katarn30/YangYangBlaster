package internal

import (
	"fmt"
	"server/msg"

	"github.com/jinzhu/gorm"
)

type Item struct {
	Usn          uint              `gorm:"type:int(10) unsigned;primary_key;not null;"`
	ItemType     msg.ITEM_TYPE     `gorm:"type:int(10) unsigned;primary_key;not null;"`
	ItemCategory msg.ITEM_CATEGORY `gorm:"type:int(10) unsigned;not null;default:0"`
	ItemName     string            `gorm:"type:varchar(44);not null"`
	ItemCount    uint64            `gorm:"type:bigint(20) unsigned;not null;default:0"`
}

type Mercenary struct {
	Usn            uint   `gorm:"type:int(10) unsigned;primary_key;not null;"`
	MercenaryName  string `gorm:"type:varchar(44);primary_key;not null"`
	MercenaryLevel uint   `gorm:"type:int(10) unsigned;not null;default:1"`
}

type Stage struct {
	Usn        uint   `gorm:"type:int(10) unsigned;primary_key;not null;"`
	StageNum   uint   `gorm:"type:int(10) unsigned;not null:default:1"`
	StageScore uint64 `gorm:"type:bigint(20) unsigned;not null;default:0"`
}

type UpgradePlayer struct {
	Usn               uint `gorm:"type:int(10) unsigned;primary_key;not null;"`
	PowerLevel        uint `gorm:"type:int(10) unsigned;not null:default:1"`
	AttackSpeedLevel  uint `gorm:"type:int(10) unsigned;not null:default:1"`
	CriticalLevel     uint `gorm:"type:int(10) unsigned;not null:default:1"`
	BuffDurationLevel uint `gorm:"type:int(10) unsigned;not null:default:1"`
	FreeCoinLevel     uint `gorm:"type:int(10) unsigned;not null:default:1"`
}

func GetItems(tx *gorm.DB, usn uint, outItems *[]Item) error {

	//var items []Item
	//db.AutoMigrate(&UserTest{})

	err := tx.Where("usn = ?", usn).Find(outItems).Error
	if nil != err {
		if gorm.IsRecordNotFoundError(err) {
			return nil
		}

		fmt.Println(err)
		return err
	}

	return nil
}

func GetMercenaries(tx *gorm.DB, usn uint,
	outMercenaries *[]Mercenary) error {

	//db.AutoMigrate(&UserTest{})

	err := tx.Where("usn = ?", usn).Find(outMercenaries).Error
	if nil != err {
		if gorm.IsRecordNotFoundError(err) {
			return nil
		}

		fmt.Println(err)
		return err
	}

	return nil
}

func GetStage(tx *gorm.DB, usn uint, outStage *Stage) error {

	//db.AutoMigrate(&UserTest{})

	err := tx.Where("usn = ?", usn).Find(outStage).Error
	if nil != err {
		if gorm.IsRecordNotFoundError(err) {
			return nil
		}

		fmt.Println(err)
		return err
	}

	return nil
}

func GetUpgradePlayer(tx *gorm.DB, usn uint,
	outUpgradePlayer *UpgradePlayer) error {

	//db.AutoMigrate(&UserTest{})

	err := tx.Where("usn = ?", usn).Limit(1).Find(outUpgradePlayer).Error
	if nil != err {
		if gorm.IsRecordNotFoundError(err) {
			return nil
		}

		fmt.Println(err)
		return err
	}

	return nil
}

func UpdateItems(tx *gorm.DB, items *[]Item) error {

	//db := mysql.MysqlDB()

	for _, item := range *items {
		ctx := tx.Model(&item).
			Where("usn = ? AND item_type = ?", item.Usn, item.ItemType).
			Updates(item)

		err := ctx.Error
		if nil != err {
			fmt.Println(err)
			return err
		}

		affected := ctx.RowsAffected
		if affected <= 0 {
			err = tx.Create(item).Error

			if nil != err {
				fmt.Println(err)
				return err
			}
		}
	}

	return nil
}

func UpdateMercenaries(tx *gorm.DB, mercenaries *[]Mercenary) error {

	//db := mysql.MysqlDB()

	for _, mercenary := range *mercenaries {
		ctx := tx.Model(&mercenary).
			Where("usn = ? AND mercenary_name = ?", mercenary.Usn, mercenary.MercenaryName).
			Updates(mercenary)

		err := ctx.Error
		if nil != err {
			fmt.Println(err)
			return err
		}

		affected := ctx.RowsAffected
		if affected <= 0 {
			err = tx.Create(mercenary).Error

			if nil != err {
				fmt.Println(err)
				return err
			}
		}
	}

	return nil
}

func UpdateStage(tx *gorm.DB, stage *Stage) error {

	//db := mysql.MysqlDB()

	ctx := tx.Model(stage).
		Where("usn = ?", stage.Usn).
		Updates(stage)

	err := ctx.Error
	if nil != err {
		fmt.Println(err)
		return err
	}

	affected := ctx.RowsAffected
	if affected <= 0 {
		err = tx.Create(stage).Error

		if nil != err {
			fmt.Println(err)
			return err
		}
	}

	return nil
}

func UpdateUpgradePlayer(tx *gorm.DB, upgradePlayer *UpgradePlayer) error {

	//db := mysql.MysqlDB()

	ctx := tx.Model(upgradePlayer).
		Where("usn = ?", upgradePlayer.Usn).
		Updates(upgradePlayer) //.RowsAffected

	err := ctx.Error
	if nil != err {
		fmt.Println(err)
		return err
	}

	affected := ctx.RowsAffected
	if affected <= 0 {
		err = tx.Create(upgradePlayer).Error

		if nil != err {
			fmt.Println(err)
			return err
		}
	}

	return nil
}
