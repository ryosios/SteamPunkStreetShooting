/// <summary>
/// ボード上のXYインデックスを表すModel。
/// </summary>
[System.Serializable]
public struct BoardPositionModel
{
    public int x;
    public int y;

    public BoardPositionModel(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

