using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/*--------------
 * 
 * 直接ヒエラルキーの空のオブジェクトなどに入れておく。
 * 制限時間など、ゲームオーバーの判定をするクラスからこのクラスを参照して、GameOver関数を呼ぶことで、一定時間後にタイトルに戻る。
 * ゲームオーバーの演出やその間の処理は、各クラスで行う。（このクラスは、あくまでもタイトルに遷移、ゲームオーバーの通知を行う）
 * ゲームオーバーを通知して欲しいクラスは、GameOverEvent_CallBackに自身のゲームオーバー時に呼び出してほしい関数を登録しておく。
 * 
 ---------------*/
public class GameOverEvent : MonoBehaviour {

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public float TIME_TO_BACK_TO_TITLE = 7.0f;
    public UnityEvent GameOverEvent_CallBack;

    //--------------------------------------
    //   変数
    //--------------------------------------
    float remainingTime_toBack_toTitle;
    bool isGameOver = false;

    //--------------------------------------
    //   制御情報更新
    //--------------------------------------
    //ゲームオーバー時の処理（スコアのセーブ、ゲームオーバーフラグをたてる、遷移までのカウント開始、外部クラスにゲームオーバーを通知）を行う
    public void GameOver()
    {
        if (isGameOver) return;
        GameObject.Find("Score").GetComponent<Score>().Save();
        isGameOver = true;
        remainingTime_toBack_toTitle = TIME_TO_BACK_TO_TITLE;
        if (GameOverEvent_CallBack != null)
        {
            GameOverEvent_CallBack.Invoke();
        }
    }

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    //可変フレームアップデート
    //ゲームオーバーになってからの経過時間を測定し、TIME_TO_BACK_TO_TITLEの時間に達するとタイトルに遷移する
    void Update()
    {
        if (isGameOver)
        {
            remainingTime_toBack_toTitle -= Time.deltaTime;
            if (remainingTime_toBack_toTitle <= 0)
            {
                SceneManager.LoadScene("Title");
            }
        }
    }
}
