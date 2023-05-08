using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : IActorManagerInterface
{
    public float HP = 15.0f;
    public float HPMax = 15.0f;
    public float ATK = 1f;//人物初始数值（暂定）
    public float DEF = 1f;

    [Header("1st order state flags")]
    public bool isGround;
    public bool isJump;
    public bool isFall;
    public bool isRoll;
    public bool isJab;
    public bool isAttack;
    public bool isHit;
    public bool isDie;
    public bool isBlocked;
    public bool isDefense;
    public bool isCounterBack; //已经抬手
    public bool isCounterBackEnable;//允许反击
    public bool canBreak;
    public bool isStunned;
    public bool isDown;
    public bool isPowerUp;
    public bool isAround;

    public bool startFlag = false;
    public bool haveOil;
    public bool warning = false;
    public bool doorOpen = false;
    [Header("2nd order state flags")]
    public bool isAllowDefense;
    public bool isImmortal;
    public bool isCounterBackSuccess;//盾反成功
    public bool isCounterBackFailure;


    public bool isWin = false;
    public int deadTime = 0;
    void Start()
    {
        HP = HPMax;
        isWin = false;
    }
    public void AddHP(float value)
    {
        HP += value;
        HP = Mathf.Clamp(HP, 0, HPMax);

    }
    void Update()
    {
        isGround = am.ac.CheckState("Ground");
        isJump = am.ac.CheckState("Jump");
        isFall = am.ac.CheckState("Falling");
        isRoll = am.ac.CheckState("Roll");
        isJab = am.ac.CheckState("Jab");
        isAttack = am.ac.CheckStateTag("attackR") || am.ac.CheckStateTag("attackL");
        isHit = am.ac.CheckState("hit");
        isDie = am.ac.CheckState("die");
        isBlocked = am.ac.CheckState("blocked");
        isCounterBack = am.ac.CheckState("counterBack");
        isStunned = am.ac.CheckState("stunned");
        isDown = am.ac.CheckState("down") || am.ac.CheckState("rise");
        isPowerUp = am.ac.CheckState("changeStep");

        isCounterBackSuccess = isCounterBackEnable;
        isCounterBackFailure = isCounterBack && !isCounterBackEnable;

        isAllowDefense = isGround || isBlocked; //除在地面与防御以外不可防御；
        isDefense = isAllowDefense && (am.ac.CheckState("Defencse1h", "Defense") || am.ac.CheckState("Defencse2h", "Defense"));

        isImmortal = isRoll || isJab;

        isAround = am.ac.CheckStateTag("Slash");

        if (deadTime > 0 && am.gm.count >= deadTime + 1)
        {
            isWin = true;
            deadTime = 0;
        }


    }

    public void Test()
    {
        if (isAround)
            print("?");
    }

}
