using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    public GameObject invisibleWall;
    public float turnRotY;
    public bool isNegativeDirX;
    public bool isNegativeDirZ;
    public bool isDirX;
    public bool isDirZ;

    public void TrueInvisibleWallInTime(float delay) => Invoke(nameof(TrueInvisibleWall),delay);
    
    public void TrueInvisibleWall() => invisibleWall.SetActive(true);
}