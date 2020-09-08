using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラの動きを司るスクリプト
/// Cameraにアタッチ
/// 
/// 作成者:田中颯馬
/// 編集者:田中颯馬
/// 作成日時:2018/11/5
/// </summary>
public class CameraMove : MonoBehaviour
{
    //間に入ったオブジェクトのメッシュコライダー
    MeshCollider meshCollider;

    //プレイヤーとの距離
    float playerDistance;

    float count;

    //判定用Ray
    Ray ray;

    //格納用MeshRenderer
    MeshRenderer hitMesh;

    //当たり判定情報
    RaycastHit tempHit;

    //Rayが当たったオブジェクトの情報を得る
    RaycastHit hitObject;

    [SerializeField]
    [Tooltip("判定の対象になるレイヤー番号")]
    int mask;

    [SerializeField]
    [Tooltip("PlayerObject")]
    private GameObject player;

    /// <summary>
    /// 外部参照用プロパティ
    /// </summary>
    public GameObject Player { get { return player; } }

    [SerializeField]
    [Tooltip("プレイヤーに対する相対位置")]
    Vector3 localPosition = new Vector3(0, 3f, -3);

    [SerializeField]
    [Tooltip("カメラの首振り角度")]
    Vector3 rotation = new Vector3(35, 0, 0);

    [SerializeField]
    [Tooltip("減衰比率")]
    float attenRate = 2.0f;

    [SerializeField]
    [Tooltip("画角")]
    [Range(0, 360)]
    float view = 75f;

    // Use this for initialization
    void Awake()
    {
        count = 0;

        transform.position = player.transform.position + player.transform.rotation * localPosition;
        transform.rotation = player.transform.rotation;
        transform.localEulerAngles += rotation;

        //プレイヤーとの距離を取る
        playerDistance = Vector3.Distance(player.transform.position, transform.position);

        //Rayの生成
        ray = new Ray(transform.position, player.transform.position);

        GetComponent<Camera>().fieldOfView = view;
    }

    // Update is called once per frame
    void Update()
    {
        //発射地点と方向の更新
        ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));

        //プレイヤーとカメラの間のオブジェクトでカメラとの距離が一定以下の場合に透過させる
        if (Physics.Raycast(ray.origin, ray.direction, out hitObject, playerDistance, LayerMask.GetMask("Default")))
        {
            MeshRenderer meshRenderer = hitObject.collider.GetComponent<MeshRenderer>();
            float objectDistance = hitObject.distance;

            //半透明で抑える
            float alpha = (0.5f <= (objectDistance / playerDistance)) ? (objectDistance / playerDistance) : 0.5f;

            if (meshRenderer != null) meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, alpha);

            if (hitMesh != meshRenderer && hitMesh != null)
            {
                //二つ以上のオブジェクトがカメラとプレイヤーの間に入ったときに半透明のままのオブジェクトが出ないようにする対策
                hitMesh.material.color = new Color(hitMesh.material.color.r, hitMesh.material.color.g, hitMesh.material.color.b, 1.0f);
            }

            hitMesh = meshRenderer;
        }
        else
        {
            if (hitMesh != null) hitMesh.material.color = new Color(hitMesh.material.color.r, hitMesh.material.color.g, hitMesh.material.color.b, 1.0f);
        }

        //カウンター中と鍔迫り合い中は専用の動きをするので、ここで返す
        if (PlayerAnimInfo.Instance.counterNow || PlayerAnimInfo.Instance.competitor) return;

        TrackingMove();
    }

    /// <summary>
    /// プレイヤーの周囲を動くカメラと補間の動き
    /// </summary>
    void TrackingMove()
    {

        AroundCamera();

        //メモ
        //クォータニオンは便利だぞ
        //"*"演算子はQuaternion*Vectorの順

        if (Physics.Linecast(player.transform.position, transform.position, out tempHit, LayerMask.GetMask("Ground")))
        {
            //ぶつかったオブジェクトを避けるように補間
            transform.position = Vector3.Lerp(tempHit.point, transform.position, Time.deltaTime * attenRate);
        }

        //最終的な位置を計算
        transform.position = new Vector3(transform.position.x, localPosition.y, transform.position.z);
    }

    /// <summary>
    /// 敵の方向を向くカメラの動き
    /// </summary>
    void AroundCamera()
    {
        //半径
        float dir = 3.0f;
        //敵の向きを取得
        Transform targetTrans = Actorinfo.Instance.GetTarget;
        Vector3 nomal = new Vector3();
        if (targetTrans != null)
        {
            nomal = (targetTrans.position - player.transform.position).normalized;
        }
        //敵の方向と反対方向にカメラを配置
        Vector3 pos = player.transform.position + (nomal * -1 * dir) + new Vector3(0, 3, 0);
        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * attenRate); // Lerp減衰

        if (targetTrans != null)
        {
            //敵の方向を向く
            transform.rotation = EnemyUtility.LookAt(transform, targetTrans.position, 3.0f);
            transform.localEulerAngles = new Vector3(rotation.x, transform.localEulerAngles.y, 0);
        }
    }


    public void CompetitorCam()
    {
        Vector3 targetTrans = (Actorinfo.Instance.GetTarget.position + player.transform.position) / 2;
        if (targetTrans != null)
        {
            //敵の方向を向く
            transform.LookAt(targetTrans);
            transform.localEulerAngles = new Vector3(-10, transform.localEulerAngles.y, 0);
        }
        //指定位置に一気に移動
        transform.position = player.transform.position + player.transform.rotation * new Vector3(1.25f, 1.5f, 1f);

        if (count < 1)
        {
            count++;
        }
        else
        {

            transform.position = transform.position + new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
            count = 0;

        }
    }
}