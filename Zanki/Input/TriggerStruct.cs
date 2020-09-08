using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Axisの値を取るための構造体
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2018/10/23
/// </summary>
public struct TriggerStruct
{
    ButtonList trigger;
    float axis;

    /// <summary>
    /// コンストラクタ 
    /// </summary>
    /// <param name="trigger">※RightTriggerかLeftTriggerを入れること！！</param>
    public TriggerStruct(ButtonList trigger)
    {
        this.trigger = trigger;
        axis = 0;
    }

    /// <summary>
    /// Axisの値を取得し、返すメソッド
    /// 
    /// ※RightTriggerかLeftTrigger出なければ判定できない
    /// </summary>
    /// <returns>axis(0か1)</returns>
    public float GetAxis()
    {
        switch (trigger)
        {
            case ButtonList.RightTrigger:
                axis = Input.GetAxis("RightTriggerAxis");
                break;
            case ButtonList.LeftTrigger:
                axis = Input.GetAxis("LeftTriggerAxis");
                break;
        }

        return axis;
    }
}
