#include "stdafx.h"
#include "Cache.h"

namespace yyb
{
	Cache& Cache::Instance()
	{
		static Cache cache;
		return cache;
	}

	void Cache::Init(const std::string& host, short port)
	{
		host_ = host;
		port_ = port;
	}

	bool Cache::CreateCache(int cacheIndex)
	{
		if (cacheMap_.find(cacheIndex) == cacheMap_.end())
		{
			redis_client_ptr cache =
				std::make_shared<cpp_redis::client>();

			cacheMap_.emplace(cacheIndex, cache);

			cache->connect(host_, port_, [](const std::string& host,
				std::size_t port, cpp_redis::client::connect_state state)
				{
					if (state == cpp_redis::client::connect_state::ok)
					{
						std::cout << "cache connected from " << host <<
							":" << port << std::endl;
					}
					if (state == cpp_redis::client::connect_state::dropped)
					{
						std::cout << "cache disconnected from " << host <<
							":" << port << std::endl;
					}
					else
					{
						std::cout << "cache disconnected from " << host <<
							":" << port << " unkown error"<< std::endl;
					}
				}, 0, -1, 5000);

			return true;
		}
		else
		{
			return false;
		}
	}

	redis_client_ptr Cache::GetCache(int cacheIndex)
	{
		if (cacheMap_.find(cacheIndex) != cacheMap_.end())
		{
			return cacheMap_[cacheIndex];
		}

		return redis_client_ptr();
	}
}