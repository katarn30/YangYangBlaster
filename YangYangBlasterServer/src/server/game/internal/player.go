package internal

import (
	"server/msg"
	"time"

	"github.com/name5566/leaf/gate"
	g "github.com/name5566/leaf/go"
	"github.com/name5566/leaf/log"
	"github.com/name5566/leaf/timer"
	"github.com/name5566/leaf/util"
)

var (
	playerID2Player = make(map[uint]*Player)
)

const (
	userLogin = iota
	userLogout
	userGame
)

type Player struct {
	gate.Agent
	*g.LinearContext
	state       int
	saveDBTimer *timer.Timer
	//초기화 이후 스스로 함수를 써서 BaseInfo와 CardsInfo를 읽어라, 저장 정보도 마찬가지이다.
	//ORM의 레벨링으로 여러 가지 습관을 발견하려고 했습니다.
	//아니면 스스로 코드를 써서 통제하에서 논리를 갖추는 것이 낫고 코드가 몇 줄도 더 늘지 않는다.
	playerBaseInfo *PlayerBaseInfo

	//...기타 정보
}

func (player *Player) login(playerID uint) {

	playerBaseInfo := new(PlayerBaseInfo)
	player.playerBaseInfo = playerBaseInfo

	skeleton.Go(func() {
		err := playerBaseInfo.initValue(playerID)
		if err != nil {
			log.Error("init acc %v data error: %v", playerID, err)
			playerBaseInfo = nil
			player.WriteMsg(&msg.LoginFaild{Code: msg.LoginFaild_InnerError})
			player.Close()
			return
		}
	}, func() {
		// network closed
		if player.state == userLogout {
			player.logout(playerID)
			return
		}

		// db error
		player.state = userGame
		if playerBaseInfo == nil {
			return
		}

		// ok
		player.playerBaseInfo = playerBaseInfo
		playerID2Player[playerID] = player
		//player.UserData().(*AgentInfo).userID = userData.UserID
		player.onLogin()
		player.autoSaveDB()
	})

}

func CreatePlayer(playerID uint) error {
	err := CreatePlayerBaseInfo(playerID)
	return err
}

func (player *Player) isOffline() bool {
	return player.state == userLogout
}

func (player *Player) logout(playerID uint) {

}

func (player *Player) autoSaveDB() {
	const duration = 5 * time.Minute
	// save
	player.saveDBTimer = skeleton.AfterFunc(duration, func() {
		data := util.DeepClone(player.playerBaseInfo)
		player.Go(func() {
			err := data.(*PlayerBaseInfo).saveValue()
			if err != nil {
				log.Error("save user %v data error: %v", player.playerBaseInfo.PlayerID, err)
			}

		}, func() {
			player.autoSaveDB()
		})
	})
}

func (player *Player) onLogin() {

}

func (player *Player) onLogout() {

}
