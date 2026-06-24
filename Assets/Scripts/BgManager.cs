using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class BgManager : MonoBehaviour
{
    //public Subject<Unit> Default = new Subject<Unit>();

    /// <summary>BGBlockのインスタンス</summary>
    [SerializeField] private BgBlock[] _blockBgBlock;


    /// <summary>Attach</summary>
    [SerializeField] private Transform _attachTrans;

    /// <summary>初期位置</summary>
    private float _initPosX = 0f;

    /// <summary>スピード</summary>
    private float _speed = -10f;

    /// <summary>位置のオフセット</summary>
    private float _offsetPos = 30f;


    private void Awake()
    {
        int selectIndex = Random.Range(0, _blockBgBlock.Length);
        var blockInstance = Instantiate(_blockBgBlock[selectIndex]);
        blockInstance.gameObject.SetActive(true);
        blockInstance.transform.SetParent(_attachTrans);
        blockInstance.transform.localScale = Vector3.one;
        blockInstance.transform.localPosition = new Vector3(_initPosX, 0, 0);
        blockInstance.SetMove(_speed);
        blockInstance.InstanceTimingSubject.Subscribe( _ =>
        {
            SetBlock(_initPosX + _offsetPos);

        }).AddTo(this);

        SetBlock(_initPosX + _offsetPos);
       
    }
    /// <summary>
    /// BGBlockを移動
    /// </summary>
    private void SetBlock(float posX)
    {
        int selectIndex = Random.Range(0, _blockBgBlock.Length);
        var blockInstance = Instantiate(_blockBgBlock[selectIndex]);
        blockInstance.gameObject.SetActive(true);
        blockInstance.transform.SetParent(_attachTrans);
        blockInstance.transform.localScale = Vector3.one;
        blockInstance.transform.localPosition = new Vector3(posX, 0, 0);
        blockInstance.SetMove(_speed);
        blockInstance.InstanceTimingSubject.Subscribe( _ =>
        {
            SetBlock(_initPosX + _offsetPos);

        }).AddTo(this);


    }

}
