using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : BaseManager<PlayerMovement>
{
    public float forwardSpeed;
    public float maxSpeed;
    public Animator playerAnimator;
    public bool isCutting;
    public bool isLeftCut;
    public bool isRightCut;
    public bool canCut;
    public bool isNegativeDirX;
    public bool isNegativeDirZ;
    public bool isDirX;
    public bool isDirZ;
    public bool isLeftWallCollided;
    public bool isRightWallCollided;
    public bool isRotating;
    public float forceX;
    public float forceZ;
    [HideInInspector]
    public Vector3 playerV;
    public Transform playerTransform;
    private Rigidbody _rb;
    private GameManager _gm;
    private Transform _targetTransform;
    private float _yRot;
    private float _yHolderRot;
    private SoundManager _soundManager;
    private DragController _dragController;
    private Turn _turn;
    
    protected override void Awake()
    {
        if (!instance)
            instance = this;
        base.Awake();
    }

    protected override void InitializeManager()
    {
        canCut = true;
        playerTransform = playerAnimator.transform;
        isDirZ = true;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _gm = GameManager.instance;
        _soundManager = SoundManager.instance;
        _dragController = DragController.instance;
        SetPlayerDir();
    }

    private void OnTriggerEnter(Collider other)
    {
        //RIGHT CUT
        if (other.gameObject.layer == 12 && canCut && _gm.isStarted && !_gm.isLosing)
        {
            Cut(-80f, other.transform.GetChild(0));
            isRightCut = true;
        }
        
        //LEFT CUT
        if (other.gameObject.layer == 17 && canCut && _gm.isStarted && !_gm.isLosing)
        {
            Cut(80f, other.transform.GetChild(0));
            isLeftCut = true;
        }
        
        //TABLE DIE
        if (other.gameObject.layer == 11 && !isCutting && !_gm.isLosing)
        {
            Die("TableDie");
            transform.position = 
                new Vector3(transform.position.x, transform.position.y, transform.GetChild(0).position.z);
            _soundManager.CreateLoseSound();
            _gm.SetLose();
        }
        
        //FINISH
        if (other.gameObject.layer == 15 && !_gm.isFinished)
        {
            GameManager.instance.SetFinish();
            other.GetComponent<Finish>().PlayParticles(transform);
        }
        
        //EXIT TABLE
        if (other.gameObject.layer == 16)
        {
            OffCut();
        }
        
        //THORN PIT
        if (other.gameObject.layer == 20)
        {
            if(_gm.isLosing)
                return;
            
            Die("ThornDie");
            _soundManager.CreateLoseSound();
            _gm.SetLose();
        }
        
        //TURN ROAD
        if (other.gameObject.layer == 21)
        {
            isRotating = true;
            _turn = other.GetComponent<Turn>();
            _yHolderRot = transform.localRotation.y + _turn.turnRotY;
            isNegativeDirX = _turn.isNegativeDirX;
            isNegativeDirZ = _turn.isNegativeDirZ;
            isDirX = _turn.isDirX;
            isDirZ = _turn.isDirZ;
            _turn.TrueInvisibleWallInTime(0.3f);
            Invoke(nameof(FalseIsLeftWallCollided), 0.1f);
            Invoke(nameof(FalseIsRightWallCollided), 0.1f);
            Invoke(nameof(ChangeDir), 0.1f);
            Invoke(nameof(FalseIsRotating), 0.5f);
        }
        
        //LEFT WALL
        if (other.gameObject.layer == 22)
        {
            isLeftWallCollided = true;
            _targetTransform = transform.GetChild(0);
        }
        
        //RIGHT WALL
        if (other.gameObject.layer == 23)
        {
            isRightWallCollided = true;
            _targetTransform = transform.GetChild(0);
        }
    }

    private void Cut(float yRot, Transform targetTransform)
    {
        playerAnimator.SetBool("Cutting", true);
        _yRot = yRot;
        this._targetTransform = targetTransform;
        isCutting = true;
        canCut = false;
    }

    public void Die(string dieAnimName)
    {
        StopPlayer();
        playerAnimator.SetBool(dieAnimName, true);
    }

    public void OffCut()
    {
        isCutting = false;
        playerAnimator.SetBool("Cutting", false);
        isLeftCut = isRightCut = false;
        Invoke(nameof(NullifyYRot),0.2f);
        TrueCanCut(0.2f);
    }

    private void ChangeDir()
    {
        StopPlayer();
        SetPlayerDir();
    }

    private void SetPlayerDir()
    {
        forceX = isDirX
            ? (isNegativeDirX ? -forwardSpeed * Time.fixedDeltaTime : forwardSpeed * Time.fixedDeltaTime)
            : 0f;
        forceZ = isDirZ
            ? (isNegativeDirZ ? -forwardSpeed * Time.fixedDeltaTime : forwardSpeed * Time.fixedDeltaTime)
            : 0f;
        playerV = new Vector3(forceX, 0f, forceZ);

        _dragController.dragX = isDirZ ? (isNegativeDirZ ? -15f : 15f) : 0f;
        _dragController.dragZ = isDirX ? (isNegativeDirX ? 15f : -15f) : 0f;
    }

    private void FixedUpdate()
    {
        if (_rb.velocity.magnitude < maxSpeed && _gm.isStarted && !_gm.isLosing && !_gm.isFinished)
        {
            _rb.AddForce(playerV, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        if(_gm.isLosing)
            return;
        
        if (isCutting || isLeftWallCollided || isRightWallCollided)
        {
            float targetX = isDirZ ? _targetTransform.position.x : transform.position.x;
            float targetZ = isDirX ? _targetTransform.position.z : transform.position.z;
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(targetX, transform.position.y, targetZ),
                10f * Time.deltaTime);
        }
        
        RotateByTransformAndYRot(transform, _yHolderRot, 150f);
        RotateByTransformAndYRot(playerTransform, _yRot, 500f);
        
        playerTransform.localPosition = Vector3.zero;
    }

    private void RotateByTransformAndYRot(Transform curTransform, float yRot, float speed)
    {
        curTransform.localRotation = Quaternion.RotateTowards(curTransform.localRotation,
            Quaternion.Euler(new Vector3(curTransform.localRotation.x, yRot, curTransform.localRotation.z)),
            speed * Time.deltaTime);
    }

    public void StopPlayer() => _rb.velocity = Vector3.zero;

    public void TrueCanCut(float time) => Invoke(nameof(AllowCut), time);

    private void AllowCut() => canCut = true;

    private void NullifyYRot() => _yRot = 0f;

    private void FalseIsLeftWallCollided() => isLeftWallCollided = false;
    
    private void FalseIsRightWallCollided() => isRightWallCollided = false;

    private void FalseIsRotating() => isRotating = false;
}
