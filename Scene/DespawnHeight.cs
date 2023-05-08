using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DespawnHeight : MonoBehaviour
{
    private Vector3 swapHeight;
    public GameObject obj;
    public GameObject rebornPoint;

    public Transform cameraPos;
    public GameObject main;
    [Header("=== UI ===")]
    public Image[] images;

    private Vector3 originPos;
    private Quaternion originRotate;
    private bool deadPhase = false;
    StateManager sm;

    // Start is called before the first frame update
    void Start()
    {
        swapHeight = transform.position;
        obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            images = obj.GetComponentsInChildren<Image>();
            main = obj.transform.DeepFind("CameraHandle").gameObject;
        }

        rebornPoint = GameObject.FindGameObjectWithTag("RebornPoint");
        originPos = rebornPoint.transform.position;
        originRotate = rebornPoint.transform.rotation;
        sm = obj.GetComponent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {

        AnimatorStateInfo animatorInfo;
        animatorInfo = sm.am.ac.anim.GetCurrentAnimatorStateInfo(0);
        if (obj.transform.position.y <= swapHeight.y)
        {
            if (!deadPhase)
            {
                deadPhase = true;
                ChangeScene();
            }
        }
        if (sm.am.sm.isDie)
        {

            if (!deadPhase)
            {
                deadPhase = true;
                ChangeScene();
            }
            //if (Input.GetKeyDown(KeyCode.X))


        }
        if (sm.isWin)
        {
            if (!deadPhase)
            {
                deadPhase = true;
                sm.startFlag = false;
                //sm.doorOpen = false;
                // sm.am.im.overLapEcastms.Clear();  //enemy死亡时清除caster内保存的元素
                StartCoroutine(DoTransitionToClear());
            }
        }


        if ((animatorInfo.normalizedTime >= 0.99f) && animatorInfo.IsName("sit"))
        {
            foreach (var image in images)
            {
                if (image.enabled == false)
                {
                    image.enabled = true;
                }

            }
            if (cameraPos != null)
            {
                main.transform.position = cameraPos.position;
                main.transform.rotation = cameraPos.rotation;
            }
            sm.am.ac.SetBool("reborn", false);
            sm.am.gm.count = 0;

        }




    }
    public void ChangeScene()
    {
        // AnimatorStateInfo animatorInfo;
        // animatorInfo = sm.am.ac.anim.GetCurrentAnimatorStateInfo(0);
        sm.startFlag = false;
        //sm.doorOpen = false;


        StartCoroutine(DoTransitionToReborn());
        //SceneManager.LoadScene("JoystickTest");


    }
    private IEnumerator DoTransitionToReborn()
    {
        GameManager.fadeService.FadeOut();
        if (!sm.isDie)
        {
            sm.am.ac.IssueTrigger("die");
        }
        foreach (var image in images)
        {
            //if (image.enabled == true)
            image.enabled = false;
        }
        yield return new WaitUntil(() => !GameManager.fadeService.IsFading());

        originPos.y = 0;
        obj.transform.position = originPos;
        obj.transform.rotation = originRotate;
        SceneManager.LoadScene("JoystickTest");
        GameManager.fadeService.FadeIn();
        deadPhase = false;

    }
    private IEnumerator DoTransitionToClear()
    {
        GameManager.fadeService.FadeOut();
        //关掉playerUI
        foreach (var image in images)
        {
            //if (image.enabled == true)
            image.enabled = false;
        }
        yield return new WaitUntil(() => !GameManager.fadeService.IsFading());

        originPos.y = 0;
        obj.transform.position = originPos;
        obj.transform.rotation = originRotate;
        SceneManager.LoadScene("ClearScene"); //换成通关界面
        GameManager.fadeService.FadeIn();
        deadPhase = false;
        sm.isWin = false;
    }
}
