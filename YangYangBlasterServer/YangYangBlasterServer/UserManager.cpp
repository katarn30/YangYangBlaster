#include "UserManager.h"
#include "User.h"

namespace yyb
{
	UserManager& UserManager::Instance()
	{
		static UserManager userManager;
		return userManager;
	}

	void UserManager::AddUser(int usn, user_ptr user)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		//if (users_.find(usn) == users_.end())
		//{
			users_.emplace(usn, user);
		//}
	}

	user_ptr UserManager::GetUser(int usn)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		if (users_.find(usn) != users_.end())
		{
			return users_[usn];
		}

		return user_ptr();
	}

	void UserManager::DeleteUser(int usn)
	{
		std::lock_guard<std::mutex> lock(mtx_);

		if (users_.find(usn) != users_.end())
		{
			users_.erase(usn);
		}
	}
}