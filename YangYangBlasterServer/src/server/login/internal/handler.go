package internal

import (
	"crypto/md5"
	"encoding/hex"
	"fmt"
	"reflect"
	"server/common"
	"server/game"
	"server/msg"
	"server/mysql"

	"github.com/jinzhu/gorm"
	"github.com/name5566/leaf/gate"
	"github.com/name5566/leaf/log"
)

func handleMsg(m interface{}, h interface{}) {
	skeleton.RegisterChanRPC(reflect.TypeOf(m), h)
}

func init() {
	handleMsg(&msg.Login{}, handleAuth)
	handleMsg(&msg.LoginRequest{}, handleLoginRequest)
}
func handleAuth(args []interface{}) {
	m := args[0].(*msg.Login)
	a := args[1].(gate.Agent)

	// 길이 제한
	if len(m.Account) < 2 || len(m.Account) > 12 {
		a.WriteMsg(&msg.LoginFaild{Code: msg.LoginFaild_AccIDInvalid})
		return
	}

	account := getAccountByAccountID(m.Account)

	data := []byte(m.Passward)
	var hash = md5.Sum(data)
	password := hex.EncodeToString(hash[:])
	if nil == account {
		//not having this account,creat account
		newAccount := creatAccountByAccountIDAndPassword(m.Account, password)
		if nil != newAccount {
			game.ChanRPC.Go("CreatePlayer", a, newAccount.PlayerID)
			game.ChanRPC.Go("UserLogin", a, newAccount.PlayerID)
		} else {
			log.Debug("create account error ", m.Account)
			a.WriteMsg(&msg.LoginFaild{Code: msg.LoginFaild_InnerError})
		}

	} else {
		// match password
		if password == account.Password {
			game.ChanRPC.Go("UserLogin", a, account.PlayerID)
		} else {
			a.WriteMsg(&msg.LoginFaild{Code: msg.LoginFaild_AccountOrPasswardNotMatch})
		}
	}
}

func handleLoginRequest(args []interface{}) {
	m := args[0].(*msg.LoginRequest)
	a := args[1].(gate.Agent)

	var sub string = ""
	var user *common.User

	if m.LoginType == msg.LoginRequest_GOOGLE {
		if m.IdToken == "" {
			a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_EMPTY_ID_TOKEN})
			log.Debug("Login failed ERROR_CODE_EMPTY_ID_TOKEN")
			return
		}

		// idToken으로 구글 유저 접속 정보 확인(JWT 변조확인)
		s, ok := CallGoogleVerifyOauth2Token(m.IdToken)
		if !ok {
			a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_GOOGLE_AUTH_FAILED})
			log.Debug("Login failed ERROR_CODE_GOOGLE_AUTH_FAILED")
			return
		}

		sub = s

		log.Debug(sub)
	}

	// 로그인했던적 없음. 새 계정 생성
	if m.LoginKey == "" {
		if m.NickName == "" {
			a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_EMPTY_NICKNAME})
			log.Debug("Login failed ERROR_CODE_EMPTY_NICKNAME")
			return
		}

		// 길이 제한
		// if len(m.NickName) < 2 || len(m.NickName) > 12 {
		// 	a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_NICKNAME_HAVE_SPECIAL_CHARACTERS})
		// 	log.Debug("Login failed 닉네임 길이 제한")
		// 	return
		// }

		// 특수문자 체크

		// 중복 체크
		if common.IsNickNameDuplication(m.NickName) {
			a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_DUP_NICKNAME})
			log.Debug("Login failed ERROR_CODE_DUP_NICKNAME")
			return
		}

		// 유저 생성. 새 usn, login_key 발급
		u, ok := common.CreateUser(uint(m.LoginType), 0, m.NickName, "KR")
		if !ok {
			a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_UNABLE_TO_CREATE_USER})
			log.Debug("Login failed ERROR_CODE_UNABLE_TO_CREATE_USER")
			return
		}
		user = u
		log.Debug("Created user : %v", user.NickName)
		fmt.Println("Created user : ", user.NickName)
	} else if m.LoginType == msg.LoginRequest_GOOGLE {
		// 비인증 -> 구글로그인으로 바꿀때
		if sub != "" && sub != m.LoginKey {

			db := mysql.MysqlDB()

			err := db.Transaction(func(tx *gorm.DB) error {
				// do some database operations in the transaction (use 'tx' from this point, not 'db')
				if err := common.UpdateUserLoginType(tx, uint(m.LoginType), m.LoginKey); err != nil {
					// return any error will rollback
					a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_FAILED_TO_CHANGE_LOGIN_TYPE})
					log.Debug("Login failed ERROR_CODE_FAILED_TO_CHANGE_LOGIN_TYPE")
					return err
				}

				if err := common.UpdateUserLoginKey(tx, m.LoginKey, sub); err != nil {
					// return any error will rollback
					a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_FAILED_TO_UPDATE_LOGIN_KEY})
					log.Debug("Login failed ERROR_CODE_FAILED_TO_UPDATE_LOGIN_KEY")
					return err
				}

				// return nil will commit
				return nil
			})

			if err != nil {
				return
			}

			// 유저 정보 획득
			u, ok := common.GetUser(sub)
			if !ok {
				a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_FAILED_TO_GET_USER})
				log.Debug("Login failed ERROR_CODE_FAILED_TO_GET_USER")
				return
			}
			user = u
		} else {
			// 유저 정보 획득
			u, ok := common.GetUser(m.LoginKey)
			if !ok {
				a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_FAILED_TO_GET_USER})
				log.Debug("Login failed ERROR_CODE_FAILED_TO_GET_USER")
				return
			}
			user = u
		}
	} else {
		// 유저 정보 획득
		u, ok := common.GetUser(m.LoginKey)
		if !ok {
			a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_FAILED_TO_GET_USER})
			log.Debug("Login failed ERROR_CODE_FAILED_TO_GET_USER")
			return
		}

		user = u

		// 요청한 로그인 타입이 DB의 로그인 타입과 다르면
		if uint(m.LoginType) != user.LoginType {
			// 그런데 DB의 로그인 타입이 비인증 타입이면
			if uint(msg.LoginRequest_NON_CERT) == user.LoginType {
				// DB의 로그인 타입 갱신
				db := mysql.MysqlDB()

				if err := common.UpdateUserLoginType(db, uint(m.LoginType), m.LoginKey); err != nil {
					a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_FAILED_TO_CHANGE_LOGIN_TYPE})
					log.Debug("Login failed ERROR_CODE_FAILED_TO_CHANGE_LOGIN_TYPE")
					return
				}

				// 다시 유저 정보 획득
				u, ok := common.GetUser(m.LoginKey)
				if !ok {
					a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_FAILED_TO_GET_USER})
					log.Debug("Login failed ERROR_CODE_FAILED_TO_GET_USER")
					return
				}

				user = u
			} else {
				a.WriteMsg(&msg.LoginReply{Error: msg.ERROR_CODE_LOGIN_TYPE_IS_DIFFERENT})
				log.Debug("Login failed ERROR_CODE_LOGIN_TYPE_IS_DIFFERENT")
				return
			}
		}
	}

	var reply = msg.LoginReply{
		Error:     msg.ERROR_CODE_OK,
		Usn:       int32(user.Usn),
		NickName:  user.NickName,
		LoginKey:  user.LoginKey,
		AccessKey: user.AccessKey,
	}

	a.SetUserData(user)

	a.WriteMsg(&reply)
	log.Debug("Logged in user : ...")
	fmt.Println("Logged in user : ", user.NickName)
	return
}
