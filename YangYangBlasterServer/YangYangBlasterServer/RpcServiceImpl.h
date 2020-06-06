#pragma once

#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"

namespace yyb
{
	class RpcServiceImpl final : public RpcService::Service
	{
		grpc::Status RpcServiceExample(grpc_impl::ServerContext* context,
			const RpcServiceExampleRequest* request, RpcServiceExampleReply* reply) override;

		grpc::Status Listen(::grpc::ServerContext* /*context*/, const ::yyb::Empty* /*request*/, 
			::grpc::ServerWriter< ::yyb::PushNotification>* /*writer*/) override;

		grpc::Status Login(grpc_impl::ServerContext* context,
			const LoginRequest* request, LoginReply* reply) override;
	};

	using promise_ptr = std::shared_ptr<std::promise<std::string>>;

	class TestNotifier
	{
	private:
		TestNotifier() /*notifier_(nullptr)*/ {}

	public:
		static TestNotifier& Instance()
		{
			static TestNotifier testNotifier;
			return testNotifier;
		}

		promise_ptr CreatePromise(const std::string& key)
		{
			std::lock_guard<std::mutex> lock(mutex_);

			promise_map_[key] = std::make_shared<std::promise<std::string>>();

			return promise_map_[key];
		}

		void ClearPromise(const std::string& key)
		{
			std::lock_guard<std::mutex> lock(mutex_);

			promise_map_.erase(key);
		}

		void SetValue(const std::string& key, const std::string& value)
		{
			std::lock_guard<std::mutex> lock(mutex_);

			if (promise_map_.find(key) != promise_map_.end())
			{
				promise_map_[key]->set_value(value);
			}
		}

		void SetValueAll(const std::string& value)
		{
			std::lock_guard<std::mutex> lock(mutex_);

			for (auto iter : promise_map_)
			{
				if (iter.second)
				{
					iter.second->set_value(value);
				}
				//promise_map_[key]->set_value(value);
			}
		}

		std::mutex mutex_;
		std::unordered_map<std::string, promise_ptr> promise_map_;
	};
}