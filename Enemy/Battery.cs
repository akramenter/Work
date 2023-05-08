using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    //炮台 间隔固定时间发射
    public GameObject missile; // 子弹
    public Transform arrowStartPos;
    float currentTime;

    void Update()
    {
        // FireTrackArrow();
        currentTime += Time.deltaTime;
    }
    public void FireTrackArrow()
    {

        if (currentTime > 1)
        {
            currentTime = 0;
            GameObject m = GameObject.Instantiate(missile, arrowStartPos.transform.position, arrowStartPos.transform.rotation);
            // m.transform.localPosition = Vector3.zero;
            m.SetActive(true);
        }
    }
}
