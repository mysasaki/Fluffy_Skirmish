using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform Target;

    private float m_rotateSpeed = 5;
    private Vector3 m_offset = new Vector3(0, -2.5f, 7f);

    private void FixedUpdate() {

        if (Target == null)
            return;

        float horizontal = Input.GetAxis("Mouse X") * m_rotateSpeed;
        Target.Rotate(0, horizontal, 0);

        float desiredAngle = Target.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = Target.position - (rotation * m_offset);

        transform.LookAt(Target);
    }
}
