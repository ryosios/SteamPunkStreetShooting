using UnityEngine;
using System.Collections.Generic;

public class BgManager : MonoBehaviour
{
    /// <summary>BGBlockのインスタンス</summary>
    [SerializeField] private BgBlock[] _blockBgBlock;

    /// <summary>Attach</summary>
    [SerializeField] private Transform _attachTrans;

    /// <summary>初期位置</summary>
    [SerializeField] private float _initPosX = 0f;

    /// <summary>スピード</summary>
    [SerializeField] private float _speed = 10f;

    /// <summary>位置のオフセット</summary>
    [SerializeField] private float _offsetPos = 30f;

    /// <summary>ブロック同士を少し重ねて隙間を消す量</summary>
    [SerializeField] private float _blockOverlap = 0.15f;

    /// <summary>速度変更時の一時的な隙間を防ぐため、早めに次ブロックを出す距離</summary>
    [SerializeField] private float _spawnLeadDistance = 1f;

    /// <summary>最初に並べるブロック数</summary>
    [SerializeField] private int _initialBlockCount = 2;

    /// <summary>現在スクロール中のBGBlock一覧</summary>
    private readonly List<BgBlock> _activeBlocks = new List<BgBlock>();

    /// <summary>実際に次のブロックを置く間隔</summary>
    private float BlockInterval => Mathf.Max(0.01f, _offsetPos - _blockOverlap);

    /// <summary>BGBlockの初期生成が完了しているか</summary>
    private bool _isInitialized;

    private void Awake()
    {
        for (int i = 0; i < _initialBlockCount; i++)
        {
            SetBlock(_initPosX + BlockInterval * i);
        }

        _isInitialized = true;
    }

    private void Update()
    {
        FillLoopBlocks();
    }

    /// <summary>
    /// 画面右側に隙間が出ないように、必要なBGBlockを補充する。
    /// </summary>
    private void FillLoopBlocks()
    {
        _activeBlocks.RemoveAll(block => block == null);

        if (_activeBlocks.Count == 0)
        {
            SetBlock(_initPosX);
        }

        float rightmostPosX = GetRightmostBlockPosX();
        float spawnThresholdX = _initPosX + BlockInterval * (_initialBlockCount - 2) + _spawnLeadDistance;

        while (_activeBlocks.Count < _initialBlockCount || rightmostPosX <= spawnThresholdX)
        {
            if (_activeBlocks.Count >= _initialBlockCount)
            {
                RemoveLeftmostBlock();
            }

            BgBlock block = SetBlock(rightmostPosX + BlockInterval);
            if (block == null)
            {
                break;
            }

            rightmostPosX = block.LocalPositionX;
        }
    }

    /// <summary>
    /// BGのスピードを取得する。
    /// </summary>
    public float GetSpeed()
    {
        return _speed; 
    }

    /// <summary>
    /// BGのスクロール速度を変更する。
    /// 生成済みのBGBlockにも即時反映され、以後に生成されるBGBlockにも同じ速度が使われる。
    /// </summary>
    /// <param name="speed">新しいスクロール速度。正の値を入れると-X方向へ流れる。</param>
    public void SetSpeed(float speed)
    {
        _speed = Mathf.Max(0f, speed);
        ApplySpeedToActiveBlocks();

        if (_isInitialized)
        {
            FillLoopBlocks();
        }
    }

    /// <summary>
    /// 現在のBGスクロール速度に加算する。
    /// ステージ進行に合わせた加速などに使う。
    /// </summary>
    /// <param name="addSpeed">加算する速度。負の値で減速できる。</param>
    public void AddSpeed(float addSpeed)
    {
        SetSpeed(_speed + addSpeed);
    }

    /// <summary>
    /// BGBlockを生成
    /// </summary>
    private BgBlock SetBlock(float posX)
    {
        if (_blockBgBlock == null || _blockBgBlock.Length == 0)
        {
            Debug.LogWarning("BGBlock prefab is not assigned.");
            return null;
        }

        int selectIndex = Random.Range(0, _blockBgBlock.Length);
        var blockInstance = Instantiate(_blockBgBlock[selectIndex], _attachTrans);
        blockInstance.gameObject.SetActive(true);
        blockInstance.transform.localScale = Vector3.one;
        blockInstance.transform.localPosition = new Vector3(posX, 0, 0);
        blockInstance.SetMove(_speed);
        _activeBlocks.Add(blockInstance);

        return blockInstance;
    }

    /// <summary>
    /// 一番左にあるBGBlockを非表示にして削除予約する。
    /// 新しいBGBlockを出す前に消すことで、一瞬だけ見える3個目を防ぐ。
    /// </summary>
    private void RemoveLeftmostBlock()
    {
        BgBlock leftmostBlock = GetLeftmostBlock();
        if (leftmostBlock == null)
        {
            return;
        }

        _activeBlocks.Remove(leftmostBlock);
        leftmostBlock.gameObject.SetActive(false);
        Destroy(leftmostBlock.gameObject);
    }

    /// <summary>
    /// 生成済みBGBlockすべてに現在の速度を反映する。
    /// </summary>
    private void ApplySpeedToActiveBlocks()
    {
        _activeBlocks.RemoveAll(block => block == null);

        foreach (BgBlock block in _activeBlocks)
        {
            block.SetMove(_speed);
        }
    }

    /// <summary>
    /// 生成済みBGBlockのうち、一番右にあるブロックのX座標を取得する。
    /// </summary>
    private float GetRightmostBlockPosX()
    {
        float rightmostPosX = _activeBlocks[0].LocalPositionX;

        for (int i = 1; i < _activeBlocks.Count; i++)
        {
            if (_activeBlocks[i].LocalPositionX > rightmostPosX)
            {
                rightmostPosX = _activeBlocks[i].LocalPositionX;
            }
        }

        return rightmostPosX;
    }

    /// <summary>
    /// 生成済みBGBlockのうち、一番左にあるブロックを取得する。
    /// </summary>
    private BgBlock GetLeftmostBlock()
    {
        if (_activeBlocks.Count == 0)
        {
            return null;
        }

        BgBlock leftmostBlock = _activeBlocks[0];

        for (int i = 1; i < _activeBlocks.Count; i++)
        {
            if (_activeBlocks[i].LocalPositionX < leftmostBlock.LocalPositionX)
            {
                leftmostBlock = _activeBlocks[i];
            }
        }

        return leftmostBlock;
    }
}
