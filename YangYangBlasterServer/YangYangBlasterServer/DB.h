#pragma once

namespace yyb
{
	using connection_pool_ptr = std::shared_ptr<soci::connection_pool>;

	class DB
	{
	public:
		static DB& Instance();
		
		void Init(size_t poolSize,
			const std::string& db, const std::string& host, short port,
			const std::string& user, const std::string& password,
			const std::string& tz = "+00:00");

		bool CreateDBConnectionPool(int poolIndex);

		connection_pool_ptr GetDBConnectionPool(int poolIndex);

		void SetDB(std::string& db) { db_ = db; }

	private:
		DB() : poolSize_(0), port_(0) {}

		size_t poolSize_;
		std::string db_;
		std::string host_;
		short port_;
		std::string user_;
		std::string password_;
		std::string tz_;

		std::unordered_map<int, connection_pool_ptr> dbMap_;
	};
}