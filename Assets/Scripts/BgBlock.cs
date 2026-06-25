using UnityEngine;

public class BgBlock : MonoBehaviour
{
    /// <summary>thisTransform</summary>
    [SerializeField] private Transform _thisTrans;

    /// <summary>古いPrefab参照。移動はTransformで制御する。</summary>
    [SerializeField] private Rigidbody2D _thisRigid;

    /// <summary>スクロール速度</summary>
    private float _speed;

    /// <summary>削除するX座標</summary>
    private float _destroyPosX = -30f;

    private void Awake()
    {
        if (_thisTrans == null)
        {
            _thisTrans = transform;
        }

        if (_thisRigid != null)
        {
            _thisRigid.simulated = false;
        }
    }

    private void Update()
    {
        _thisTrans.localPosition += Vector3.left * (_speed * Time.deltaTime);

        if (_thisTrans.localPosition.x <= _destroyPosX)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// BGBlockを移動
    /// </summary>
    public BgBlock SetMove(float speed)
    {
        _speed = speed;
        return this;
    }

    /// <summary>
    /// 削除位置を設定
    /// </summary>
    public BgBlock SetDestroyPosition(float posX)
    {
        _destroyPosX = posX;
        return this;
    }
}
