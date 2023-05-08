using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitationController : MonoBehaviour
{
    public GameObject cannonName;
    private Vector3 cannonDampVelocity;
    public float cannonDampValue = 0.5f;
    public LevitationCannon cannon; //boss面向的左上0(，左下1，右上2)，右下3
    void Start()
    {
        cannon = cannonName.GetComponentInChildren<LevitationCannon>();
    }
    void LateUpdate()
    {
        if (cannonName == null) return;
        cannonName.transform.position = Vector3.SmoothDamp(
           cannonName.transform.position, transform.position,
           ref cannonDampVelocity, cannonDampValue);
        //camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.2f);
        // mainCamera.transform.eulerAngles = transform.eulerAngles;
        //cannon.transform.LookAt(cameraHandle.transform);
        cannonName.transform.rotation = Quaternion.Lerp(
           cannonName.transform.rotation, transform.rotation,
          0.1f);
    }
}
