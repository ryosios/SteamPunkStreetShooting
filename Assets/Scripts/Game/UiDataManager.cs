using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public class UiDataManager : MonoBehaviour
{
    //ゲームデータの受け渡し等
    public IReadOnlyReactiveProperty<int> Hp => _hp;
    private readonly ReactiveProperty<int> _hp = new();

    public IReadOnlyReactiveProperty<int> Score => _score;
    private readonly ReactiveProperty<int> _score = new();

    public void SetHp(int hp)
    {
        _hp.Value = hp;
    }

    public void SetScore(int score)
    {
        _score.Value = score;
    }

}
