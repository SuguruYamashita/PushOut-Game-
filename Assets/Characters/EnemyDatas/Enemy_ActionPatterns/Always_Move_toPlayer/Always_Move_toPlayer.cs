using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*---------------------
 * 
 * 敵の行動パターン。
 * 常にプレイヤーの方へ向かう。
 * 
 ---------------------*/ 
public class Always_Move_toPlayer : Enemy_ActionPattern {

    //--------------------------------------
    //   更新処理（固定）
    //--------------------------------------
    //プレイヤーの方へ向かう
    protected override void FixedUpdate_WhileActive()
    {
        base.FixedUpdate_WhileActive();
        Move_toPlayer();
    }

}
