using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TesterJoystick : MonoBehaviour
{
    public GameObject obj;
    public Transform bornPoint;
    public StateManager sm;
    public GameObject main;
    public Transform cameraPos;

    private bool deadPhase = false;
    private Vector3 originPos;
    void Start()
    {
        obj = GameObject.FindGameObjectWithTag("Player");
        main = obj.transform.DeepFind("CameraHandle").gameObject;
        sm = obj.GetComponent<StateManager>();
        originPos = obj.transform.position;
        obj.transform.position = bornPoint.position;
        obj.transform.rotation = bornPoint.transform.rotation;
    }
    void Update()
    {
        AnimatorStateInfo animatorInfo;
        animatorInfo = sm.am.ac.anim.GetCurrentAnimatorStateInfo(0);
        if ((animatorInfo.normalizedTime >= 1f) && animatorInfo.IsName("die"))
        {
            //sm.am.ac.anim.ResetTrigger("die");
            sm.am.ac.SetBool("reborn", true);
        }
        if (Input.anyKeyDown)
        {
            if (!deadPhase)
            {
                deadPhase = true;
                ChangeScene();
            }

        }
        if (cameraPos != null)
        {
            main.transform.position = cameraPos.position;
            main.transform.rotation = cameraPos.rotation;
        }

    }
    public void ChangeScene()
    {

        //SceneManager.LoadSceneAsync("SampleScene");
        StartCoroutine(DoTransitionToCheckPoint());

    }
    private IEnumerator DoTransitionToCheckPoint()
    {
        GameManager.fadeService.FadeOut();
        yield return new WaitUntil(() => !GameManager.fadeService.IsFading());
        sm.HP = sm.HPMax;
        originPos.y = 0;
        obj.transform.position = originPos;
        SceneManager.LoadScene("SampleScene");
        GameManager.fadeService.FadeIn();
        deadPhase = false;
    }
}
