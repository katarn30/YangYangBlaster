using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTestManager : SingleTon<ServerTestManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void ServerTest()
    {
        Debug.Log("Server Test Button");
    }
}
