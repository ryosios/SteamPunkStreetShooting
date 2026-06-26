using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;

public class BossManager : MonoBehaviour
{

    private enum BossActionState
    {
        Default,
        In,//入場
        Wait,//待機
        Move,//移動
        Out,//退場
    }

    /// <summaryDirectionkind</summary>
    private BossActionState _bossActionState = BossActionState.Default;
 
    /// <summary>リジッドボディ</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>Transform</summary>
    [SerializeField] private Transform _thisTrans;

    /// <summary>AttackオブジェクトのTransform</summary>
    [SerializeField] private Transform _attackTrans;

    /// <summary>動ける範囲:左</summary>
    private float _movePosLeftLimit = 1f;

    /// <summary>動ける範囲:右</summary>
    private float _movePosRightLimit = 1.8f;

    /// <summary>動ける範囲:上</summary>
    private float _movePosUpLimit = 2f;

    /// <summary>動ける範囲:下</summary>
    private float _movePosDownLimit = -2f;

    /// <summary>ボスの移動後の位置</summary>
    private Vector3 _currentBossPos;

    private Tween _moveTween;



   

    private void Awake()
    {
        _currentBossPos = _thisTrans.localPosition;  

    }

    private async void Start()
    {
        SetBossAction(_bossActionState);

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        SetBossAction(BossActionState.In);

    }

    private async void SetBossAction(BossActionState bossActionState) 
    {
        var state = bossActionState;
        switch (state)
        {
            case BossActionState.In:
                //登場演出予定
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                SetBossAction(BossActionState.Wait);
                break;

            case BossActionState.Wait:
                //待機時間
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                SetBossAction(BossActionState.Move);
                break;

            case BossActionState.Move:
                SetMove();
                break;
           
            case BossActionState.Out:
                //退場演出予定
                break;
            
        }

    }

    /// <summary>
    /// 移動後の位置を計算
    /// </summary>
    private void CulculateMovePos()
    {   
        _currentBossPos = new Vector3(UnityEngine.Random.Range(_movePosLeftLimit, _movePosRightLimit),0f, UnityEngine.Random.Range(_movePosUpLimit, _movePosDownLimit));
    }

    /// <summary>
    /// Bossを移動
    /// </summary>
    private void SetMove()
    {
        CulculateMovePos();

        _moveTween = DOTween.To(
                GetRigidLocalPosition,
                MoveRigidLocalPosition,
                _currentBossPos,
                1f
            )
            .SetEase(Ease.OutCubic)
            .SetUpdate(UpdateType.Fixed)
            .SetLink(gameObject);
        _moveTween.OnComplete(()=> 
        {
            SetBossAction(BossActionState.Wait);

        });
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    private void SetAttack()
    {
        
       
    }

    /// <summary>
    /// Rigidbodyの現在位置を親から見たローカル座標として取得
    /// </summary>
    private Vector3 GetRigidLocalPosition()
    {
        if (_thisTrans.parent == null)
        {
            return _thisRigid.position;
        }

        return _thisTrans.parent.InverseTransformPoint(_thisRigid.position);
    }

    /// <summary>
    /// ローカル座標で指定した位置へRigidbodyを移動
    /// </summary>
    private void MoveRigidLocalPosition(Vector3 localPosition)
    {
        Vector3 worldPosition = _thisTrans.parent == null
            ? localPosition
            : _thisTrans.parent.TransformPoint(localPosition);

        _thisRigid.MovePosition(worldPosition);
    }


}
