#pragma once

#include "stdafx.h"

namespace yyb
{
	struct User
	{
		User() : usn_(0), loginType_(0),
			marketPlatformType_(0), accessKeyUpdateTime_(0) {}

		inline void SetAccessKey(const std::string& accessKey)
		{
			accessKey_ = accessKey;
		}
		inline const std::string& GetAccessKey() const
		{
			return accessKey_;
		}
		inline void SetAccessKeyUpdateTime(int accessKeyUpdateTime)
		{
			accessKeyUpdateTime_ = accessKeyUpdateTime;
		}
		inline int GetAccessKeyUpdateTime() const
		{
			return accessKeyUpdateTime_;
		}
		inline void SetNickName(const std::string& nickName)
		{
			nickName_ = nickName;
		}
		inline const std::string& GetNickName() const
		{
			return nickName_;
		}

		int usn_;
		int loginType_;
		int marketPlatformType_;
		int accessKeyUpdateTime_;
		std::string countryCode_;
		std::string nickName_;
		std::string loginKey_;
		std::string accessKey_;
	};
}
