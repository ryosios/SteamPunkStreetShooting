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

    /// <summary>ボスの防御力（1に対しての割合）</summary>
    private float _guard = 10f;

    /// <summary>UIDataManager</summary>
    [SerializeField] private UiDataManager _uiDataManager;

    /// <summary>UIDataManager</summary>
    [SerializeField] private PlayerManager _playerManager;

    /// <summary>リジッドボディ</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>Transform</summary>
    [SerializeField] private Transform _thisTrans;

    /// <summary>AttackオブジェクトのTransform</summary>
    [SerializeField] private Transform _attackTrans;

    /// <summary>BoardManager</summary>
    [SerializeField] private BoardManager _boardManager;

    /// <summary>動けるBoard範囲:左</summary>
    [SerializeField] private int _moveIndexLeftLimit = 10;

    /// <summary>動けるBoard範囲:右</summary>
    [SerializeField] private int _moveIndexRightLimit = 11;

    /// <summary>動けるBoard範囲:上</summary>
    [SerializeField] private int _moveIndexUpLimit = 0;

    /// <summary>動けるBoard範囲:下</summary>
    [SerializeField] private int _moveIndexDownLimit = 4;

    /// <summary>ボスの移動Tween</summary>
    private Tween _moveTween;

    /// <summary>現在のボスがいるBoardインデックス</summary>
    public struct BossIndex
    {
        public int x;
        public int z;
    }

    private BossIndex _currentBossIndex;
    private BossIndex _beforeBossIndex;

    private void Awake()
    {
        if (_uiDataManager == null)
        {
            _uiDataManager = FindFirstObjectByType<UiDataManager>();
        }

        if (_playerManager == null)
        {
            _playerManager = FindFirstObjectByType<PlayerManager>();
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

        _currentBossIndex = new BossIndex
        {
            x = Mathf.Clamp(_moveIndexRightLimit, _moveIndexLeftLimit, _moveIndexRightLimit),
            z = Mathf.Clamp(2, _moveIndexUpLimit, _moveIndexDownLimit)
        };

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
    /// 移動後のBoardインデックスを計算
    /// </summary>
    private void CulculateMoveIndex()
    {   
        _beforeBossIndex = _currentBossIndex;
        _currentBossIndex.x = UnityEngine.Random.Range(_moveIndexLeftLimit, _moveIndexRightLimit + 1);
        _currentBossIndex.z = UnityEngine.Random.Range(_moveIndexUpLimit, _moveIndexDownLimit + 1);
    }

    /// <summary>
    /// Bossを移動
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
            .GetBoardFromIndex(_currentBossIndex.x, _currentBossIndex.z)
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
    /// Playerの弾がヒットしたときに呼ばれる
    /// </summary>
    public void OnHitBullet()
    {
        if (_uiDataManager == null || _playerManager == null || _playerManager.CurrentActiveCharacter == null)
        {
            Debug.LogWarning("Boss damage references are not assigned.");
            return;
        }

        float safeGuard = Mathf.Max(0.0001f, _guard);
        float activeCharacterPower = _playerManager.CurrentActiveCharacter.Power;
        float normalizedDamage = activeCharacterPower / safeGuard;
        _uiDataManager.AddBossHp(-normalizedDamage);
    }


}
