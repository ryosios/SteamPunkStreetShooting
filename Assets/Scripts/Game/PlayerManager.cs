using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private enum DirectionKind
    {
        Right,
        Left,
        Up,
        Down,
    }

    /// <summary>現在の入力方向</summary>
    private DirectionKind _directionKind;

    /// <summary>UIデータ管理</summary>
    [SerializeField] private UiDataManager _uiDataManager;

    /// <summary>ボード管理</summary>
    [SerializeField] private BoardManager _boardManager;

    /// <summary>プレイヤーのRigidbody</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>背景管理</summary>
    [SerializeField] private BgManager _bgManager;

    /// <summary>使用可能なキャラクター配列</summary>
    [SerializeField] private CharacterManager[] _characterManagerArray = new CharacterManager[3];

    /// <summary>グレイズ判定用コライダー</summary>
    [SerializeField] private Transform _grazeCollider;

    /// <summary>グレイズ判定用コライダー</summary>
    public Transform GrazeCollider => _grazeCollider;

    /// <summary>グレイズ判定のクールタイム</summary>
    private float _grazeCoolTime = 0.1f;

    private float _grazeCoolTimeCount;

    private bool _isGrazeTimerStart = true;

    private bool _isPossibleGraze = false;

    /// <summary>パリィ判定用コライダー</summary>
    [SerializeField] private Transform _parryCollider;

    /// <summary>キャラクター変更のクールタイム</summary>
    private float _characterChangeCoolTime = 2f;

    private float _characterChangeCoolTimeCount;

    private bool _isCharacterChangeCoolTimerStart = true;

    private bool _isPossibleCharacterChange = true;

    /// <summary>キャラクター変更候補のリスト</summary>
    private List<CharacterManager> _characterList = new List<CharacterManager>();

    /// <summary>現在アクティブなキャラクター</summary>
    private CharacterManager _currentActiveCharacter;

    public CharacterManager CurrentActiveCharacter => _currentActiveCharacter;

    /// <summary>前回アクティブだったキャラクター</summary>
    private CharacterManager _beforeActiveCharacter;

    /// <summary>キャラクター変更中かどうか</summary>
    private bool _isUpdateCharacterChange;

    /// <summary>移動Tween</summary>
    private Tween _moveTween;

    /// <summary>背景の初期スクロール速度</summary>
    private float _initBgSpeed;

    /// <summary>被ダメージ後の無敵時間</summary>
    private float _invincibleTime = 0.5f;

    private float _invincibleTimeCount;

    private bool _isHitPossible = true;

    private bool _isHitTimerStart = false;

    public bool IsParryActive => _parryCollider.gameObject.activeSelf;

    /// <summary>グレイズで得られるスコア</summary>
    private int _grazeScorePoint = 1;

    /// <summary>パリィで得られるスコア</summary>
    private int _parryScorePoint = 200;

    /// <summary>パリィ成功時のヒットストップ時間</summary>
    [SerializeField] private float _parryHitStopTime = 0.03f;

    /// <summary>ヒットストップ中のTimeScale</summary>
    [SerializeField] private float _parryHitStopTimeScale = 0f;

    /// <summary>ヒットストップ後にTimeScaleを戻す時間</summary>
    [SerializeField] private float _parryHitStopRecoverTime = 0.08f;

    /// <summary>ヒットストップ中かどうか</summary>
    private bool _isHitStopping;

    /// <summary>現在のプレイヤーのボードインデックス</summary>
    public struct PlayerIndex
    {
        public int x;
        public int y;
    }

    private PlayerIndex _currentPlayerIndex;
    private PlayerIndex _beforePlayerIndex;

    private void Awake()
    {
        _characterList.Clear();
        for (int i = 0; i < _characterManagerArray.Length; i++)
        {
            CharacterManager characterManager = _characterManagerArray[i];
            if (characterManager == null || _characterList.Contains(characterManager))
            {
                continue;
            }

            _characterList.Add(characterManager);
        }

        if (_characterList.Count <= 0)
        {
            Debug.LogWarning("CharacterManager is not assigned.");
            return;
        }

        _currentActiveCharacter = _characterList[0];
        for (int i = 0; i < _characterList.Count; i++)
        {
            _characterList[i].SetActivateObjectActive(_characterList[i] == _currentActiveCharacter);
        }
        _currentActiveCharacter.UseAllAbilities();

        _currentPlayerIndex = new PlayerIndex();
        _currentPlayerIndex.x = 0;
        _currentPlayerIndex.y = 2;

        _initBgSpeed = _bgManager.GetSpeed();

        _parryCollider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetMove(DirectionKind.Up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetMove(DirectionKind.Down);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetMove(DirectionKind.Right);
            SetBgSpeed(2f);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetMove(DirectionKind.Left);
            SetBgSpeed(-2f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // スキル使用予定
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_isPossibleCharacterChange)
            {
                _isPossibleCharacterChange = false;
                _isCharacterChangeCoolTimerStart = true;
                ChangeCharacter();
            }
        }

        UpdateTimers();
    }

    /// <summary>
    /// 移動後のボードインデックスを計算
    /// </summary>
    private void CulculateMoveIndex(DirectionKind directionKind)
    {
        _directionKind = directionKind;
        _beforePlayerIndex = _currentPlayerIndex;
        switch (_directionKind)
        {
            case DirectionKind.Down:
                if (_currentPlayerIndex.y < 4)
                {
                    _currentPlayerIndex.y += 1;
                }
                break;
            case DirectionKind.Up:
                if (_currentPlayerIndex.y > 0)
                {
                    _currentPlayerIndex.y -= 1;
                }
                break;
            case DirectionKind.Right:
                if (_currentPlayerIndex.x < 11)
                {
                    _currentPlayerIndex.x += 1;
                }
                break;
            case DirectionKind.Left:
                if (_currentPlayerIndex.x > 0)
                {
                    _currentPlayerIndex.x -= 1;
                }
                break;
        }
    }

    /// <summary>
    /// キャラクターを指定方向へ移動
    /// </summary>
    private void SetMove(DirectionKind directionKind)
    {
        CulculateMoveIndex(directionKind);

        Vector3 currentPos = _boardManager
            .GetBoardFromIndex(_currentPlayerIndex.x, _currentPlayerIndex.y)
            .transform.position;

        _moveTween?.Kill();

        _moveTween = DOTween.To(
                () => _thisRigid.position,
                x => _thisRigid.MovePosition(x),
                currentPos,
                0.5f
            )
            .SetEase(Ease.OutCubic)
            .SetUpdate(UpdateType.Fixed)
            .SetLink(gameObject);
    }

    /// <summary>
    /// 移動に合わせて背景スクロール速度を一時的に変更
    /// </summary>
    private void SetBgSpeed(float speed)
    {
        var afterSpeed = _initBgSpeed + speed;
        _bgManager.SetSpeed(afterSpeed);
        DOVirtual.Float(afterSpeed, _initBgSpeed, 0.35f, value =>
        {
            _bgManager.SetSpeed(value);
        }).SetEase(Ease.OutSine);
    }

    /// <summary>
    /// キャラクターを交代
    /// </summary>
    private async void ChangeCharacter()
    {
        if (!_isUpdateCharacterChange)
        {
            _isUpdateCharacterChange = true;

            _characterList.Remove(_currentActiveCharacter);
            _beforeActiveCharacter = _currentActiveCharacter;
            int nextCharacterIndex = UnityEngine.Random.Range(0, _characterList.Count);
            _currentActiveCharacter = _characterList[nextCharacterIndex];
            _characterList.Add(_beforeActiveCharacter);

            SetParryCollider().Forget();

            _beforeActiveCharacter.SetActivateObjectActive(true);
            _currentActiveCharacter.SetActivateObjectActive(true);
            _currentActiveCharacter.UseAllAbilities();

            // クロスフェード中は前後のキャラクターを同時に表示する
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            _beforeActiveCharacter.SetActivateObjectActive(false);
            _isUpdateCharacterChange = false;
        }
    }

    /// <summary>
    /// プレイヤーが被弾したときに呼ばれる
    /// </summary>
    public void OnHitBullet()
    {
        if (_isHitPossible)
        {
            Debug.Log("ヒット");
            _isHitPossible = false;
            _isHitTimerStart = true;

            int characterIndex = GetCharacterIndex(_currentActiveCharacter);
            _currentActiveCharacter.AddHp(-1).SetHpView(_uiDataManager, characterIndex);
        }
    }

    /// <summary>
    /// UI表示に使う固定のキャラインデックスを取得
    /// </summary>
    private int GetCharacterIndex(CharacterManager characterManager)
    {
        for (int i = 0; i < _characterManagerArray.Length; i++)
        {
            if (_characterManagerArray[i] == characterManager)
            {
                return i;
            }
        }

        Debug.LogWarning("Current character is not found in CharacterManager array.");
        return 0;
    }

    /// <summary>
    /// パリィ用コライダーを短時間だけ有効化
    /// </summary>
    private async UniTaskVoid SetParryCollider()
    {
        _parryCollider.gameObject.SetActive(true);
        await UniTask.DelayFrame(5);
        _parryCollider.gameObject.SetActive(false);
    }

    /// <summary>
    /// パリィ成功時に呼ばれる
    /// </summary>
    public void OnParryBullet()
    {
        Debug.Log("パリィ");
        _uiDataManager.AddScore(_parryScorePoint);
        PlayParryHitStop().Forget();
    }

    /// <summary>
    /// グレイズ成功時に呼ばれる
    /// </summary>
    public void OnGrazeBullet()
    {
        if (_isPossibleGraze)
        {
            _isPossibleGraze = false;
            _isGrazeTimerStart = true;
            _uiDataManager.AddScore(_grazeScorePoint);
        }
    }

    /// <summary>
    /// キャラクターに自動射撃させる
    /// </summary>
    private void PlayCharacterBullet()
    {
    }

    /// <summary>
    /// パリィ成功時に短いヒットストップを入れる
    /// </summary>
    private async UniTaskVoid PlayParryHitStop()
    {
        if (_isHitStopping)
        {
            return;
        }

        _isHitStopping = true;
        float beforeTimeScale = Time.timeScale;
        float beforeFixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = _parryHitStopTimeScale;
        Time.fixedDeltaTime = Mathf.Max(0.0001f, beforeFixedDeltaTime * Time.timeScale);

        await UniTask.Delay(TimeSpan.FromSeconds(_parryHitStopTime), DelayType.UnscaledDeltaTime);

        if (_parryHitStopRecoverTime <= 0f)
        {
            Time.timeScale = beforeTimeScale;
            Time.fixedDeltaTime = beforeFixedDeltaTime;
            _isHitStopping = false;
            return;
        }

        float recoverTimeCount = 0f;
        while (recoverTimeCount < _parryHitStopRecoverTime)
        {
            recoverTimeCount += Time.unscaledDeltaTime;
            float rate = Mathf.Clamp01(recoverTimeCount / _parryHitStopRecoverTime);
            float easedRate = 1f - Mathf.Pow(1f - rate, 2f);
            Time.timeScale = Mathf.Lerp(_parryHitStopTimeScale, beforeTimeScale, easedRate);
            Time.fixedDeltaTime = Mathf.Max(0.0001f, beforeFixedDeltaTime * Time.timeScale);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        Time.timeScale = beforeTimeScale;
        Time.fixedDeltaTime = beforeFixedDeltaTime;
        _isHitStopping = false;
    }

    private void UpdateTimers()
    {
        if (_isHitTimerStart)
        {
            _invincibleTimeCount += Time.deltaTime;
            if (_invincibleTimeCount >= _invincibleTime)
            {
                _invincibleTimeCount = 0f;
                _isHitTimerStart = false;
                _isHitPossible = true;
            }
        }

        if (_isGrazeTimerStart)
        {
            _grazeCoolTimeCount += Time.deltaTime;
            if (_grazeCoolTimeCount >= _grazeCoolTime)
            {
                _grazeCoolTimeCount = 0f;
                _isGrazeTimerStart = false;
                _isPossibleGraze = true;
            }
        }

        if (_isCharacterChangeCoolTimerStart)
        {
            _characterChangeCoolTimeCount += Time.deltaTime;
            if (_characterChangeCoolTimeCount >= _characterChangeCoolTime)
            {
                _characterChangeCoolTimeCount = 0f;
                _isCharacterChangeCoolTimerStart = false;
                _isPossibleCharacterChange = true;
            }
        }
    }
}


