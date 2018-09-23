using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform Target;

    //TODO: - setar tudo pra private depois. ta public pra ajustar os valores
    public float m_rotateSpeed = 8;
    public Vector3 m_offset = new Vector3(0, -2.5f, 7f);
    public Vector3 m_offset2 = new Vector3(0, 0, 0);

    private void FixedUpdate() {

        if (Target == null)
            return;

        float horizontal = Input.GetAxis("Mouse X") * m_rotateSpeed;
        Target.Rotate(0, horizontal, 0);

        float desiredAngle = Target.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = Target.position + m_offset2 - (rotation * m_offset);

        transform.LookAt(Target);
    }
}
