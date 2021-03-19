using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public ParticleSystem[] finishParticles;
    private PlayerMovement _playerMovement;

    private void Start()
    {
        _playerMovement = PlayerMovement.instance;
    }

    public void PlayParticles(Transform playerTransform)
    {
        float targetX = _playerMovement.isDirX ? 0f : 2f;
        float targetZ = _playerMovement.isDirZ ? 0f : 2f;
        
        finishParticles[0].transform.position =
            particleVector(playerTransform.position,-targetX, -targetZ);
        finishParticles[1].transform.position =
            particleVector(playerTransform.position,targetX, targetZ);
        for (int i = 0; i < finishParticles.Length; i++)
        {
            finishParticles[i].Play();
        }
    }

    private Vector3 particleVector(Vector3 targetPos, float offsetX, float offsetZ)
    {
        return new Vector3(targetPos.x+offsetX, targetPos.y+0.3f, targetPos.z+offsetZ);
    }
}