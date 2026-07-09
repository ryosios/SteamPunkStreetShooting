using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;


public abstract class TweenAnimationBase : MonoBehaviour
{

    private CancellationTokenSource _cts;

    public bool IsPlaying { get; set; }

    protected CancellationToken Token => _cts.Token;

    public void Kill()
    {
        if (_cts == null) return;
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;

        //Kill時に呼ばれる
        OnKill();
        IsPlaying = false;
    }

    protected virtual UniTask InAnimAsync(CancellationToken cancellationToken)
    {
        //子クラスでオーバーライドしてないときはすぐCompletedTaskを返す
        return UniTask.CompletedTask;

    }
    protected virtual UniTask OutAnimAsync(CancellationToken cancellationToken)
    {
        //子クラスでオーバーライドしてないときはすぐCompletedTaskを返す
        return UniTask.CompletedTask;

    }

    protected virtual void OnKill()
    {

    }

    public async UniTask PlayInAnimAsync()
    {
        Kill();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        IsPlaying = true;

        try
        {
            await InAnimAsync(_cts.Token);

        }
        finally
        {
            IsPlaying = false;
        }
    }

    public async UniTask PlayOutAnimAsync()
    {
        Kill();

        _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        IsPlaying = true;

        try
        {
            await OutAnimAsync(_cts.Token);

        }
        finally
        {
            IsPlaying = false;
        }
    }
}
