#pragma once

#include "AsyncHandler.h"

namespace yyb
{
	class AsyncHandlerLogin :
		public AsyncHandler<LoginRequest, LoginReply>
	{
	public:
		void OnRead(const LoginRequest& request,
			LoginReply& reply) override
		{
			std::cout << __FUNCTION__ << " : " << status_ << std::endl;

			//Finish();
		}

		void OnWrite() override
		{

		}
	};
}