using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//计时器
public class MyTimer
{
    public enum STATE
    {
        IDLE,
        RUN,
        FINISHED
    }
    public STATE state; //状态

    public float duration = 1.0f; //延长时间
    private float elapsedTime = 0.0f; //已经过时间

    public void Tick()
    {
        switch (state)
        {
            case STATE.IDLE:
                break;

            case STATE.RUN:
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= duration)
                {
                    state = STATE.FINISHED;
                }
                break;

            case STATE.FINISHED:
                break;

            default:
                Debug.Log("MyTimer error");
                break;
        }

    }
    //开始计时
    public void Go()
    {
        elapsedTime = 0.0f;
        state = STATE.RUN;
    }

}
