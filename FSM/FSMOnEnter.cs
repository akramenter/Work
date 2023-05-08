using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMOnEnter : StateMachineBehaviour
{
    public string[] onEnterMessages;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var message in onEnterMessages)
        {
            //animator.gameObject.SendMessage(message);
            animator.gameObject.SendMessageUpwards(message);
        }
    }


}
