using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput
{
    //Variable
    [Header("===== Keys Settings ======")]
    public string KeyUp;
    public string KeyDown;
    public string KeyLeft;
    public string KeyRight;

    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;
    //右摇杆摄像机
    public string keyJRight;
    public string keyJLeft;
    public string keyJUp;
    public string keyJDown;

    public string keyLT;
    public string keyRT;
    public string keyRB;
    public string keyLB;

    //锁定
    public string keyTab;

    [Header("===== Mouse Settings ======")]
    //public bool mouseEnable = true;
    public float mouseSensitivityX = 1.0f; //灵敏度
    public float mouseSensitivityY = 1.0f;

    MyButton buttonA = new MyButton();
    MyButton buttonB = new MyButton();
    MyButton buttonC = new MyButton();
    MyButton buttonD = new MyButton();
    MyButton buttonTab = new MyButton();
    MyButton buttonLT = new MyButton();
    MyButton buttonRT = new MyButton();
    MyButton buttonLB = new MyButton();
    MyButton buttonRB = new MyButton();
    void Update()
    {
        buttonA.Tick(Input.GetKey(keyA));
        buttonB.Tick(Input.GetKey(keyB));
        buttonC.Tick(Input.GetKey(keyC));
        buttonD.Tick(Input.GetKey(keyD));
        buttonTab.Tick(Input.GetKey(keyTab));

        buttonLT.Tick(Input.GetKey(keyLT));
        buttonRT.Tick(Input.GetKey(keyRT));
        buttonLB.Tick(Input.GetKey(keyLB));
        buttonRB.Tick(Input.GetKey(keyRB));
        //camera
        if (mouseEnable)
        {
            Jup = Input.GetAxis("Mouse Y") * 4.0f * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * 5.0f * mouseSensitivityX;
        }
        else
        {
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);
        }

        //move
        targetDup = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0);

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

        run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;
        defense = buttonLB.IsPressing;
        jump = buttonB.OnPressed;
        action = buttonC.OnPressed;
        rb = buttonRB.OnPressed;
        lb = buttonLB.OnPressed;
        rt = buttonRT.OnPressed;
        lt = buttonLT.OnPressed;
        //lock on
        lockon = buttonTab.OnPressed;
    }
}
