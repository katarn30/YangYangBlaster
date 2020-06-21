using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour
{
    public Image loadingGauge;    

    public void SetLoadingUI()
    {        
        loadingGauge.fillAmount = 0;
    }

    public void UpdateLoadingBar(int _count, int _maxCount)
    {
        loadingGauge.fillAmount = (float)_count / (float)_maxCount;
    }
}
