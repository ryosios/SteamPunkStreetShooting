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

        }
    }

    /// <summary>
    /// キャラを移動
    /// </summary>
    private void SetMove(DirectionKind directionKind)
    {
        CulculateMoveIndex(directionKind);
        float beforePosX = _boardManager.GetBoardFromIndex(_beforeCharaIndex.x, _beforeCharaIndex.y).transform.position.x;
        float beforePosY = _boardManager.GetBoardFromIndex(_beforeCharaIndex.x, _beforeCharaIndex.y).transform.position.y;
        float currentPosX = _boardManager.GetBoardFromIndex(_currentCharaIndex.x, _currentCharaIndex.y).transform.position.x;
        float currentPosY = _boardManager.GetBoardFromIndex(_currentCharaIndex.x, _currentCharaIndex.y).transform.position.y;

        Vector2 beforePos = new Vector2(beforePosX,beforePosY);
        Vector2 currentPos = new Vector2(currentPosX, currentPosY);

        _moveTween?.Kill();

        _moveTween = DOTween.To(
                () => beforePos,
                x => _thisRigid2D.MovePosition(x),
                currentPos,
                0.5f
            )
            .SetEase(Ease.OutCubic)
            .SetUpdate(UpdateType.Fixed)
            .SetLink(gameObject);


    }

}
