using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*---------------------------------
 * 
 * プレイヤーに追従する、メインカメラを制御するクラス。
 * カメラにつける。
 * プレイヤーの追従と、プレイヤーの状態に応じて視野角を変更するだけ。
 * 
 ---------------------------------*/
public class PlayerCameraController : MonoBehaviour
{

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public Vector3 relativePosition;
    public Vector3 rotation;
    public GameObject player;
    public float move_SmoothTime = 0.13f;

    //--------------------------------------
    //   変数
    //--------------------------------------
    Camera playerCamera;

    //--------------------------------------
    //   初期化処理
    //--------------------------------------
    //角度調整と、コンポーネントの取得
    void Start () {
        this.transform.rotation = Quaternion.Euler( rotation ); //このゲームでは、基本的にカメラの角度は変わらない。
        playerCamera = GetComponent<Camera>();
	}


    //--------------------------------------
    //   更新処理(可変)
    //--------------------------------------
    float mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease;
    const float MAX_mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease = 0.4f;
    const float MIN_Filed_ofView = 60.0f;
    const float MAX_Filed_ofView = 75.0f;
    const float START_mouseButton_Count_Time = 0.1f;
    //攻撃準備中は視野角を広げる処理。START_mouseButton_Count_Time秒たってから視野角が変わり始める。戻るときは、視野角が戻ってからSTARTのカウント分がリセットされるため、実質関係ない
    void Update()
    {
        if (Input.GetMouseButton(0))    //クリックが押されてる＝攻撃チャージ中
        {
            mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease += Time.deltaTime;
        }
        else {
            mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease -= Time.deltaTime*10.0f;//戻るときの方が速く。定数はてきとう
        }
        mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease = 
            Mathf.Clamp
            (
                mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease,
                0.0f,
                MAX_mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease + START_mouseButton_Count_Time 
            );

        float now_count = mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease - START_mouseButton_Count_Time;
        now_count = Mathf.Max( now_count, 0.0f );
        float mouseButton_Count_Rate = now_count / MAX_mouseButton_Count_WhilePushedIncrease_WhileReleasedDecrease;

        playerCamera.fieldOfView = Mathf.Lerp( MIN_Filed_ofView, MAX_Filed_ofView, mouseButton_Count_Rate );    //視野角を変更
    }


    //--------------------------------------
    //   更新処理(固定)
    //--------------------------------------
    // 固定フレームアップデート
    Vector3 velocity_forMove;
    //プレイヤーの追従処理。常に完璧にくっついて回るのではなく、若干の遅延をしながら近づく
    void FixedUpdate () {
        Vector3 targetPosition = player.transform.position + relativePosition;
        this.transform.position = Vector3.SmoothDamp( this.transform.position, targetPosition, ref velocity_forMove, move_SmoothTime );
	}
}
