using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggerをボタンと同じように使用するためのクラス
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2018/10/23
/// </summary>
public class TriggerAsButton : MonoBehaviour
{

    TriggerStruct triggerStruct;    //Axisの値を利用できる構造体
    float previousTrigger;             //前のフレームのTriggerの状態
    float currentTrigger;               //現在のフレームのTriggerの状態

    public ButtonList GetButton { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// 
    /// ※Awakeで必ず呼ぶこと！！
    /// </summary>
    /// <param name="trigger"></param>
    public TriggerAsButton(ButtonList trigger)
    {
        triggerStruct = new TriggerStruct(trigger);
    }

    /// <summary>
    /// 前のフレームに押されておらず、現在のフレームで押されているか？
    /// </summary>
    /// <returns></returns>
    public bool IsTriggerDown
    {
        get { return (previousTrigger == 0) && (currentTrigger == 1); }
    }

    /// <summary>
    /// ボタンは押されていないか
    /// </summary>
    /// <returns></returns>
    public bool IsTriggerUp
    {
        get { return currentTrigger == 0; }
    }

    /// <summary>
    /// ボタンが押されているか？
    /// </summary>
    public bool GetTriggerState
    {
        get { return currentTrigger == 1; }
    }

    /// <summary>
    /// Triggerの状態の更新処理
    /// </summary>
    void Update()
    {
        previousTrigger = currentTrigger;
        currentTrigger = triggerStruct.GetAxis();
    }
}
