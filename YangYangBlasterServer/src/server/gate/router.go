package gate

import (
	"server/game"
	"server/login"
	"server/msg"
)

func init() {
	// 여기서 지정된 소식 Hello 라우터에서 game 블록까지
	// 모듈 간 ChanRPC 통신 사용, 메시지 라우팅도 예외가 아님
	//msg.Processor.SetRouter(&msg.TosChat{}, game.ChanRPC)
	//msg.Processor.SetRouter(&msg.Login{}, login.ChanRPC)
	msg.Processor.SetRouter(&msg.LoginRequest{}, login.ChanRPC)
	msg.Processor.SetRouter(&msg.LoadGameDataRequest{}, game.ChanRPC)
	msg.Processor.SetRouter(&msg.SaveGameDataRequest{}, game.ChanRPC)
}
