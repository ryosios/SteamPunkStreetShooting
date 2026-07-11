using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine.Serialization;

public class GamePresenter : MonoBehaviour
{
    private enum GameState
    {
        BeforeStart,
        GameStart,
        GamePlay,
        GameEnd,
        Result
    }

    /// <summary>現在のゲーム状態</summary>
    private GameState _gameState = GameState.BeforeStart;

    /// <summary>ゲーム全体のHUD表示View</summary>
    [FormerlySerializedAs("_uiDataManager")]
    [SerializeField] private GameHudView _gameHudView;

    /// <summary>プレイヤーPresenter</summary>
    [SerializeField] private PlayerPresenter _playerPresenter;

    /// <summary>ボスPresenter</summary>
    [SerializeField] private BossPresenter _bossPresenter;

    /// <summary>背景Presenter</summary>
    [FormerlySerializedAs("_bgManager")]
    [SerializeField] private BgPresenter _bgPresenter;

    /// <summary>背景速度を一時変更したあと基準速度へ戻す時間</summary>
    [SerializeField] private float _bgSpeedRecoverTime = 0.35f;

    /// <summary>背景の基準スクロール速度</summary>
    private float _baseBgSpeed;

    /// <summary>背景速度変更Tween</summary>
    private Tween _bgSpeedTween;

    private void Awake()
    {
        if (_gameHudView == null)
        {
            _gameHudView = FindFirstObjectByType<GameHudView>();
        }

        if (_playerPresenter == null)
        {
            _playerPresenter = FindFirstObjectByType<PlayerPresenter>();
        }

        if (_bossPresenter == null)
        {
            _bossPresenter = FindFirstObjectByType<BossPresenter>();
        }

        if (_bgPresenter == null)
        {
            _bgPresenter = FindFirstObjectByType<BgPresenter>();
        }

        if (_bgPresenter != null)
        {
            _baseBgSpeed = _bgPresenter.GetSpeed();
        }

        RegisterPresenterEvents();
    }

    private void OnDestroy()
    {
        _bgSpeedTween?.Kill();
    }

    /// <summary>
    /// 各Presenterからのゲームイベントを購読
    /// </summary>
    private void RegisterPresenterEvents()
    {
        if (_playerPresenter != null)
        {
            _playerPresenter.CharacterHpChanged
                .Subscribe(value => SetCharacterHp(value.Hp, value.CharacterIndex))
                .AddTo(this);

            _playerPresenter.ScoreAdded
                .Subscribe(AddScore)
                .AddTo(this);

            _playerPresenter.BgSpeedOffsetRequested
                .Subscribe(SetTemporaryBgSpeedOffset)
                .AddTo(this);
        }

        if (_bossPresenter != null)
        {
            _bossPresenter.BossHpAdded
                .Subscribe(AddBossHp)
                .AddTo(this);

            _bossPresenter.BossHitBulletRequested
                .Subscribe(_ => ApplyPlayerDamageToBoss())
                .AddTo(this);
        }
    }

    /// <summary>
    /// キャラクターHP表示を更新
    /// </summary>
    private void SetCharacterHp(int hp, int characterIndex)
    {
        if (_gameHudView == null)
        {
            return;
        }

        _gameHudView.SetHp(hp, characterIndex);
    }

    /// <summary>
    /// スコア表示を加算更新
    /// </summary>
    private void AddScore(int score)
    {
        if (_gameHudView == null)
        {
            return;
        }

        _gameHudView.AddScore(score);
    }

    /// <summary>
    /// ボスHP表示を加算更新
    /// </summary>
    private void AddBossHp(float hp)
    {
        if (_gameHudView == null)
        {
            return;
        }

        _gameHudView.AddBossHp(hp);
    }

    /// <summary>
    /// 現在のプレイヤー攻撃力を使ってボスにダメージを適用
    /// </summary>
    private void ApplyPlayerDamageToBoss()
    {
        if (_playerPresenter == null || _bossPresenter == null)
        {
            return;
        }

        _bossPresenter.ApplyDamage(_playerPresenter.CurrentPower);
    }

    /// <summary>
    /// 背景速度を一時的に変更し、基準速度へ戻す。
    /// </summary>
    /// <param name="speedOffset">基準速度に加算する一時速度。</param>
    private void SetTemporaryBgSpeedOffset(float speedOffset)
    {
        if (_bgPresenter == null)
        {
            return;
        }

        float afterSpeed = _baseBgSpeed + speedOffset;
        _bgPresenter.SetSpeed(afterSpeed);

        _bgSpeedTween?.Kill();
        _bgSpeedTween = DOVirtual.Float(afterSpeed, _baseBgSpeed, _bgSpeedRecoverTime, value =>
            {
                _bgPresenter.SetSpeed(value);
            })
            .SetEase(Ease.OutSine)
            .SetLink(gameObject);
    }

    /// <summary>
    /// ゲーム状態を変更
    /// </summary>
    private void SetGameState(GameState thisState)
    {
        _gameState = thisState;

        switch (_gameState)
        {
            case GameState.BeforeStart:
                SetGameState(GameState.GameStart);
                break;
            case GameState.GameStart:
                break;
            case GameState.GamePlay:
                break;
            case GameState.GameEnd:
                break;
            case GameState.Result:
                break;
        }
    }
}
