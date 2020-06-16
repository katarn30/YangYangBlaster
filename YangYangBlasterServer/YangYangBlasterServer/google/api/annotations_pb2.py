# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: google/api/annotations.proto

from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


from google.api import http_pb2 as google_dot_api_dot_http__pb2
from google.protobuf import descriptor_pb2 as google_dot_protobuf_dot_descriptor__pb2


DESCRIPTOR = _descriptor.FileDescriptor(
  name='google/api/annotations.proto',
  package='google.api',
  syntax='proto3',
  serialized_options=b'\n\016com.google.apiB\020AnnotationsProtoP\001\242\002\004GAPI',
  serialized_pb=b'\n\x1cgoogle/api/annotations.proto\x12\ngoogle.api\x1a\x15google/api/http.proto\x1a google/protobuf/descriptor.proto:E\n\x04http\x12\x1e.google.protobuf.MethodOptions\x18\xb0\xca\xbc\" \x01(\x0b\x32\x14.google.api.HttpRuleB+\n\x0e\x63om.google.apiB\x10\x41nnotationsProtoP\x01\xa2\x02\x04GAPIb\x06proto3'
  ,
  dependencies=[google_dot_api_dot_http__pb2.DESCRIPTOR,google_dot_protobuf_dot_descriptor__pb2.DESCRIPTOR,])


HTTP_FIELD_NUMBER = 72295728
http = _descriptor.FieldDescriptor(
  name='http', full_name='google.api.http', index=0,
  number=72295728, type=11, cpp_type=10, label=1,
  has_default_value=False, default_value=None,
  message_type=None, enum_type=None, containing_type=None,
  is_extension=True, extension_scope=None,
  serialized_options=None, file=DESCRIPTOR)

DESCRIPTOR.extensions_by_name['http'] = http
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

http.message_type = google_dot_api_dot_http__pb2._HTTPRULE
google_dot_protobuf_dot_descriptor__pb2.MethodOptions.RegisterExtension(http)

DESCRIPTOR._options = None
# @@protoc_insertion_point(module_scope)
