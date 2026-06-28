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

    /// <summaryDirectionkind</summary>
    private DirectionKind _directionKind;

    /// <summary>UiDataManager</summary>
    [SerializeField] private UiDataManager _uiDataManager;

    /// <summary>BoardManager</summary>
    [SerializeField] private BoardManager _boardManager;

    /// <summary>リジッドボディ</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>BGManager</summary>
    [SerializeField] private BgManager _bgManager;

    /// <summary>CharacterManager</summary>
    [SerializeField] private CharacterManager[] _characterManagerArray = new CharacterManager[3];

    /// <summary>グレイズコライダー</summary>
    [SerializeField] private Transform _grazeCollider;

    /// <summary>グレイズコライダー</summary>
    public Transform GrazeCollider => _grazeCollider;

    /// <summary>グレイズの判定クールタイム</summary>
    private float _grazeCoolTime = 0.1f;//0.1秒ごとにグレイズ判定発生

    private float _grazeCoolTimeCount;

    private bool _isGrazeTimerStart = true;

    private bool _isPossibleGraze = false;

    /// <summary>アクティブなCharacterのリスト</summary>
    private List<CharacterManager> _characterList = new List<CharacterManager>();

    /// <summary>現在アクティブのキャラクター(交代後)</summary>
    private CharacterManager _currentActiveCharacter;

    /// <summary>前回アクティブだったキャラクター(交代前)</summary>
    private CharacterManager _beforeActiveCharacter;

    /// <summary>キャラチェンジ中のフラグ</summary>
    private bool _isUpdateCharacterChange;

    /// <summary>Tween</summary>
    private Tween _moveTween;

    /// <summary>BGの初期スピード</summary>
    private float _initBgSpeed;

    /// <summary>被ダメ時の無敵時間</summary>
    private float _invincibleTime = 0.5f;

    private float _invincibleTimeCount;

    private bool _isHitPossible = true;

    private bool _isHitTimerStart = false;

    /// <summary>現在のキャラがいるインデックス</summary>
    public struct PlayerIndex 
    {
        public int x;
        public int z;
       
    }

    private PlayerIndex _currentPlayerIndex;
    private PlayerIndex _beforePlayerIndex;

    private void Awake()
    {
        
        _currentActiveCharacter = _characterManagerArray[0];
        _characterManagerArray[0].gameObject.SetActive(true);
        _characterManagerArray[1].gameObject.SetActive(false);
        _characterManagerArray[2].gameObject.SetActive(false);


        for (int i = 0; i < _characterManagerArray.Length; i++)
        {
            _characterList.Add(_characterManagerArray[i]);

        }

        _currentPlayerIndex = new PlayerIndex();
        _currentPlayerIndex.x = 0;
        _currentPlayerIndex.z = 2;

        _initBgSpeed = _bgManager.GetSpeed();

       


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
            SetBgSpeed(7f);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetMove(DirectionKind.Left);
            SetBgSpeed(-7f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //スキル予定
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //キャラチェンジ
            ChangeCharacter();
        }
    }


    /// <summary>
    /// 移動後のインデックスを計算
    /// </summary>
    private void CulculateMoveIndex(DirectionKind directionKind)
    {
        _directionKind = directionKind;
        _beforePlayerIndex = _currentPlayerIndex;
       switch (_directionKind)
        {
            case DirectionKind.Down:
                if (_currentPlayerIndex.z < 4)
                {
                    _currentPlayerIndex.z += 1;
                }
                break;
            case DirectionKind.Up:
                if (_currentPlayerIndex.z > 0)
                {
                    _currentPlayerIndex.z -= 1;
                }
                break;
            case DirectionKind.Right:
                if (_currentPlayerIndex.x < 9)
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

        if (_isHitTimerStart)
        {
            _invincibleTimeCount += Time.deltaTime;
            if (_invincibleTimeCount <= _invincibleTime)
            {
                _invincibleTimeCount = 0;
                _isHitTimerStart = false;
                _isHitPossible = true;
              
            }
            
        }


        if (_isGrazeTimerStart)
        {
            _grazeCoolTimeCount += Time.deltaTime;
            if (_grazeCoolTimeCount <= _grazeCoolTime)
            {
                _grazeCoolTimeCount = 0;
                _isGrazeTimerStart = false;
                _isPossibleGraze = true;

            }

        }        

    }

    /// <summary>
    /// キャラを移動
    /// </summary>
    private void SetMove(DirectionKind directionKind)
    {
        CulculateMoveIndex(directionKind);

        Vector3 currentPos = _boardManager
            .GetBoardFromIndex(_currentPlayerIndex.x, _currentPlayerIndex.z)
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
    /// 移動に伴って背景のスピードを変更する
    /// </summary>
    private void SetBgSpeed(float speed)
    {
        
        //bg速度変更
        var afterSpeed = _initBgSpeed + speed;
        _bgManager.SetSpeed(afterSpeed); 
        DOVirtual.Float(afterSpeed, _initBgSpeed, 0.35f, value =>
        {
            _bgManager.SetSpeed(value); 
        }).SetEase(Ease.OutSine);

    }

    /// <summary>
    /// キャラを交代（Player配下にCharacterを3種類持つ。クロスフェード的な演出で切り替える）
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

            _beforeActiveCharacter.gameObject.SetActive(true);
            _currentActiveCharacter.gameObject.SetActive(true);

            //キャラ交代演出予定。Spineでアニメーション。描画順を決めるためにSpineのOrderInLayerでcurrentの方を必ず上にする。
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            _beforeActiveCharacter.gameObject.SetActive(false);
            _isUpdateCharacterChange = false;

        }
    }

    /// <summary>
    /// パーティクルから被弾したとき呼ばれる
    /// </summary>
    public void OnHitBullet()
    {
        Debug.Log("ヒット");

        if (_isHitPossible)
        {
            _isHitPossible = false;
            _isHitTimerStart = true;
            //キャラのHPを減らす。UI表示も変更。
            _currentActiveCharacter.AddHp(-1).SetHpView(_uiDataManager);

        }       
       
    }

    /// <summary>
    /// パーティクルから被弾（グレイズ用）したとき呼ばれる
    /// </summary>
    public void OnGrazeBullet()
    {
        Debug.Log("グレイズ");
        if (_isPossibleGraze)
        {
            _isPossibleGraze = false;
            _isGrazeTimerStart = true;
            _uiDataManager.SetScore(1);         


        }
        
       
    }
}
