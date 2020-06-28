#include "UserManager.h"
#include "User.h"

namespace yyb
{
	UserManager& UserManager::Instance()
	{
		static UserManager userManager;
		return userManager;
	}

	void UserManager::AddUser(const std::string& accesskey, user_ptr user)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		if (user)
		{
			keyUsnMap_.emplace(accesskey, user->usn_);
			users_.emplace(user->usn_, user);
		}
	}

	user_ptr UserManager::GetUser(const std::string& accesskey)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		auto usnIter = keyUsnMap_.find(accesskey);
		if (usnIter != keyUsnMap_.end())
		{
			auto userIter = users_.find(usnIter->second);
			if (userIter != users_.end())
			{
				return userIter->second;
			}
		}

		return user_ptr();
	}

	void UserManager::DeleteUser(const std::string& accesskey)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		auto usnIter = keyUsnMap_.find(accesskey);
		if (usnIter != keyUsnMap_.end())
		{
			auto userIter = users_.find(usnIter->second);
			if (userIter != users_.end())
			{
				users_.erase(userIter);
				keyUsnMap_.erase(usnIter);
			}
		}
	}

	void UserManager::CheckUserAll()
	{
		int now = std::time(nullptr);

		std::lock_guard<std::mutex> lock(mtx_);

		auto userIter = users_.begin();
		for (userIter; userIter != users_.end();)
		{
			const auto user = userIter->second;
			if (user)
			{
				int validTime = 
					user->GetAccessKeyUpdateTime() + 20 * 60; // 20Ка

				if (validTime < now)
				{
					userIter = users_.erase(userIter);
				}
				else
				{
					++userIter;
				}
			}
			else
			{
				userIter = users_.erase(userIter);
			}
		}
	}

	void UserManager::addUser(int usn, user_ptr user)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		//if (users_.find(usn) == users_.end())
		//{
			users_.emplace(usn, user);
		//}
	}

	user_ptr UserManager::getUser(int usn)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		if (users_.find(usn) != users_.end())
		{
			return users_[usn];
		}

		return user_ptr();
	}

	void UserManager::deleteUser(int usn)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		if (users_.find(usn) != users_.end())
		{
			users_.erase(usn);
		}
	}
}