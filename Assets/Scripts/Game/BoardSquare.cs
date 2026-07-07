using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    /// <summary>ボードマスのXYインデックス</summary>
    [System.Serializable]
    public struct BoardIndex
    {
        [SerializeField] private int _x;
        [SerializeField] private int _y;

        public int X => _x;
        public int Y => _y;
    }

    /// <summary>このマスのXYインデックス</summary>
    [SerializeField] private BoardIndex _boardIndex;

    /// <summary>このマスのTransform</summary>
    [SerializeField] private Transform _thisTrans;

    public BoardIndex Index => _boardIndex;
    public Transform ThisTrans => _thisTrans != null ? _thisTrans : transform;
}
