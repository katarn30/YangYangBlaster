#include "stdafx.h"
#include "DB.h"

namespace yyb
{
	class failover_callback_impl : public soci::failover_callback
	{
	public:
		virtual void started()
		{
			std::cout << "session started" << std::endl;
		}

		// Called after successful failover and creating a new connection;
		// the sql parameter denotes the new connection and allows the user
		// to replay any initial sequence of commands (like session configuration).
		virtual void finished(soci::session& sql)
		{
			std::cout << "session finished" << std::endl;
		}

		// Called when the attempt to reconnect failed,
		// if the user code sets the retry parameter to true,
		// then new connection will be attempted;
		// the newTarget connection string is a hint that can be ignored
		// by external means.
		virtual void failed(bool& retry, std::string& newTarget)
		{
			std::cout << "session failed" << newTarget << std::endl;
		}

		// Called when there was a failure that prevents further failover attempts.
		virtual void aborted()
		{
			std::cout << "session aborted" << std::endl;
		}
	};

	DB::QueryScopeTran::QueryScopeTran(int dbPoolIndex) : handled_(false)
	{
		auto pool = DB::Instance().GetDBConnectionPool(dbPoolIndex);
		if (pool)
		{
			sql_ = std::make_shared<soci::session>(*pool);
			tr_ = std::make_shared<soci::transaction>(*sql_);
		}
	}

	DB::QueryScopeTran::~QueryScopeTran()
	{
		if (false == handled_)
		{
			if (tr_)
			{
				tr_->rollback();
			}
		}
	}

	bool DB::QueryScopeTran::Execute(std::function<bool(soci::session&)> query)
	{
		try
		{
			if (sql_)
			{
				return query(*sql_);
			}
			else
			{
				return false;
			}
		}
		catch (soci::mysql_soci_error const& e)
		{
			if (2013 == e.err_num_)
			{
				if (sql_)
				{
					sql_->reconnect();
				}
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
		
		return true;
	}

	void DB::QueryScopeTran::Commit()
	{
		if (tr_)
		{
			tr_->commit();

			handled_ = true;
		}
	}

	DB& DB::Instance()
	{
		static DB db;
		return db;
	}

	bool DB::QueryScope(int dbPoolIndex, 
		std::function<bool(soci::session&)> query)
	{
		auto pool = DB::Instance().GetDBConnectionPool(dbPoolIndex);
		if (pool)
		{
			soci::session sql(*pool);

			try
			{
				return query(sql);
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

	void DB::Init(size_t poolSize,
		const std::string& db, const std::string& host, short port,
		const std::string& user, const std::string& password,
		const std::string& tz)
	{
		poolSize_ = poolSize;
		db_ = db;
		host_ = host;
		port_ = port;
		user_ = user;
		password_ = password;
		tz_ = tz;
	}

	bool DB::CreateDBConnectionPool(int poolIndex)
	{
		static failover_callback_impl fci;

		if (dbMap_.find(poolIndex) == dbMap_.end())
		{
			connection_pool_ptr pool =
				std::make_shared<soci::connection_pool>(poolSize_);

			dbMap_.emplace(poolIndex, pool);

			std::stringstream ss;

			ss << "db=" << db_;
			ss << " host=" << host_;
			ss << " user=" << user_;
			ss << " port=" << port_;
			ss << " password=" << password_;
			ss << " charset=utf8mb4";

			std::string connectString = ss.str();

			for (size_t i = 0; i < poolSize_; ++i)
			{
				try
				{
					soci::connection_parameters param;

					soci::session& sql = pool->at(i);

					sql.open(soci::mysql, connectString);
					
					// mysql 에서 작동 안하는듯
					sql.set_failover_callback(fci);

					sql << "SET TIME_ZONE='" + tz_ + "'";

					soci::mysql_session_backend* sessionBackEnd
						= static_cast<soci::mysql_session_backend*>(sql.get_backend());
					std::string version = mysql_get_server_info(sessionBackEnd->conn_);

					std::cout << "DB session " << i << " connected from " << host_ <<
						":" << port_ << " version:" << version << std::endl;
				}
				catch (soci::mysql_soci_error const& e)
				{
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
		else
		{
			return false;
		}
	}

	connection_pool_ptr DB::GetDBConnectionPool(int poolIndex)
	{
		if (dbMap_.find(poolIndex) != dbMap_.end())
		{
			return dbMap_[poolIndex];
		}

		return connection_pool_ptr();
	}
}

// EXAMPLE

/*try
				{*/
				//soci::session sql(pool);
				/*failover_callback fc;
				sql.set_failover_callback(fc);*/

				// TEST SELECT
				/*soci::rowset<soci::row> rs{ (sql.prepare << "SELECT * FROM global_settings") };
				std::ostringstream doc;

				for (const auto& r : rs)
				{
					for (size_t i = 0; i < r.size(); ++i)
					{
						if (r.get_indicator(i) == soci::i_null)
						{
							continue;
						}

						const soci::column_properties& props = r.get_properties(i);

						doc << '<' << props.get_name() << '>';

						switch (props.get_data_type())
						{
						case soci::dt_string:
							doc << r.get<std::string>(i);
							break;
						case soci::dt_double:
							doc << r.get<double>(i);
							break;
						case soci::dt_integer:
							doc << r.get<int>(i);
							break;
						case soci::dt_long_long:
							doc << r.get<long long>(i);
							break;
						case soci::dt_unsigned_long_long:
							doc << r.get<unsigned long long>(i);
							break;
						case soci::dt_date:
							std::tm when = r.get<std::tm>(i);
							doc << std::asctime(&when);
							break;
						default:
							break;
						}

						doc << "</" << props.get_name() << '>' << std::endl;
					}
				}

				std::cout << doc.str();*/




				// TEST UPDATE, INSERT
				/*std::stringstream ss;
				ss << boost::this_thread::get_id();

				std::string key_name = ss.str();
				std::string value = ss.str();

				soci::transaction tr(sql);

				sql << "INSERT INTO global_settings(key_name, value) VALUES (:key_name, :value)", soci::use(key_name, "key_name"), soci::use(value, "value");

				value = "에헤야디야 잘바꼈다";
				sql << "UPDATE global_settings SET value=:value WHERE key_name=:key_name", soci::use(key_name, "key_name"), soci::use(value, "value");

				tr.commit();*/

				//std::string no = "35";
				//std::string value = "abcdefg";
				///*sql << "UPDATE global_settings SET value=:value WHERE key_name=:key_name", 
				//	soci::use(key_name, "key_name"), soci::use(value, "value");*/

				//soci::statement stmt = (sql.prepare << "UPDATE global_settings SET value=:value WHERE no > :no");
				//stmt.exchange(soci::use(no, "no"));
				//stmt.exchange(soci::use(value, "value"));
				//stmt.define_and_bind();
				//stmt.execute(true);

				//auto affected_rows = stmt.get_affected_rows();
				//if (0 < affected_rows)
				//{
				//	std::cout << "stmt.execute true affected " << affected_rows << std::endl;
				//}
				//else
				//{
				//	std::cout << "stmt.execute false affected " << affected_rows << std::endl;
				//}
				//stmt.bind_clean_up();
			/*}
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
			}*/


// EXAMPLE 2

/* Standard C++ includes */
//#include <stdlib.h>
//#include <iostream>
//#include <iostream>
//#include <mysql/jdbc.h>
//
//using namespace std;
//
//int main(void)
//{
//	try {
//		/* Create a connection */
//		sql::Driver* driver = get_driver_instance();
//		sql::Connection* con = driver->connect("tcp://192.168.1.5:3306", "satel", "369369");
//		/* Connect to the MySQL test database */
//		con->setSchema("NBI");
//
//		sql::Statement* stmt = con->createStatement();
//
//		sql::ResultSet* res = stmt->executeQuery("SELECT idx, enName, korName, jobpName FROM Corps");
//		//res = stmt->executeQuery("SELECT idx, enName, korName, jobpName FROM Corps");
//		while (res->next()) {
//			/* Access column data by alias or column name */
//			cout << res->getInt("idx") << endl;
//			cout << res->getString("enName").c_str() << endl;
//			/* Access column data by numeric offset, 1 is the first column */
//			cout << res->getString(3).c_str() << endl;
//			cout << res->getString(4).c_str() << endl;
//		}
//
//		delete res;
//		delete stmt;
//		delete con;
//	}
//	catch (sql::SQLException& e) {
//		cout << "# ERR: SQLException in " << __FILE__;
//		cout << "(" << __FUNCTION__ << ") on line " << __LINE__ << endl;
//		cout << "# ERR: " << e.what();
//		cout << " (MySQL error code: " << e.getErrorCode();
//		cout << ", SQLState: " << e.getSQLState() << " )" << endl;
//	}
//
//	cout << endl;
//
//	return EXIT_SUCCESS;
//}