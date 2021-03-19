using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : BaseManager<SpawnManager>
{
    public GameObject tablePref;
    public GameObject roadPref;
    private float _zRoadOffset;
    private float _yRoadOffset;
    private float _zTableOffset;
    private float _yTableOffset;
    
    protected override void Awake()
    {
        if (!instance)
            instance = this;
        base.Awake();
    }

    protected override void InitializeManager()
    {
        _zRoadOffset = roadPref.transform.localPosition.z;
        _yRoadOffset = roadPref.transform.localPosition.y;
        _zTableOffset = tablePref.transform.localPosition.z;
        _yTableOffset = tablePref.transform.localPosition.y;
    }

    private void Start()
    {
        for (int i = 0; i < 6; i++)
            CreateRoad();
    }

    public void SpawnInTime(float spawnTime) => InvokeRepeating(nameof(Spawn), 1f, spawnTime);

    private void Spawn() => CreateRoad();

    private void CreateRoad()
    {
        Instantiate(roadPref, new Vector3(0f, _yRoadOffset, _zRoadOffset), Quaternion.identity);
        _zRoadOffset += 50f;
    }

    private void CreateTable()
    {
        Instantiate(roadPref, new Vector3(0f, _yTableOffset, _zTableOffset), Quaternion.identity);
        _zTableOffset += 1.4f;
    }
}
