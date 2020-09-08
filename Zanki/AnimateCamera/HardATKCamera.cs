using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// HardAttack時にカメラを操作するスクリプト
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/7
/// </summary>
public class HardATKCamera : StateMachineBehaviour
{
    Camera main;
    CameraMove moveCam;
    GameObject player;
    PostProcessingBehaviour postProcessingBehaviour;
    PostProcessingProfile postProcessingProfile;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        main = Camera.main;
        moveCam = main.GetComponent<CameraMove>();
        player = moveCam.Player;

        //減衰比率
        float attenRate =2f;

        Vector3 pos = player.transform.position + player.transform.rotation * new Vector3(0, 3, -1f);
        main.transform.position = Vector3.Lerp(main.transform.position, pos, Time.deltaTime * attenRate); // Lerp減衰
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        main = Camera.main;
        moveCam = main.GetComponent<CameraMove>();
        player = moveCam.Player;

        //減衰比率
        float attenRate = 2f;

        Vector3 pos = player.transform.position + player.transform.rotation * new Vector3(0, 3, -1f);
        main.transform.position = Vector3.Lerp(main.transform.position, pos, Time.deltaTime * attenRate); // Lerp減衰
    }
}
