package mysql

import (
	"github.com/jinzhu/gorm"
	_ "github.com/jinzhu/gorm/dialects/mysql"

	"fmt"
)

var (
	db *gorm.DB
)

func OpenDB() {
	fmt.Println("mysqldb->open db")
	db1, err := gorm.Open("mysql", "satel:369369@tcp(192.168.1.5:3306)/YYB?charset=utf8mb4&parseTime=true")
	if err != nil {
		panic("connect db error")
	}
	db1.DB().SetMaxIdleConns(10)
	db1.DB().SetMaxOpenConns(10)

	db = db1
}

func MysqlDB() *gorm.DB {
	return db
}
