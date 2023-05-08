using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : IUserInput
{
    public enum BossState
    {
        Idle,
        Walk,
        Run,
        Pursuid,
        Arrow,//远距离弓箭
        AttackLance,//双手武器 枪
        AttackSword,//剑盾
        Return,//回到idle
        Die
    }
    public enum BossStep
    {
        Step1,//开场弓箭直射一套，若不靠近直线弓箭，靠近长枪普攻 hp：75~100
        Step2,//跳开龙车，结束后距离过远继续拉弓，近距离长枪    //50~75
        Step3,//剑盾   25~50
        Step4,//随机弹幕
        Null
    }
    public GameObject targetPlayer;
    public ActorManager targetAm;
    public ActorManager am;
    public BossState bossState;
    public BossStep bossStep;
    [Header("===UI & Audio===")]
    public BossHp hpBar;
    [Header("=== Navigation ===")]
    public NavMeshAgent nma; /// NavMeshAgent
    [SerializeField] private Mesh cylinderMesh;
    [SerializeField] private float closeRange; //近距离攻击范围
    [SerializeField] private float attackRange;//超过此范围追击
    [SerializeField] private Vector3 originPos;
    [SerializeField] private Battery battery;

    //public AnimatorOverrideController oneHandAnim;
    //public AnimatorOverrideController twoHandAnimSword;
    //public AnimatorOverrideController twoHandAnimHand;
    public AnimatorOverrideController twoHandAnimLance;
    public AnimatorOverrideController twoHandAnimArrow;
    public AnimatorOverrideController oneHandAnimSword;

    public WeaponData[] weapon;
    //p4浮游炮
    public GameObject livatations;
    public bool onFire = false;

    //换p无敌动画
    private BossStep current;
    private bool changeStep = false;
    // public bool onAim = false;




    [SerializeField]
    private int rand;
    float lastTime;
    float currentTime;
    //GameManager gm;
    void Start()
    {
        lastTime = Time.time;
        targetAm = targetPlayer.GetComponent<ActorManager>();
        am = GetComponent<ActorManager>();
        nma = GetComponent<NavMeshAgent>();
        originPos = transform.position;
        battery = GetComponentInChildren<Battery>();
        battery.enabled = false;
        bossState = BossState.Idle;  //换成idle

        hpBar = GetComponentInChildren<BossHp>();
        if (hpBar != null)
            hpBar.gameObject.SetActive(false);

        targetPlayer = GameObject.Find("PlayerHandle");
        targetAm = GameObject.Find("PlayerHandle").GetComponent<ActorManager>();
        weapon = GetComponentsInChildren<WeaponData>();
        livatations.SetActive(false);
        //gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
    }
    public BossState GetBossState()
    {
        return bossState;
    }

    void Update()
    {
        //ui
        UiHp();


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
        ChangeStep();
        //State
        switch (bossStep)
        {
            case BossStep.Step1:
                current = BossStep.Step1;
                StepOne();
                break;
            case BossStep.Step2:
                current = BossStep.Step2;
                StepTwo();
                break;
            case BossStep.Step3:
                current = BossStep.Step3;
                StepThree();
                break;
            case BossStep.Step4:
                current = BossStep.Step4;
                StepFour();
                break;

        }

        if (am.sm.isStunned)
        {
            am.ac.anim.ResetTrigger("Attack");
            rb = false;
        }
        //SetWeapon(weapon);

        //if (am.sm.HP > 0)
        //{
        //    targetAm.sm.isWin = false;
        //}
        Dead();

    }
    public void Dead()
    {
        GameManager gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        AnimatorStateInfo animatorInfo;
        animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);
        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("die"))
        {
            //print("deadTime:" + targetAm.sm.deadTime);
            targetAm.sm.deadTime = gm.count;
        }


    }
    public void UiHp()
    {
        //Vector3 vec = transform.position - targetPlayer.transform.position; //player到敌人的向量（player是敌人的target！）
        //float dot = Vector3.Dot(targetAm.transform.forward, vec);
        //Vector3 toTarget = targetPlayer.transform.position - transform.position;
        if (bossState != BossState.Idle)
        {
            hpBar.gameObject.SetActive(true);
        }
        else
        {
            hpBar.gameObject.SetActive(false);

        }



    }
    public void BossIdle()
    {

        am.ac.anim.runtimeAnimatorController = twoHandAnimLance;

        if (targetAm.sm.startFlag == true)  //加上进门开怪
        {
            bossState = BossState.Walk;
        }

        SetWeapon(weapon);
    }
    public void BossChangeMode()
    {

        if (!CloseToPlayer())
        {
            am.ac.anim.ResetTrigger("Attack");
            rb = false;
            bossState = BossState.Arrow;
        }
        else
        {
            am.ac.anim.ResetTrigger("Attack");
            rb = false;
            bossState = BossState.Pursuid;
        }
        if (targetAm.sm.isDie)
        {
            targetAm.sm.startFlag = false;
            bossState = BossState.Return;
        }
        SetWeapon(weapon);
    }
    public void BossPursuid()
    {
        FaceToTarget();
        MoveToTarget();
        rb = false;
        if (nma == null) return;
        if (!CloseToPlayer())
        {
            bossState = BossState.Arrow;
        }
        else
        {
            if (bossStep == BossStep.Step1 || bossStep == BossStep.Step2)
            {
                am.ac.anim.runtimeAnimatorController = twoHandAnimLance;
            }
            else
            {
                am.ac.anim.runtimeAnimatorController = oneHandAnimSword;
            }
            if (IsInAttackRange()) //切换攻击
            {
                targetDup = 0;
                targetDright = 0;
                Dvec = new Vector3(0, 0, 0);
                if (bossStep == BossStep.Step1 || bossStep == BossStep.Step2)
                {
                    bossState = BossState.AttackLance;
                    SetWeapon(weapon);
                }
                else
                {
                    bossState = BossState.AttackSword;
                    SetWeapon(weapon);
                }

            }
        }


        if (targetAm.sm.isDie)
        {
            targetAm.sm.startFlag = false;
            bossState = BossState.Return;
        }
        SetWeapon(weapon);
    }

    private int count = 0;
    public void BossAttackLanceOne()
    {
        if (targetAm.sm.isDie)
        {
            targetAm.sm.startFlag = false;
            bossState = BossState.Return;
        }
        am.ac.anim.runtimeAnimatorController = twoHandAnimLance;

        if (!IsInAttackRange())
        {
            am.ac.anim.ResetTrigger("Attack");
            count = 0;
            bossState = BossState.Pursuid;

        }
        if (!CloseToPlayer())
        {
            am.ac.anim.ResetTrigger("Attack");
            count = 0;
            FaceToTarget();
            bossState = BossState.Arrow;
        }

        targetDup = 0;
        targetDright = 0;
        // Dvec = new Vector3(0, 0, 0);
        if (am.ac.CheckState("Ground"))
        {
            rb = true;
        }

        AnimatorStateInfo animatorInfo;
        animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);
        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Attack1hLance"))
        {
            FaceToTarget();
            count++;
        }
        if (count >= 5)
        {
            am.ac.anim.ResetTrigger("Attack");
            rb = false;
            if ((animatorInfo.normalizedTime >= 0.9f) && (animatorInfo.IsName("Ground")))
            {
                count++;
            }
            FaceToTarget();
        }
        if (count >= 8)
        {
            rb = false;

            am.ac.anim.SetTrigger("around");

            //Slash
            count = 0;

        }


        if (am.sm.HP <= 50)
        {
            bossState = BossState.AttackSword;
        }



        SetWeapon(weapon);
    }
    public void BossAttackLanceTwo()
    {
        if (targetAm.sm.isDie)
        {
            targetAm.sm.startFlag = false;
            bossState = BossState.Return;
        }
        am.ac.anim.runtimeAnimatorController = twoHandAnimLance;

        AnimatorStateInfo animatorInfo;
        animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);

        if (current == BossStep.Step3 && (animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("changeStep"))
        {
            am.ac.anim.runtimeAnimatorController = oneHandAnimSword;
            bossState = BossState.AttackSword;
            SetWeapon(weapon);
        }

        //if (!IsInAttackRange())
        //{
        //    am.ac.anim.ResetTrigger("Attack");
        //    am.ac.anim.SetTrigger("trust");
        //    count = 0;
        //    bossState = BossState.Pursuid;

        //}

        if (!CloseToPlayer())
        {
            am.ac.anim.ResetTrigger("Attack");
            count = 0;
            FaceToTarget();
            bossState = BossState.Arrow;
        }

        targetDup = 0;
        targetDright = 0;
        // Dvec = new Vector3(0, 0, 0);
        if (am.ac.CheckState("Ground"))
        {
            if (count == 0)
                FaceToTarget();
            rb = true;

        }

        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Attack1hLance"))
        {
            FaceToTarget();
            count++;
        }
        if (count >= 7)
        {
            am.ac.anim.ResetTrigger("Attack");
            rb = false;
            // if ((animatorInfo.normalizedTime >= 0.9f) && (animatorInfo.IsName("Ground")))
            // {
            count++;
            // }
            //FaceToTarget();
        }

        if (count >= 13 && count < 30)
        {
            if (!CloseToPlayer())
            {
                am.ac.anim.ResetTrigger("Attack");
                count = 0;
                FaceToTarget();
                bossState = BossState.Arrow;
            }
            count = 15;
            rb = false;
            am.ac.IssueTrigger("Jump");
            if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Jab"))
            {
                am.ac.anim.SetTrigger("trust");
                am.ac.anim.ResetTrigger("Jump");
            }

            if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Trust"))
            {
                am.ac.anim.ResetTrigger("Jump");
                //FaceToTarget();
                //rb = false;  
                count = 30;
                count++;
            }
            if (!CloseToPlayer())
            {
                am.ac.anim.ResetTrigger("Attack");
                count = 0;
                FaceToTarget();
                bossState = BossState.Arrow;
            }

        }
        if (count >= 150)
        {
            //FaceToTarget();
            count = 0;
        }

        if (am.sm.HP <= 50)
        {
            bossState = BossState.AttackSword;
            SetWeapon(weapon);
        }
        SetWeapon(weapon);
    }

    public void BossAttackSword()
    {
        if (targetAm.sm.isDie)
        {
            targetAm.sm.startFlag = false;
            bossState = BossState.Return;
        }
        am.ac.anim.runtimeAnimatorController = oneHandAnimSword;
        if (!IsInAttackRange())
        {
            am.ac.anim.ResetTrigger("Attack");
            rb = false;
            bossState = BossState.Pursuid;
            count = 0;
        }

        targetDup = 0;
        targetDright = 0;
        //FaceToTarget();
        if (am.ac.CheckState("Ground"))
        {
            if (count == 0)
                FaceToTarget();
            rb = true;
        }

        AnimatorStateInfo animatorInfo;
        animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);
        if ((animatorInfo.normalizedTime >= 0.9f) && (animatorInfo.IsName("Attack1hC")/* || animatorInfo.IsName("Attack1hD")*/ || animatorInfo.IsName("Attack1hE")))
        {
            FaceToTarget();
            count++;
        }
        if (count >= 3)
        {
            am.ac.anim.ResetTrigger("Attack");
            rb = false;
            if ((animatorInfo.normalizedTime >= 0.9f) && (animatorInfo.IsName("Ground")))
            {
                FaceToTarget();
                count++;
            }

        }
        if (count >= 5)
        {
            rand = Random.Range(0, 2);
            if (rand == 0)
            {
                rb = true;
                am.ac.anim.SetFloat("random", 0);

            }
            else
            {
                rb = true;
                am.ac.anim.SetFloat("random", 1);
            }
            if ((animatorInfo.normalizedTime >= 0.9f) && (animatorInfo.IsName("Attack1hC") || animatorInfo.IsName("Attack1hE") /*|| animatorInfo.IsName("Attack1hD")*/))
            {
                am.ac.anim.ResetTrigger("Attack");
                rb = false;
                count = 0;
            }
        }
        SetWeapon(weapon);
    }

    public void BossArrowOne()
    {
        if (CloseToPlayer())
        {
            count = 0;
            am.ac.IssueTrigger("Attack");
            bossState = BossState.Pursuid;
        }
        am.ac.anim.ResetTrigger("Attack");
        targetDup = 0;
        targetDright = 0;
        am.ac.anim.runtimeAnimatorController = twoHandAnimArrow;
        if (targetAm.sm.isDie)
        {
            targetAm.sm.startFlag = false;
            bossState = BossState.Return;
        }

        AnimatorStateInfo animatorInfo;
        animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);
        if (count == 0)
        {
            FaceToTarget();
            am.ac.IssueTrigger("aim");
        }

        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Aim"))
        {
            count++;
            // 
        }
        if (count >= 50)
        {
            am.ac.anim.ResetTrigger("aim");
            am.ac.IssueTrigger("Attack");

        }
        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("AttackArrow"))
        {

            FaceToTarget();
        }
        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Ground"))
        {
            count = 0;
        }
        //if (count >= 12)
        //{
        //    FaceToTarget();
        //    count = 0;
        //}
        SetWeapon(weapon);
    }
    public void BossArrowTwo()
    {
        if (CloseToPlayer())
        {
            count = 0;
            am.ac.IssueTrigger("Attack");
            bossState = BossState.Pursuid;
        }
        am.ac.anim.ResetTrigger("Attack");
        targetDup = 0;
        targetDright = 0;
        am.ac.anim.runtimeAnimatorController = twoHandAnimArrow;
        if (targetAm.sm.isDie)
        {
            targetAm.sm.startFlag = false;
            bossState = BossState.Return;
        }
        battery.enabled = true;
        AnimatorStateInfo animatorInfo;
        animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);
        if (count == 0)
        {
            FaceToTarget();
            am.ac.IssueTrigger("aim");
        }

        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Aim"))
        {
            count++;
            battery.FireTrackArrow();

        }
        if (count >= 800)
        {
            am.ac.anim.ResetTrigger("aim");

            am.ac.IssueTrigger("Attack");

        }
        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("AttackArrow"))
        {

            FaceToTarget();
        }
        if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("Ground"))
        {

            count = 0;
        }

        //if (count >= 12)
        //{
        //    FaceToTarget();
        //    count = 0;
        //}
        SetWeapon(weapon);
    }
    public void BossReturn()
    {
        targetAm.sm.startFlag = false;
        rb = false;
        //transform.position = originPos;
        //transform.rotation = originRoation;
        //bossState = BossState.Idle;
        if (nma == null) return;
        am.sm.HP = am.sm.HPMax;
        Vector3 vd = nma.destination - transform.position;
        Vector3 normal = vd.normalized;
        DirectionUp = -normal.z;
        DirectionRight = -normal.x;
        nma.SetDestination(originPos);
        transform.SetPositionAndRotation(transform.position, Quaternion.Lerp(transform.rotation, new Quaternion(0, -180, 0, 0), 0.5f)); //转向恢复默认
        if (!nma.pathPending && nma.remainingDistance < 0.5f)
        {
            DirectionUp = 0;
            DirectionRight = 0;
            bossState = BossState.Idle;

        }
    }
    public void StepOne()
    {
        if (livatations.activeSelf)
        {
            livatations.SetActive(false);
        }
        //随机攻击
        //rand = Random.Range(0, 2);
        switch (bossState)
        {
            case BossState.Idle:
                BossIdle();
                break;
            case BossState.Walk:
                BossChangeMode();
                break;
            case BossState.Pursuid:
                BossPursuid();
                break;
            case BossState.Arrow:
                BossArrowOne();
                break;
            case BossState.AttackLance:
                BossAttackLanceOne();
                break;
            case BossState.AttackSword:
                BossAttackSword();
                break;
            case BossState.Return:
                BossReturn();
                break;

        }

    }
    public void StepTwo()
    {
        if (livatations.activeSelf)
        {
            livatations.SetActive(false);
        }
        //随机攻击
        // rand = Random.Range(0, 2);
        switch (bossState)
        {
            case BossState.Idle:
                BossIdle();
                break;
            case BossState.Walk:
                BossChangeMode();
                break;
            case BossState.Pursuid:
                BossPursuid();
                break;
            case BossState.Arrow:
                BossArrowTwo();
                break;
            case BossState.AttackLance:
                BossAttackLanceTwo();
                break;
            case BossState.AttackSword:
                BossAttackSword();
                break;

            case BossState.Return:
                BossReturn();
                break;

        }

    }
    public void StepThree()
    {
        if (livatations.activeSelf)
        {
            livatations.SetActive(false);
        }
        //随机攻击
        //rand = Random.Range(0, 2);
        switch (bossState)
        {
            case BossState.Idle:
                BossIdle();
                break;
            case BossState.Walk:
                BossChangeMode();
                break;
            case BossState.Pursuid:
                BossPursuid();
                break;
            case BossState.Arrow:
                BossArrowTwo();
                break;
            case BossState.AttackLance:
                BossAttackLanceTwo();
                break;
            case BossState.AttackSword:
                BossAttackSword();
                break;

            case BossState.Return:
                BossReturn();
                break;

        }

    }
    public int fireMode = 0;
    public void StepFour()
    {
        if (!livatations.activeSelf)
        {
            AnimatorStateInfo animatorInfo;
            animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);
            if ((animatorInfo.normalizedTime >= 0.9f) && animatorInfo.IsName("changeStep"))
            {
                livatations.SetActive(true);
            }
        }
        OnFire();
        //随机攻击
        rand = Random.Range(0, 2);
        switch (bossState)
        {
            case BossState.Idle:
                BossIdle();
                break;
            case BossState.Walk:
                BossChangeMode();
                break;
            case BossState.Pursuid:
                BossPursuid();
                break;
            case BossState.Arrow:
                BossArrowOne();
                break;
            case BossState.AttackLance:
                BossAttackLanceTwo();
                break;
            case BossState.AttackSword:
                BossAttackSword();
                break;

            case BossState.Return:
                BossReturn();
                break;
        }
    }

    public void OnFire()
    {
        currentTime = Time.time;
        if (currentTime - lastTime >= 10)
        {
            //print(30);
            fireMode = Random.Range(0, 3);
            onFire = true;
            lastTime = currentTime;
        }
        else
        {
            onFire = false;
        }

    }




    public void ChangeStepAnim()
    {

        if (changeStep && bossStep != current)
        {
            am.ac.IssueTrigger("changeStep");
            changeStep = false;
        }

    }
    public void ChangeStep()
    {
        if (am.sm.HP > 0.75f * am.sm.HPMax)
        {
            bossStep = BossStep.Step1;
            //changeStep = true;
            // StepOne();
        }
        else if (am.sm.HP > 0.5f * am.sm.HPMax && am.sm.HP <= 0.75f * am.sm.HPMax)
        {
            changeStep = true;
            bossStep = BossStep.Step2;
            ChangeStepAnim();
            //StepTwo();
        }
        else if (am.sm.HP > 0.25f * am.sm.HPMax && am.sm.HP <= 0.5f * am.sm.HPMax)
        {
            changeStep = true;

            bossStep = BossStep.Step3;
            ChangeStepAnim();
            //StepTwo();
        }
        else /*if (am.sm.HP > 0f && am.sm.HP <= 0.25f * am.sm.HPMax)*/
        {
            changeStep = true;

            bossStep = BossStep.Step4;
            ChangeStepAnim();
            //StepOne();
        }
        //else //防止一拳打死
        //{
        //    AnimatorStateInfo animatorInfo;
        //    animatorInfo = am.ac.anim.GetCurrentAnimatorStateInfo(0);
        //    if (!animatorInfo.IsName("die"))
        //        am.ac.IssueTrigger("die");
        //}
    }
    public bool CloseToPlayer()//是否为近战攻击
    {
        if (targetPlayer == null) return false;
        Vector3 vec = targetPlayer.transform.position - transform.position;

        if (vec.sqrMagnitude < closeRange * closeRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MoveToTarget()
    {
        if (targetPlayer == null)
        {
            return;
        }
        Vector3 targetPos = targetPlayer.transform.position;
        Vector3 vec = targetPos - transform.position;
        vec.y = 0;
        targetPos -= vec.normalized * attackRange * 0.9f;
        targetDup = 1;
        targetDright = 0;

        SetTarget(targetPos);

    }
    public void FaceToTarget()
    {
        if (targetPlayer != null)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(targetPlayer.transform.position.x, 0, targetPlayer.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)), 50 * Time.deltaTime);
    }
    public void SetTarget(Vector3 tarPos)
    {
        nma.destination = tarPos;
        nma.isStopped = true;
    }
    public bool IsInAttackRange()
    {
        if (targetPlayer == null)
        {
            return false;
        }
        Vector3 toTarget = targetPlayer.transform.position - transform.position;
        return toTarget.sqrMagnitude < attackRange * attackRange;
    }
    public void SetWeapon(WeaponData[] weaponData)
    {

        switch (bossState)
        {
            case BossState.Idle:
                foreach (var wd in weaponData)
                {
                    if (wd.name == "Lance")
                    {
                        wd.gameObject.SetActive(true);
                    }
                    else
                    {
                        wd.gameObject.SetActive(false);
                    }
                }
                break;
            case BossState.Walk:
                if (am.sm.HP > 0.5f * am.sm.HPMax)
                {
                    foreach (var wd in weaponData)
                    {
                        if (wd.name == "Lance")
                        {
                            wd.gameObject.SetActive(true);
                            am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                        }
                        else
                        {
                            wd.gameObject.SetActive(false);
                        }
                    }
                }
                else if (am.sm.HP > 0.25f * am.sm.HPMax && am.sm.HP <= 0.5f * am.sm.HPMax)
                {
                    foreach (var wd in weaponData)
                    {
                        if (wd.name == "BKSword" || wd.name == "BKShild")
                        {
                            wd.gameObject.SetActive(true);
                            if (wd.name == "BKSword")
                            {
                                //am.wm.whR = wd.gameObject;
                                // am.wm.wcR = am.wm.BindWeaponController(am.wm.whR);
                                am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                            }
                        }
                        else
                        {
                            wd.gameObject.SetActive(false);
                        }
                    }
                }
                else if (am.sm.HP <= 0.25f * am.sm.HPMax)
                {
                    foreach (var wd in weaponData)
                    {
                        if (wd.name == "BKSword" || wd.name == "BKShild")
                        {
                            wd.gameObject.SetActive(true);
                            if (wd.name == "BKSword")
                            {

                                am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                            }
                        }
                        else
                        {
                            wd.gameObject.SetActive(false);
                        }
                    }
                }
                break;
            case BossState.Pursuid:
                if (am.sm.HP > 0.5f * am.sm.HPMax)
                {
                    foreach (var wd in weaponData)
                    {
                        if (wd.name == "Lance")
                        {
                            wd.gameObject.SetActive(true);
                            am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                        }
                        else
                        {
                            wd.gameObject.SetActive(false);
                        }
                    }
                }
                else if (/*am.sm.HP > 0.25f * am.sm.HPMax && */am.sm.HP <= 0.5f * am.sm.HPMax)
                {
                    //foreach (var wd in weaponData)
                    //{
                    //    if (wd.name == "BKSword" || wd.name == "BKShild")
                    //    {
                    //        wd.gameObject.SetActive(true);
                    //        am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                    //    }
                    //    else
                    //    {
                    //        wd.gameObject.SetActive(false);
                    //    }
                    //}
                    foreach (var wd in weaponData)
                    {
                        if (wd.name == "BKSword" || wd.name == "BKShild")
                        {
                            wd.gameObject.SetActive(true);
                            if (wd.name == "BKSword")
                            {

                                am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                            }
                        }
                        else
                        {
                            wd.gameObject.SetActive(false);
                        }
                    }
                }
                //else if (am.sm.HP <= 0.25f * am.sm.HPMax)
                //{
                //    foreach (var wd in weaponData)
                //    {
                //        if (wd.name == "BKSword" || wd.name == "BKShild")
                //        {
                //            wd.gameObject.SetActive(true);
                //            am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                //        }
                //        else
                //        {
                //            wd.gameObject.SetActive(false);
                //        }
                //    }
                //}
                break;
            case BossState.Arrow:
                foreach (var wd in weaponData)
                {
                    if (wd.name == "Arrow1" || wd.name == "Arrow2" || wd.name == "Arche")
                    {
                        wd.gameObject.SetActive(true);
                    }
                    else
                    {
                        wd.gameObject.SetActive(false);
                    }
                }
                break;
            case BossState.AttackLance:
                foreach (var wd in weaponData)
                {
                    if (wd.name == "Lance")
                    {
                        wd.gameObject.SetActive(true);
                        am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                    }
                    else
                    {
                        wd.gameObject.SetActive(false);
                    }
                }
                break;
            case BossState.AttackSword:
                foreach (var wd in weaponData)
                {
                    if (wd.name == "BKSword" || wd.name == "BKShild")
                    {
                        wd.gameObject.SetActive(true);
                        if (wd.name == "BKSword")
                        {

                            am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                        }
                        //am.wm.weaponColR = wd.gameObject.GetComponent<Collider>();
                    }
                    else
                    {
                        wd.gameObject.SetActive(false);
                    }
                }
                break;
            case BossState.Return:

                break;

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireMesh(cylinderMesh, transform.position, Quaternion.identity,
            new Vector3(closeRange * 2f, 0.5f, closeRange * 2f));

        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(cylinderMesh, transform.position, Quaternion.identity,
          new Vector3(attackRange * 2f, 0.5f, attackRange * 2f));
    }
}
