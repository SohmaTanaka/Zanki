using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// Dodge時にカメラを操作するスクリプト
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2019/1/7
/// </summary>
public class DodgeCamera : StateMachineBehaviour
{
    Camera main;
    CameraMove moveCam;
    GameObject player;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Round();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Round();
    }

    void Round()
    {
        main = Camera.main;
        moveCam = main.GetComponent<CameraMove>();
        player = moveCam.Player;

        //半径
        float dir = 3.0f;

        //減衰比率
        float attenRate = 3f;

        //敵の向きを取得
        Transform targetTrans = Actorinfo.Instance.GetTarget;
        Vector3 nomal = new Vector3();

        if (targetTrans != null)
        {
            nomal = (targetTrans.position - player.transform.position).normalized;
        }

        //敵の方向と反対方向にカメラを配置
        Vector3 pos = player.transform.position + (nomal * -1 * dir) + new Vector3(0, 3, 0);
        main.transform.position = Vector3.Lerp(main.transform.position, pos, Time.deltaTime * attenRate); // Lerp減衰

        if (targetTrans != null)
        {
            //敵の方向を向く
            main.transform.rotation = EnemyUtility.LookAt(main.transform, targetTrans.position, 3.0f);
            main.transform.localEulerAngles = new Vector3(35, main.transform.localEulerAngles.y, 0);
        }
    }
}
