// YangYangBlasterServer.cpp : 이 파일에는 'main' 함수가 포함됩니다. 거기서 프로그램 실행이 시작되고 종료됩니다.
//

#include "stdafx.h"
#include "rpc_service.pb.h"
#include "rpc_service.grpc.pb.h"
#include "health_check_service.pb.h"
#include "health_check_service.grpc.pb.h"
#include "GlobalDefine.h"
#include "Cache.h"
#include "DB.h"
#include "Python.h"
#include "RpcServiceImpl.h"
#include "RpcServerImpl.h"
#include "HealthCheckServiceImpl.h"
#include "UserManager.h"
#include "User.h"

namespace yyb
{
    bool InitCache()
    {
        std::string host = "127.0.0.1";
        short port = 6379;

        Cache::Instance().Init(host, port);

        if (false == Cache::Instance().CreateCache(CACHE_INDEX_GLOBAL))
        {
            return false;
        }

        return true;
    }

    bool InitDB()
    {
        const size_t poolSize = 
            static_cast<size_t>(std::thread::hardware_concurrency()) * 2;
        std::string db = "YYB";
        std::string host = "192.168.1.5";
        short port = 3306;
        std::string user = "satel";
        std::string password = "369369";

        DB::Instance().Init(poolSize, db, host, port, user, password);
        
        if (false == DB::Instance().CreateDBConnectionPool(DB_POOL_INDEX_GLOBAL))
        {
            return false;
        }

        return true;
    }

    bool InitPython()
    {
        Python::Instance().Init();

        return true;
    }

    // 동기 서버
    void RunServer() {
        std::string server_address("0.0.0.0:20051");
        RpcServiceImpl rpcService;
        HealthCheckServiceImpl healthCheckService;

        grpc::EnableDefaultHealthCheckService(false);
        grpc::reflection::InitProtoReflectionServerBuilderPlugin();
        grpc::ServerBuilder builder;
        // Options..
        std::unique_ptr<grpc::HealthCheckServiceInterface> service(
            new CustomHealthCheckService(&healthCheckService));
        std::unique_ptr<grpc::ServerBuilderOption> option(
            new grpc::HealthCheckServiceServerBuilderOption(std::move(service)));
        builder.SetOption(std::move(option));

        /*builder.AddChannelArgument(GRPC_ARG_KEEPALIVE_TIME_MS, 2000);
        builder.AddChannelArgument(GRPC_ARG_KEEPALIVE_TIMEOUT_MS, 1000);
        builder.AddChannelArgument(GRPC_ARG_HTTP2_BDP_PROBE, 1);
        builder.AddChannelArgument(GRPC_ARG_MAX_CONNECTION_IDLE_MS, 1000);*/
        // Listen on the given address without any authentication mechanism.
        builder.AddListeningPort(server_address, grpc::InsecureServerCredentials());
        // Register "service" as the instance through which we'll communicate with
        // clients. In this case it corresponds to an *synchronous* service.
        builder.RegisterService(&rpcService);
        builder.RegisterService(&healthCheckService);

        /*std::vector<
            std::unique_ptr<grpc::experimental::ServerInterceptorFactoryInterface>>
            creators;
        creators.push_back(
            std::unique_ptr<grpc::experimental::ServerInterceptorFactoryInterface>(
                new SyncSendMessageVerifierFactory()));
        builder.experimental().SetInterceptorCreators(std::move(creators));*/
        //auto cq = builder.AddCompletionQueue();
        /*grpc::ResourceQuota rq;
        rq.SetMaxThreads(static_cast<int>(std::thread::hardware_concurrency()) * 2);
        builder.SetResourceQuota(rq);*/
        // Finally assemble the server.
        std::unique_ptr<grpc::Server> server(builder.BuildAndStart());
        std::cout << "Server listening on " << server_address << std::endl;

        // Wait for the server to shutdown. Note that some other thread must be
        // responsible for shutting down the server for this call to ever return.
        server->Wait();
    }
}

void test_process()
{
    while (true)
    {
        /*if (yyb::TestNotifier::Instance().p_)*/
        {
            /*yyb::PushNotification noti;
            noti.set_payload("보내짐");*/
            //yyb::TestNotifier::Instance().notifier_->Write(noti);

            std::this_thread::sleep_for(std::chrono::seconds(10));

            yyb::TestNotifier::Instance().SetValueAll("abcd");
        }
    }
}

//void load_root_certificates(boost::asio::ssl::context& ctx, 
//    boost::system::error_code& ec)
//{
//    std::string const cert =
//        /*  This is the DigiCert Global Root CA
//
//            CN = DigiCert High Assurance EV Root CA
//            OU = www.digicert.com
//            O = DigiCert Inc
//            C = US
//
//            Valid to: 10 November 2031
//
//            Serial #:
//            08:3B:E0:56:90:42:46:B1:A1:75:6A:C9:59:91:C7:4A
//
//            SHA1 Fingerprint:
//            A8:98:5D:3A:65:E5:E5:C4:B2:D7:D6:6D:40:C6:DD:2F:B1:9C:54:36
//
//            SHA256 Fingerprint:
//            43:48:A0:E9:44:4C:78:CB:26:5E:05:8D:5E:89:44:B4:D8:4F:96:62:BD:26:DB:25:7F:89:34:A4:43:C7:01:61
//        */
//        "-----BEGIN CERTIFICATE-----\nMIIDHDCCAgSgAwIBAgIIA/0Gc2VF4bowDQYJKoZIhvcNAQEFBQAwMTEvMC0GA1UE\nAxMmc2VjdXJldG9rZW4uc3lzdGVtLmdzZXJ2aWNlYWNjb3VudC5jb20wHhcNMjAw\nNTIxMDkxOTU3WhcNMjAwNjA2MjEzNDU3WjAxMS8wLQYDVQQDEyZzZWN1cmV0b2tl\nbi5zeXN0ZW0uZ3NlcnZpY2VhY2NvdW50LmNvbTCCASIwDQYJKoZIhvcNAQEBBQAD\nggEPADCCAQoCggEBAKr26Af3m1BL8wqTjxyo9avaAIgzKFPlWBXWKMHtWJJsQBfS\ncBhZxdd8lNO28FoxB/zNeu3zcClXyKrOC1rbyFHeR/ES4eu8bO5NlP6/paf4wc4m\n8yJlDNzrtms78pvrKjbf6Lv9sOVRZl5zVd/2VK7RpJ8M7cD/5r4SloLXcUTc5uNR\ntnBaO1V8sHpyEFsA5RMnG7OKTfKHCMbTLL9314A8Z0TIjDIn7waix3+1UZYR5Avv\n80fWcFaLxVC4mh588ki30fGq8rfsiDBa3MX4Cd4WaaCRz9IT87EjrgTCreQID/Z9\noDvMi2o0L9w300e5cbttyRZocA9k0qI/znOq2fcCAwEAAaM4MDYwDAYDVR0TAQH/\nBAIwADAOBgNVHQ8BAf8EBAMCB4AwFgYDVR0lAQH/BAwwCgYIKwYBBQUHAwIwDQYJ\nKoZIhvcNAQEFBQADggEBACgNLX268lJRni1knwNrbte+ItJDbu0etkXFHbF7z2pw\nZphUBHGttkWXWv4FF3PnX6HRFr/QhiRqf/zastUoSTwGeXZqWlVRkjRWEA0zvmjv\nVY+yIFJ3G6HqIMhtfJCYgxjIXu4mHd/s7tsnF57XE45b6No0uh5B40pTUEv5LDOH\nTBp+PN38rPfFEUsQFQBEbc0YtuvEeNVaS001G3PMN+DS2koXDwSCWXI2REAzJ36A\nncYinAdTfrjD5Nn7pb/21SVhfu9pRRB8o3SKEIwDs7DrgEsA6cG/XbHRHCF5UlIF\ncPUlA19XtukSzkXFjvn+Ux/OTllClILc+L4DsHd8HFQ=\n-----END CERTIFICATE-----\n"
//
//        /*  This is the GeoTrust root certificate.
//
//            CN = GeoTrust Global CA
//            O = GeoTrust Inc.
//            C = US
//            Valid to: Friday, ‎May ‎20, ‎2022 9:00:00 PM
//
//            Thumbprint(sha1):
//            ‎de 28 f4 a4 ff e5 b9 2f a3 c5 03 d1 a3 49 a7 f9 96 2a 82 12
//        */
//        "-----BEGIN CERTIFICATE-----\nMIIDHDCCAgSgAwIBAgIIKHPob6sU1ywwDQYJKoZIhvcNAQEFBQAwMTEvMC0GA1UE\nAxMmc2VjdXJldG9rZW4uc3lzdGVtLmdzZXJ2aWNlYWNjb3VudC5jb20wHhcNMjAw\nNTI5MDkxOTU3WhcNMjAwNjE0MjEzNDU3WjAxMS8wLQYDVQQDEyZzZWN1cmV0b2tl\nbi5zeXN0ZW0uZ3NlcnZpY2VhY2NvdW50LmNvbTCCASIwDQYJKoZIhvcNAQEBBQAD\nggEPADCCAQoCggEBAKvNxIzEeX/6ZOFQ9VtRMqTqKP5PtUyky4INHdgpzJdR6Mn7\n2XZ7R1VA4Bxlg0CymGQ3UR/ia7mSexL1vmqDCED8+ZUsdg1xqvP1tkPSX5A44qsh\niEDlDmqvIiv37XH7pcdo6BvMCisi2zJsapLFl0r393wixtTAMrokhFkoI4cIbmac\nVWdZDL5CG8VG4XGmjnJGiNcDt/kcBeUK9+jjAWUY3NadMADU/5sGXblZ8mJtQIwr\nsNz2CiN75MaPX3rrDYgolei0Hsld2OC+JxE9GJRGLtxSzCAL/uP88QgqMdsRvtzQ\nOq9zK5XqA8sUtYEoGx1QlZ4uuHqHZYRUv75qeQECAwEAAaM4MDYwDAYDVR0TAQH/\nBAIwADAOBgNVHQ8BAf8EBAMCB4AwFgYDVR0lAQH/BAwwCgYIKwYBBQUHAwIwDQYJ\nKoZIhvcNAQEFBQADggEBAKSeI7a2V/YY9X6xlFSF594A2Kx5aLAuyYnsve3I/D0T\n//XXnClpqcFb+3lkGBOvdjD13CdBNYTZ5ICK5Ww04sMf7o0AcGiv0Zi+3/DfcdDf\n+WipBQTs9dIOAHmDrtnxKtnfcW5l5w2fX7MQ1I5GYbOAUw5S3gLOdYQYTplGE/PD\nySGyqeM82V04MA35C2VqgwE/7XDpk7SjzUrPw6laIh52wissYgwC+D6u37iVZ0l0\nT207kz6BfJCh04uUkqKfhlpAIPTvwqtFqaot9sNVU4kaHQBah8x2jGHuh+N3tN19\nCw+rdBRmBRbuFKzSkpvyL7p0XMRLMOHKqL5g1UyNbMs=\n-----END CERTIFICATE-----\n"
//        ;
//
//    ctx.add_certificate_authority(
//        boost::asio::buffer(cert.data(), cert.size()), ec);
//    if (ec)
//        return;
//}

//void test_http()
//{
    //std::cout << __FUNCTION__ << " : " << std::endl;

    ////return true;

    //try
    //{
    //    std::string host = "oauth2.googleapis.com";// tokeninfo ? id_token = ";
    //        //+ idToken;

    //    boost::asio::io_context ioc;
    //    // The SSL context is required, and holds certificates
    //    boost::asio::ssl::context ctx(boost::asio::ssl::context::tlsv12_client);
    //    boost::system::error_code error;
    //    // This holds the root certificate used for verification
    //    load_root_certificates(ctx, error);
    //    // Verify the remote server's certificate
    //    ctx.set_verify_mode(boost::asio::ssl::verify_peer);

    //    boost::asio::ip::tcp::resolver resolver(ioc);
    //    boost::beast::ssl_stream<boost::beast::tcp_stream> stream(ioc, ctx);

    //    // Set SNI Hostname (many hosts need this to handshake successfully)
    //    if (!SSL_set_tlsext_host_name(stream.native_handle(), host.c_str()))
    //    {
    //        boost::beast::error_code ec{ 
    //            static_cast<int>(::ERR_get_error()), 
    //            boost::asio::error::get_ssl_category() };
    //        throw boost::beast::system_error{ ec };
    //    }

    //    // Look up the domain name
    //    auto const results = resolver.resolve(host, "443");
    //    // Make the connection on the IP address we get from a lookup
    //    boost::beast::get_lowest_layer(stream).connect(results);
    //    //stream.connect(results);

    //    // Perform the SSL handshake
    //    stream.handshake(boost::asio::ssl::stream_base::client);

    //    // Set up an HTTP GET request message
    //    boost::beast::http::request<boost::beast::http::string_body> req
    //    {
    //        boost::beast::http::verb::post, "/tokeninfo?id_token=12345", 10
    //    };




    //    /*req.set(http::field::host, host);
    //    req.set(http::field::user_agent, BOOST_BEAST_VERSION_STRING);

    //    boost::beast::http::request<boost::beast::http::string_body> req
    //    {
    //        boost::beast::http::verb::get, "/tokeninfo?id_token=12345", version
    //    };
    //    req.set(boost::beast::http::field::host, host);
    //    req.set(boost::beast::http::field::user_agent, BOOST_BEAST_VERSION_STRING);*/

    //    // Send the HTTP request to the remote host
    //    boost::beast::http::write(stream, req);

    //    // This buffer is used for reading and must be persisted
    //    boost::beast::flat_buffer buffer;

    //    // Declare a container to hold the response
    //    boost::beast::http::response<boost::beast::http::dynamic_body> res;

    //    // Receive the HTTP response
    //    boost::beast::http::read(stream, buffer, res);

    //    // Write the message to standard out
    //    std::cout << res << std::endl;

    //    // Gracefully close the socket
    //    boost::beast::error_code ec;
    //    stream.shutdown(ec);
    //    if (ec == boost::asio::error::eof)
    //    {
    //        // Rationale:
    //        // http://stackoverflow.com/questions/25587403/boost-asio-ssl-async-shutdown-always-finishes-with-an-error
    //        ec = {};
    //    }
    //    if (ec)
    //        throw boost::beast::system_error{ ec };

    //    //stream.socket().shutdown(boost::asio::ip::tcp::socket::shutdown_both, ec);

    //    // not_connected happens sometimes
    //    // so don't bother reporting it.
    //    //
    //    if (ec && ec != boost::beast::errc::not_connected)
    //        throw boost::beast::system_error{ ec };

    //    // If we get here then the connection is closed gracefully
    //}
    //catch (std::exception const& e)
    //{
    //    std::cerr << "Standard error: " << e.what() << std::endl;
    //}
    //catch (...)
    //{
    //    std::cerr << "Some other error" << std::endl;
    //}
//}

void test_cache()
{
    auto redis = yyb::Cache::Instance().GetCache(yyb::CACHE_INDEX_GLOBAL);
    if (redis)
    {
        /*std::multimap< std::string, std::string> score_members;
        std::vector< std::string > options;*/

        //std::string key = "ranking";
        //std::string value = "a";
        //std::string score = "3";

        ///*score_members.insert(
        //    { score, value });*/

        //score_members.insert({ "10", "a" });
        //score_members.insert({ "20", "b" });
        //score_members.insert({ "30", "c" });
        //score_members.insert({ "40", "d" });
        //score_members.insert({ "50", "e" });

        //// 자기 랭킹+1
        //auto reply = redis->zrank(key, value);

        // 특정 점수대 누군지?
        //redis->zrangebyscore(key, min, max, withscore);

        // 랭킹 추가
        //reply = redis->zadd(key, options, score_members);
        
        // 특정 점수대 몇명인지
        //redis->zcount(key, min, max);

        //redis->sync_commit();

        std::string key = "ranking";
        std::string value = "e";
        int score = 50;
        int storedScore = 0;
        std::string start = "0";
        std::string stop = "14"; // max

        //// 내 저장된 랭킹 점수
        //auto reply = redis->zrank(key, value);
        //redis->sync_commit();

        //if (reply.get().is_integer())
        //{
        //	storedScore = reply.get().as_integer();
        //}

        //// 내 저장된 랭킹 점수와 지금 획득한 점수 비교
        //if (storedScore < score)
        //{
        //	std::vector< std::string > members;

        //	members.push_back(value);

        //	// 지금 획득한 점수가 더 크면 저장된 랭킹 점수 제거
        //	redis->zrem(key, members);
        //}

        // 지금 획득한 점수 저장
        std::multimap<std::string, std::string> score_members;
        std::vector<std::string> options;

        std::string scoreStr = std::to_string(score);
        score_members.insert({ scoreStr, value });

        redis->zadd(key, options, score_members);

        redis->sync_commit();


        // 랭킹, 점수 획득
        auto futureReply1 = redis->zrevrange(key, start, stop, true);
        auto futureReply2 = redis->zrevrank(key, value);
        auto futureReply3 = redis->zscore(key, value);

        redis->sync_commit();

        auto reply1 = futureReply1.get();

        if (reply1)
        {
            auto arr = reply1.as_array();

            for (int i = 0; i < arr.size(); i += 2)
            {
                std::string nickName = arr[i].as_string();
                int score = boost::lexical_cast<int>(arr[i + 1].as_string());
            }
        }

        auto reply2 = futureReply2.get();
        if (reply2)
        {
            int rank = reply2.as_integer();
            rank += 1;
        }

        auto reply3 = futureReply3.get();
        if (reply3)
        {
            int score = boost::lexical_cast<int>(reply3.as_string());
        }
    }
}

//void test_interceptor()
//{
//
//}
//
//#define HANDLER_MACRO(KEWORD)   \
//    AsyncHandler##KEWORD##::CreateRequest(service, cq, io_service,  \
//    std::move(std::bind(&grpc::RpcService::AsyncService::Request##KEWORD##, service,   \
//        std::placeholders::_1, std::placeholders::_2,   \
//        std::placeholders::_3, std::placeholders::_4,   \
//        std::placeholders::_5, std::placeholders::_6)), \
//        [] {return new AsyncHandler##KEWORD##; })

//void RpcServerImpl::createHandlers(RpcService::AsyncService* service,
//	grpc::ServerCompletionQueue* cq, boost::asio::io_service* io_service)
//{
//	HANDLER_MACRO(RpcServiceExample);
//	//HANDLER_MACRO(Listen);
//	HANDLER_MACRO(Login);
//	HANDLER_MACRO(Ranking);
//	HANDLER_MACRO(RankingList);
//}

int main()
{
#ifdef _WIN32
    //! Windows netword DLL init
    WORD version = MAKEWORD(2, 2);
    WSADATA data;

    if (WSAStartup(version, &data) != 0)
    {
        std::cerr << "WSAStartup() failure" << std::endl;
        return 0;
    }
#endif

    //test_http();

    //std::thread t(test_process);

    /*boost::asio::io_service io_service;
    boost::asio::ip::tcp::socket socket(io_service);*/

    if (false == yyb::InitCache())
    {
        return 0;
    }

    if (false == yyb::InitDB())
    {
        return 0;
    }

    if (false == yyb::InitPython())
    {
        return 0;
    }

    //test_cache();

	//yyb::RunServer();
	yyb::RpcServerImpl server;
    server.Run();

    //t.join();

#ifdef _WIN32
    WSACleanup();
#endif 

    return 0;
}