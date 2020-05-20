using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : SingleTon<LoginManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void GoogleLogin()
    {

    }
}
