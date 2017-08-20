using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*------------------------
 * 
 *制限時間の管理・更新、描画情報の更新、制限時間に達した時の処理を行う。
 *
 ------------------------*/

public class TimeLimit : MonoBehaviour {

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public float    remainingTime = 60.0f;
    public Canvas   TimeUpMessage;

    //--------------------------------------
    //   変数
    //--------------------------------------
    Text timeLimit_Text;

    //--------------------------------------
    //   初期化
    //--------------------------------------
    //使用するコンポーネントの取得
    void Start () {
        timeLimit_Text = GetComponent<Text>();
	}

    //--------------------------------------
    //   情報更新
    //--------------------------------------
    //残り時間を追加する。単位は秒
    public void AddRemainingTime( float addSeconds )
    {
        remainingTime += addSeconds;
    }

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    bool isGameOver = false;
	// 残り時間を毎フレーム減らしていき、0になるとゲームオーバーにする処理。また、それらの描画情報を更新する。
	void Update () {
        if (isGameOver) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max( remainingTime, 0.0f );
        timeLimit_Text.text = remainingTime.ToString("###0.00");

        if (remainingTime <= 0.0f)
        {
            isGameOver = true;
            Instantiate( TimeUpMessage );
            GameObject.Find("GameOverEvent").GetComponent<GameOverEvent>().GameOver();
        }
	}
}
