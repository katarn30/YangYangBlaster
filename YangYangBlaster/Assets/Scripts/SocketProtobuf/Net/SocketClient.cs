using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using Net;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];

    // Use this for initialization
    public SocketClient()
    {
    }

    /// <summary>
    /// 등록
    /// </summary>
    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    /// <summary>
    /// 에이전트 제거
    /// </summary>
    public void OnRemove()
    {
        this.Close();
        reader.Close();
        memStream.Close();
    }

    /// <summary>
    /// 서버 연결
    /// </summary>
    void ConnectServer(string host, int port)
    {
        client = null;
        client = new TcpClient();
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;
        try
        {
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Close();
            Debug.LogError(e.Message);
        }
    }

    void SyncConnectServer(string host, int port)
    {
        client = null;
        client = new TcpClient();
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;
        try
        {
            client.Connect(host, port);

            if (client.Connected)
            {
                OnConnect(null);
            }
        }
        catch (Exception e)
        {
            Close();
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 서버에 연결
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        outStream = client.GetStream();
        client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
        NetManager.Instance.OnConnect();
    }

    /// <summary>
    /// 메세지에 데이터 쓰기
    /// </summary>
    void WriteMessage(byte[] message)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected)
            {
                byte[] payload = ms.ToArray();
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Debug.LogError("client.connected----->>false");
            }
        }
    }

    /// <summary>
    /// 읽기
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {         //판독 바이트가 버퍼로 흐름
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {                //사이즈에 문제가 있어 연결 끊김 처리
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }
            OnReceive(byteBuffer, bytesRead);   //데이터 패킷 내용을 분석하여 논리 계층에 매핑
            lock (client.GetStream())
            {         //석 완료, 서버에서 보내온 새로운 소식 다시 감청
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //클리어
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            //PrintBytes();
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 연결해제
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        Debug.Log("OnDisconnected" + msg);
        Close();   //클라이언트 연결 해제
        NetManager.Instance.OnDisConnect();
    }

    /// <summary>
    /// 바이트 출력
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        Debug.LogError(returnStr);
    }

    /// <summary>
    /// 보내기
    /// </summary>
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 받기
    /// </summary>
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 2)
        {
            ushort messageLen = reader.ReadUInt16();
            if (RemainingBytes() >= messageLen)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(ms);
            }
            else
            {
                memStream.Position = memStream.Position - 2;
                break;
            }
        }
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);    
        memStream.Write(leftover, 0, leftover.Length);
    }

    /// <summary>
    /// 남은 바이트
    /// </summary>
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 메세지 받기
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));        
        ByteBuffer buffer = new ByteBuffer(message);
        int mainId = buffer.ReadShort();
        int pbDataLen = message.Length - 2;
        byte[] pbData = buffer.ReadBytes(pbDataLen);
        NetManager.Instance.DispatchProto(mainId, pbData);
    }


    /// <summary>
    /// 세션 보내기
    /// </summary>
    void SessionSend(byte[] bytes)
    {
        WriteMessage(bytes);
    }

    /// <summary>
    /// 닫기
    /// </summary>
    public void Close()
    {
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
    }

    /// <summary>
    /// 연결 요청 보내기
    /// </summary>
    public void SendConnect()
    {
        ConnectServer(AppConst.SocketAddress, AppConst.SocketPort);
    }

    public void SendSyncConnect()
    {
        SyncConnectServer(AppConst.SocketAddress, AppConst.SocketPort);
    }

    /// <summary>
    /// 소켓 메세지 보내기
    /// </summary>
    public void SendMessage(ByteBuffer buffer)
    {
        SessionSend(buffer.ToBytes());
        buffer.Close();
    }

    public bool IsConnected()
    {
        if (client != null)
        {
            return client.Connected;
        }

        return false;
    }
}
