using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*-----------------------
 * 
 * 敵の行動パターン。
 * 最初、プレイヤーがいた方向に進み続ける。
 * 直進する弾丸などに使う
 * 
 ------------------------*/ 
public class MoveStart_toPlayer_Direction_atFirst : Enemy_ActionPattern {

    //--------------------------------------
    //   初期化
    //--------------------------------------
    //オブジェクトをこの時プレイヤーの居た方向に向かって直進させる
    protected override void ChildStart()
    {
        base.ChildStart();
        Move_toPlayerDirection();
    }
}
