using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// ChageAttack時にカメラを操作するスクリプト
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/7
/// </summary>
public class ChageCamera : StateMachineBehaviour
{
    Camera main;
    CameraMove moveCam;
    GameObject player;
    PostProcessingBehaviour postProcessingBehaviour;
    PostProcessingProfile postProcessingProfile;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        main = Camera.main;
        postProcessingBehaviour = main.GetComponent<PostProcessingBehaviour>();
        postProcessingProfile = postProcessingBehaviour.profile;
        postProcessingProfile.motionBlur.enabled = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        main = Camera.main;
        moveCam = main.GetComponent<CameraMove>();
        player = moveCam.Player;

        //減衰比率
        float attenRate = 4f;

        Vector3 pos = player.transform.position + player.transform.rotation * new Vector3(1, 2, -2);
        main.transform.position = Vector3.Lerp(main.transform.position, pos, Time.deltaTime * attenRate); // Lerp減衰
        main.transform.LookAt(player.transform);
        main.transform.localEulerAngles = new Vector3(35, main.transform.localEulerAngles.y, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //PostEffectManagerの代わりにビネットをオフにする
        main = Camera.main;
        postProcessingBehaviour = main.GetComponent<PostProcessingBehaviour>();
        postProcessingProfile = postProcessingBehaviour.profile;
        postProcessingProfile.vignette.enabled = false;
        postProcessingProfile.motionBlur.enabled = false;
    }
}
