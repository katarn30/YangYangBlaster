#include "stdafx.h"
#include "Python.h"

namespace yyb
{
	Python& Python::Instance()
	{
		static Python python;
		return python;
	}

	void Python::Init()
	{
		wchar_t* program = Py_DecodeLocale("yyb::python", NULL);
		if (program == NULL)
		{
			fprintf(stderr, "Fatal error: cannot decode argv[0]\n");
			exit(1);
		}

		Py_SetProgramName(program);

		Py_Initialize();

		mainModule_ = boost::python::import("__main__");
		mainNamespace_ = mainModule_.attr("__dict__");
	}

	bool Python::GoogleVerifyOauth2Token(const std::string& idToken)
	{
		try
		{
			std::string pythonRoot = "C:\\ProgramData\\Anaconda3\\envs\\yyb";
			//std::string idToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjQ5MjcxMGE3ZmNkYjE1Mzk2MGNlMDFmNzYwNTIwYTMyYzg0NTVkZmYiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI0MjM0NzI3MzE0ODUtZjY1M2Rzb2Z0NzRkajJjMTJhb3R2aTRrYnVoN25pOWEuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJhdWQiOiI0MjM0NzI3MzE0ODUtZDhzZHQ0ZzNvdGsxNTZiM3N1cDFyYmUxdG5xaTQ5dGYuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJzdWIiOiIxMDc4MzgwNTA1OTU0ODEzNjAxNjUiLCJpYXQiOjE1OTEyNDk1NzUsImV4cCI6MTU5MTI1MzE3NX0.cCBcnGJGWa7ZqHbh6d38kWHZCaXHjVQfAvJTQYOIrFCjCQZEojWxPP2uPdmXGVjalJ7NT1RqqmV3WbwaGcyQqThHgCw_3qsIYHa2U-_YrSwFMD34x0uvEEhYwovl61IeKTngm2jzVcdimVyA1chz85gUe1OuG0dIII5ND1CNYI9ztBppKDkGjxPza9lKEtnQjmOi0gGhv-_u_yRaegTlX6WyFtP6tG6XGLhCp-mhFOqw8BM4ondwWl3lwtrVgb-R6ni4pR0gld0EOjv_AQcwz_OGUL8o6b7His6OOY7lAQ5D0eRKIs75xPAA9L_eOUBn8NN8L4_cK1XolzKjsB9b5w";
			std::string clientId = 
				"423472731485-d8sdt4g3otk156b3sup1rbe1tnqi49tf.apps.googleusercontent.com";

			std::string pythonGoogleAuthSource =
				"import sys\n"
				//"sys.path.append('C:\\Users\\katarn30\\Desktop\\etc\\BoostPython\\hello\\external\\python-3.6.5-embed-amd64\\python36.zip')\n"
				"sys.path.append('" + pythonRoot + "')\n"
				"sys.path.append('" + pythonRoot + "\\DLLs')\n"
				"sys.path.append('" + pythonRoot + "\\Lib')\n"
				"sys.path.append('" + pythonRoot + "\\Scripts')\n"
				"sys.path.append('" + pythonRoot + "\\Lib\\site-packages')\n"
				"import socket\n"
				"from google.oauth2 import id_token\n"
				"from google.auth.transport import requests\n"
				"token = '" + idToken + "'\n"
				"CLIENT_ID = '" + clientId + "'\n"
				"try:\n"
				"	idinfo = id_token.verify_oauth2_token(token, requests.Request(), CLIENT_ID)\n"
				"	if idinfo['iss'] not in['accounts.google.com', 'https://accounts.google.com']:\n"
				"		raise ValueError('Wrong issuer.')\n"
				//"	userid = idinfo['sub']\n"
				"	iss = idinfo['iss']\n"
				"	sub = idinfo['sub']\n"
				"	aud = idinfo['aud']\n"
				"	iat = idinfo['iat']\n"
				"	exp = idinfo['exp']\n"
				"except ValueError:\n"
				"	print('Invalid token')\n"
				;

			boost::python::api::object ignored = boost::python::exec(
				//"google_auth.py"
				//"result = abcd.get()"
				pythonGoogleAuthSource.c_str()
				, mainNamespace_);
			std::string iss = boost::python::extract<std::string>(mainNamespace_["iss"]);
			std::string sub = boost::python::extract<std::string>(mainNamespace_["sub"]);
			std::string aud = boost::python::extract<std::string>(mainNamespace_["aud"]);
			int iat = boost::python::extract<int>(mainNamespace_["iat"]);
			int exp = boost::python::extract<int>(mainNamespace_["exp"]);

			std::cout << iss << std::endl;
			std::cout << sub << std::endl;
			std::cout << aud << std::endl;
			std::cout << iat << std::endl;
			std::cout << exp << std::endl;

			if (clientId != aud)
			{
				return false;
			}
		}
		catch (boost::python::error_already_set const& e)
		{
			if (PyErr_ExceptionMatches(PyExc_ZeroDivisionError))
			{
				// handle ZeroDivisionError specially
				std::cout << "PyExc_ZeroDivisionError" << std::endl;
			}
			else
			{
				// print all other errors to stderr
				PyErr_Print();
			}

			return false;
		}

		return true;
	}
}