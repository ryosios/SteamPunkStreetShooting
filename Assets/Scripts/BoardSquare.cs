using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public class BoardSquare : MonoBehaviour
{

    /// <summary>ボートのマスのインデックス</summary>
    [System.Serializable]
    public struct BoardIndex
    {
        public int _x;
        public int _z;
        public int X => _x;
        public int Y => _z;
    }

    [SerializeField] private BoardIndex _boardIndex;

    [SerializeField] private Transform _thisTrans;


}
