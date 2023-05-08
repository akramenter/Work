using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnUpdate : StateMachineBehaviour
{
    public string[] onUpdateMessages;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var msg in onUpdateMessages)
        {
            animator.SendMessageUpwards(msg);
        }
    }


}
