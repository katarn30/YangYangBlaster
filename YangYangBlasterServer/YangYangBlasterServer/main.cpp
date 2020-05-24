// YangYangBlasterServer.cpp : 이 파일에는 'main' 함수가 포함됩니다. 거기서 프로그램 실행이 시작되고 종료됩니다.
//

#include "stdafx.h"
#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"
#include "GlobalDefine.h"
#include "DB.h"
#include "RpcServiceImpl.h"
#include "RpcServerImpl.h"

namespace yyb
{
    void InitDB()
    {
        const size_t poolSize = 
            static_cast<size_t>(std::thread::hardware_concurrency()) * 2;
        std::string db = "YYB";
        std::string host = "192.168.1.5";
        short port = 3306;
        std::string user = "satel";
        std::string password = "369369";

        DB::Instance().Init(poolSize, db, host, port, user, password);
        DB::Instance().CreateDBConnectionPool(DB_POOL_INDEX_GLOBAL);
    }

    void RunServer() {
        std::string server_address("0.0.0.0:20051");
        RpcServiceImpl service;

        grpc::EnableDefaultHealthCheckService(true);
        grpc::reflection::InitProtoReflectionServerBuilderPlugin();
        grpc::ServerBuilder builder;
        // Listen on the given address without any authentication mechanism.
        builder.AddListeningPort(server_address, grpc::InsecureServerCredentials());
        // Register "service" as the instance through which we'll communicate with
        // clients. In this case it corresponds to an *synchronous* service.
        builder.RegisterService(&service);
        //auto cq = builder.AddCompletionQueue();
        /*grpc::ResourceQuota rq;
        rq.SetMaxThreads(static_cast<int>(std::thread::hardware_concurrency()) * 2);
        builder.SetResourceQuota(rq);*/
        // Finally assemble the server.
        std::unique_ptr<grpc::Server> server(builder.BuildAndStart());
        std::cout << "Server listening on " << server_address << std::endl;

        // Wait for the server to shutdown. Note that some other thread must be
        // responsible for shutting down the server for this call to ever return.
        server->Wait();
    }
}

int main()
{
    /*boost::asio::io_service io_service;
    boost::asio::ip::tcp::socket socket(io_service);*/

    yyb::InitDB();

    //yyb::RunServer();
    yyb::RpcServerImpl server;
    server.Run();


    return 0;
}