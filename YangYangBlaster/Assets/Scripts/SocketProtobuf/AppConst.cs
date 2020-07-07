using UnityEngine;
using System.Collections;

public class AppConst {
    public static int SocketPort = 20051;
#if UNITY_EDITOR
    //public static string SocketAddress = "127.0.0.1";
    //public static string SocketAddress = "183.99.10.187";
    public static string SocketAddress = "183.99.10.187";
#else
    //public static string SocketAddress = "183.99.10.187";
    public static string SocketAddress = "ec2-18-218-253-188.us-east-2.compute.amazonaws.com";
#endif
}
