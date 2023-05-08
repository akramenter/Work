using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Header("===== Output Signals ======")]
    public float DirectionUp;
    public float DirectionRight;
    //api
    public float Dmag;
    public Vector3 Dvec;

    public float Jup;
    public float Jright;

    //左按键上下左右
    public float LR;
    public float currentLR;
    public bool left;
    public bool right;


    //pressing signals
    public bool run;
    public bool defense;

    //trigger type signal
    public bool action;
    public bool jump;
    protected bool lastJump;
    // public bool attack;
    protected bool lastAttack;
    public bool roll;
    public bool lockon;
    public bool lb;
    public bool lt;
    public bool rb;
    public bool rt;
    //double trigger

    public bool mouseEnable = true;
    [Header("===== Others ======")]
    public bool inputEnabled = true;
    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;

    //避免斜方向速度为根号2(椭圆映射)
    protected Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }
    protected void UpdateDmagDvec(float DirectionUp2, float DirectionRight2)
    {
        Dmag = Mathf.Sqrt(DirectionUp2 * DirectionUp2 + DirectionRight2 * DirectionRight2);
        Dvec = DirectionRight2 * transform.right + DirectionUp2 * transform.forward;
    }
}
