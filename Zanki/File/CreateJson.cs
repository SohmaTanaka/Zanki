using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jsonの中身
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/21
/// </summary>
[SerializeField]
public class CreateJson
{
    [SerializeField]
    private static string filePath = Application.dataPath + "/StreamingAssets/SaveFile/AudioSettings.json";     //セーブデータのファイルパス

    public static string FilePath
    {
        //ファイルパスのプロパティ
        get { return filePath; }
    }

    [SerializeField]
    public float master = 100;
    [SerializeField]
    public float bgm = 100;
    [SerializeField]
    public float se = 100;

}
