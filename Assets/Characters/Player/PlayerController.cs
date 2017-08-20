using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*------------------------------------------
 * 
 * 大急ぎで実装してたら、ものすごく肥大化したクラス。
 * 分けられる部分も多いと思うので、また時間のある時に分ける。
 * 主にプレイヤー周りの事をほぼやっている。
 * 移動、攻撃判定、コンボ判定、チャージ・ヒットのパーティクル描画、落下処理
 * かなりごり押ししていたため、一部名前が合ってなかったりするかもしれない。
 * 丁寧に直してる時間が無いため、今はこのままでいく。
 * 大文字だけだったり混ざったりしてるのは、constでやってた名残です・・・ごめんなさい
 * 
 ------------------------------------------*/
public class PlayerController : MonoBehaviour
{

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public float move_to_MouseDirection_Speed   = 0.4f;
    public float APPLY_SPEED_WHEN_ATTACK        = 1.4f;
    public float APPLY_SPEED_WHEN_ACCEL         = 0.65f;    //画面をチャージせずクリックすると、速く移動ができる
    public float BE_ATTACK_CHARGE_SECONDS       = 0.6f;     //アイテムで強化して、チャージ不要になってもいい
    public float FALL_LINE                      = -30.0f;   //タイムロスさせるためにあえて下のほうにしている
    public float APLLY_SCALE_WHEN_ATTACK        = 2.5f;     //攻撃中膨らむ
    public float COMBO_BREAK_TIME               = 1.5f;
    public GameObject chargingParticle_Prefab;
    public GameObject hitParticle_Prefab;

    //--------------------------------------
    //   変数
    //--------------------------------------
    Rigidbody rb;   //Rigidbody
    Attack_EachOther attack_EachOther;
    Vector3 XZDirection_fromScreenCenter_toMouse;
    float addMoveSpeed_byAttack;
    float addScale_byAttack;
    float addMoveSpeed_byAccel;
    float moveSpeedRate = 1.0f;
    Vector3 defaultPosition;
    Vector3 defaultScale;
    int combo;
    bool can_ComboAttack = false;   //敵に攻撃を当てた場合、コンボが途切れるまでチャージ無しで1回だけ連続攻撃できる。その攻撃を当てると、もう一度攻撃できる
    float remainingTime_toBreakCombo;
    Score score;
    ParticleSystem chargingParticle;
    ParticleSystem hitParticle;
    float default_ComboRemaining_Gage_Length;
    RectTransform ComboRemaining_Gage_RectTransform;
    Text comboNumber_Text;
    Text comboMessage;
    
    //--------------------------------------
    //   初期化処理
    //--------------------------------------
    //各種コンポーネントの取得と、初期値の保持
    void Start()
    {
        rb = GetComponent< Rigidbody >();
        attack_EachOther = GetComponent< Attack_EachOther >();
        defaultPosition = transform.position;
        defaultScale = transform.localScale;
        score = GameObject.Find("Score").GetComponent<Score>();
        chargingParticle = Instantiate(chargingParticle_Prefab, transform).GetComponent<ParticleSystem>();
        hitParticle = Instantiate(hitParticle_Prefab, transform).GetComponent<ParticleSystem>();

        Transform Combo_Canvas = transform.Find("Canvas");
        ComboRemaining_Gage_RectTransform = Combo_Canvas.Find("ComboRemainingTime_Gage").GetComponent<RectTransform>();
        default_ComboRemaining_Gage_Length = ComboRemaining_Gage_RectTransform.sizeDelta.x;
        comboNumber_Text = Combo_Canvas.Find("ComboNumber").GetComponent<Text>();
        comboMessage = Combo_Canvas.Find("Combo!").GetComponent<Text>();
        comboMessage.text = "NoCombo";
    }

    //--------------------------------------
    //  　イベント
    //--------------------------------------
    //攻撃ヒット時に呼ばれる、コンボ情報を更新する
    public void Attack_SuccessEvent()
    {
        remainingTime_toBreakCombo = COMBO_BREAK_TIME;
        if (combo > 0)
        {
            score.AddScore( combo * 50 );
        }
        combo++;
        hitParticle.Stop();    //Restart的な関数ないかな
        hitParticle.Play();
        can_ComboAttack = true;
    }

    //--------------------------------------
    //   情報更新
    //--------------------------------------
    //攻撃を辞める処理
    void FinishAttack()
    {
        addMoveSpeed_byAttack = 0.0f;
        rb.isKinematic = false;
        attack_EachOther.Inactivate();
    }


    //--------------------------------------
    //   更新処理(可変)
    //--------------------------------------
    //画面の中心とマウスの位置が成す方向をXZDirection_fromScreenCenter_toMouseに代入する
    void Update_XZDirection_fromScreenCenter_toMouse()
    {
        //---方向を求める
        Vector3 screenCenter                        = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f);
        Vector3 XYPosition_fromScreenCenter_toMouse = Input.mousePosition - screenCenter;
        Vector3 XZPosition_fromScreenCenter_toMouse = new Vector3(XYPosition_fromScreenCenter_toMouse.x, 0.0f, XYPosition_fromScreenCenter_toMouse.y);   //unityはUE4と座標系が違うから辛い、よく分からんことになる。この処理ほんとにいるんか？
        XZDirection_fromScreenCenter_toMouse        = XZPosition_fromScreenCenter_toMouse.normalized;
    }
   
    //攻撃と、攻撃に至るまでのチャージの処理
    float chargingSeconds = 0.0f;
    void Update_Attack_Charge_andAccel()
    {
        if (Input.GetMouseButton(0))    //チャージ中なら
        {
            chargingSeconds += Time.deltaTime;
            moveSpeedRate = 0.35f;
            FinishAttack();
        }
        else if( chargingSeconds > 0 )  //クリックが離された瞬間なら
        {
            if (chargingSeconds > BE_ATTACK_CHARGE_SECONDS || can_ComboAttack )   //コンボ中は当てるたびに1回だけ連続攻撃できる
            {
                addMoveSpeed_byAttack = APPLY_SPEED_WHEN_ATTACK;
                addScale_byAttack = APLLY_SCALE_WHEN_ATTACK;
                attack_EachOther.Activate();
                rb.isKinematic = true;      //攻撃中は無敵
                can_ComboAttack = false;
            }
            else if(Input.GetMouseButtonUp(0)) {                  //チャージ不足で攻撃にならなくても、クリックが離された瞬間なら
                addMoveSpeed_byAccel = APPLY_SPEED_WHEN_ACCEL;    //加速だけする
            }
            chargingSeconds = 0.0f;
            moveSpeedRate = 1.0f;
        }

        if (addMoveSpeed_byAttack < 0.1f) { //ほぼ0なら攻撃終了と判断し、無敵状態を終わらせる
            FinishAttack();
        }

        //チャージパーティクルの表示
        if (chargingSeconds > BE_ATTACK_CHARGE_SECONDS )
        {
            if (!chargingParticle.isPlaying)
            {
                chargingParticle.Play();
            }
        }
        else
        {
            if (chargingParticle.isPlaying)
            {
                chargingParticle.Stop();
            }
        }
    }

    //ステージ外へ落下した際に戻ってくる処理
    void Update_isFall_toOutSide()
    {
        if ( transform.position.y < FALL_LINE ) {
            transform.position = defaultPosition;
            rb.velocity = Vector3.zero;
        }
    }

    //コンボ情報を更新する処理。コンボの残り時間と、その描画情報を更新する
    void Update_Combo()
    {
        if (remainingTime_toBreakCombo > 0)
        {
            remainingTime_toBreakCombo -= Time.deltaTime;
            comboMessage.text = "Combo!";
        }
        else if( combo > 0)
        {
            combo = 0;
            can_ComboAttack = false;
            comboMessage.text = "NoCombo";
        }
        ComboRemaining_Gage_RectTransform.sizeDelta = new Vector2(default_ComboRemaining_Gage_Length * (remainingTime_toBreakCombo / COMBO_BREAK_TIME), ComboRemaining_Gage_RectTransform.sizeDelta.y);
        comboNumber_Text.text = combo.ToString("###");
    }

    //可変フレームアップデート
    void Update()
    {
        Update_XZDirection_fromScreenCenter_toMouse();
        Update_Attack_Charge_andAccel();
        Update_isFall_toOutSide();
        Update_Combo();
    }



    //--------------------------------------
    //   更新処理(固定)
    //--------------------------------------
    //マウスの方向へプレイヤーを移動させる
    void Move_to_MouseDirection()
    {
        float speed = move_to_MouseDirection_Speed + addMoveSpeed_byAttack + addMoveSpeed_byAccel;
        speed *= moveSpeedRate;
        //---実際に移動させる
        transform.position += XZDirection_fromScreenCenter_toMouse * speed ;
    }

    //突進攻撃を再現するために加速したものを急激に減速させていく
    void Update_addMoveSpeed_andScale_byAttack()
    {
        addMoveSpeed_byAttack /= 1.065f;    //割る数を定数にしようかとも思ったが、ちょうどいい数を見つけたら変える事ないと判断したのでこの形に。
        addScale_byAttack /= 1.065f;
    }
    //単純加速も、急激に減速させていく
    void Update_addMoveSpeed_byAccel()
    {
        addMoveSpeed_byAccel /= 1.065f;    //割る数を定数にしようかとも思ったが、ちょうどいい数を見つけたら変える事ないと判断したのでこの形に。
    }

    //攻撃中は、プレイヤーが大きくなる処理
    void Update_Scale()
    {
        transform.localScale = defaultScale * (1.0f + addScale_byAttack);
    }

    // 固定フレームアップデート
    void FixedUpdate ()
    {
        Move_to_MouseDirection();   //固定フレームでも可変フレームでもいいと思ってる。だから、DeltaTimeの処理が減るこっちに書いてる
        Update_addMoveSpeed_andScale_byAttack();
        Update_addMoveSpeed_byAccel();
        Update_Scale();
    }
}
