using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const int BoardWidth = 12;
    private const int BoardHeight = 5;

    /// <summary>ボードマスの配列</summary>
    [SerializeField] private BoardSquare[] _boardSquareArray;

    /// <summary>XYインデックスで参照するボードマス配列</summary>
    private readonly BoardSquare[,] _boardSquare2DArray = new BoardSquare[BoardWidth, BoardHeight];

    private void Awake()
    {
        for (int x = 0; x < BoardWidth; x++)
        {
            for (int y = 0; y < BoardHeight; y++)
            {
                _boardSquare2DArray[x, y] = _boardSquareArray[x + y * BoardWidth];
            }
        }
    }

    /// <summary>
    /// XYインデックスからボードマスを取得
    /// </summary>
    public BoardSquare GetBoardFromIndex(int indexX, int indexY)
    {
        return _boardSquare2DArray[indexX, indexY];
    }
}
