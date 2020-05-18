using UnityEngine;

// RpcService 사용 예시 코드

public class RpcServiceExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 사용 예시
        RpcServiceExampleRequest request = new RpcServiceExampleRequest();
        request.Arg1 = 1;
        request.Arg2 = 2.0f;
        request.Arg3 = "3";
        request.Arg4 = true;
        request.Arg5.Add(4);
        request.Arg5.Add(5);

        // 요청
        RpcServiceManager.Instance.RpcServiceExample(request, delegate (RpcServiceExampleReply reply)
        {
            // 응답
            Debug.Log("RpcService : " + reply);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
