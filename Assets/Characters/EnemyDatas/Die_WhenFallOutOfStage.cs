using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*-----------------------
 * 
 *このコンポーネントを付けたオブジェクトは、-3.0fより下に落ちると消えて無くなる。
 * 今回はステージが固定なので、-3.0fで固定している。どこかに定数として定義しておくべきであるが、
 * そもそも、落下と判定する処理を後で作りたいので、今はこの形でいい。（一定時間落下し続けたら死亡とか？ステージの作りによらず、うまく判定したい）
 *
 -----------------------*/
public class Die_WhenFallOutOfStage : MonoBehaviour {

    //--------------------------------------
    //   更新処理(可変)
    //--------------------------------------
    //-3.0f以下で消す
    void Update () {
        if (transform.position.y < -3.0f) {
            Destroy(this.gameObject);
        }
	}
}
