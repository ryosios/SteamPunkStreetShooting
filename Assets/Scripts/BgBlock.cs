using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BgBlock : MonoBehaviour
{
    

    /// <summary>thisTransform</summary>
    [SerializeField] private Transform _thisTrans;

    /// <summary>thisTransform</summary>
    [SerializeField] private Rigidbody2D _thisRigid;

    /// <summary>初期位置</summary>
    private float _initPosX;

    /// <summary>移動量</summary>
    private float _distance;

    public Subject<Unit> InstanceTimingSubject = new Subject<Unit>();

    /// <summary></summary>
    private bool _isOneCount;

    private void Awake()
    {
        _initPosX = _thisTrans.transform.position.x;

    }

    private void Update()
    {
        _distance = _initPosX - _thisTrans.localPosition.x;
        if (_distance >= 30f)
        {
            if (!_isOneCount)
            {
                _isOneCount = true;
                InstanceTimingSubject.OnNext(Unit.Default);
            }
            
        }
        if (_distance >= 60f)
        {
            Destroy(this.gameObject);
        }

    }

    /// <summary>
    /// BGBlockを移動
    /// </summary>
    public void SetMove(float speed)
    {
        _thisRigid.linearVelocityX = speed;

    }

   

}
