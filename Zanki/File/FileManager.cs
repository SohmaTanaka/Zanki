using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// ファイルのマネージャー
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/21
/// </summary>
public class FileManager : MonoBehaviour
{
    public CreateJson Json { get; private set; }

    public float master;
    public float bgm;
    public float se;

    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private AudioMixerGroup masterMixer;
    [SerializeField]
    private AudioMixerGroup bgmMixer;
    [SerializeField]
    private AudioMixerGroup seMixer;

    // Use this for initialization
    void Start()
    {

        DontDestroyOnLoad(gameObject);
        Json = LoadFromJson(CreateJson.FilePath);

        master = Json.master;
        bgm = Json.bgm;
        se = Json.se;

        mixer.SetFloat(masterMixer.ToString(), master);
        mixer.SetFloat(bgmMixer.ToString(), bgm);
        mixer.SetFloat(seMixer.ToString(), se);
    }

    void Update()
    {
        mixer.GetFloat(masterMixer.ToString(), out master);
        mixer.GetFloat(bgmMixer.ToString(), out bgm);
        mixer.GetFloat(seMixer.ToString(), out se);

        if (Json == null)
        {
            Json = new CreateJson();
            return;
        }

        if (Json.master != master || Json.bgm != bgm || Json.se != se)
        {
            //エディタ上で変更したらデータを書き換える.
            Json.master = master;
            Json.bgm = bgm;
            Json.se = se;
        }
    }

    private void OnApplicationQuit()
    {
        SaveToJson(CreateJson.FilePath, Json);
    }

    /// <summary>
    /// ファイル書き込み
    /// </summary>
    /// <param name="filePath">ファイルのある場所</param>
    public static void SaveToJson(string filePath, CreateJson data)
    {
        if (data == null)
        {
            data = new CreateJson();
        }

        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(JsonUtility.ToJson(data));
            }
        }
    }

    /// <summary>
    /// ファイル読み込みする
    /// </summary>
    /// <param name="filePath">ファイルのある場所</param>
    /// <returns></returns>
    public static CreateJson LoadFromJson(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                //FromJson<CreateJson>の型指定重要
                CreateJson sd = JsonUtility.FromJson<CreateJson>(sr.ReadToEnd());


                if (sd == null) return new CreateJson();

                return sd;
            }
        }
    }
}
