using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// Post Effect Stackの情報を動的に変更する
/// 
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 
/// 作成日時:2018/12/7
/// </summary>
public class PostEffectManager : MonoBehaviour
{
    //外部から変更できるように
    PostProcessingBehaviour postProcessingBehaviour;
    PostProcessingProfile postProcessingProfile;

    //ビネットのセッティング
    VignetteModel.Settings vignetteSettings;
    //ブラーのセッティング
    MotionBlurModel.Settings motionBlurSettings;
    //ブルームのセッティング
    BloomModel.Settings bloomSetting;

    #region ビネット

    [Header("Vignetteの設定")]

    [SerializeField]
    [Tooltip("中心にどれだけ近づくか")]
    float vignetteIntensity;
    [SerializeField]
    [Tooltip("色")]
    Color vignetteColor;
    [SerializeField]
    [Tooltip("ぼかし")]
    float smoothness;
    [SerializeField]
    [Tooltip("周回率")]
    float roundness;

    #endregion

    #region ブラー

    [Header("MotionBlurの設定")]

    [SerializeField]
    [Tooltip("シャッター角度")]
    [Range(0, 360)]
    float shutterAngle;
    [SerializeField]
    [Tooltip("ブラーの数")]
    int sampleCount;
    [SerializeField]
    [Tooltip("ブレンドの強さ")]
    [Range(0, 1)]
    float frameBlending;

    #endregion

    #region ブルーム

    [Header("Bloomの設定")]

    [SerializeField]
    [Tooltip("強さ")]
    float bloomIntensity;
    [SerializeField]
    [Tooltip("ガンマ値（0の方が強い）")]
    [Range(0, 1)]
    float thereshold;
    [SerializeField]
    [Tooltip("ぼかし")]
    [Range(0, 1)]
    float softKnee;
    [SerializeField]
    [Tooltip("大きさ")]
    [Range(0, 7)]
    float radius;

    #endregion

    #region フォグ
    [Header("Fogの設定")]

    [SerializeField]
    [Tooltip("霧の色")]
    Color fogColor;
    [SerializeField]
    [Tooltip("深度（霧の濃度）")]
    [Range(0, 1)]
    float fogDensity;

    #endregion

    [Header("その他変数")]

    [SerializeField]
    [Tooltip("HPのキャンバス")]
    HpCanvas hpCanvas;

    //正の計算をするか
    bool calc;

    //ダメージを受けたか？
    public bool Damage { private get; set; }

    // Use this for initialization
    void Awake()
    {
        //大元
        postProcessingBehaviour = transform.GetComponent<PostProcessingBehaviour>();
        //プロファイルを取得
        postProcessingProfile = postProcessingBehaviour.profile;

        #region それぞれのポストエフェクトのセッティングを取得

        //ビネットのセッティングを取得
        vignetteSettings = postProcessingProfile.vignette.settings;
        //ブラーのセッティングを取得
        motionBlurSettings = postProcessingProfile.motionBlur.settings;
        //ブルームのセッティングを取得
        bloomSetting = postProcessingProfile.bloom.settings;

        #endregion

        #region ビネットの初期化

        //ビネットは一回オフに
        postProcessingProfile.vignette.enabled = false;
        //各種設定
        vignetteSettings.center = new Vector2(0.5f, 0.5f);

        #endregion

        #region ブラーの初期化

        //ブラーはオフに
        postProcessingProfile.motionBlur.enabled = false;
        //各種設定
        motionBlurSettings.shutterAngle = shutterAngle;
        motionBlurSettings.sampleCount = sampleCount;
        motionBlurSettings.frameBlending = frameBlending;

        #endregion

        #region ブルームの初期化

        //ブルームはオフに
        postProcessingProfile.bloom.enabled = false;
        //各種設定
        bloomSetting.bloom.intensity = bloomIntensity;
        bloomSetting.bloom.threshold = thereshold;
        bloomSetting.bloom.softKnee = softKnee;
        bloomSetting.bloom.radius = radius;

        #endregion

        #region フォグの設定

        //フォグはオンに
        postProcessingProfile.fog.enabled = true;
        //各種設定
        FogReverse();

        #endregion

        #region 変更した情報を元の場所に戻す

        postProcessingProfile.vignette.settings = vignetteSettings;
        postProcessingProfile.motionBlur.settings = motionBlurSettings;
        postProcessingProfile.bloom.settings = bloomSetting;

        #endregion

        #region その他各種変数の初期化

        if (hpCanvas == null) hpCanvas = GameObject.Find("Hp_Canvas").GetComponent<HpCanvas>();

        calc = true;
        Damage = false;

        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ダメージを受けた時の処理
        if (Damage && calc)
        {
            postProcessingProfile.vignette.enabled = !postProcessingProfile.vignette.enabled;
            vignetteColor = Color.red;
            vignetteIntensity = 0.5f;
            vignetteSettings.color = vignetteColor;
            vignetteSettings.intensity = 0.5f;
            calc = false;
        }

        if (hpCanvas.isLastHP)
        {
            postProcessingProfile.vignette.enabled = true;
            vignetteColor = Color.red;
            vignetteIntensity = 0.5f;
            vignetteSettings.color = vignetteColor;
        }

        if (PlayerAnimInfo.Instance.counterNow)
        {
            postProcessingProfile.vignette.enabled = true;
            vignetteColor = Color.black;
            vignetteIntensity = 0.5f;
            vignetteSettings.color = vignetteColor;
            vignetteSettings.intensity = vignetteIntensity;
        }
        else
        {
            DamageEffect();
        }

        CompetitorBloom();

        //変更した情報を元の場所に戻す
        postProcessingProfile.vignette.settings = vignetteSettings;
        postProcessingProfile.motionBlur.settings = motionBlurSettings;
        postProcessingProfile.bloom.settings = bloomSetting;
        FogReverse();
    }

    #region Vignette

    /// <summary>
    /// Vignetteの変更
    /// </summary>
    void VignetteChange()
    {
        vignetteSettings.color = vignetteColor;
        vignetteSettings.intensity = vignetteIntensity;
        vignetteSettings.smoothness = smoothness;
        vignetteSettings.roundness = roundness;
    }

    /// <summary>
    /// ダメージを受けた時のVignetteの挙動
    /// </summary>
    void DamageEffect()
    {
        if (postProcessingProfile.vignette.enabled)
        {
            if (vignetteSettings.intensity < vignetteIntensity && calc)
            {
                vignetteSettings.intensity += 0.01f;

                if (vignetteSettings.intensity > vignetteIntensity) calc = false;
            }
            else
            {
                vignetteSettings.intensity -= 0.01f;

                if (vignetteSettings.intensity < 0.05)
                {
                    postProcessingProfile.vignette.enabled = false;
                    calc = true;
                    Damage = false;
                    vignetteSettings.intensity = 0;
                }
            }

            if (hpCanvas.isLastHP)
            {
                if (calc)
                {
                    vignetteSettings.intensity += 0.001f;
                    if (vignetteSettings.intensity > vignetteIntensity) calc = false;
                }
                else
                {
                    vignetteSettings.intensity -= 0.001f;
                    if (vignetteSettings.intensity < 0.05)
                    {
                        calc = true;
                    }
                }
            }
        }
    }

    #endregion

    #region Fog

    /// <summary>
    /// Fogのデータを元の場所に
    /// </summary>
    void FogReverse()
    {
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    /// <summary>
    /// Fogのデータを初期化
    /// </summary>
    public void FogInit()
    {
        postProcessingProfile.fog.enabled = true;
        RenderSettings.fogColor = new Color(128, 128, 128);
        RenderSettings.fogDensity = 0.1f;
    }

    /// <summary>
    /// Fogのデータの変更
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="color"></param>
    /// <param name="density"></param>
    public void FogDataChange(bool enable, Color color, float density)
    {
        postProcessingProfile.fog.enabled = enable;
        fogColor = color;
        fogDensity = density;

        FogReverse();
    }

    #endregion

    #region Bloom

    void CompetitorBloom()
    {
        //鍔迫り合い中
        if (PlayerAnimInfo.Instance.competitor)
        {
            //まだ0でないなら
            if (bloomSetting.bloom.intensity > 0)
            {
                postProcessingProfile.bloom.enabled = true;
                bloomSetting.bloom.intensity -= 0.1f;
            }
            else
            {
                postProcessingProfile.bloom.enabled = false;
            }
        }
        else
        {
            bloomSetting.bloom.intensity = bloomIntensity;
        }
    }

    #endregion

}