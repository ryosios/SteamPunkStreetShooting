using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public class CharacterManager : MonoBehaviour
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

    /// <summary>BoardManager</summary>
    [SerializeField] private BoardManager _boardManager;

    /// <summary>リジッドボディ</summary>
    [SerializeField] private Rigidbody _thisRigid;

    /// <summary>BGManager</summary>
    [SerializeField] private BgManager _bgManager;

    private Tween _moveTween;

    private float _initBgSpeed;

    /// <summary>現在のキャラがいるインデックス</summary>
    public struct CharaIndex 
    {
        public int x;
        public int z;
       
    }

    private CharaIndex _currentCharaIndex;
    private CharaIndex _beforeCharaIndex;

    private void Awake()
    {
        _currentCharaIndex = new CharaIndex();
        _currentCharaIndex.x = 0;
        _currentCharaIndex.z = 0;

        _initBgSpeed = _bgManager.GetSpeed();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetMove(DirectionKind.Up);

        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetMove(DirectionKind.Down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetMove(DirectionKind.Right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetMove(DirectionKind.Left);
        }
    }


    /// <summary>
    /// 移動後のインデックスを計算
    /// </summary>
    private void CulculateMoveIndex(DirectionKind directionKind)
    {
        _directionKind = directionKind;
        _beforeCharaIndex = _currentCharaIndex;
       switch (_directionKind)
        {
            case DirectionKind.Down:
                if (_currentCharaIndex.z < 4)
                {
                    _currentCharaIndex.z += 1;
                }
                break;
            case DirectionKind.Up:
                if (_currentCharaIndex.z > 0)
                {
                    _currentCharaIndex.z -= 1;
                }
                break;
            case DirectionKind.Right:
                if (_currentCharaIndex.x < 9)
                {
                    _currentCharaIndex.x += 1;
                }
                break;
            case DirectionKind.Left:
                if (_currentCharaIndex.x > 0)
                {
                    _currentCharaIndex.x -= 1;
                }
                break;

        }
    }

    /// <summary>
    /// キャラを移動
    /// </summary>
    private void SetMove(DirectionKind directionKind)
    {
        CulculateMoveIndex(directionKind);

        Vector3 currentPos = _boardManager
            .GetBoardFromIndex(_currentCharaIndex.x, _currentCharaIndex.z)
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

        //bg速度変更
        var afterSpeed = _initBgSpeed + 10f;
        SetBgSpeed(afterSpeed);
        DOVirtual.Float(afterSpeed, _initBgSpeed, 0.35f, value =>
        {
            SetBgSpeed(value);
        }).SetEase(Ease.OutSine);
    }

    /// <summary>
    /// 移動に伴って背景のスピードを変更する
    /// </summary>
    private void SetBgSpeed(float speed)
    {
        _bgManager.SetSpeed(speed);
       
    }
}
