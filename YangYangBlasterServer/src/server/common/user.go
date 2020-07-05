package common

import (
	"errors"
	"fmt"
	"server/mysql"
	"time"

	"github.com/google/uuid"
	"github.com/jinzhu/gorm"
)

type User struct {
	Usn                 uint      `gorm:"type:int(10) unsigned;primary_key;not null;AUTO_INCREMENT"`
	NickName            string    `gorm:"type:varchar(44);not null;unique"`
	LoginKey            string    `gorm:"type:varchar(64);null"`
	LoginType           uint      `gorm:"type:tinyint(3) unsigned;not null"`
	CountryCode         string    `gorm:"type:char(2);null"`
	MarketPlatformType  uint      `gorm:"type:int(10) unsigned;null"`
	LastLoginTime       time.Time `gorm:"type:timestamp;not null;default:CURRENT_TIMESTAMP"`
	AccessKey           string    `gorm:"type:varchar(64);null"`
	AccessKeyUpdateTime time.Time `gorm:"type:timestamp;not null;default:CURRENT_TIMESTAMP"`
	IsDeleted           []uint8   `gorm:"type:bit(1);not null;default:b'0'"`
	RegDate             time.Time `gorm:"type:timestamp;not null;default:CURRENT_TIMESTAMP"`
}

func IsNickNameDuplication(nickName string) bool {

	var user User
	db := mysql.MysqlDB()
	//db.AutoMigrate(&UserTest{})

	err := db.Where("nick_name = ?", nickName).Limit(1).Find(&user).Error
	if nil != err {
		fmt.Println(err)
		return false
	}
	fmt.Println("password:", user.NickName)
	return true
}

func CreateUser(loginType uint, marketPlatform uint,
	nickName string, countryCode string) (*User, bool) {

	loginKey := uuid.New().String()
	accessKey := uuid.New().String()

	db := mysql.MysqlDB()
	user := new(User)
	user.LoginType = loginType
	user.NickName = nickName
	user.LoginKey = loginKey
	user.CountryCode = countryCode
	user.MarketPlatformType = marketPlatform
	user.AccessKey = accessKey
	user.IsDeleted = []uint8{0}

	err := db.Create(user).Error
	if nil != err {
		fmt.Println(err)
		return user, false
	}

	return user, true
}

func GetUser(loginKey string) (*User, bool) {

	user := new(User)
	db := mysql.MysqlDB()
	//db.AutoMigrate(&UserTest{})

	err := db.Where("login_key = ?", loginKey).Limit(1).Find(user).Error
	if nil != err {
		fmt.Println(err)
		return user, false
	}

	return user, true
}

func UpdateUserLoginKey(tx *gorm.DB, loginKey string, sub string) error {

	var user User
	//db := mysql.MysqlDB()
	ctx := tx.Model(&user).
		Where("login_key = ? AND is_deleted = 0", loginKey).
		Updates(User{LoginKey: sub})

	affected := ctx.RowsAffected
	if affected <= 0 {
		return errors.New("RowsAffected 0")
	}

	return ctx.Error
}

func UpdateUserLoginType(tx *gorm.DB, loginType uint, loginKey string) error {

	var user User
	//db := mysql.MysqlDB()

	ctx := tx.Model(&user).
		Where("login_key = ? AND is_deleted = 0", loginKey).
		Updates(User{LoginType: loginType, LoginKey: loginKey})

	affected := ctx.RowsAffected
	if affected <= 0 {
		return errors.New("RowsAffected 0")
	}

	return ctx.Error
}
