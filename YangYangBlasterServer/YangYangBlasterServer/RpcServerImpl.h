#pragma once

#include "stdafx.h"
#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"
#include "health_check_service.pb.h"
#include "health_check_service.grpc.pb.h"

namespace yyb
{
    using scq_ptr = std::unique_ptr<grpc::ServerCompletionQueue>;
    
	class RpcServerImpl final
	{
	public:
        RpcServerImpl() : work_(io_service_) {}
        ~RpcServerImpl();

        void Run();

	private:
        static void createHandlers(RpcService::AsyncService* service, 
            grpc::ServerCompletionQueue* cq, boost::asio::io_service* io_service);
        static void handleRpcs(RpcService::AsyncService* service, 
            grpc::ServerCompletionQueue* cq, boost::asio::io_service* io_service);

        RpcService::AsyncService service_;
        std::unique_ptr<grpc::Server> server_;
        std::vector<scq_ptr> scqs_;
        boost::thread_group thread_group_;
        boost::asio::io_service io_service_;
        boost::asio::io_service::work work_;
	};

    //class SyncSendMessageVerifier : public grpc::experimental::Interceptor {
    //public:
    //    SyncSendMessageVerifier(grpc::experimental::ServerRpcInfo* info) {}

    //    void Intercept(grpc::experimental::InterceptorBatchMethods* methods) override;

    //private:
    //    //grpc::health::v1::HealthCheckRequest new_msg_;
    //};

    //class SyncSendMessageVerifierFactory
    //    : public grpc::experimental::ServerInterceptorFactoryInterface {
    //public:
    //    virtual grpc::experimental::Interceptor* CreateServerInterceptor(
    //        grpc::experimental::ServerRpcInfo* info) override {
    //        return new SyncSendMessageVerifier(info);
    //    }
    //};
}