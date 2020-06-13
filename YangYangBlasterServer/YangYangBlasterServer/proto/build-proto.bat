protoc_csharp -I=./ --csharp_out=../ --grpc_out=../ --plugin=protoc-gen-grpc=./grpc_csharp_plugin.exe rpc_service.proto health_check_service.proto google/api/annotations.proto google/api/http.proto

protoc -I=./ --cpp_out=../ --grpc_out=../ --plugin=protoc-gen-grpc=./grpc_cpp_plugin.exe rpc_service.proto health_check_service.proto google/api/annotations.proto google/api/http.proto

python -m grpc_tools.protoc -I. --python_out=../ --grpc_python_out=../ rpc_service.proto health_check_service.proto google/api/annotations.proto google/api/http.proto

pause