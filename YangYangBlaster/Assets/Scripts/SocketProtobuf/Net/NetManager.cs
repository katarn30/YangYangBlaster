using UnityEngine;
using Util;
using System.Collections.Generic;
using System;
using Proto;
using System.IO;
using Google.Protobuf;

namespace Net
{
    public class NetManager : SingleTon<NetManager>
    {
        //private static NetManager _instance;
        //public static NetManager Instance
        //{
        //    get
        //    {
        //        return _instance;
        //    }
        //}

        private void Awake()
        {
            //_instance = this;
            Init();
            //
            SendConnect();

        }

        private Dictionary<Type, TocHandler> _handlerDic;
        private SocketClient _socketClient;
        SocketClient socketClient
        {
            get
            {
                if (_socketClient == null)
                {
                    _socketClient = new SocketClient();
                }
                return _socketClient;
            }
        }

        void Start()
        {
            //Init();
        }

        public void Init()
        {
            _handlerDic = new Dictionary<Type, TocHandler>();
            socketClient.OnRegister();
        }

        /// <summary>
        /// 연결 요청 보내기
        /// </summary>
        public void SendConnect()
        {
            socketClient.SendConnect();
        }

        /// <summary>
        /// 네트워크 닫기
        /// </summary>
        public void OnRemove()
        {
            socketClient.OnRemove();
        }

        /// <summary>
        /// 소켓 메세지 보내기
        /// </summary>
        public void SendMessage(ByteBuffer buffer)
        {
            socketClient.SendMessage(buffer);
        }

        /// <summary>
        /// 소켓 메세지 보내기
        /// </summary>
        public void SendMessage(IMessage obj)
        {
            if (!ProtoDic.ContainProtoType(obj.GetType()))
            {
                Debug.LogError("알 수 없는 프로토콜 유형");
                return;
            }
            ByteBuffer buff = new ByteBuffer();
            int protoId = ProtoDic.GetProtoIdByProtoType(obj.GetType());

            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                obj.WriteTo(ms);
                result = ms.ToArray();
            }

            UInt16 lengh = (UInt16)(result.Length + 2);
            Debug.Log("lengh" + lengh + ",protoId" + protoId);
            buff.WriteShort((UInt16)lengh);

            buff.WriteShort((UInt16)protoId);
            buff.WriteBytes(result);
            SendMessage(buff);
        }

        /// <summary>
        /// 연결 
        /// </summary>
        public void OnConnect()
        {
            Debug.Log("======OnConnect========");
        }

        /// <summary>
        /// 연결해제
        /// </summary>
        public void OnDisConnect()
        {
            Debug.Log("======OnDisConnect========");
        }

        /// <summary>
        /// 프로토콜 처리
        /// </summary>
        /// <param name="protoId"></param>
        /// <param name="buff"></param>
        public void DispatchProto(int protoId, byte[] buff)
        {
            if (!ProtoDic.ContainProtoId(protoId))
            {
                Debug.LogError("알 수 없는 프로토콜 번호");
                return;
            }
            Type protoType = ProtoDic.GetProtoTypeByProtoId(protoId);
            try
            {
                MessageParser messageParser = ProtoDic.GetMessageParser(protoType.TypeHandle);
                object toc = messageParser.ParseFrom(buff);
                sEvents.Enqueue(new KeyValuePair<Type, object>(protoType, toc));
            }
            catch
            {
                Debug.Log("DispatchProto Error:" + protoType.ToString());
            }

        }

        static Queue<KeyValuePair<Type, object>> sEvents = new Queue<KeyValuePair<Type, object>>();
        /// <summary>
        /// Command에 맡기고 여기서는 누구에게 보낼지 관심을 갖지 않으려고 합니다.
        /// </summary>
        void Update()
        {
            if (sEvents.Count > 0)
            {
                while (sEvents.Count > 0)
                {
                    KeyValuePair<Type, object> _event = sEvents.Dequeue();
                    if (_handlerDic.ContainsKey(_event.Key))
                    {
                        _handlerDic[_event.Key](_event.Value);
                    }
                }
            }
        }

        public void AddHandler(Type type, TocHandler handler)
        {
            if (_handlerDic.ContainsKey(type))
            {
                _handlerDic[type] += handler;
            }
            else
            {
                _handlerDic.Add(type, handler);
            }
        }
    }

}
