#pragma once

namespace yyb
{
	class Python
	{
	public:
		static Python& Instance();
		void Init();

		bool GoogleVerifyOauth2Token(const std::string& idToken);

	private:
		Python() {}

		boost::python::api::object mainModule_;// import("__main__");
		boost::python::api::object mainNamespace_;// = main_module.attr("__dict__");
	};
}