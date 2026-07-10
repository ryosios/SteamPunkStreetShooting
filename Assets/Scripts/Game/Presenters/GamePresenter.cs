using UnityEngine;
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

        RegisterPresenterEvents();
    }

    private void OnDestroy()
    {
        UnregisterPresenterEvents();
    }

    /// <summary>
    /// 各Presenterからのゲームイベントを購読
    /// </summary>
    private void RegisterPresenterEvents()
    {
        if (_playerPresenter != null)
        {
            _playerPresenter.CharacterHpChanged += SetCharacterHp;
            _playerPresenter.ScoreAdded += AddScore;
        }

        if (_bossPresenter != null)
        {
            _bossPresenter.BossHpAdded += AddBossHp;
        }
    }

    /// <summary>
    /// 各Presenterからのゲームイベント購読を解除
    /// </summary>
    private void UnregisterPresenterEvents()
    {
        if (_playerPresenter != null)
        {
            _playerPresenter.CharacterHpChanged -= SetCharacterHp;
            _playerPresenter.ScoreAdded -= AddScore;
        }

        if (_bossPresenter != null)
        {
            _bossPresenter.BossHpAdded -= AddBossHp;
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
