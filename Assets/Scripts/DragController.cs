using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragController : BaseManager<DragController>, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float dragX;
    public float dragZ;
    private GameManager _gm;
    private Camera _camera;
    private PlayerMovement _playerMovement;
    private Vector3 _offset;
    private float _offsetFactor;

    protected override void Awake()
    {
        if (!instance)
            instance = this;
        base.Awake();
    }

    protected override void InitializeManager()
    {
        _offsetFactor = 2.5f;
        dragX = 15f;
        dragZ = 0f;
    }

    private void Start()
    {
        _gm = GameManager.instance;
        _camera = Camera.main;
        _playerMovement = PlayerMovement.instance;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            _gm.StartGame();

        if (_gm.isFinished || _gm.isLosing || !_gm.isStarted || _playerMovement.isRotating)
            return;

        if (Input.GetKey(KeyCode.A) && !_playerMovement.isLeftWallCollided
                                    && (!_playerMovement.isCutting || _playerMovement.isLeftCut))
        {
            _playerMovement.transform.localPosition +=
                new Vector3(-dragX * Time.deltaTime, 0f, -dragZ * Time.deltaTime);
            if (_playerMovement.isCutting || _playerMovement.isRightWallCollided)
                PushLeft();
            if(_playerMovement.isCutting)
                _playerMovement.OffCut();
            if (_playerMovement.isRightWallCollided)
                _playerMovement.isRightWallCollided = false;
        }

        if (Input.GetKey(KeyCode.D) && !_playerMovement.isRightWallCollided
                                    && (!_playerMovement.isCutting || _playerMovement.isRightCut))
        {
            _playerMovement.transform.localPosition +=
                new Vector3(dragX * Time.deltaTime, 0f, dragZ * Time.deltaTime);
            if (_playerMovement.isCutting || _playerMovement.isLeftWallCollided)
                PushRight();
            if(_playerMovement.isCutting)
                _playerMovement.OffCut();
            if (_playerMovement.isLeftWallCollided)
                _playerMovement.isLeftWallCollided = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _gm.StartGame();
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
                     
        _offset = _camera.ScreenToWorldPoint(mousePos);
        _offset.x = _playerMovement.transform.localPosition.x / _offsetFactor - _offset.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_gm.isFinished || _gm.isLosing || !_gm.isStarted || _playerMovement.isRotating)
            return;

        if (eventData.delta.x < -3f && !_playerMovement.isLeftWallCollided
                                    && (!_playerMovement.isCutting || _playerMovement.isLeftCut))
        {
            _playerMovement.transform.localPosition +=
                new Vector3(-dragX * Time.deltaTime, 0f, -dragZ * Time.deltaTime);
            if (_playerMovement.isCutting || _playerMovement.isRightWallCollided)
                PushLeft();
            if(_playerMovement.isCutting)
                _playerMovement.OffCut();
            if (_playerMovement.isRightWallCollided)
                _playerMovement.isRightWallCollided = false;
        }

        if (eventData.delta.x > 3f && !_playerMovement.isRightWallCollided
                                   && (!_playerMovement.isCutting || _playerMovement.isRightCut))
        {
            _playerMovement.transform.localPosition +=
                new Vector3(dragX * Time.deltaTime, 0f, dragZ * Time.deltaTime);
            if (_playerMovement.isCutting || _playerMovement.isLeftWallCollided)
                PushRight();
            if(_playerMovement.isCutting)
                _playerMovement.OffCut();
            if (_playerMovement.isLeftWallCollided)
                _playerMovement.isLeftWallCollided = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {}

    private void PushLeft()
    {
        _playerMovement.transform.localPosition +=
            new Vector3(-dragX * Time.deltaTime, 0f, -dragZ * Time.deltaTime);
    }

    private void PushRight()
    {
        _playerMovement.transform.localPosition +=
            new Vector3(dragX * Time.deltaTime, 0f, dragZ * Time.deltaTime);
    }
}
