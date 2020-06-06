#pragma once

#include "stdafx.h"

namespace yyb
{
	struct User
	{
		User() : usn_(0), loginType_(0),
			marketPlatformType_(0), accessKeyUpdateTime_(0) {}

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
