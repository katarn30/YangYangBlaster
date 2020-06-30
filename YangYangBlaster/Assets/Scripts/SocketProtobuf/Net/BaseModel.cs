using UnityEngine;
using System.Collections;
using System;
using Net;
using Util;
using Google.Protobuf;

public delegate void TocHandler(object data);
public abstract class BaseModel<T> : MonoBehaviour where T : BaseModel<T>
{
    private static T _instance = null;
    private static object _syncobj = new object();
    private static bool appIsClosing = false;

    public static T Instance
    {
        get
        {
            if (appIsClosing)
                return null;

            lock (_syncobj)
            {
                if (_instance == null)
                {
                    T[] objs = FindObjectsOfType<T>();

                    if (objs.Length > 0)
                        _instance = objs[0];

                    if (objs.Length > 1)
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");

                    if (_instance == null)
                    {
                        string goName = typeof(T).ToString();
                        GameObject go = GameObject.Find(goName);
                        if (go == null)
                            go = new GameObject(goName);
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
    }

    protected virtual void OnApplicationQuit()
    {
        // release reference on exit
        appIsClosing = true;
    }

    public static bool Exists
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
            Exists = true;
        }
        else if (_instance != this)
        {
            throw new InvalidOperationException("Can't have two instances of a view");
        }
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        InitAddTocHandler();
    }

    protected abstract void InitAddTocHandler();

    protected void AddTocHandler(Type type, TocHandler handler)
    {
        NetManager.Instance.AddHandler(type, handler);
    }

    protected void SendTos(IMessage obj)
    {
        NetManager.Instance.SendMessage(obj);
    }
}
