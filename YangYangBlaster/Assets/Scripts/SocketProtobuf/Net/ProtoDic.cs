
using Google.Protobuf;
using Msg;
using System;
using System.Collections.Generic;

namespace Proto
{
   public class ProtoDic
   {
       private static List<int> _protoId = new List<int>
       {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
        };

      private static List<Type>_protoType = new List<Type>
      {
            typeof(RpcServiceExampleRequest),
            typeof(RpcServiceExampleReply),
            typeof(LoginRequest),
            typeof(LoginReply),
            typeof(LoadGameDataRequest),
            typeof(LoadGameDataReply),
            typeof(SaveGameDataRequest),
            typeof(SaveGameDataReply),
            typeof(RankingRequest),
            typeof(RankingReply),
            typeof(RankingListRequest),
            typeof(RankingListReply),
       };

       private static readonly Dictionary<RuntimeTypeHandle, MessageParser> Parsers = new Dictionary<RuntimeTypeHandle, MessageParser>()
       {
            {typeof(RpcServiceExampleRequest).TypeHandle,RpcServiceExampleRequest.Parser },
            {typeof(RpcServiceExampleReply).TypeHandle,RpcServiceExampleReply.Parser },
            {typeof(LoginRequest).TypeHandle,LoginRequest.Parser },
            {typeof(LoginReply).TypeHandle,LoginReply.Parser },
            {typeof(LoadGameDataRequest).TypeHandle,LoadGameDataRequest.Parser },
            {typeof(LoadGameDataReply).TypeHandle,LoadGameDataReply.Parser },
            {typeof(SaveGameDataRequest).TypeHandle,SaveGameDataRequest.Parser },
            {typeof(SaveGameDataReply).TypeHandle,SaveGameDataReply.Parser },
            {typeof(RankingRequest).TypeHandle,RankingRequest.Parser },
            {typeof(RankingReply).TypeHandle,RankingReply.Parser },
            {typeof(RankingListRequest).TypeHandle,RankingListRequest.Parser },
            {typeof(RankingListReply).TypeHandle,RankingListReply.Parser },
       };

        public static MessageParser GetMessageParser(RuntimeTypeHandle typeHandle)
        {
            MessageParser messageParser;
            Parsers.TryGetValue(typeHandle, out messageParser);
            return messageParser;
        }

        public static Type GetProtoTypeByProtoId(int protoId)
        {
            int index = _protoId.IndexOf(protoId);
            return _protoType[index];
        }

        public static int GetProtoIdByProtoType(Type type)
        {
            int index = _protoType.IndexOf(type);
            return _protoId[index];
        }

        public static bool ContainProtoId(int protoId)
        {
            if(_protoId.Contains(protoId))
            {
                return true;
            }
            return false;
        }

        public static bool ContainProtoType(Type type)
        {
            if(_protoType.Contains(type))
            {
                return true;
            }
            return false;
        }
    }
}