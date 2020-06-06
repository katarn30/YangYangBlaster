#include "AsyncHandlerLogin.h"
#include "Cache.h"
#include "DB.h"
#include "Python.h"
#include "GlobalDefine.h"
#include "UserManager.h"
#include "User.h"

namespace yyb
{
	void AsyncHandlerLogin::OnRead(const LoginRequest& request,
		LoginReply& reply)
	{
		// request 중복 처리

		User user;
		std::string sub = "";

		if (LoginRequest::LOGIN_TYPE_GOOGLE == request.logintype())
		{
			if (request.idtoken().empty())
			{
				reply.set_error(LoginReply::ERROR_CODE_EMPTY_ID_TOKEN);
				return;
			}

			// idToken으로 구글 유저 접속 정보 확인
			if (false == CallGoogleVerifyOauth2Token(request.idtoken(), sub))
			{
				reply.set_error(LoginReply::ERROR_CODE_GOOGLE_AUTH_FAILED);
				return;
			}
		}

		// 로그인했던적 없음. 새 계정 생성
		if (request.loginkey().empty())
		{
			// 유저 새로 만들어야하는데 닉네임이 없음..
			if (request.nickname().empty())
			{
				reply.set_error(LoginReply::ERROR_CODE_EMPTY_NICKNAME);
				return;
			}

			// 특수문자 체크
			if (DoesNickNameHaveSpecialCharacters(request.nickname()))
			{
				reply.set_error(
					LoginReply::ERROR_CODE_NICKNAME_HAVE_SPECIAL_CHARACTERS);
				return;
			}

			// nickName 중복 체크
			if (IsNickNameDuplication(request.nickname()))
			{
				reply.set_error(LoginReply::ERROR_CODE_DUP_NICKNAME);
				return;
			}

			// 유저 생성. 새 usn, login_key 발급
			if (false == CreateUser(request.nickname(), request.logintype(),
				"KR", 0, sub, user))
			{
				reply.set_error(LoginReply::ERROR_CODE_UNABLE_TO_CREATE_USER);
				return;
			}
		}
		else if (LoginRequest::LOGIN_TYPE_GOOGLE == request.logintype())
		{
			// 비인증 -> 구글로그인으로 바꿀때
			if (false == sub.empty() && request.loginkey() != sub)
			{
				// 로그인키를 구글 아이디로 업데이트
				if (false == UpdateUserLoginKey(request.loginkey(), sub))
				{
					reply.set_error(
						LoginReply::ERROR_CODE_FAILED_TO_UPDATE_LOGIN_KEY);
					return;
				}

				// DB의 로그인 타입 갱신
				if (false == UpdateUserLoginType(sub,
					request.logintype()))
				{
					reply.set_error(
						LoginReply::ERROR_CODE_FAILED_TO_CHANGE_LOGIN_TYPE);
					return;
				}

				// 유저 정보 획득
				if (false == GetUser(sub, user))
				{
					reply.set_error(
						LoginReply::ERROR_CODE_FAILED_TO_ACQUIRE_USER_INFO);
					return;
				}
			}
			else
			{
				// 유저 정보 획득
				if (false == GetUser(request.loginkey(), user))
				{
					reply.set_error(
						LoginReply::ERROR_CODE_FAILED_TO_ACQUIRE_USER_INFO);
					return;
				}
			}
		}
		else
		{
			// 유저 정보 획득
			if (false == GetUser(request.loginkey(), user))
			{
				reply.set_error(
					LoginReply::ERROR_CODE_FAILED_TO_ACQUIRE_USER_INFO);
				return;
			}

			// 요청한 로그인 타입이 DB의 로그인 타입과 다르면
			if (request.logintype() != user.loginType_)
			{
				// 그런데 DB의 로그인 타입이 비인증 타입이면
				if (LoginRequest::LOGIN_TYPE_NON_CERT == user.loginType_)
				{
					// DB의 로그인 타입 갱신
					if (false == UpdateUserLoginType(request.loginkey(),
						request.logintype()))
					{
						reply.set_error(
							LoginReply::ERROR_CODE_FAILED_TO_CHANGE_LOGIN_TYPE);
						return;
					}

					// 다시 유저 정보 획득
					if (false == GetUser(request.loginkey(), user))
					{
						reply.set_error(
							LoginReply::ERROR_CODE_FAILED_TO_ACQUIRE_USER_INFO);
						return;
					}
				}
				else
				{
					reply.set_error(
						LoginReply::ERROR_CODE_LOGIN_TYPE_IS_DIFFERENT);
					return;
				}
			}
		}

		//if (LoginRequest::LOGIN_TYPE_NON_CERT == request.logintype())
		//{
		//	// LOGINTYPE == 0

		//	// usn == 0 이면 DB에 새 계정 입력하고 usn 발급. 아니면 DB 조회
		//	if (0 == request.usn() || request.loginkey().empty())
		//	{
		//		// 특수문자 체크
		//		if (DoesNickNameHaveSpecialCharacters(request.nickname()))
		//		{
		//			reply.set_error(
		//				LoginReply::ERROR_CODE_NICKNAME_HAVE_SPECIAL_CHARACTERS);
		//			return;
		//		}

		//		// nickName 중복 체크
		//		if (IsNickNameDuplication(request.nickname()))
		//		{
		//			reply.set_error(LoginReply::ERROR_CODE_DUP_NICKNAME);
		//			return;
		//		}

		//		// 유저 생성. 새 usn, login_key 발급
		//		if (false == CreateUser(request.nickname(), request.logintype(),
		//			"KR", 0, user))
		//		{
		//			reply.set_error(LoginReply::ERROR_CODE_UNABLE_TO_CREATE_USER);
		//			return;
		//		}
		//	}
		//	else
		//	{
		//		// 유저 정보 획득
		//		if (false == GetUser(request.usn(), request.nickname(), user))
		//		{
		//			reply.set_error(
		//				LoginReply::ERROR_CODE_FAILED_TO_ACQUIRE_USER_INFO);
		//			return;
		//		}
		//	}
		//}
		//else if (LoginRequest::LOGIN_TYPE_GOOGLE == request.logintype())
		//{
		//	// LOGINTYPE == 1
		//	// usn == 0 이면 DB에 새 계정 입력하고 usn 발급.아니면 DB 조회
		//	if (0 == request.usn())
		//	{
		//		// 특수문자 체크
		//		if (DoesNickNameHaveSpecialCharacters(request.nickname()))
		//		{
		//			reply.set_error(
		//				LoginReply::ERROR_CODE_NICKNAME_HAVE_SPECIAL_CHARACTERS);
		//			return;
		//		}

		//		// idToken으로 구글 유저 접속 정보 확인
		//		if (false == CallHttpGoogleApiTokenInfo(request.idtoken()))
		//		{
		//			reply.set_error(LoginReply::ERROR_CODE_GOOGLE_AUTH_FAILED);
		//			return;
		//		}

		//		// nickName 중복 체크
		//		if (IsNickNameDuplication(request.nickname()))
		//		{
		//			reply.set_error(LoginReply::ERROR_CODE_DUP_NICKNAME);
		//			return;
		//		}

		//		// 유저 생성. 새 usn 발급
		//		if (false == CreateUser(request.nickname(), request.logintype(),
		//			"KR", 0, user))
		//		{
		//			reply.set_error(LoginReply::ERROR_CODE_UNABLE_TO_CREATE_USER);
		//			return;
		//		}
		//	}
		//	else
		//	{
		//		// idToken으로 구글 유저 접속 정보 확인
		//		if (false == CallHttpGoogleApiTokenInfo(request.idtoken()))
		//		{
		//			reply.set_error(LoginReply::ERROR_CODE_GOOGLE_AUTH_FAILED);
		//			return;
		//		}

		//		// 유저 정보 획득
		//		if (false == GetUser(request.usn(), request.nickname(), user))
		//		{
		//			reply.set_error(
		//				LoginReply::ERROR_CODE_FAILED_TO_ACQUIRE_USER_INFO);
		//			return;
		//		}
		//	}
		//}
		//else if (LoginRequest::LOGIN_TYPE_FACEBOOK == request.logintype())
		//{

		//}
		//else
		//{
		//	// logintype error
		//}

		// 유저정보 획득하여 메모리에 올림
		UserManager::Instance().AddUser(user.usn_,
			std::make_shared<User>(user));

		// 결과 리턴
		reply.set_error(LoginReply::ERROR_CODE_OK);
		reply.set_usn(user.usn_);
		reply.set_nickname(user.nickName_);
		reply.set_loginkey(user.loginKey_);
		reply.set_accesskey(user.accessKey_);
		return;

		// 토큰으로 구글 유저 접속 정보 확인
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

		/*std::cout << "name : " << request.name() << 
			", id_token : " << request.serial_key() << std::endl;*/

		// DB에 user 정보 존재하는지 확인하고 있으면 얻어옴
		/*if (false == GetDBUserInfo(request.name(), request.serial_key()))
		{
			reply.set_error("System error");
			return;
		}*/
	}

	void AsyncHandlerLogin::OnWrite()
	{

	}

	bool AsyncHandlerLogin::CallGoogleVerifyOauth2Token(const std::string& idToken, 
		OUT std::string& oustSub)
	{
		std::cout << __FUNCTION__ << " : " << status_ << std::endl;
		std::cout << idToken << std::endl;

		if (false == Python::Instance().GoogleVerifyOauth2Token(idToken, oustSub))
		{
			return false;
		}

		//return true;

		//try
		//{
		//	std::string host = "oauth2.googleapis.com";//"https://oauth2.googleapis.com/tokeninfo?id_token="
		//		//+ idToken;

		//	boost::asio::io_context ioc;
		//	boost::asio::ip::tcp::resolver resolver(ioc);
		//	boost::beast::tcp_stream stream(ioc);
		//	auto const results = resolver.resolve(host, "443");
		//	stream.connect(results);

		//	int version = 10;

		//	boost::beast::http::request<boost::beast::http::string_body> req
		//	{
		//		boost::beast::http::verb::get, "/tokeninfo?id_token=" 
		//		+ idToken, version
		//	};
		//	req.set(boost::beast::http::field::host, host);
		//	req.set(boost::beast::http::field::user_agent, BOOST_BEAST_VERSION_STRING);

		//	boost::beast::http::write(stream, req);

		//	boost::beast::flat_buffer buffer;

		//	// Declare a container to hold the response
		//	boost::beast::http::response<boost::beast::http::dynamic_body> res;

		//	// Receive the HTTP response
		//	boost::beast::http::read(stream, buffer, res);

		//	// Write the message to standard out
		//	std::cout << res << std::endl;

		//	// Gracefully close the socket
		//	boost::beast::error_code ec;
		//	stream.socket().shutdown(boost::asio::ip::tcp::socket::shutdown_both, ec);

		//	// not_connected happens sometimes
		//	// so don't bother reporting it.
		//	//
		//	if (ec && ec != boost::beast::errc::not_connected)
		//		throw boost::beast::system_error{ ec };
		//}
		//catch (std::exception const& e)
		//{
		//	std::cerr << "Standard error: " << e.what() << std::endl;
		//	return false;
		//}
		//catch (...)
		//{
		//	std::cerr << "Some other error" << std::endl;
		//	return false;
		//}

		return true;
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

	bool AsyncHandlerLogin::IsNickNameDuplication(const std::string& nickName)
	{
		int usn = 0;

		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,// [] (soci::session& sql){return true; });
			[&](soci::session& sql)
			{
				sql << "SELECT usn FROM user "
					"WHERE nick_name=:nick_name AND is_deleted=0 LIMIT 1",
					soci::into(usn), soci::use(nickName, "nick_name");

				if (0 < usn)
				{
					// duplication!!
					return true;
				}

				return false;
			});

		return queryResult;
	}

	bool AsyncHandlerLogin::DoesNickNameHaveSpecialCharacters(
		const std::string& nickName)
	{
		return false;
	}

	bool AsyncHandlerLogin::CreateUser(const std::string& nickName, 
		int loginType, const std::string& countryCode,
		int marketPlatformType, const std::string& sub,
		OUT User& outUser)
	{
		int isDeleted = 0;
		int accessKeyUpdateTime = 0;
		boost::uuids::uuid uuid = boost::uuids::random_generator()();
		std::string loginKey = "";

		if (sub.empty())
		{
			loginKey = boost::lexical_cast<std::string>(uuid);
		}
		else
		{
			loginKey = sub;
		}

		boost::uuids::uuid uuid2 = boost::uuids::random_generator()();
		std::string accessKey = boost::lexical_cast<std::string>(uuid2);
		
		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"INSERT INTO user(nick_name, login_type, country_code, "
					"access_key, access_key_update_time, "
					"login_key, market_platform_type, is_deleted) "
					"VALUES (:nick_name, :login_type, :country_code, "
					":access_key, NOW(), "
					":login_key, :market_platform_type, :is_deleted)",
					soci::use(nickName, "nick_name"),
					soci::use(loginType, "login_type"),
					soci::use(loginKey, "login_key"),
					soci::use(countryCode, "country_code"),
					soci::use(accessKey, "access_key"),
					soci::use(marketPlatformType, "market_platform_type"),
					soci::use(isDeleted, "is_deleted"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				int usn = 0;

				sql << "SELECT usn, UNIX_TIMESTAMP(access_key_update_time) "
					"FROM user "
					"WHERE nick_name=:nick_name LIMIT 1",
					soci::into(usn), soci::into(accessKeyUpdateTime),
					soci::use(nickName, "nick_name");

				if (0 == usn)
				{
					return false;
				}

				outUser.usn_ = usn;
				outUser.nickName_ = nickName;
				outUser.loginType_ = loginType;
				outUser.loginKey_ = loginKey;
				outUser.countryCode_ = countryCode;
				outUser.marketPlatformType_ = marketPlatformType;
				outUser.accessKey_ = accessKey;
				outUser.accessKeyUpdateTime_ = accessKeyUpdateTime;

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerLogin::GetUser(const std::string& loginKey, OUT User& outUser)
	{
		int usn = 0;
		/*LoginRequest::LOGIN_TYPE loginType =
			LoginRequest::LOGIN_TYPE_NON_CERT;*/
		int loginType = 0;
		std::string nickName = "";
		std::string countryCode = "";
		int marketPlatformType = 0;
		int accessKeyUpdateTime = 0;

		boost::uuids::uuid uuid = boost::uuids::random_generator()();
		std::string accessKey = boost::lexical_cast<std::string>(uuid);

		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare << 
					"UPDATE user SET access_key=:access_key, "
					"access_key_update_time=NOW() "
					"WHERE login_key=:login_key AND is_deleted=0 LIMIT 1",
					soci::use(accessKey, "access_key"),
					soci::use(loginKey, "login_key"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		if (queryResult)
		{
			queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
				[&](soci::session& sql)
				{
					sql << "SELECT usn, nick_name, login_type, country_code, "
						"market_platform_type, "//access_key, "
						"UNIX_TIMESTAMP(access_key_update_time) "
						"FROM user "
						"WHERE login_key=:login_key AND is_deleted=0 LIMIT 1",
						soci::into(usn),
						soci::into(nickName),
						soci::into(loginType),
						soci::into(countryCode),
						soci::into(marketPlatformType),
						soci::into(accessKeyUpdateTime),
						soci::use(loginKey, "login_key");

					if (0 == usn)
					{
						return false;
					}

					outUser.usn_ = usn;
					outUser.nickName_ = nickName;
					outUser.loginType_ = loginType;
					outUser.loginKey_ = loginKey;
					outUser.countryCode_ = countryCode;
					outUser.marketPlatformType_ = marketPlatformType;
					outUser.accessKey_ = accessKey;
					outUser.accessKeyUpdateTime_ = accessKeyUpdateTime;

					return true;
				});
		}

		return queryResult;
	}

	bool AsyncHandlerLogin::UpdateUserLoginKey(const std::string& loginKey,
		const std::string& sub)
	{
		int isDeleted = 0;

		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"UPDATE user SET login_key=:sub "
					"WHERE login_key=:login_key",
					soci::use(sub, "sub"),
					soci::use(loginKey, "login_key"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return queryResult;
	}

	bool AsyncHandlerLogin::UpdateUserLoginType(const std::string& loginKey,
		int loginType)
	{
		bool queryResult = DB::QueryScope(DB_POOL_INDEX_GLOBAL,
			[&](soci::session& sql)
			{
				soci::statement stmt = (sql.prepare <<
					"UPDATE user SET login_type=:login_type "
					"WHERE login_key=:login_key AND is_deleted=0 LIMIT 1",
					soci::use(loginType, "login_type"),
					soci::use(loginKey, "login_key"));

				stmt.execute();

				auto affected_rows = stmt.get_affected_rows();
				if (affected_rows <= 0)
				{
					return false;
				}

				return true;
			});

		return true;
	}

	/*bool AsyncHandlerLogin::UpdateUserAccessKey(int usn)
	{
		return true;
	}
	bool AsyncHandlerLogin::GetUserAccessKey(int usn, OUT std::string& outAccessKey,
		OUT int outAccessKeyUpdateTime)
	{
		return true;
	}*/
}