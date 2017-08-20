using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*-----------------------
 * 
 * スコアはシーンを超えて共有したいため、ScoreクラスのStartが呼び出されたタイミングで初期化してはいけない。
 * このクラスをScoreクラスと同じオブジェクトにつけておくと、そのScoreクラスが生成されたとき、現在のスコアも0にリセットされるようになる。
 * 
 -----------------------*/
public class ResetScore : MonoBehaviour {

    //--------------------------------------
    //   初期化
    //--------------------------------------
    //現在のスコアを0にリセットする
    void Start ()
    {
        GetComponent<Score>().Init();
	}

}
