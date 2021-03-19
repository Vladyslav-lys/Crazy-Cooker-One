using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFog : MonoBehaviour
{
    public Transform targetTransform;

    private void LateUpdate()
    {
        Vector3 followPos = new Vector3(transform.position.x, transform.position.y, targetTransform.position.z);
        transform.position = followPos;
    }
}
