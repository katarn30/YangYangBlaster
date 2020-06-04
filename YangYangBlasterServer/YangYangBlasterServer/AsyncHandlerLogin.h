#pragma once

#include "AsyncHandler.h"

namespace yyb
{
	class User;

	class AsyncHandlerLogin :
		public AsyncHandler<LoginRequest, LoginReply>
	{
	public:
		void OnRead(const LoginRequest& request,
			LoginReply& reply) override;

		void OnWrite() override;

		bool CallGoogleVerifyOauth2Token(const std::string& idToken);
		bool GetCacheTokenInfo(const std::string& serial_ey);
		bool IsNickNameDuplication(const std::string& nickName);
		bool DoesNickNameHaveSpecialCharacters(const std::string& nickName);
		bool CreateUser(const std::string& nickName,
			int loginType, const std::string& countryCode,
			int marketPlatformType, OUT User& outUser);
		bool GetUser(const std::string& loginKey, OUT User& outUser);
	};
}