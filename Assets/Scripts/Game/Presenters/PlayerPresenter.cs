using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class PlayerPresenter : MonoBehaviour
{
    private enum DirectionKind
    {
        Right,
        Left,
        Up,
        Down,
    }

    public struct CharacterHpChangedEvent
    {
        public readonly int Hp;
        public readonly int CharacterIndex;

        public CharacterHpChangedEvent(int hp, int characterIndex)
        {
            Hp = hp;
            CharacterIndex = characterIndex;
        }
    }

    /// <summary>現在の入力方向</summary>
    private DirectionKind _directionKind;

    /// <summary>ボード管理</summary>
    [SerializeField] private BoardManager _boardManager;

    /// <summary>プレイヤーのRigidbody</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>使用可能なキャラクター配列</summary>
    [FormerlySerializedAs("_characterManagerArray")]
    [SerializeField] private CharacterPresenter[] _characterPresenters = new CharacterPresenter[3];

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
    private List<CharacterPresenter> _characterList = new List<CharacterPresenter>();

    /// <summary>現在アクティブなキャラクター</summary>
    private CharacterPresenter _currentActiveCharacter;

    public CharacterPresenter CurrentActiveCharacter => _currentActiveCharacter;

    /// <summary>現在アクティブなキャラクターの攻撃力</summary>
    public float CurrentPower => _currentActiveCharacter != null ? _currentActiveCharacter.Power : 0f;

    /// <summary>前回アクティブだったキャラクター</summary>
    private CharacterPresenter _beforeActiveCharacter;

    /// <summary>キャラクター変更中かどうか</summary>
    private bool _isUpdateCharacterChange;

    /// <summary>移動Tween</summary>
    private Tween _moveTween;

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

    /// <summary>現在のプレイヤーのボードインデックスModel</summary>
    private BoardPositionModel _currentPlayerIndex;

    /// <summary>前回のプレイヤーのボードインデックスModel</summary>
    private BoardPositionModel _beforePlayerIndex;

    /// <summary>キャラクターHP変更通知の発信元</summary>
    private readonly Subject<CharacterHpChangedEvent> _characterHpChanged = new Subject<CharacterHpChangedEvent>();

    /// <summary>スコア加算通知の発信元</summary>
    private readonly Subject<int> _scoreAdded = new Subject<int>();

    /// <summary>背景速度一時変更通知の発信元</summary>
    private readonly Subject<float> _bgSpeedOffsetRequested = new Subject<float>();

    /// <summary>キャラクターHPが変わったときの通知</summary>
    public IObservable<CharacterHpChangedEvent> CharacterHpChanged => _characterHpChanged;

    /// <summary>スコアを加算したいときの通知</summary>
    public IObservable<int> ScoreAdded => _scoreAdded;

    /// <summary>背景速度を一時的に変更したいときの通知</summary>
    public IObservable<float> BgSpeedOffsetRequested => _bgSpeedOffsetRequested;

    private void Awake()
    {
        _characterList.Clear();
        for (int i = 0; i < _characterPresenters.Length; i++)
        {
            CharacterPresenter characterPresenter = _characterPresenters[i];
            if (characterPresenter == null || _characterList.Contains(characterPresenter))
            {
                continue;
            }

            _characterList.Add(characterPresenter);
        }

        if (_characterList.Count <= 0)
        {
            Debug.LogWarning("CharacterPresenter array is not assigned.");
            return;
        }

        _currentActiveCharacter = _characterList[0];
        for (int i = 0; i < _characterList.Count; i++)
        {
            _characterList[i].SetActivateObjectActive(_characterList[i] == _currentActiveCharacter);
        }
        _currentActiveCharacter.UseAllAbilities();

        _currentPlayerIndex = new BoardPositionModel(0, 2);

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
            _bgSpeedOffsetRequested.OnNext(2f);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetMove(DirectionKind.Left);
            _bgSpeedOffsetRequested.OnNext(-2f);
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

    private void OnDestroy()
    {
        _moveTween?.Kill();
        _characterHpChanged.Dispose();
        _scoreAdded.Dispose();
        _bgSpeedOffsetRequested.Dispose();
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
            _currentActiveCharacter.AddHp(-1);
            _characterHpChanged.OnNext(new CharacterHpChangedEvent(_currentActiveCharacter.Hp, characterIndex));
        }
    }

    /// <summary>
    /// UI表示に使う固定のキャラインデックスを取得
    /// </summary>
    private int GetCharacterIndex(CharacterPresenter characterPresenter)
    {
        for (int i = 0; i < _characterPresenters.Length; i++)
        {
            if (_characterPresenters[i] == characterPresenter)
            {
                return i;
            }
        }

        Debug.LogWarning("Current character is not found in CharacterPresenter array.");
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
        _scoreAdded.OnNext(_parryScorePoint);
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
            _scoreAdded.OnNext(_grazeScorePoint);
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





