using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//--------------------------------------
//   識別用
//--------------------------------------
public enum AttackerType{
    PLAYER,
    ENEMY,
    NEUTRAL
}

/*---------------------------------
 * 
 * このクラスを保有するオブジェクトは、何かのオブジェクトに当たった際、自動で対象にAddForceを行うようになる（吹っ飛ばすようになる）。
 * ただし、設定するAttackTypeが同じ相手に対しては効果が無い。
 * また、Activeの間しか効果が無い。
 * Attack_SuccessEventに設定しておけば、何かのオブジェクトを吹っ飛ばした時に、その関数を呼んでくれる。（プレイヤーの連続攻撃システムに使用）
 * 
 * 
 ---------------------------------*/
public class Attack_EachOther : SwitchActivation
{

    public UnityEvent Attack_SuccessEvent;

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public AttackerType attackerType;
    public float pushPower;
    public bool isDestroyedInstance_WhenHit_theOther = false;

    //--------------------------------------
    //   ヒットイベント
    //--------------------------------------
    //Activeなら触れた相手を吹っ飛ばす
    void OnCollisionEnter( Collision collision )
    {
        if (!IsActive()) return;

        Rigidbody other_rb = collision.gameObject.GetComponent<Rigidbody>();        //懸念：当たるたびにGetComponentしてると敵が増えた時重くなるかも。
                                                                                    //対策：このゲームは処理そんな多いゲームにならないから今はこれでいい。もし重いようなら、管理クラスを作って、自身のオブジェクト名とpair型とかで保持させといて、そこからcolision.gameObject.Nameとかで取ってこれるようにすればいい

        Attack_EachOther other_Attack_EachOther = collision.gameObject.GetComponent<Attack_EachOther>();
        if (other_rb == null) return;           //RigidBody持ってるものはとりあえず吹っ飛ばす！！
        if (other_Attack_EachOther != null)     //相手が攻撃するオブジェクトじゃない場合は問答無用で吹っ飛ばす
        {
            if (other_Attack_EachOther.attackerType == attackerType) return;        //仲間の場合は無視
        }

        Vector3 positionDifference = collision.gameObject.transform.position - transform.position;
        Vector3 direction = positionDifference.normalized;
        direction.y = 0.0f;     //上下には吹っ飛ばさない
        Vector3 force = direction * pushPower;
        other_rb.AddForce( force, ForceMode.Impulse );

        if (Attack_SuccessEvent != null)        //unityは自称nullだったりして怖いから、とりあえずこの判定方法で行う
        {
            Attack_SuccessEvent.Invoke();
        }

        if (isDestroyedInstance_WhenHit_theOther)
        {
            Destroy(this.gameObject);
        }
    }
}
