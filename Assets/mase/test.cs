using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [AddComponentMenu("mase/test")]

    private IEnumerator Start()
    {
        while (true)
        {
            Debug.LogFormat("Time:{0}", System.DateTime.Now);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
