using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading;


public class TweenAnimationTemplate : TweenAnimationBase
{

    [SerializeField] private CanvasGroup canvasGroup;

    private Sequence _sequence;

 

    protected override async UniTask InAnimAsync(CancellationToken token)
    {
        _sequence = DOTween.Sequence();

        //アニメーション

        await _sequence.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);

    }
    protected override async UniTask OutAnimAsync(CancellationToken cancellationToken)
    {
        _sequence = DOTween.Sequence();

        //アニメーション

        await _sequence.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);

    }

    protected override void OnKill()
    {
        _sequence?.Kill();
    }


    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            PlayInAnimAsync().Forget();
        }

#endif
    }
}
