using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Title : MonoBehaviour
{
    public static FadeService fadeService;
    public GameObject obj;
    public Transform bornPoint;
    private Vector3 originPos;
    public StateManager sm;
    public Camera main;
    public Transform cameraPos;

    private bool deadPhase = false;
    public Image[] images;
    public Camera mainCam;

    void Awake()
    {
        fadeService = GetComponent<FadeService>();
        obj = GameObject.FindGameObjectWithTag("Player");


        if (obj != null)
        {
            main = obj.transform.DeepFind("CameraHandle").GetComponentInChildren<Camera>();
            sm = obj.GetComponent<StateManager>();
            originPos = obj.transform.position;
            obj.transform.position = bornPoint.position;
            obj.transform.rotation = bornPoint.rotation;

            images = obj.GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                image.enabled = false;
            }

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (!deadPhase)
            {
                deadPhase = true;
                ChangeScene();
            }

        }
        if (mainCam != null)
        {
            mainCam.transform.position = cameraPos.position;
            mainCam.transform.rotation = cameraPos.rotation;
        }

    }
    public void ChangeScene()
    {

        //SceneManager.LoadSceneAsync("SampleScene");
        StartCoroutine(DoTransitionToCheckPoint());

    }
    private IEnumerator DoTransitionToCheckPoint()
    {
        fadeService.FadeOut();
        yield return new WaitUntil(() => !fadeService.IsFading());
        SceneManager.LoadScene("SampleScene");
        if (obj != null)
        {
            sm.HP = sm.HPMax;
            originPos.y = 0;
            obj.transform.position = originPos;
        }
        fadeService.FadeIn();
        deadPhase = false;
    }
}
