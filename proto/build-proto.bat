protoc -I=./ --csharp_out=../YangYangBlaster/Assets/Scripts/SocketProtobuf/Net/proto rpc_service.proto

protoc -I=./ --go_out=../YangYangBlasterServer/src/server/msg rpc_service.proto

pause