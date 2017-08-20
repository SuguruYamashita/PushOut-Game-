using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*-----------------------------
 * 
 * Activeの時のみ、継承先で使用するためのUpdate系関数などが呼ばれるクラス。
 * 
 -----------------------------*/
public class SwitchActivation : MonoBehaviour
{

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public bool beActivated_WhenStart;

    //--------------------------------------
    //   変数
    //--------------------------------------
    bool isActive = false;                          //辛い:このバージョンのUnityだとC#6が使えないから読み取り専用変数を定義できないのか・・・
    public bool IsActive() { return isActive; }

    float remainingTime_ofActivation;

    //--------------------------------------
    //   初期化処理
    //--------------------------------------
    //継承先で初期化処理を実装する時用の関数
    protected virtual void ChildStart() { }
    
    //初期化時、もしアクティブになるならアクティブにする
    void Start()
    {
        if (beActivated_WhenStart)
        {
            Activate();
        }
        ChildStart();
    }

    //--------------------------------------
    //   情報更新
    //--------------------------------------
    //引数を与えない場合、終了時間は無く、意図的に止められるまで発動したままになる
    public void Activate()
    {
        Activate(9999999.0f); //このゲーム2000時間ぶっ通しでプレイするやついたら驚く(居るわけないので、実質時間制限なしです)
    }
    public void Activate(float length_ofAttackTime)
    {
        remainingTime_ofActivation = length_ofAttackTime;
        isActive = true;
    }
    public void Inactivate()
    {
        remainingTime_ofActivation = -1;
        isActive = false;
    }

    //--------------------------------------
    //   更新処理（固定）
    //--------------------------------------
    //アクティブの間のみ呼ばれるUpdate処理
    protected virtual void FixedUpdate_WhileActive() { }
    //継承先用のUpdate関数を呼ぶ
    void FixedUpdate()
    {
        if (isActive)
        {
            FixedUpdate_WhileActive();
        }
    }

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    //アクティブの間のみ呼ばれるUpdate処理
    protected virtual void Update_WhileActive() { }
    //アクティブの残り時間を計算し、切り替える
    void Update()
    {
        remainingTime_ofActivation -= Time.deltaTime;
        if (remainingTime_ofActivation < 0)
        {
            isActive = false;
        }

        if (isActive)
        {
            Update_WhileActive();
        }
    }
}
