#pragma once

//wsock32.lib
//ws2_32.lib
//kernel32.lib
//user32.lib
//gdi32.lib
//winspool.lib
//shell32.lib
//ole32.lib
//oleaut32.lib
//uuid.lib
//comdlg32.lib
//advapi32.lib

#ifdef _DEBUG
#pragma comment(lib, "libprotobufd.lib")
#pragma comment(lib, "libprotocd.lib")
#pragma comment(lib, "grpc_cronet.lib")
#pragma comment(lib, "zlibd.lib")
#pragma comment(lib, "zlibstaticd.lib")
#else
#pragma comment(lib, "libprotobuf.lib")
#pragma comment(lib, "libprotoc.lib")
#pragma comment(lib, "zlib.lib")
#pragma comment(lib, "zlibstatic.lib")
#endif
#pragma comment(lib, "libmysql.lib")
#pragma comment(lib, "mysqlclient.lib")
#pragma comment(lib, "mysqlcppconn.lib")
#pragma comment(lib, "libsoci_core_4_0.lib")
#pragma comment(lib, "libsoci_empty_4_0.lib")
#pragma comment(lib, "libsoci_mysql_4_0.lib")
#pragma comment(lib, "soci_core_4_0.lib")
#pragma comment(lib, "soci_empty_4_0.lib")
#pragma comment(lib, "soci_mysql_4_0.lib")
#pragma comment(lib, "address_sorting.lib")
#pragma comment(lib, "gpr.lib")
#pragma comment(lib, "grpc.lib")
#pragma comment(lib, "grpc_csharp_ext.lib")
#pragma comment(lib, "grpc_plugin_support.lib")
#pragma comment(lib, "grpc_unsecure.lib")
#pragma comment(lib, "grpc++.lib")
#pragma comment(lib, "grpc++_alts.lib")
#pragma comment(lib, "grpc++_error_details.lib")
#pragma comment(lib, "grpc++_reflection.lib")
#pragma comment(lib, "grpc++_unsecure.lib")
#pragma comment(lib, "grpcpp_channelz.lib")
#pragma comment(lib, "upb.lib")
#pragma comment(lib, "crypto.lib")
#pragma comment(lib, "ssl.lib")
#pragma comment(lib, "cares.lib")
#pragma comment(lib, "absl_strings.lib")
#pragma comment(lib, "absl_status.lib")
#pragma comment(lib, "absl_base.lib")
#pragma comment(lib, "absl_bad_any_cast_impl.lib")
#pragma comment(lib, "absl_bad_optional_access.lib")
#pragma comment(lib, "absl_bad_variant_access.lib")
#pragma comment(lib, "absl_base.lib")
#pragma comment(lib, "absl_city.lib")
#pragma comment(lib, "absl_civil_time.lib")
#pragma comment(lib, "absl_cord.lib")
#pragma comment(lib, "absl_debugging_internal.lib")
#pragma comment(lib, "absl_demangle_internal.lib")
#pragma comment(lib, "absl_dynamic_annotations.lib")
#pragma comment(lib, "absl_examine_stack.lib")
#pragma comment(lib, "absl_exponential_biased.lib")
#pragma comment(lib, "absl_failure_signal_handler.lib")
#pragma comment(lib, "absl_flags.lib")
#pragma comment(lib, "absl_flags_config.lib")
#pragma comment(lib, "absl_flags_internal.lib")
#pragma comment(lib, "absl_flags_marshalling.lib")
#pragma comment(lib, "absl_flags_parse.lib")
#pragma comment(lib, "absl_flags_program_name.lib")
#pragma comment(lib, "absl_flags_registry.lib")
#pragma comment(lib, "absl_flags_usage.lib")
#pragma comment(lib, "absl_flags_usage_internal.lib")
#pragma comment(lib, "absl_graphcycles_internal.lib")
#pragma comment(lib, "absl_hash.lib")
#pragma comment(lib, "absl_hashtablez_sampler.lib")
#pragma comment(lib, "absl_int128.lib")
#pragma comment(lib, "absl_leak_check.lib")
#pragma comment(lib, "absl_leak_check_disable.lib")
#pragma comment(lib, "absl_log_severity.lib")
#pragma comment(lib, "absl_malloc_internal.lib")
#pragma comment(lib, "absl_periodic_sampler.lib")
#pragma comment(lib, "absl_random_distributions.lib")
#pragma comment(lib, "absl_random_internal_distribution_test_util.lib")
#pragma comment(lib, "absl_random_internal_pool_urbg.lib")
#pragma comment(lib, "absl_random_internal_randen.lib")
#pragma comment(lib, "absl_random_internal_randen_hwaes.lib")
#pragma comment(lib, "absl_random_internal_randen_hwaes_impl.lib")
#pragma comment(lib, "absl_random_internal_randen_slow.lib")
#pragma comment(lib, "absl_random_internal_seed_material.lib")
#pragma comment(lib, "absl_random_seed_gen_exception.lib")
#pragma comment(lib, "absl_random_seed_sequences.lib")
#pragma comment(lib, "absl_raw_hash_set.lib")
#pragma comment(lib, "absl_raw_logging_internal.lib")
#pragma comment(lib, "absl_scoped_set_env.lib")
#pragma comment(lib, "absl_spinlock_wait.lib")
#pragma comment(lib, "absl_stacktrace.lib")
#pragma comment(lib, "absl_status.lib")
#pragma comment(lib, "absl_strings.lib")
#pragma comment(lib, "absl_strings_internal.lib")
#pragma comment(lib, "absl_str_format_internal.lib")
#pragma comment(lib, "absl_symbolize.lib")
#pragma comment(lib, "absl_synchronization.lib")
#pragma comment(lib, "absl_throw_delegate.lib")
#pragma comment(lib, "absl_time.lib")
#pragma comment(lib, "absl_time_zone.lib")

#include <iostream>
#include <memory>
#include <string.h>
#include <ctime>
#include <mutex>

#include <google/protobuf/reflection.h>

#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/thread.hpp>

#include <grpcpp/grpcpp.h>
#include <grpcpp/health_check_service_interface.h>
#include <grpcpp/ext/proto_server_reflection_plugin.h>

#include <soci/soci.h>
#include <soci/error.h>
#include <soci/connection-pool.h>
#include <soci/empty/soci-empty.h>
#include <soci/mysql/soci-mysql.h>
#include <soci/callbacks.h>