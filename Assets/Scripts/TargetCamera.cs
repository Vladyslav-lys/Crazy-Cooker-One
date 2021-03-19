using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    public Transform targetTransform;
    public Transform finishTransform;
    public Vector3 offset;
    private Camera _cam;
    private GameManager _gm;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _gm = GameManager.instance;
    }

    private void LateUpdate()
    {
        if(_gm.isFinished)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, finishTransform.position, 50f * Time.deltaTime);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, finishTransform.rotation, 300f * Time.deltaTime);
        }
    }
}
