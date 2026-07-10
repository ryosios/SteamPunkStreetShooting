using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Serialization;

public class BossPresenter : MonoBehaviour
{
    private enum BossActionState
    {
        Default,
        In,
        Wait,
        Move,
        Out,
    }

    /// <summary>現在のボス行動状態</summary>
    private BossActionState _bossActionState = BossActionState.Default;

    /// <summary>ボスの状態Model</summary>
    [SerializeField] private BossModel _model = new BossModel();

    /// <summary>プレイヤー管理</summary>
    [FormerlySerializedAs("_playerManager")]
    [SerializeField] private PlayerPresenter _playerPresenter;

    /// <summary>ボスのRigidbody</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>ボスのTransform</summary>
    [SerializeField] private Transform _thisTrans;

    /// <summary>攻撃オブジェクトのTransform</summary>
    [SerializeField] private Transform _attackTrans;

    /// <summary>ボード管理</summary>
    [SerializeField] private BoardManager _boardManager;

    /// <summary>移動可能なボード範囲: 左端X</summary>
    [SerializeField] private int _moveIndexLeftLimit = 10;

    /// <summary>移動可能なボード範囲: 右端X</summary>
    [SerializeField] private int _moveIndexRightLimit = 11;

    /// <summary>移動可能なボード範囲: 上端Y</summary>
    [SerializeField] private int _moveIndexUpLimit = 0;

    /// <summary>移動可能なボード範囲: 下端Y</summary>
    [SerializeField] private int _moveIndexDownLimit = 4;

    /// <summary>ボス移動Tween</summary>
    private Tween _moveTween;

    /// <summary>ボスの現在ボードインデックスModel</summary>
    private BoardPositionModel _currentBossIndex;

    /// <summary>ボスの前回ボードインデックスModel</summary>
    private BoardPositionModel _beforeBossIndex;

    /// <summary>ボスHPを増減させたいときに通知</summary>
    public event Action<float> BossHpAdded;

    private void Awake()
    {
        if (_playerPresenter == null)
        {
            _playerPresenter = FindFirstObjectByType<PlayerPresenter>();
        }

        if (_thisRigid == null)
        {
            _thisRigid = GetComponent<Rigidbody>();
        }

        if (_thisTrans == null)
        {
            _thisTrans = transform;
        }

        if (_boardManager == null)
        {
            _boardManager = FindFirstObjectByType<BoardManager>();
        }

        _currentBossIndex = new BoardPositionModel(
            Mathf.Clamp(_moveIndexRightLimit, _moveIndexLeftLimit, _moveIndexRightLimit),
            Mathf.Clamp(2, _moveIndexUpLimit, _moveIndexDownLimit)
        );
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
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                SetBossAction(BossActionState.Wait);
                break;

            case BossActionState.Wait:
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                SetBossAction(BossActionState.Move);
                break;

            case BossActionState.Move:
                SetMove();
                break;

            case BossActionState.Out:
                break;
        }
    }

    /// <summary>
    /// 移動後のボードインデックスを計算
    /// </summary>
    private void CulculateMoveIndex()
    {
        _beforeBossIndex = _currentBossIndex;
        _currentBossIndex.x = UnityEngine.Random.Range(_moveIndexLeftLimit, _moveIndexRightLimit + 1);
        _currentBossIndex.y = UnityEngine.Random.Range(_moveIndexUpLimit, _moveIndexDownLimit + 1);
    }

    /// <summary>
    /// ボスをボード上のランダム位置へ移動
    /// </summary>
    private void SetMove()
    {
        CulculateMoveIndex();

        if (_boardManager == null)
        {
            Debug.LogWarning("BoardManager is not assigned.");
            return;
        }

        Vector3 currentPos = _boardManager
            .GetBoardFromIndex(_currentBossIndex.x, _currentBossIndex.y)
            .transform.position;

        _moveTween?.Kill();

        _moveTween = DOTween.To(
                () => _thisRigid.position,
                x => _thisRigid.MovePosition(x),
                currentPos,
                1f
            )
            .SetEase(Ease.OutCubic)
            .SetUpdate(UpdateType.Fixed)
            .SetLink(gameObject);
        _moveTween.OnComplete(() =>
        {
            SetBossAction(BossActionState.Wait);
        });
    }

    /// <summary>
    /// ボス攻撃処理
    /// </summary>
    private void SetAttack()
    {
    }

    /// <summary>
    /// プレイヤー弾がボスにヒットしたときに呼ばれる
    /// </summary>
    public void OnHitBullet()
    {
        if (_playerPresenter == null || _playerPresenter.CurrentActiveCharacter == null)
        {
            Debug.LogWarning("Boss damage references are not assigned.");
            return;
        }

        float activeCharacterPower = _playerPresenter.CurrentActiveCharacter.Power;
        float normalizedDamage = _model.CalculateNormalizedDamage(activeCharacterPower);
        BossHpAdded?.Invoke(-normalizedDamage);
    }
}




