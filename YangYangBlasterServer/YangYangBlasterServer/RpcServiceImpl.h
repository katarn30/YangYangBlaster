#pragma once

#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"

namespace yyb
{
	class RpcServiceImpl final : public RpcService::Service
	{
		grpc::Status RpcServiceExample(grpc_impl::ServerContext* context,
			const RpcServiceExampleRequest* request, RpcServiceExampleReply* reply) override;

		grpc::Status Login(grpc_impl::ServerContext* context,
			const LoginRequest* request, LoginReply* reply) override;
	};
}