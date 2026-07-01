using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //ゲーム進行を管理するクラス

    private enum GameState
    {       
        BeforeStart,//ゲーム開始前の待機時間
        GameStart,//ゲーム開始
        GamePlay,//ゲーム中       
        GameEnd,//ゲーム終了       
        Result//リザルト
    }

    private GameState _gameState = GameState.BeforeStart; 

    //public Subject<Unit> Default = new Subject<Unit>();
    
    private void Awake()
    {

    }
    /// <summary>
    /// ステート
    /// </summary>
    private void SetThisState(GameState thisState)
    {
        _gameState = thisState;

        switch (_gameState)
        {
            case GameState.BeforeStart:         
                
                break;
           
        }
    }

}
