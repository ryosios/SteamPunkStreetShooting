using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BgBlock : MonoBehaviour
{
    /// <summary>thisTransform</summary>
    [SerializeField] private Transform _thisTrans;

    /// <summary>背景ブロックを物理移動させる3D Rigidbody。</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>スクロール速度</summary>
    private float _speed;

    /// <summary>親Transformから見た現在のX座標。</summary>
    public float LocalPositionX => _thisTrans != null ? _thisTrans.localPosition.x : transform.localPosition.x;

    private void Awake()
    {
        if (_thisTrans == null)
        {
            _thisTrans = transform;
        }

        if (_thisRigid == null)
        {
            _thisRigid = GetComponent<Rigidbody>();
        }

        if (_thisRigid == null)
        {
            _thisRigid = gameObject.AddComponent<Rigidbody>();
        }

        SetupRigidbody();

        Rigidbody2D legacyRigid2D = GetComponent<Rigidbody2D>();
        if (legacyRigid2D != null)
        {
            legacyRigid2D.simulated = false;
        }
    }

    private void FixedUpdate()
    {
        if (_thisRigid != null)
        {
            _thisRigid.linearVelocity = Vector3.left * _speed;
        }
    }

    /// <summary>
    /// BGBlockを移動
    /// </summary>
    public BgBlock SetMove(float speed)
    {
        _speed = speed;

        if (_thisRigid != null)
        {
            _thisRigid.linearVelocity = Vector3.left * _speed;
        }

        return this;
    }

    /// <summary>
    /// 背景ブロック用のRigidbody設定。
    /// 子オブジェクトに3D Colliderを置くと、このRigidbodyの移動としてCollision判定される。
    /// </summary>
    private void SetupRigidbody()
    {
        _thisRigid.useGravity = false;
        _thisRigid.isKinematic = false;
        _thisRigid.interpolation = RigidbodyInterpolation.Interpolate;
        _thisRigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _thisRigid.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;
    }
}
