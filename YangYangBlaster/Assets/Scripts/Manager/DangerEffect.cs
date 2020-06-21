using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerEffect : MonoBehaviour
{
    private void OnEnable()
    {        
        StartCoroutine(waitDisableEffect());
    }

    IEnumerator waitDisableEffect()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}
