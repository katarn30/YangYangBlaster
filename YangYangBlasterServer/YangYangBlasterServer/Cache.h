#pragma once

namespace yyb
{
	using redis_client_ptr = std::shared_ptr<cpp_redis::client>;

	class Cache
	{
	public:
		static Cache& Instance();

		void Init(const std::string& host, short port);

		bool CreateCache(int cacheIndex);

		redis_client_ptr GetCache(int cacheIndex);

	private:
		Cache() : port_(0) {}

		std::string host_;
		short port_;

		std::unordered_map<int, redis_client_ptr> cacheMap_;
	};
}