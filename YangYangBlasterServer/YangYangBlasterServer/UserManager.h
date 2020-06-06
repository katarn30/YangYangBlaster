#pragma once

#include "stdafx.h"

namespace yyb
{
	class User;
	using user_ptr = std::shared_ptr<User>;

	class UserManager
	{
	public:
		static UserManager& Instance();

		void AddUser(int usn, user_ptr user);
		user_ptr GetUser(int usn);
		void DeleteUser(int usn);

	private:
		UserManager() {}

		std::mutex mtx_;
		// <usn, user_ptr>
		std::unordered_map<int, user_ptr> users_;
	};
}

