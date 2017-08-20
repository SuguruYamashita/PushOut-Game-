using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*-------------------------------
 * 
 *  継承して、各種敵の行動パターンの基盤を作る。
 *  そのための便利関数群みたいなもの。
 *  これを継承して作ったクラスのActiveを切り替えるような管理クラスを用意すれば、複数の行動パターンを混ぜ合わせることも可能。
 *  移動命令は、種類が違っても重複せず、最後に命令されたもののみ実行される。
 * 
 -------------------------------*/
public class Enemy_ActionPattern : SwitchActivation
{
    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public float moveSpeed;
    public float ARRIVAL_DISTANCE_LENGTH = 0.3f;    //移動時、到着したと判断する距離。当たり判定が大きくてプレイヤーに触れている時に、ずががががと変な動きしてたらここの値を大きくする

    //--------------------------------------
    //   変数
    //--------------------------------------
    GameObject  player;
    Vector3     targetPosition;
    bool        isThereTargetPosition;
    Vector3     moveDirection;

    //--------------------------------------
    //   情報更新
    //--------------------------------------
    //指定されたPositionまで向かい続ける。到着すると止まる
    protected void Set_MoveTargetPosition( Vector3 Position )
    {
        isThereTargetPosition = true;
        targetPosition = Position;
    }
    //指定された方向へ動き続ける。
    protected void Set_MoveDirection( Vector3 Direction )
    {
        isThereTargetPosition = false;  //他の移動は全て無効に
        Direction.y = 0.0f; //上下には動かさない
        moveDirection = Direction;
    }
    //移動をやめる（すでに到着した座標を与える事で無理やり止めてる（楽なので））
    protected void Stop_Move() {
        Set_MoveTargetPosition(transform.position);
    }
    //指定された座標へ向かうが、こちらは止まらずに、指定された座標を超えても、その方向へ動き続ける。
    protected void Set_MoveDirection_ByTargetPosition(Vector3 Position )
    {
        isThereTargetPosition = false;  //他の移動は全て無効に
        Vector3 distance = Position - transform.position;
        Vector3 direction = distance.normalized;
        direction.y = 0.0f; //高さは変えない
        moveDirection = direction;
    }


    //--------------------------------------
    //  　初期化
    //--------------------------------------
    protected override void ChildStart() {
        player = GameObject.Find("Player");
        if (player == null) {
            Debug.Log("PlayerNotFound");
        }

        Set_MoveTargetPosition( transform.position );
    }

    //--------------------------------------
    //   更新処理(固定)
    //--------------------------------------
    //ターゲット移動か向き移動のどちらか最後に指定されてる方で実際に移動する
    protected override void FixedUpdate_WhileActive()
    {
        if (isThereTargetPosition)
        {
            Vector3 distance = targetPosition - transform.position;
            if (distance.magnitude < ARRIVAL_DISTANCE_LENGTH) return;           //ほぼ目的地なら移動を辞める
            Vector3 direction = distance.normalized;
            direction.y = 0.0f; //高さは変えない
            transform.position += direction * moveSpeed;
            transform.LookAt( targetPosition );
        }
        else
        {
            transform.position += moveDirection * moveSpeed;
            transform.LookAt( transform.position + moveDirection );
        }
    }


    //--------------------------------------
    //  情報更新
    //--------------------------------------
    //移動可能範囲内のランダムな位置へターゲット移動する
    protected void Move_toRandom() {
        float x = Random.Range( -18.0f, 18.0f );    //ステージ幅とってもいいけど、今回ステージはスケールしないんでてきとう
        float z = Random.Range( -18.0f, 18.0f );
        Set_MoveTargetPosition( new Vector3(x, 0, z) );
    }
    //プレイヤーの座標へターゲット移動する
    protected void Move_toPlayer() {
       Set_MoveTargetPosition( player.transform.position );
    }
    //Move_toPlayerと違い、プレイヤーを通過してずっと奥へ進み続ける
    protected void Move_toPlayerDirection()
    {
        Set_MoveDirection_ByTargetPosition( player.transform.position );
    }
}
