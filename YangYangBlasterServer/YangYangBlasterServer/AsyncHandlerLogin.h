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

		bool CallGoogleVerifyOauth2Token(const std::string& idToken, 
			OUT std::string& oustSub);
		bool GetCacheTokenInfo(const std::string& serial_ey);
		bool IsNickNameDuplication(const std::string& nickName);
		bool DoesNickNameHaveSpecialCharacters(const std::string& nickName);
		bool CreateUser(const std::string& nickName,
			int loginType, const std::string& countryCode,
			int marketPlatformType, const std::string& sub, 
			OUT User& outUser);
		bool GetUser(const std::string& loginKey, OUT User& outUser);
		bool UpdateUserLoginKey(const std::string& loginKey, 
			const std::string& sub);
		/*bool DeleteUser(const std::string& nickName,
			int loginType, const std::string& countryCode,
			int marketPlatformType, OUT User& outUser);*/
		bool UpdateUserLoginType(const std::string& loginKey,
			int loginType);
		/*bool UpdateUserAccessKey(int usn);
		bool GetUserAccessKey(int usn, OUT std::string& outAccessKey,
			OUT int outAccessKeyUpdateTime);*/
	};
}