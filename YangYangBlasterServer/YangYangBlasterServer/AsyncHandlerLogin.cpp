#include "AsyncHandlerLogin.h"
#include "Cache.h"
#include "DB.h"
#include "GlobalDefine.h"

namespace yyb
{
	void AsyncHandlerLogin::OnRead(const LoginRequest& request,
		LoginReply& reply)
	{
		/*if (false == CallHttpGoogleApiTokenInfo())
		{
			reply.set_error("System error");
			return;
		}
		
		if (false == GetCacheTokenInfo(request.serial_key()))
		{
			reply.set_error("System error");
			return;
		}*/

		if (false == GetDBUserInfo(request.name(), request.serial_key()))
		{
			reply.set_error("System error");
			return;
		}
	}

	void AsyncHandlerLogin::OnWrite()
	{

	}

	bool AsyncHandlerLogin::CallHttpGoogleApiTokenInfo()
	{
		std::cout << __FUNCTION__ << " : " << status_ << std::endl;

		try
		{
			std::string host = "https://oauth2.googleapis.com/tokeninfo?id_token=";

			boost::asio::io_context ioc;
			boost::asio::ip::tcp::resolver resolver(ioc);
			boost::beast::tcp_stream stream(ioc);
			auto const results = resolver.resolve(host, "443");
			stream.connect(results);

			int version = 10;

			boost::beast::http::request<boost::beast::http::string_body> req
			{
				boost::beast::http::verb::get, "/", version
			};
			req.set(boost::beast::http::field::host, host);
			req.set(boost::beast::http::field::user_agent, BOOST_BEAST_VERSION_STRING);

			boost::beast::http::write(stream, req);

			boost::beast::flat_buffer buffer;

			// Declare a container to hold the response
			boost::beast::http::response<boost::beast::http::dynamic_body> res;

			// Receive the HTTP response
			boost::beast::http::read(stream, buffer, res);

			// Write the message to standard out
			std::cout << res << std::endl;

			// Gracefully close the socket
			boost::beast::error_code ec;
			stream.socket().shutdown(boost::asio::ip::tcp::socket::shutdown_both, ec);

			// not_connected happens sometimes
			// so don't bother reporting it.
			//
			if (ec && ec != boost::beast::errc::not_connected)
				throw boost::beast::system_error{ ec };
		}
		catch (std::exception const& e)
		{
			std::cerr << "Standard error: " << e.what() << std::endl;
			return false;
		}
		catch (...)
		{
			std::cerr << "Some other error" << std::endl;
			return false;
		}
	}

	bool AsyncHandlerLogin::GetCacheTokenInfo(const std::string& serial_key)
	{
		auto cache = Cache::Instance().GetCache(CACHE_INDEX_GLOBAL);
		if (!cache)
		{
			return false;
		}
		else if (false == cache->is_connected())
		{
			if (cache->is_reconnecting())
			{
				std::cerr << "Cache is reconnecting..." << std::endl;
			}

			return false;
		}

		auto set = cache->set(CACHE_KEY_USER_ID_TOKEN, serial_key);
		auto get = cache->get(CACHE_KEY_USER_ID_TOKEN);

		cache->sync_commit();

		std::cout << get.get().as_string() << std::endl;

		return true;
	}

	bool AsyncHandlerLogin::GetDBUserInfo(const std::string& name,
		const std::string& serial_key)
	{
		auto pool = DB::Instance().GetDBConnectionPool(DB_POOL_INDEX_GLOBAL);
		if (pool)
		{
			soci::session sql(*pool);

			try
			{
				int usn = 0;

				sql << "SELECT usn FROM user WHERE user_id=:user_id LIMIT 1",
					soci::into(usn), soci::use(name, "user_id");

				if (0 == usn)
				{
					std::cout << "User not exists" << std::endl;

					/*std::string source_name = request.name();
					std::string utf8_name =
						boost::locale::conv::to_utf<char>(source_name, "utf8");*/

					std::string user_id = name;
					std::string country_code = "KR";
					int market_platform_type = MARKET_PLATFORM_TYPE_PLAY_STORE;
					std::string refresh_token = "";// serial_key;
					std::string id_token = serial_key;
					int is_deleted = 0;

					soci::statement stmt = (sql.prepare <<
						"INSERT INTO user(usn, user_id, country_code, "
						"market_platform_type, id_token, refresh_token, is_deleted) "
						"VALUES (:usn, :user_id, :country_code, :market_platform_type, "
						" :id_token, :refresh_token, :is_deleted)",
						soci::use(usn, "usn"),
						soci::use(user_id, "user_id"),
						soci::use(country_code, "country_code"),
						soci::use(market_platform_type, "market_platform_type"),
						soci::use(id_token, "id_token"),
						soci::use(refresh_token, "refresh_token"),
						soci::use(is_deleted, "is_deleted"));

					stmt.execute();

					auto affected_rows = stmt.get_affected_rows();
					if (affected_rows <= 0)
					{
						return false;
					}
				}
				else
				{
					std::cout << "User usn : " << usn << std::endl;
				}
			}
			catch (soci::mysql_soci_error const& e)
			{
				if (2013 == e.err_num_)
				{
					sql.reconnect();
				}

				std::cerr << "MySQL error: " << e.err_num_
					<< " " << e.what() << std::endl;

				return false;
			}
			catch (std::exception const& e)
			{
				std::cerr << "Standard error: " << e.what() << std::endl;
				return false;
			}
			catch (...)
			{
				std::cerr << "Some other error" << std::endl;
				return false;
			}

		}

		return true;
	}
}