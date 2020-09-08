using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// ポストエフェクトをリセットするスクリプト
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/14
/// </summary>
public class ResetPostEffect : StateMachineBehaviour
{
    Camera main;
    PostProcessingBehaviour postProcessingBehaviour;
    PostProcessingProfile postProcessingProfile;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //PostEffectManagerの代わりにビネットをオフにする
        main = Camera.main;
        postProcessingBehaviour = main.GetComponent<PostProcessingBehaviour>();
        postProcessingProfile = postProcessingBehaviour.profile;
        postProcessingProfile.vignette.enabled = false;
    }
}
