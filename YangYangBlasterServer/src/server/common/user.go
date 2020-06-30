package common

import (
	"fmt"
	"server/mysql"
	"time"

	"github.com/google/uuid"
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
	var user = User{
		LoginType:          loginType,
		NickName:           nickName,
		LoginKey:           loginKey,
		CountryCode:        countryCode,
		MarketPlatformType: marketPlatform,
		AccessKey:          accessKey,
		IsDeleted:          []uint8{0},
	}
	err := db.Create(&user).Error
	if nil != err {
		fmt.Println(err)
		return &user, false
	}

	return &user, true
}

func GetUser(loginKey string) (*User, bool) {

	var user User
	db := mysql.MysqlDB()
	//db.AutoMigrate(&UserTest{})

	err := db.Where("login_key = ?", loginKey).Limit(1).Find(&user).Error
	if nil != err {
		fmt.Println(err)
		return &user, false
	}
	fmt.Println("NickName: ", user.NickName)
	return &user, true
}

func UpdateUserLoginKey(loginKey string, sub string) bool {

	var user User
	db := mysql.MysqlDB()
	err := db.Model(&user).
		Where("login_key = ? AND is_deleted = 0", loginKey).
		Updates(User{LoginKey: sub})
	if nil != err {
		fmt.Println(err)
		return false
	}

	return true
}

func UpdateUserLoginType(loginType uint, loginKey string) bool {

	var user User
	db := mysql.MysqlDB()
	err := db.Model(&user).
		Where("login_key = ? AND is_deleted = 0", loginKey).
		Updates(User{LoginType: loginType, LoginKey: loginKey})
	if nil != err {
		fmt.Println(err)
		return false
	}

	return true
}
