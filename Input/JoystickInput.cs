using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : IUserInput
{
    [Header("===== Joystick Settings ======")]

    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJright = "axis3";
    public string axisJup = "axis6";
    public string axisLR = "axis7";

    public string btnA = "btn0"; //□
    public string btnB = "btn1";//×
    public string btnC = "btn2";//⭕
    public string btnD = "btn3";//△
    public string btnLB = "btn4";
    public string btnLT = "btn6";
    public string btnRB = "btn5";
    public string btnRT = "btn7";
    public string btnJstick = "btn11"; //右摇杆按压


    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonD = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonLT = new MyButton();
    public MyButton buttonRB = new MyButton();
    public MyButton buttonRT = new MyButton();
    public MyButton buttonJstick = new MyButton();
    public MyButton buttonLR = new MyButton();

    void Update()
    {
        //呼出按键设置
        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        buttonC.Tick(Input.GetButton(btnC));
        buttonD.Tick(Input.GetButton(btnD));
        buttonLB.Tick(Input.GetButton(btnLB));
        buttonLT.Tick(Input.GetButton(btnLT));
        buttonRB.Tick(Input.GetButton(btnRB));
        buttonRT.Tick(Input.GetButton(btnRT));
        buttonJstick.Tick(Input.GetButton(btnJstick));
        // buttonLR.Tick(Input.GetButton(axisLR));

        //camera
        if (mouseEnable)
        {
            Jup = -1 * Input.GetAxis(axisJup);
            Jright = Input.GetAxis(axisJright);
        }


        //move
        targetDup = Input.GetAxis(axisY);
        targetDright = Input.GetAxis(axisX);

        if (!inputEnabled)
        {
            targetDup = 0;
            targetDright = 0;
        }

        DirectionUp = Mathf.SmoothDamp(DirectionUp, targetDup, ref velocityDup, 0.1f);
        DirectionRight = Mathf.SmoothDamp(DirectionRight, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(DirectionUp, DirectionRight));
        float DirectionUp2 = tempDAxis.x;
        float DirectionRight2 = tempDAxis.y;

        Dmag = Mathf.Sqrt(DirectionUp2 * DirectionUp2 + DirectionRight2 * DirectionRight2);
        Dvec = DirectionRight2 * transform.right + DirectionUp2 * transform.forward;

        //run(长按+抬手后延迟)<-缓冲
        run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;

        //defense
        defense = buttonLB.IsPressing;

        //roll(与跑步为同一按键)
        roll = buttonA.OnReleased && buttonA.IsDelaying;

        action = buttonC.OnPressed;
        //jump（按两次跑步的时候跳跃）
        //jump = buttonA.OnPressed && buttonA.IsExtending;
        jump = buttonB.OnPressed;
        //attack
        //attack = buttonC.OnPressed;
        rb = buttonRB.OnPressed;
        rt = buttonRT.OnPressed;
        lb = buttonLB.OnPressed;
        lt = buttonLT.OnPressed;

        //lockon
        lockon = buttonJstick.OnPressed;

        //← →
        LR = Input.GetAxis(axisLR);
        AxisPress(LR);

    }
    public void AxisPress(float press)
    {
        if (press == 0)
        {
            currentLR = 0;
            right = false;
            left = false;
        }
        else if (currentLR == 0 && press == 1)
        {
            right = true;
            currentLR = 1;
        }
        else if (currentLR == 0 && press == -1)
        {
            left = true;
            currentLR = -1;
        }
        else
        {
            right = false;
            left = false;
        }
    }
}
