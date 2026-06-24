using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    /// <summary>ボードのマスの配列</summary>
    [SerializeField] private BoardSquare[] _boardSquareArray;

    private BoardSquare[,] _boardSquare2DArray = new BoardSquare[10, 5];


    private void Awake()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                _boardSquare2DArray[x, y] = _boardSquareArray[x + y * 10];
               
            }            

        }

        Debug.Log(_boardSquare2DArray[2,3].gameObject.name);
        
    }

    public BoardSquare GetBoardFromIndex(int indexX,int indexY)
    {
        return _boardSquare2DArray[indexX, indexY];
    }


}
