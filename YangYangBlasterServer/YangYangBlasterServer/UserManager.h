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

		void AddUser(const std::string& accesskey, user_ptr user);
		user_ptr GetUser(const std::string& accesskey);
		void DeleteUser(const std::string& accesskey);
		void CheckUserAll();

	private:
		UserManager() {}

		void addUser(int usn, user_ptr user);
		user_ptr getUser(int usn);
		void deleteUser(int usn);
		
		std::mutex mtx_;
		// <access_key, usn>
		std::unordered_map<std::string, int> keyUsnMap_;
		// <usn, user_ptr>
		std::unordered_map<int, user_ptr> users_;
	};
}

