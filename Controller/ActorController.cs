using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public IUserInput pi;
    public float walkSpeed = 2.4f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 5.0f;
    public float rollVelocity = 1.0f;
    public CameraController camcon;

    [Space(10)]
    [Header("===== Friction Settings =====")]
    //摩擦，防止在坡侧面挂住
    public PhysicMaterial frictionone;
    public PhysicMaterial frictionzero;

    // [SerializeField]
    public Animator anim;
    public ActorManager am;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec; //冲量
    [SerializeField]
    private bool canAttack;
    private bool lockPlanar = false;
    private bool trackDirection = false;
    private CapsuleCollider col;
    // private float lerpTarget;
    private Vector3 deltaPos;


    public bool leftIsShield = true;

    public delegate void OnActionDelegate();
    public event OnActionDelegate OnAction;

    //public WeaponChange weaponChange;

    void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach (var input in inputs)
        {
            if (input.enabled)
            {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        camcon = GetComponentInChildren<CameraController>();
        am = GetComponent<ActorManager>();

    }
    void Update()
    {
        if (camcon == null || pi == null) return;
        //LockOn
        if (pi.lockon)
        {
            camcon.LockUnLock();
        }

        //----animation------
        //自由移动与锁定移动
        if (!camcon.lockState)
        {
            anim.SetFloat("Forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("Forward"), ((pi.run) ? 2.0f : 1.0f), 0.5f));
            anim.SetFloat("Right", 0);
        }
        else
        {
            Vector3 localDVec = transform.InverseTransformVector(pi.Dvec);
            if (camcon.isAI)
            {
                anim.SetFloat("Forward", Mathf.Lerp(localDVec.z, localDVec.z * ((pi.run) ? 2.0f : 1.0f), 1 - 0.1f * Time.deltaTime));
                anim.SetFloat("Right", Mathf.Lerp(localDVec.x, localDVec.x * ((pi.run) ? 2.0f : 1.0f), 1 - 0.1f * Time.deltaTime));
            }
            else
            {
                anim.SetFloat("Forward", localDVec.z * ((pi.run) ? 2.0f : 1.0f));
                anim.SetFloat("Right", localDVec.x * ((pi.run) ? 2.0f : 1.0f));
            }
        }

        //roll
        if (pi.roll/*(pi.jump && rigid.velocity.magnitude > 0.0f) || rigid.velocity.magnitude > 7.0f*/)
        {
            anim.SetTrigger("Roll");
            canAttack = false;
        }

        //jump
        if (pi.jump)
        {
            anim.SetTrigger("Jump");
            canAttack = false;
        }

        //左右肩键（普攻与防御）
        if ((pi.rb || pi.lb)
            && (CheckState("Ground") || CheckStateTag("attackR") || CheckStateTag("attackL"))
            && canAttack)
        {
            if (pi.rb)
            {
                anim.SetBool("R0L1", false);
                anim.SetTrigger("Attack");
            }
            else if (pi.lb && !leftIsShield)//不是 盾
            {
                anim.SetBool("R0L1", true);
                anim.SetTrigger("Attack");
            }

        }

        //左右扳机键（重攻击与盾反）
        if ((pi.rt || pi.lt)
            && (CheckState("Ground")
            || CheckStateTag("attackR") || CheckStateTag("attackL"))
            && canAttack)
        {
            if (pi.rt)
            {
                //右重击
            }
            else
            {
                if (!leftIsShield)//不是 盾
                {
                    //左重击
                }
                else
                {
                    //盾返
                    if (am.wm.wcL.GetName() == "Shild")
                        anim.SetTrigger("counterBack");

                }
            }

        }

        //左手是否为盾
        if (leftIsShield)
        {
            if (CheckState("Ground") || CheckState("blocked"))
            {
                anim.SetLayerWeight(anim.GetLayerIndex("Defense"), 1);
                anim.SetBool("Defense", pi.defense);
            }
            else
            {
                anim.SetBool("Defense", false);
                anim.SetLayerWeight(anim.GetLayerIndex("Defense"), 0);
            }

        }
        else
        {
            anim.SetLayerWeight(anim.GetLayerIndex("Defense"), 0);
        }

        //角色Forward设置
        if (!camcon.lockState)
        {
            if (pi.inputEnabled == true)
            {
                if (pi.Dmag > 0.1f) //不回到原先的朝向，将转动后的面向作为新的forward
                {
                    model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
                }
            }
            if (!lockPlanar)
            {
                planarVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }
        }
        else
        {
            if (!trackDirection)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planarVec.normalized;
            }

            if (!lockPlanar)
            {
                planarVec = pi.Dvec * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }
        }

        //有演出的互动按键（斩杀开箱等）
        if (pi.action)
        {
            if (am.im.overLapEcastms.Count != 0)
            {
                if (am.im.overLapEcastms[0].eventName != "frontStab")
                {
                    OnAction.Invoke();
                }
                else
                {
                    if (am.sm.canBreak)
                        OnAction.Invoke();
                }
            }
            else
            {
                // print("what`s up");
            }


        }

        //切换武器
        if (am.weaponChange != null)
        {
            if (pi.left || Input.GetKeyDown(KeyCode.E))
            {
                am.weaponChange.inHandNo++;
            }
            else if (pi.right || Input.GetKeyDown(KeyCode.Q))
            {
                am.weaponChange.inHandNo--;
            }
        }


    }
    void FixedUpdate()
    {
        rigid.position += deltaPos;
        //rigid.position += planarVec * Time.fixedDeltaTime; //1/50 
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    public bool CheckState(string stateName, string layerName = "Base Layer")
    {
        //int layerIndex = anim.GetLayerIndex(layerName);
        //bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }
    public bool CheckStateTag(string tagName, string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsTag(tagName);
    }
    /// 
    /// Message Processing Block
    /// 
    public void OnJumpEnter()
    {
        thrustVec = new Vector3(0, jumpVelocity, 0);
        pi.inputEnabled = false;
        lockPlanar = true;
        trackDirection = true;
    }

    public void IsGround()
    {
        anim.SetBool("IsGround", true);
    }
    public void IsNotGround()
    {
        anim.SetBool("IsGround", false);
    }
    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlanar = false;
        trackDirection = false;
        canAttack = true;
        col.material = frictionone;
    }
    public void OnGroundExit()
    {
        col.material = frictionzero;
    }
    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;
    }
    public void OnRollEnter()
    {
        thrustVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnabled = false;
        lockPlanar = true;
        trackDirection = true;
    }
    public void OnJabEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;

    }
    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
    }
    public void OnTrustEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;
    }
    public void OnTrustUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("trustVelocity");
    }
    public void OnAttack1hAEnter()
    {
        pi.inputEnabled = false;
    }

    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("Attack1hAVelocity");
    }
    public void OnAttack1hBUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("Attack1hAVelocity");
    }
    public void OnAttack1hCUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("Attack1hAVelocity");
    }
    public void OnAttackHandUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("AttackHandVelocity");
    }

    public void OnAttackExit()
    {
        model.SendMessage("WeaponDisable");
    }

    public void OnUpdateRM(object _deltaPos)
    {
        //第三段攻击向前一步
        if (CheckState("Attack1hC")) //right hand
        {
            //if (camcon.isAI)
            //{ deltaPos += 10 * (Vector3)_deltaPos; }
            //else
            deltaPos += /*0.8f * deltaPos + 0.2f **/ (Vector3)_deltaPos;

        }
        if (CheckState("Attack1hC 0")) //left hand
        {
            deltaPos += /*0.8f * deltaPos + 0.2f **/ (Vector3)_deltaPos;
        }
        if (CheckState("Attack1hLance")) //right hand
        {
            deltaPos += (Vector3)_deltaPos;
        }


    }

    public void OnHitEnter()
    {
        //原地硬直
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");

    }
    public void OnBlockedEnter()
    {
        pi.inputEnabled = false;
    }
    public void OnDieEnter()
    {
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }
    public void OnStunnedEnter()
    {
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }
    public void OnCounterBackEnter()
    {
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }
    public void OnLockEnter()
    {
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
        anim.ResetTrigger("Attack");


    }
    public void OnSitEnter()
    {
        pi.inputEnabled = false;
        pi.mouseEnable = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
        anim.ResetTrigger("Attack");
    }
    public void OnStandEnter()
    {
        planarVec = Vector3.zero;
        am.sm.HP = am.sm.HPMax;
        am.bm.defCol.enabled = true;
        camcon.enabled = true;
        model.SendMessage("WeaponDisable");
        anim.ResetTrigger("Attack");
        anim.ResetTrigger("Jump");
        pi.mouseEnable = true;
    }
    public void OnDownEnter()
    {
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
        model.SendMessage("WeaponDisable");
    }
    public void OnRiseEnter()
    {
        pi.inputEnabled = false;
        planarVec = Vector3.zero;
    }
    public void IssueTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
    public void SetBool(string boolName, bool value)
    {
        anim.SetBool(boolName, value);
    }


}
