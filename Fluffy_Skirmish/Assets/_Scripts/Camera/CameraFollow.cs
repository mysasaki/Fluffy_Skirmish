using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform Target;

    private float m_smoothSpeed;
    private Vector3 m_offset;

    private void Awake() {
        m_smoothSpeed = 0.125f;
        m_offset = new Vector3(0, 5, -10);
    }

    private void FixedUpdate() {

        if (Target == null)
            return;

        Vector3 desiredPosition = Target.position + m_offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_smoothSpeed);
        transform.position = smoothedPosition;   
    }
}
