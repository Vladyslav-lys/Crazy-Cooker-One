using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
    public GameObject[] skinParts;

    public void ActiveSkin(bool isActive)
    {
        for (int i = 0; i < skinParts.Length; i++)
        {
            skinParts[i].SetActive(isActive);
        }
    }
}
