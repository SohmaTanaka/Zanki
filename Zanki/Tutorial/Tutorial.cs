using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// チュートリアルシーン用Script
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/25
/// </summary>
public class Tutorial : MonoBehaviour
{
    [SerializeField, Tooltip("次に移行するシーン名")]
    string sceneName;

    VideoPlayer videoPlayer;
    float videoLength;
    float finTimer;

    // Use this for initialization
    void Start()
    {
        ResultSingleton.Instance.SceneName = sceneName;
        //VideoPlayerコンポーネントを取得
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.frame = 0;
        //ビデオクリップの長さ取得
        videoLength = (float)videoPlayer.clip.length;
    }

    // Update is called once per frame
    void Update()
    {
        //ビデオの再生時間を過ぎたら
        if (videoLength < (finTimer * Time.deltaTime) || InputManager.GetInstance.GetKeyDown(InputCode.Decision))
        {
            FadeSceneChanger.Instance.LoadScene(sceneName);
        }

        finTimer++;
    }
}
