using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : SingleTon<LoadingManager>
{
    public List<GameObject> loadingManagerList = new List<GameObject>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public IEnumerator SetLoading()
    {
        for (int i = 0; i < loadingManagerList.Count; i++)
        {
            UIManager.Instance.loadingUI.UpdateLoadingBar(i, loadingManagerList.Count);

            yield return new WaitForSeconds(0.2f);

            Instantiate(loadingManagerList[i].gameObject);            
        }

        UIManager.Instance.loadingUI.UpdateLoadingBar(loadingManagerList.Count, loadingManagerList.Count);

        yield return new WaitForSeconds(0.2f);

        
    }
}
