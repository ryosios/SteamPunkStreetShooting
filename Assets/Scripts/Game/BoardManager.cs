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
            for (int z = 0; z < 5; z++)
            {
                _boardSquare2DArray[x, z] = _boardSquareArray[x + z * 10];
               
            }            

        }

        Debug.Log(_boardSquare2DArray[2,3].gameObject.name);
        
    }

    public BoardSquare GetBoardFromIndex(int indexX,int indexZ)
    {
        return _boardSquare2DArray[indexX, indexZ];
    }


}
