using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*-------------------------
 * 
 * 敵の行動パターン（別に敵以外にも付けて運用できる場面もあるだろうけど、今回のゲームでは敵の行動パターン）
 * bulletに設定されたオブジェクトを、設定された間隔で、ただ生成する。
 * 実際にプレイヤーに向かっていくといった処理は、生成されるオブジェクト側で定義する。
 * 
 * まず、SHOOT_START_TIME秒待機し、そこからSHOOT_INTERVAL秒おきにbulletを生成、BULLET_LOADING_NUMBER発撃ったら、RELOAD_TIME秒待機して、再びSHOOT_INTERVAL秒おきに発射する
 * 
 -------------------------*/

public class Shoot : Enemy_ActionPattern {

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public GameObject   bullet;
    public float  SHOOT_INTERVAL            = 0.6f;
    public int    BULLET_LOADING_NUMBER     = 5;        //装弾数。この数を撃ちきると、リロード時間になる
    public float  RELOAD_TIME               = 1.7f;     //弾を打ち切った後、何秒後に次の弾を撃ち始めるか
    public float  SHOOT_START_TIME          = 3.5f;     //弾を撃ち始めるまでの時間

    //--------------------------------------
    //   変数
    //--------------------------------------
    float   remainingTime_toStartShooting;
    float   remainingTime_toShoot;
    int     remainingBulletNumber;
    float   remainingTime_toReload;

    //--------------------------------------
    //   初期化
    //--------------------------------------
    //値を設定されたものに初期化しておく
    protected override void ChildStart()
    {
        base.ChildStart();
        remainingBulletNumber           = BULLET_LOADING_NUMBER;
        remainingTime_toReload          = RELOAD_TIME;
        remainingTime_toStartShooting   = SHOOT_START_TIME;
    }

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    //アクティブの間のみ。時間情報を更新し、決められた時間になると、bulletを生成する
    protected override void Update_WhileActive()
    {
        base.Update_WhileActive();

        if (remainingTime_toStartShooting > 0)
        {
            remainingTime_toStartShooting -= Time.deltaTime;
        }
        else
        {
            if (remainingBulletNumber > 0)
            {
                remainingTime_toShoot -= Time.deltaTime;
                if (remainingTime_toShoot <= 0.0f)
                {
                    Instantiate(bullet, new Vector3( transform.position.x, transform.position.y - 0.5f, transform.position.z), transform.rotation);   //俺はただ、初期値を親と同じにしてやりたいだけなのに・・どうして参照渡しになってしまうんだ・・・どうすればいいんだ、毎回わざわざpositionとrotationを与えるのか、何かましな方法はないのか。
                    remainingBulletNumber--;
                    remainingTime_toShoot = SHOOT_INTERVAL;
                }
            }
            else
            {
                remainingTime_toReload -= Time.deltaTime;
                if (remainingTime_toReload < 0.0f)
                {
                    remainingTime_toReload = RELOAD_TIME;
                    remainingBulletNumber = BULLET_LOADING_NUMBER;
                }
            }
        }
    }

}
