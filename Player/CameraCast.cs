using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCast : MonoBehaviour
{
    void detect(Vector3 dir, Vector3 inverse_dir)
    {
        RaycastHit hitInfo;
        Vector3 _dir = Camera.main.transform.TransformDirection(dir);
        if (Physics.Raycast(Camera.main.transform.position, _dir, out hitInfo, 0.5f))
        {
            float dis = hitInfo.distance;
            //Vector3 correction = Vector3.Normalize(Camera.main.transform.TransformDirection(inverse_dir)) * dis;
            Vector3 correction = Vector3.Normalize(Camera.main.transform.TransformDirection(Vector3.back)) * dis * Time.deltaTime;
            Camera.main.transform.position += correction;
        }
    }


    void Update()
    {
        detect(Vector3.forward, Vector3.back);
        detect(Vector3.back, Vector3.forward);
        detect(Vector3.left, Vector3.right);
        detect(Vector3.right, Vector3.left);
    }
}
