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
    [SerializeField] private Rigidbody2D _thisRigid2D;

    private Tween _moveTween;

    /// <summary>現在のキャラがいるインデックス</summary>
    public struct CharaIndex 
    {
        public int x;
        public int y;
       
    }

    private CharaIndex _currentCharaIndex;
    private CharaIndex _beforeCharaIndex;

    private void Awake()
    {
        _currentCharaIndex = new CharaIndex();
        _currentCharaIndex.x = 0;
        _currentCharaIndex.y = 0;

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
                if (_currentCharaIndex.y < 4)
                {
                    _currentCharaIndex.y += 1;
                }
                break;
            case DirectionKind.Up:
                if (_currentCharaIndex.y > 0)
                {
                    _currentCharaIndex.y -= 1;
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

        Vector2 currentPos = _boardManager
            .GetBoardFromIndex(_currentCharaIndex.x, _currentCharaIndex.y)
            .transform.position;

        _moveTween?.Kill();

        _moveTween = DOTween.To(
                () => _thisRigid2D.position,
                x => _thisRigid2D.MovePosition(x),
                currentPos,
                0.5f
            )
            .SetEase(Ease.OutCubic)
            .SetUpdate(UpdateType.Fixed)
            .SetLink(gameObject);
    }

}
