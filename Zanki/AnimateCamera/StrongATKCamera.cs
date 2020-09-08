using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// StrongAttack時にカメラを操作するスクリプト
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/7
/// </summary>
public class StrongATKCamera : StateMachineBehaviour
{
    Camera main;
    CameraMove moveCam;
    GameObject player;
    PostProcessingBehaviour postProcessingBehaviour;
    PostProcessingProfile postProcessingProfile;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        main = Camera.main;
        moveCam = main.GetComponent<CameraMove>();
        player = moveCam.Player;

        //減衰比率
        float attenRate = 3f;

        Vector3 pos = player.transform.position + player.transform.rotation * new Vector3(-0.5f, 2, -1);
        main.transform.position = Vector3.Lerp(main.transform.position, pos, Time.deltaTime * attenRate); // Lerp減衰
        //main.transform.LookAt(player.transform);
        Vector3 local = new Vector3(15, main.transform.localEulerAngles.y, 0);
         Vector3.Lerp(main.transform.localEulerAngles, local, Time.deltaTime * 2); // Lerp減衰
    }
}
