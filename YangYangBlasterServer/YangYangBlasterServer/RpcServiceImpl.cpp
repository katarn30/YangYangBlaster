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
		reply->set_error("this is test error");

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
				std::string user_id = request->name();

				int usn = 0;
				sql << "SELECT usn FROM user WHERE user_id=:user_id", 
					soci::into(usn), soci::use(user_id);

				if (0 == usn)
				{
					reply->set_error("invalid user");
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

		reply->set_error("login ok");
		return grpc::Status::OK;
	}
}