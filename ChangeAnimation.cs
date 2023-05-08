using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;

public class ChangeAnimation : MonoBehaviour
{
    //RuntimeAnimatorController
    //public AnimatorController anim;

    //public string strPath;
    //public AnimatorController oneHandAnim;
    //public AnimatorController twoHandSword;
    //public AnimatorController twoHandAnimHand;
    //public AnimatorController twoHandAnimLance;
    //public AnimatorController twoHandAnimArrow;
    public RuntimeAnimatorController anim;

    public string strPath;
    public RuntimeAnimatorController oneHandAnim;
    public RuntimeAnimatorController twoHandSword;
    public RuntimeAnimatorController twoHandAnimHand;
    public RuntimeAnimatorController twoHandAnimLance;
    public RuntimeAnimatorController twoHandAnimArrow;
    void Start()
    {
        //am = GetComponentInParent<ActorManager>();
        gameObject.GetComponent<Animator>().runtimeAnimatorController = anim;
    }

    public void ChangeDualHands(bool dualOn, string name)
    {

        //if (dualOn)
        //{
        //    if (name == "sword") //大剑
        //        strPath = "AnimatorController/Delta";
        //    //anim = twoHandSword;
        //    if (name == "lance")
        //    {
        //        if (twoHandAnimLance != null)
        //            anim = twoHandAnimLance;
        //    }
        //    if (name == "Arrow")
        //    {
        //        anim = twoHandAnimArrow;
        //    }
        //    if (name == "hand")
        //    {
        //        anim = twoHandAnimHand;
        //    }
        //}
        //else
        //{
        //    anim = oneHandAnim;
        //}
        //RuntimeAnimatorController animator = (RuntimeAnimatorController)Resources.Load(strPath);//放在Resources下
        //gameObject.GetComponent<Animator>().runtimeAnimatorController = animator;
        if (dualOn)
        {
            if (name == "sword") //大剑
                strPath = "AnimatorController/Actor 2h";
            // ac.anim.runtimeAnimatorController = twoHandAnimSword;
            if (name == "lance")
            {
                //if (twoHandAnimLance != null)
                // ac.anim.runtimeAnimatorController = twoHandAnimLance;
            }
            if (name == "Arrow")
            {
                //ac.anim.runtimeAnimatorController = twoHandAnimArrow;
            }
            if (name == "hand")
            {
                //ac.anim.runtimeAnimatorController = twoHandAnimHand;
                strPath = "AnimatorController/Actor 2h hand";
            }
        }
        else
        {
            //ac.anim.runtimeAnimatorController = oneHandAnim;
            strPath = "AnimatorController/Actor";
        }
        RuntimeAnimatorController animator = (RuntimeAnimatorController)Resources.Load(strPath);//放在Resources下
        gameObject.GetComponent<Animator>().runtimeAnimatorController = animator;

    }

    void Update()
    {

    }
}
