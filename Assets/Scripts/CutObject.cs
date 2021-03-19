using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutObject : MonoBehaviour
{
    public GameObject plane;
    public GameObject bombEffect;
    public Transform objectContainer;
    public bool isRight;
    public bool drawPlane;
    private float _separation;
    private Plane _slicePlane = new Plane();
    private GameManager _gm;
    private MeshCutter _meshCutter;
    private TempMesh _biggerMesh, _smallerMesh;
    private PlayerMovement _playerMovement;
    private SoundManager _soundManager;
    
    private void Start()
    {
        _gm = GameManager.instance;
        _separation = 0.1f;
        _meshCutter = new MeshCutter(256);
        _playerMovement = PlayerMovement.instance;
        _soundManager = SoundManager.instance;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(_gm.isLosing)
            return;
        
        //CUTTABLE
        if (other.gameObject.layer == 9)
        {
            CutCollider(other);
            if (other.gameObject.GetComponent<CuttableObj>())
                other.gameObject.GetComponent<CuttableObj>().juiceEffect.Play();
            _soundManager.CreateCutSound();
            _soundManager.CreateJuiceSound();
            _gm.SetScore();
        }
        
        //BOMB
        if (other.gameObject.layer == 14)
        {
            _playerMovement.OffCut();
            _playerMovement.Die("BombDie");
            Vector3 pos = new Vector3(other.transform.parent.position.x - 0.3f, other.transform.parent.position.y,
                other.transform.parent.position.z);
            _soundManager.CreateCutSound();
            _soundManager.CreateBombSound();
            _gm.SetLose();
            Destroy(Instantiate(bombEffect,pos,Quaternion.identity), 3f);
            Destroy(other.transform.parent.gameObject);
        }
        
        //COIN
        if (other.gameObject.layer == 19)
        {
            CutCollider(other);
            _soundManager.CreateCutSound();
            _soundManager.CreateCoinCutSound();
            _gm.SetPlayCoins();
        }
    }

    private void CutCollider(Collider other)
    {
        Quaternion rot = transform.rotation;
        rot *= isRight ? Quaternion.Euler(0f, 0f ,0f) : Quaternion.Euler(0f, 180f, 0f);
        SliceObjects(transform.position, rot.ToEulerAngles(), other.gameObject, false);
        PushSliceObj(new Vector3(0f,100f,0f), other.gameObject.GetComponent<Rigidbody>());
    }
    
    public void PushSliceObj(Vector3 forces, Rigidbody slicedObjRb) => slicedObjRb.AddForce(forces);

    private void SliceObjects(Vector3 point, Vector3 normal, GameObject sliceObj, bool isPlayer)
    {
        List<Transform> positive = new List<Transform>(), negative = new List<Transform>();

        bool slicedAny = false;
        var transformedNormal = ((Vector3) (sliceObj.transform.localToWorldMatrix.transpose * normal)).normalized;

        _slicePlane.SetNormalAndPosition(
            transformedNormal,
            sliceObj.transform.InverseTransformPoint(point));
        
        slicedAny = SliceObject(ref _slicePlane, sliceObj, positive, negative, isPlayer) || slicedAny;
        
        if (slicedAny)
            SeparateMeshes(positive, negative, normal);
    }

    private bool SliceObject(ref Plane slicePlane, GameObject obj, List<Transform> positiveObjects, List<Transform> negativeObjects,  bool isPlayer)
    {
        var mesh = obj.GetComponent<MeshFilter>().mesh;
        
        if (!_meshCutter.SliceMesh(mesh, ref slicePlane))
        {
            if (slicePlane.GetDistanceToPoint(_meshCutter.GetFirstVertex()) >= 0)
                positiveObjects.Add(obj.transform);
            else
                negativeObjects.Add(obj.transform);

            return false;
        }
        
        bool posBigger = _meshCutter.PositiveMesh.surfacearea > _meshCutter.NegativeMesh.surfacearea;
        if (posBigger)
        {
            _biggerMesh = _meshCutter.PositiveMesh;
            _smallerMesh = _meshCutter.NegativeMesh;
        }
        else
        {
            _biggerMesh = _meshCutter.NegativeMesh;
            _smallerMesh = _meshCutter.PositiveMesh;
        }
        
        GameObject newObject = Instantiate(obj, objectContainer);
        newObject.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
        var newObjMesh = newObject.GetComponent<MeshFilter>().mesh;
        
        obj.GetComponent<Rigidbody>().useGravity = true;
        obj.GetComponent<Rigidbody>().isKinematic = false;
        newObject.layer = 18;
        obj.layer = 18;
        // Destroy(obj.GetComponent<BoxCollider>());
        //
        // obj.AddComponent<SphereCollider>();
        obj.GetComponent<Rigidbody>().AddForce(isPlayer ? 10f : 0f, 10f, 0f);
        
        ReplaceMesh(mesh, _biggerMesh);
        ReplaceMesh(newObjMesh, _smallerMesh);

        (posBigger ? positiveObjects : negativeObjects).Add(obj.transform);
        (posBigger ? negativeObjects : positiveObjects).Add(newObject.transform);

        return true;
    }
    
    private void ReplaceMesh(Mesh mesh, TempMesh tempMesh, MeshCollider collider = null)
    {
        mesh.Clear();
        mesh.SetVertices(tempMesh.vertices);
        mesh.SetTriangles(tempMesh.triangles, 0);
        mesh.SetNormals(tempMesh.normals);
        mesh.SetUVs(0, tempMesh.uvs);
        
        mesh.RecalculateTangents();

        if (collider != null && collider.enabled)
        {
            collider.sharedMesh = mesh;
            collider.convex = true;
        }
    }

    private void SeparateMeshes(List<Transform> positives, List<Transform> negatives, Vector3 worldPlaneNormal)
    {
        int i;
        var separationVector = worldPlaneNormal * _separation;

        for(i = 0; i <positives.Count; ++i)
            positives[i].transform.position += separationVector;

        for (i = 0; i < negatives.Count; ++i)
            negatives[i].transform.position -= separationVector;
    }
}
