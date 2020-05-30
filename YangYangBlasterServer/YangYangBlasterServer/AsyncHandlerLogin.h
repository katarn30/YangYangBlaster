#pragma once

#include "AsyncHandler.h"

namespace yyb
{
	class AsyncHandlerLogin :
		public AsyncHandler<LoginRequest, LoginReply>
	{
	public:
		void OnRead(const LoginRequest& request,
			LoginReply& reply) override;

		void OnWrite() override;

		bool CallHttpGoogleApiTokenInfo();
		bool GetCacheTokenInfo(const std::string& serial_ey);
		bool GetDBUserInfo(const std::string& name, 
			const std::string& serial_key);
	};
}