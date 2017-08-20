using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*----------------------------------
 * 
 * 敵の行動パターン。
 * 常にランダムな位置へ動き続ける。
 * 目的地へ着いていなくても、移動開始からMOVE_INTERVAL秒たつと、次の移動を始める
 * 
 ----------------------------------*/

public class Always_Move_toRandom : Enemy_ActionPattern
{
    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public float MOVE_INTERVAL = 1.7f;                  //辛い：public constはinspectorに公開されないのか・・・やっぱコンパイル的に無理なんか

    //--------------------------------------
    //   変数
    //--------------------------------------
    float remainingTime_toMove;

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    // 次の移動までのカウント
    protected override void Update_WhileActive()
    {
        base.Update_WhileActive();
        remainingTime_toMove -= Time.deltaTime;
    }

    //--------------------------------------
    //   更新処理（固定）
    //--------------------------------------
    // MOVE_INTERVAL秒たったら次の移動を行う
    protected override void FixedUpdate_WhileActive()
    {
        base.FixedUpdate_WhileActive();

        if (remainingTime_toMove < 0.0f)
        {
            remainingTime_toMove = MOVE_INTERVAL;
            Move_toRandom();
        }
    }

}