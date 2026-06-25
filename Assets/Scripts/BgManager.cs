using UnityEngine;

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

    /// <summary>左端の削除位置</summary>
    [SerializeField] private float _destroyPosX = -30f;

    /// <summary>最初に並べるブロック数</summary>
    [SerializeField] private int _initialBlockCount = 2;

    private float _spawnDistance;

    private void Awake()
    {
        for (int i = 0; i < _initialBlockCount; i++)
        {
            SetBlock(_initPosX + _offsetPos * i);
        }
    }

    private void Update()
    {
        _spawnDistance += _speed * Time.deltaTime;

        while (_spawnDistance >= _offsetPos)
        {
            _spawnDistance -= _offsetPos;
            SetBlock(_initPosX + _offsetPos * (_initialBlockCount - 1));
        }
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
        blockInstance.SetMove(_speed).SetDestroyPosition(_destroyPosX);

        return blockInstance;
    }
}
