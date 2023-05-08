using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ActorManager : MonoBehaviour
{
    public GameManager gm;
    public ActorController ac;
    [Header("=== Auto Generate if Null ===")]
    public WeaponManager wm;
    public BattleManager bm;
    public StateManager sm;
    public DirectorManager dm;
    public InteractionManager im;
    [Header("=== Override Animator ===")]
    public ChangeAnimation ca;
    [Header("=== Renderer ===")]
    public ChangeMaterials[] dissovle;

    public WeaponChange weaponChange;
    public bool blurEffect = false;
    private int currentCount;
    private Collider col;

    void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameManager>();
        ac = GetComponent<ActorController>();
        GameObject model = ac.model;
        GameObject sensor = null;
        try
        {
            sensor = transform.Find("Senser").gameObject;
        }
        catch
        {
            //
            // if there is no sensor object
            //
        }

        bm = Bind<BattleManager>(sensor);
        wm = Bind<WeaponManager>(model);
        sm = Bind<StateManager>(gameObject);
        dm = Bind<DirectorManager>(gameObject);
        im = Bind<InteractionManager>(sensor);
        ac.OnAction += DoAction;


        weaponChange = GetComponent<WeaponChange>();
        col = GetComponent<Collider>();
        dissovle = GetComponentsInChildren<ChangeMaterials>();
        foreach (var d in dissovle)
        {
            d.enabled = false;
        }


        ca = GetComponentInChildren<ChangeAnimation>();

    }

    public void DoAction()
    {
        if (im.overLapEcastms.Count != 0)
        {
            //互动存在同时没在播放其他动画
            if (im.overLapEcastms[0].active == true && !dm.IsPlaying())
            {
                //play corresponding timeline
                switch (im.overLapEcastms[0].eventName)
                {
                    //斩杀
                    case "frontStab":
                        dm.PlayFrontStab("frontStab", this, im.overLapEcastms[0].am);
                        break;

                    //开宝箱
                    case "openBox":
                        if (BattleManager.CheckAnglePlayer(ac.model, im.overLapEcastms[0].am.gameObject, 180))
                        {
                            im.overLapEcastms[0].active = false;
                            transform.position = im.overLapEcastms[0].am.transform.position
                                + im.overLapEcastms[0].am.transform.TransformVector(im.overLapEcastms[0].offset);
                            ac.model.transform.LookAt(im.overLapEcastms[0].am.transform, Vector3.up);
                            dm.PlayFrontStab("openBox", this, im.overLapEcastms[0].am);
                            sm.haveOil = true;
                        }
                        break;

                    //拉杆
                    case "leverUp":
                        if (BattleManager.CheckAnglePlayer(ac.model, im.overLapEcastms[0].am.gameObject, 180))
                        {
                            if (!sm.haveOil)
                            {
                                sm.warning = true;
                                //print("no oil");
                                //声音                               
                            }
                            else
                            {
                                im.overLapEcastms[0].active = false;
                                transform.position = im.overLapEcastms[0].am.transform.position
                                    + im.overLapEcastms[0].am.transform.TransformVector(im.overLapEcastms[0].offset);
                                ac.model.transform.LookAt(im.overLapEcastms[0].am.transform, Vector3.up);
                                dm.PlayFrontStab("leverUp", this, im.overLapEcastms[0].am);
                                sm.haveOil = false;
                                sm.doorOpen = true;
                            }

                        }
                        break;

                }
            }


        }
    }

    private T Bind<T>(GameObject go) where T : IActorManagerInterface
    {
        T tempInstance;
        if (go == null)
        {
            return null;
        }
        tempInstance = go.GetComponent<T>();
        if (tempInstance == null)
        {
            tempInstance = go.AddComponent<T>();
        }
        tempInstance.am = this;
        return tempInstance;
    }

    void Update()
    {
        if (gm != null)
        {
            if (currentCount > 0 && gm.count >= currentCount + ac.anim.GetCurrentAnimatorStateInfo(0).length) //播放完die动画之后，开始溶解
            {
                if (ac.anim.GetCurrentAnimatorStateInfo(0).IsName("die"))
                    foreach (var d in dissovle)
                    {
                        d.enabled = true;
                    }
                //GameObject caster = transform.DeepFind("caster").gameObject;
                //if (caster != null)
                //    caster.SetActive(false);
            }
            if (currentCount > 0 && gm.count >= currentCount + 5)
            {
                if (ac.camcon.isAI)
                {
                    Destroy(gameObject);

                }

            }
        }


    }
    public void TryDoDamage(WeaponController targetWc, bool attackValid, bool counterValid)
    {
        if (sm.isCounterBackSuccess)
        {
            if (counterValid)
            {
                //自己没有动作
                //对方动作
                targetWc.wm.am.Stunned();
                sm.canBreak = true;
                blurEffect = true;
            }

        }
        else if (targetWc.wm.am.sm.isAround)
        {
            if (!sm.isImmortal)
            {
                Down();
                HitOrDie(targetWc, false);
            }

        }
        else if (sm.isDown)
        {
            if (attackValid)
                HitOrDie(targetWc, false);
        }
        else if (sm.isCounterBackFailure)
        {
            if (attackValid)
            {
                //endure 霸体(掉血但不会被打动画)
                HitOrDie(targetWc, false);

            }

        }
        else if (sm.isImmortal || sm.isPowerUp)
        {
            //无敌什么都不会发生
            if (sm.isPowerUp) //boss换p时打上去会破防动画
            {
                targetWc.wm.am.Stunned();
                // sm.canBreak = true;
            }
        }
        else if (sm.isDefense)
        {
            //should be blocked
            if (bm.canDefance)
            {
                Blocked();
                sm.canBreak = false;
            }
            else
            {
                HitOrDie(targetWc, true);
            }

        }
        else if (sm.isStunned)
        {
            if (attackValid)
            {
                //endure 霸体(掉血但不会被打动画)
                HitOrDie(targetWc, false);

            }
        }
        else
        {
            if (attackValid)
            {
                HitOrDie(targetWc, true);

            }
        }

    }
    public void HitOrDie(WeaponController targetWc, bool doHitAnim)
    {
        if (sm.HP <= 0)
        {
            //dead

        }
        else
        {

            sm.AddHP(-1 * targetWc.GetATK());

            if (sm.HP > 0)
            {
                if (doHitAnim)
                {
                    Hit();
                }
                //effect
            }
            else
            {
                if (!targetWc.wm.am.ac.camcon.isAI) //对方为player
                    targetWc.wm.am.im.overLapEcastms.Clear();  //enemy死亡时清除caster内保存的元素
                Die();
            }
        }
    }
    public void SetIsCounterBack(bool value)
    {
        sm.isCounterBackEnable = value;
    }
    public void Down()
    {
        ac.IssueTrigger("down");
    }
    public void Stunned()
    {
        ac.IssueTrigger("stunned");
    }
    public void Blocked()
    {
        ac.IssueTrigger("blocked");
    }
    public void Hit()
    {
        ac.IssueTrigger("hit");
    }
    public void Die()
    {
        currentCount = gm.count;
        ac.IssueTrigger("die");
        ac.pi.inputEnabled = false; //不可输入
        bm.defCol.enabled = false;
        if (ac.camcon.lockState)
        {
            ac.camcon.LockUnLock();
        }
        ac.camcon.enabled = false;//相机固定
        if (ac.camcon.isAI)
        {
            col.isTrigger = true;

        }


    }
    public void LockUnlockActorController(bool value)
    {
        ac.SetBool("lock", value);
    }
    public void ChangeDualHands(bool dualOn, string name)
    {
        ca.ChangeDualHands(dualOn, name);
        //if (dualOn)
        //{
        //    if (name == "sword") //大剑
        //        ac.anim.runtimeAnimatorController = twoHandAnimSword;
        //    if (name == "lance")
        //    {
        //        if (twoHandAnimLance != null)
        //            ac.anim.runtimeAnimatorController = twoHandAnimLance;
        //    }
        //    if (name == "Arrow")
        //    {
        //        ac.anim.runtimeAnimatorController = twoHandAnimArrow;
        //    }
        //    if (name == "hand")
        //    {
        //        ac.anim.runtimeAnimatorController = twoHandAnimHand;
        //    }
        //}
        //else
        //{
        //    ac.anim.runtimeAnimatorController = oneHandAnim;
        //}

    }
}
