using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool IsPressing = false; //正在被按压
    public bool OnPressed = false;  //刚刚按下
    public bool OnReleased = false;  //抬起
    public bool IsExtending = false; //快速二次按压等待时间
    public bool IsDelaying = false; //长按（比平时按下的时间延后一点）

    public float extendingDuration = 0.15f; //抬手后等待的时间
    public float delayingDuration = 0.15f; //按下后的延长时间

    private bool curState = false;
    private bool lastState = false;//前一次

    //计时器
    private MyTimer extTimer = new MyTimer();
    private MyTimer delayTimer = new MyTimer();

    public void Tick(bool input)
    {
        extTimer.Tick(); //呼出计时器
        delayTimer.Tick();

        curState = input;
        IsPressing = curState;

        OnPressed = false;
        OnReleased = false;
        IsExtending = false;
        IsDelaying = false;

        if (curState != lastState)
        {
            if (curState == true)
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration); //按下后开始计时
            }
            else
            {
                OnReleased = true;
                StartTimer(extTimer, extendingDuration); //抬手后开始计时
            }
        }

        lastState = curState;

        if (extTimer.state == MyTimer.STATE.RUN)
        {
            IsExtending = true;
        }
        if (delayTimer.state == MyTimer.STATE.RUN)
        {
            IsDelaying = true;
        }


    }

    private void StartTimer(MyTimer timer, float duration)
    {
        timer.duration = duration;
        timer.Go();
    }
}
