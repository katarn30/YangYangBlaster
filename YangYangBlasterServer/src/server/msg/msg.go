package msg

import (
	"github.com/name5566/leaf/network/protobuf"
)

var (
	Processor = protobuf.NewProcessor()
)

func init() { // 这里我们注册了一个 protobuf 消息)
	Processor.SetByteOrder(true)
	// Processor.Register(&StartFight{})
	// Processor.Register(&FightResult{})
	// Processor.Register(&EnterFight{})
	// Processor.Register(&SignUpResponse{})
	// Processor.Register(&TosChat{})
	// Processor.Register(&TocChat{})
	// Processor.Register(&Login{})
	// Processor.Register(&PlayerBaseInfo{})
	// Processor.Register(&LoginSuccessfull{})
	// Processor.Register(&LoginFaild{})
	Processor.Register(&RpcServiceExampleRequest{})
	Processor.Register(&RpcServiceExampleReply{})
	Processor.Register(&LoginRequest{})
	Processor.Register(&LoginReply{})
	Processor.Register(&LoadGameDataRequest{})
	Processor.Register(&LoadGameDataReply{})
	Processor.Register(&SaveGameDataRequest{})
	Processor.Register(&SaveGameDataReply{})
	Processor.Register(&RankingRequest{})
	Processor.Register(&RankingReply{})
	Processor.Register(&RankingListRequest{})
	Processor.Register(&RankingListReply{})
}
