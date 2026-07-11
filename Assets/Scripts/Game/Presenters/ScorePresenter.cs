/// <summary>
/// スコアModelとScoreViewをつなぐPresenter。
/// </summary>
public class ScorePresenter
{
    /// <summary>スコアの状態Model</summary>
    private readonly ScoreModel _model = new ScoreModel();

    /// <summary>スコア表示View</summary>
    private ScoreView _view;

    /// <summary>
    /// 表示先のViewを設定し、現在スコアを反映
    /// </summary>
    /// <param name="view">スコア表示View。</param>
    public void SetView(ScoreView view)
    {
        _view = view;
        UpdateView();
    }

    /// <summary>
    /// スコアを加算して表示を更新
    /// </summary>
    /// <param name="value">加算するスコア。</param>
    public void AddScore(int value)
    {
        _model.AddScore(value);
        UpdateView();
    }

    /// <summary>
    /// 現在のスコアを取得
    /// </summary>
    public int GetScore()
    {
        return _model.Score;
    }

    /// <summary>
    /// 現在のスコアをViewへ反映
    /// </summary>
    private void UpdateView()
    {
        if (_view == null)
        {
            return;
        }

        _view.SetScore(_model.Score);
    }
}
