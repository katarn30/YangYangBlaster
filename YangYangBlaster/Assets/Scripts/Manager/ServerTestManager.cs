using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using Grpc.Health.V1;

public class ServerTestManager : SingleTon<ServerTestManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Q))
        {
            GameManager.Instance.StageClear();
        }
#endif
    }

    public void ServerTest()
    {
        Debug.Log("Server Test Button");
    }
}
