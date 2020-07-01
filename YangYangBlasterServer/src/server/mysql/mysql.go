package mysql

import (
	"fmt"
	"server/conf"
	"strconv"

	"github.com/jinzhu/gorm"
	_ "github.com/jinzhu/gorm/dialects/mysql"
)

var (
	db *gorm.DB
)

func OpenDB() {
	args := (conf.Server.DBUser + ":" +
		conf.Server.DBPassword + "@tcp(" + conf.Server.DBHost + ":" +
		strconv.Itoa(conf.Server.DBPort) + ")/" + conf.Server.DBDatabase +
		"?charset=utf8mb4&parseTime=true")

	db1, err := gorm.Open("mysql", args) //"satel:369369@tcp(192.168.1.5:3306)/YYB?charset=utf8mb4&parseTime=true")
	if err != nil {
		panic("connect db error")
	}

	db1.DB().SetMaxIdleConns(10)
	db1.DB().SetMaxOpenConns(10)

	db = db1

	fmt.Println("mysqldb->open db (" + conf.Server.DBHost + ":" +
		strconv.Itoa(conf.Server.DBPort) + ")")
}

func MysqlDB() *gorm.DB {
	return db
}
