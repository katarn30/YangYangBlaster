#pragma once

#include "AsyncHandler.h"

namespace yyb
{
	class AsyncHandlerRpcServiceExample 
		: public AsyncHandler<RpcServiceExampleRequest, RpcServiceExampleReply>
	{
    public:
		void OnRead(const RpcServiceExampleRequest& request,
			RpcServiceExampleReply& reply) override;

		void OnWrite() override;
	};
}