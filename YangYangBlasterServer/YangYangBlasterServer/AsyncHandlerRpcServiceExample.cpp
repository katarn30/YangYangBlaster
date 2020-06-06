#include "AsyncHandlerRpcServiceExample.h"

namespace yyb
{
	void AsyncHandlerRpcServiceExample::OnRead(
		const RpcServiceExampleRequest& request,
		RpcServiceExampleReply& reply)
	{
		std::cout << __FUNCTION__ << " : " << status_ << std::endl;

	}

	void AsyncHandlerRpcServiceExample::OnWrite()
	{

	}
}