#include "stdafx.h"
#include "RpcServiceImpl.h"
#include "DB.h"
#include "GlobalDefine.h"

namespace yyb
{
	grpc::Status RpcServiceImpl::RpcServiceExample(grpc_impl::ServerContext* context,
		const RpcServiceExampleRequest* request, RpcServiceExampleReply* reply)
	{
		std::cout << __FUNCTION__ << std::endl;

		// request input
		int arg1 = request->arg1();
		float arg2 = request->arg2();
		std::string arg3 = request->arg3();
		bool arg4 = request->arg4();

		for (int i = 0; i < request->arg5_size(); ++i)
		{
			int element = request->arg5(i);
		}

		// reply output
		reply->set_error(ERROR_CODE_OK);

		return grpc::Status::OK;
	}

	grpc::Status RpcServiceImpl::Listen(::grpc::ServerContext* context, const ::yyb::Empty* request,
		::grpc::ServerWriter< ::yyb::PushNotification>* writer)
	{
		std::string access_key;
		auto& metadata = context->client_metadata();
		auto iter = metadata.find("access_key");
		if (iter != metadata.end())
		{
			std::cout << "Start listen notifier " << iter->second << std::endl;

			access_key = iter->second.data();
		}
		else
		{
			GPR_ASSERT(false);
		}

		while (true)
		{
			auto p = TestNotifier::Instance().CreatePromise(access_key);

			std::shared_future<std::string> data = p->get_future();

			//data.wait();

			/*time_t t;
			time(&t);
			std::string ts = std::to_string(t);*/
			//std::chrono::duration<std::string> diff = 0;

			yyb::PushNotification noti;
			noti.set_payload(data.get());

			if (false == writer->Write(noti))
			{
				break;
			}
			if (false == writer->Write(noti))
			{
				break;
			}
			if (false == writer->Write(noti))
			{
				break;
			}

			std::this_thread::sleep_for(std::chrono::seconds(1));
		}

		TestNotifier::Instance().ClearPromise(access_key);

		return grpc::Status::OK;
	}

	grpc::Status RpcServiceImpl::Login(grpc_impl::ServerContext* context,
		const LoginRequest* request, LoginReply* reply)
	{
		std::cout << __FUNCTION__ << std::endl;

		try
		{
			auto pool = DB::Instance().GetDBConnectionPool(DB_POOL_INDEX_GLOBAL);
			if (pool)
			{
				soci::session sql(*pool);

				/*soci::rowset<soci::row> rs = (sql.prepare << "SELECT * FROM user");
				for (const auto& r : rs)
				{

				}*/
				std::string user_id = "";// request->name();

				int usn = 0;
				sql << "SELECT usn FROM user WHERE user_id=:user_id", 
					soci::into(usn), soci::use(user_id);

				if (0 == usn)
				{
					//reply->set_error("invalid user");
					return grpc::Status::OK;
				}
			}
		}
		catch (soci::mysql_soci_error const& e)
		{
			std::cerr << "MySQL error: " << e.err_num_
				<< " " << e.what() << std::endl;
		}
		catch (std::exception const& e)
		{
			std::cerr << "Standard error: " << e.what() << std::endl;
		}
		catch (...)
		{
			std::cerr << "Some other error" << std::endl;
		}

		//reply->set_error("login ok");
		return grpc::Status::OK;
	}
}