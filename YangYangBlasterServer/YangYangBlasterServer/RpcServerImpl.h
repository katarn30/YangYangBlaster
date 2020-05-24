#pragma once

#include "stdafx.h"
#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"

namespace yyb
{
    using scq_ptr = std::unique_ptr<grpc::ServerCompletionQueue>;

	class RpcServerImpl final
	{
	public:
        ~RpcServerImpl();

        void Run();

	private:
        static void createHandlers(RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq);
        static void handleRpcs(RpcService::AsyncService* service, grpc::ServerCompletionQueue* cq);

        RpcService::AsyncService service_;
        std::unique_ptr<grpc::Server> server_;
        std::vector<scq_ptr> scqs_;
        boost::thread_group thread_group_;
	};
}

