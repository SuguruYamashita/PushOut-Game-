using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*--------------------------------
 * 
 * 一定間隔で膨れて、膨れてる間のみ攻撃判定を持つ
 * 
 --------------------------------*/
public class BalloonAttack : Enemy_ActionPattern {

    public float MAX_MULTIPLE_SCALE = 3.0f;
    public float INTERVAL_TO_ATTACK = 3.0f;
    public float ATTACK_TIME        = 2.0f;
    public float BE_MAX_SCALE_TIME  = 1.0f;
    public float BE_MIN_SCALE_TIME  = 0.3f;

    float remainingTime_toBeAttack;
    float remainingTime_ofAttack;
    bool attacking = false;
    Vector3 defaultScale;
    Attack_EachOther attack_EachOther;
    Rigidbody rb;

    protected override void ChildStart()
    {
        base.ChildStart();
        defaultScale = transform.localScale;
        attack_EachOther = GetComponent<Attack_EachOther>();
        attack_EachOther.Inactivate();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected override void Update_WhileActive()
    {
        base.FixedUpdate_WhileActive();

        if (!attacking)
        {
            remainingTime_toBeAttack -= Time.deltaTime;
            if (remainingTime_toBeAttack <= 0)
            {
                attacking = true;
                remainingTime_ofAttack = ATTACK_TIME;
                attack_EachOther.Activate( ATTACK_TIME );
                rb.isKinematic = true;
            }
        }
        else
        {
            remainingTime_ofAttack -= Time.deltaTime;
            if (ATTACK_TIME - remainingTime_ofAttack < BE_MAX_SCALE_TIME)
            {
                transform.localScale = defaultScale * ((((ATTACK_TIME - remainingTime_ofAttack) / BE_MAX_SCALE_TIME) * MAX_MULTIPLE_SCALE)+1.0f);
            }
            else if ( remainingTime_ofAttack < BE_MIN_SCALE_TIME )
            {
                transform.localScale = defaultScale * (((remainingTime_ofAttack / BE_MIN_SCALE_TIME) * MAX_MULTIPLE_SCALE)+1.0f);
                if (remainingTime_ofAttack <= 0)
                {
                    attacking = false;
                    remainingTime_toBeAttack = INTERVAL_TO_ATTACK;
                    rb.isKinematic = false;
                }
            }
        }
    }

}
