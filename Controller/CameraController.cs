using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private IUserInput pi;
    public float horizontalSpeed = 100.0f;
    public float vertivalSpeed = 80.0f;
    public float cameraDampValue = 0.5f;
    public Image lockDot;
    public bool lockState;
    public bool isAI = false;//是否是player
    [SerializeField]
    public LockTarget lockTarget;
    public List<Collider> targets = new List<Collider>();
    private GameObject playerHandle;
    private GameObject cameraHandle;
    private float tempEulerX;
    //锁定

    [HideInInspector]
    private GameObject model;
    [SerializeField]
    private GameObject mainCamera;
    private Vector3 camreaDampVelocity;


    void Start()
    {
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerX = 20.0f;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        pi = ac.pi;
        if (!isAI)
        {
            mainCamera = Camera.main.gameObject;
            //mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            lockDot.enabled = false;
            //隐藏鼠标
            //Cursor.lockState = CursorLockMode.Locked;
        }

        lockState = false;

    }

    void LateUpdate()
    {
        if (lockTarget == null) //freelook
        {
            //if (pi == null) { pi = playerHandle.GetComponent<ActorController>().pi; }
            Vector3 tempModelEuler = model.transform.eulerAngles;
            //左右
            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
            //上下
            tempEulerX -= pi.Jup * vertivalSpeed * Time.fixedDeltaTime;
            tempEulerX = Mathf.Clamp(tempEulerX, -40, 30);
            //延迟移动追迹
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerX, 0, 0);
            model.transform.eulerAngles = tempModelEuler;



        }
        else//lock on
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);
        }

        if (!isAI)
        {
            mainCamera.transform.position = Vector3.SmoothDamp(
            mainCamera.transform.position, transform.position,
            ref camreaDampVelocity, cameraDampValue);
            //camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.2f);
            // mainCamera.transform.eulerAngles = transform.eulerAngles;
            mainCamera.transform.LookAt(cameraHandle.transform);
        }

    }
    void FixedUpdate()
    {
        if (lockTarget == null)
        {
            LockProcessA(null, false, false, isAI);
        }
        if (lockTarget != null)
        {

            if (!isAI)
            {
                //ui在敌人半高，视角锁定脚底
                lockDot.rectTransform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + new Vector3(0, lockTarget.halfHeight + 0.3f, 0));
            }
            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10.0f)//距离过远解除锁定
            {
                LockProcessA(null, false, false, isAI);
            }
            else if (lockTarget.am != null && lockTarget.am.sm.isDie)//敌人死亡
            {
                LockProcessA(null, false, false, isAI);
            }


        }
    }
    public void LockUnLock()
    {
        //if (lockTarget == null)
        //{
        //还没锁定时锁定 try to lock
        Vector3 modelOrigin1 = model.transform.position; //脚下
        Vector3 modelOrigin2 = modelOrigin1 + new Vector3(0, 1, 0); //脖子
        Vector3 boxCenter = modelOrigin2 + model.transform.forward * 5.0f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f), model.transform.rotation, LayerMask.GetMask(isAI ? "Player" : "Enemy"));
        if (cols.Length == 0)
        {
            LockProcessA(null, false, false, isAI);
        }
        else
        {
            foreach (var col in cols)
            {
                if (lockTarget != null && lockTarget.obj == col.gameObject)
                {
                    LockProcessA(null, false, false, isAI);
                    break;
                }
                LockProcessA(new LockTarget(col.gameObject, col.bounds.extents.y), true, true, isAI);
                break;
            }
        }

    }
    public class LockTarget
    {
        public GameObject obj;
        public float halfHeight;
        public ActorManager am;

        public LockTarget(GameObject _obj, float _halfHeight)
        {
            obj = _obj;
            halfHeight = _halfHeight;
            am = _obj.GetComponent<ActorManager>();
        }
    }
    public void LockProcessA(LockTarget _lockTarget, bool _lockDotEnable, bool _lockState, bool _isAI)
    {
        lockTarget = _lockTarget;
        if (!isAI)
        {
            lockDot.enabled = _lockDotEnable;
        }
        lockState = _lockState;
    }

}
