using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ※継承禁止
/// Inputの管理
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2018/9/20
/// </summary>
public sealed class InputManager : MonoBehaviour
{
    //インスタンス用フィールド変数
    static InputManager inputManager;

    #region プロパティ

    /// <summary>
    /// キーコンフィグの設定を収納するDictionaryプロパティ
    /// </summary>
    public Dictionary<InputCode, KeyCode> CodeDictionary { get; private set; }

    /// <summary>
    /// ゲームパッドのボタンの名前を収納するDictionaryプロパティ
    /// </summary>
    public Dictionary<ButtonList, KeyCode> ButtonNameDictionary { get; private set; }

    /// <summary>
    /// キーコンフィグ（Trigger）の設定を収納するDictionaryプロパティ
    /// </summary>
    public Dictionary<InputCode, TriggerAsButton> CodeTriggerDictionary { get; private set; }

    /// <summary>
    /// ゲームパッドのTriggerの名前を収納するDictionaryプロパティ
    /// </summary>
    public Dictionary<ButtonList, TriggerAsButton> TriggerNameDictionary { get; private set; }

    /// <summary>
    /// 再処理までのカウントをするタイマー
    /// </summary>
    float RestartTimer { get; set; }

    /// <summary>
    /// RT用
    /// </summary>
    TriggerAsButton RightTrigger { get; set; }

    /// <summary>
    /// LT用
    /// </summary>
    TriggerAsButton LeftTrigger { get; set; }

    #region Axis処理

    /// <summary>
    /// 水平方向の入力の大きさ取得
    /// </summary>
    /// <returns>水平方向の入力の大きさ</returns>
    public float GetAxisHorizontal
    {
        get { return Input.GetAxis("Horizontal"); }
    }

    /// <summary>
    /// 垂直方向の入力の大きさ取得
    /// </summary>
    /// <returns>垂直方向の入力の大きさ</returns>
    public float GetAxisVertical
    {
        get { return Input.GetAxis("Vertical"); }
    }

    /// <summary>
    /// 右スティック水平方向の入力の大きさ取得
    /// </summary>
    /// <returns>右スティック水平方向の入力の大きさ</returns>
    public float GetAxisRightHorizontal
    {
        get { return Input.GetAxis("RightStickHorizontal"); }
    }

    /// <summary>
    /// 右スティック垂直方向の入力の大きさ取得
    /// </summary>
    /// <returns>右スティック垂直方向の入力の大きさ</returns>
    public float GetAxisRightVertical
    {
        get { return Input.GetAxis("RightStickVertical"); }
    }

    #endregion

    #endregion

    /// <summary>
    /// インスタンス取得
    /// </summary>
    /// <returns>実体</returns>
    public static InputManager GetInstance
    {
        get
        {
            if (inputManager == null)
            {
                GameObject gameObject = new GameObject("InputManagerGO");
                gameObject.AddComponent<InputManager>();
                DontDestroyOnLoad(gameObject);
                return inputManager = gameObject.GetComponent<InputManager>();
            }
            else { return inputManager; }
        }
    }

    /// <summary>
    /// 生成直後に一回だけ実行
    /// </summary>
    void Awake()
    {
        //初期化
        RightTrigger = new TriggerAsButton(ButtonList.RightTrigger);
        LeftTrigger = new TriggerAsButton(ButtonList.LeftTrigger);
        CodeDictionary = new Dictionary<InputCode, KeyCode>();
        ButtonNameDictionary = new Dictionary<ButtonList, KeyCode>();
        CodeTriggerDictionary = new Dictionary<InputCode, TriggerAsButton>();
        TriggerNameDictionary = new Dictionary<ButtonList, TriggerAsButton>();

        RestartTimer = 0.0f;

        SetButtonName();
        SetCode();
    }

    #region Awakeの中身

    /// <summary>
    /// ※Awakeで呼ぶこと
    /// ※事故防止のためfor文などでは回していない
    /// 
    /// ButtonListとKeyCodeを結びつけるメソッド
    /// </summary>
    void SetButtonName()
    {
        //WindowsOS用
        //Button
        ButtonNameDictionary.Add(ButtonList.GamePad_A, KeyCode.Joystick1Button0);   //Aボタン
        ButtonNameDictionary.Add(ButtonList.GamePad_B, KeyCode.Joystick1Button1);   //Bボタン
        ButtonNameDictionary.Add(ButtonList.GamePad_X, KeyCode.Joystick1Button2);   //Xボタン
        ButtonNameDictionary.Add(ButtonList.GamePad_Y, KeyCode.Joystick1Button3);   //Yボタン
        ButtonNameDictionary.Add(ButtonList.GamePad_L, KeyCode.Joystick1Button4);   //Lボタン
        ButtonNameDictionary.Add(ButtonList.GamePad_R, KeyCode.Joystick1Button5);   //Rボタン
        ButtonNameDictionary.Add(ButtonList.GamePad_Back, KeyCode.Joystick1Button6);   //Backボタン
        ButtonNameDictionary.Add(ButtonList.GamePad_Start, KeyCode.Joystick1Button7);   //Startボタン
        //Trigger
        TriggerNameDictionary.Add(ButtonList.LeftTrigger, LeftTrigger);         //LT
        TriggerNameDictionary.Add(ButtonList.RightTrigger, RightTrigger);   //RT
    }

    /// <summary>
    /// ※Awakeで呼ぶこと
    /// ※事故防止のためfor文などでは回していない
    /// 
    /// InputCodeとKeyCodeを結びつけるメソッド
    /// </summary>
    void SetCode()
    {
        //デフォルトのキー設定
        CodeDictionary.Add(InputCode.Guard, ButtonNameDictionary[ButtonList.GamePad_R]);    //ガード
        CodeDictionary.Add(InputCode.Start, ButtonNameDictionary[ButtonList.GamePad_Start]);    //スタート
        CodeDictionary.Add(InputCode.Back, ButtonNameDictionary[ButtonList.GamePad_Back]);    //バック
        CodeDictionary.Add(InputCode.Attack, ButtonNameDictionary[ButtonList.GamePad_X]);   //抜刀/攻撃
        CodeDictionary.Add(InputCode.PaidSword, ButtonNameDictionary[ButtonList.GamePad_Y]);  //納刀
        CodeDictionary.Add(InputCode.Dodge, ButtonNameDictionary[ButtonList.GamePad_A]);   //回避
        CodeDictionary.Add(InputCode.Decision, ButtonNameDictionary[ButtonList.GamePad_B]); //決定
    }

    #endregion

    #region キーの状態取得

    /// <summary>
    /// キーを押してない状態から押したときtrue
    /// </summary>
    /// <param name="inputCode">操作の名前</param>
    /// <returns>キーの状態</returns>
    public bool GetKeyDown(InputCode inputCode)
    {
        if (!CodeDictionary.ContainsKey(inputCode))
        {
            return CodeTriggerDictionary[inputCode].IsTriggerDown;
        }
        return Input.GetKeyDown(CodeDictionary[inputCode]);
    }

    /// <summary>
    /// キーを押し続けている時true
    /// </summary>
    /// <param name="inputCode">操作の名前</param>
    /// <returns>キーの状態</returns>
    public bool GetKey(InputCode inputCode)
    {
        if (!CodeDictionary.ContainsKey(inputCode))
        {
            return CodeTriggerDictionary[inputCode].GetTriggerState;
        }

        return Input.GetKey(CodeDictionary[inputCode]);
    }

    /// <summary>
    /// キーを押している状態から離した時true
    /// </summary>
    /// <param name="inputCode">操作の名前</param>
    /// <returns>キーの状態</returns>
    public bool GetKeyUp(InputCode inputCode)
    {
        if (!CodeDictionary.ContainsKey(inputCode))
        {
            return CodeTriggerDictionary[inputCode].IsTriggerUp;
        }

        return Input.GetKeyUp(CodeDictionary[inputCode]);
    }

    #endregion

    /// <summary>
    /// キーの設定
    /// </summary>
    /// <param name="inputCord">操作の名前</param>
    /// <param name="key">キーコード</param>
    public void SetButton(InputCode inputCode, KeyCode key)
    {
        if (CodeTriggerDictionary.ContainsKey(inputCode))
        {
            CodeTriggerDictionary.Remove(inputCode);
            CodeDictionary.Add(inputCode, key);
        }
        else
        {
            CodeDictionary[inputCode] = key;
        }
    }

    /// <summary>
    /// キーの設定
    /// </summary>
    /// <param name="inputCord">操作の名前</param>
    /// <param name="trigger">キーコード</param>
    public void SetTrigger(InputCode inputCode, TriggerAsButton trigger)
    {
        if (CodeDictionary.ContainsKey(inputCode))
        {
            CodeDictionary.Remove(inputCode);
            CodeTriggerDictionary.Add(inputCode, trigger);
        }
        else
        {
            CodeTriggerDictionary[inputCode] = trigger;
        }
    }

    /// <summary>
    /// KeyCodeを返す
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public KeyCode GetButtonCode(ButtonList button)
    {
        return ButtonNameDictionary[button];
    }

    public bool GetKeyCodeButtonDown(ButtonList button)
    {
        return Input.GetKeyDown(GetButtonCode(button));
    }

    /// <summary>
    /// キーを押し続けている時true
    /// </summary>
    /// <param name="inputCode">操作の名前</param>
    /// <returns>キーの状態</returns>
    public bool GetKeyCodeButton(ButtonList button)
    {
        return Input.GetKey(GetButtonCode(button));
    }

    /// <summary>
    /// キーを押している状態から離した時true
    /// </summary>
    /// <param name="inputCode">操作の名前</param>
    /// <returns>キーの状態</returns>
    public bool GetKeyCodeButtonUp(ButtonList button)
    {
        return Input.GetKeyUp(GetButtonCode(button));
    }
}
