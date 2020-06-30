using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Msg;

public class SaveGameDataModel : BaseModel<SaveGameDataModel>
{
    protected override void InitAddTocHandler()
    {
        AddTocHandler(typeof(SaveGameDataReply), STocSaveGameDataReply);
    }

    private void STocSaveGameDataReply(object data)
    {
        SaveGameDataReply reply = data as SaveGameDataReply;

        if (ERROR_CODE.Ok == reply.Error)
        {
            Debug.Log("Ok");
        }
        else
        {
            Debug.Log(reply.Error);
        }
    }

    public void CTosSaveGameDataRequest(SaveGameDataRequest request)
    {
        //var request = new SaveGameDataRequest();

        //RpcServiceManager.Instance.SaveGameData(request, (SaveGameDataReply reply) =>
        //{
        //    if (ERROR_CODE.Ok == reply.Error)
        //    {
        //        Debug.Log("Ok");
        //    }
        //    else
        //    {
        //        Debug.Log(reply.Error);
        //    }
        //});

        SendTos(request);
    }
}
