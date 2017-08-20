using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/*--------------------------
 * 
 * スコア、ハイスコアのデータを管理、描画情報の更新を行う。
 * 
 --------------------------*/
public class Score : MonoBehaviour {

    //--------------------------------------
    //   静的変数
    //--------------------------------------
    static float highScore  = 0.0f;
    static float score      = 0.0f;
    public float GetScore() { return score; }

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public bool  isBattleScene = true;

    //--------------------------------------
    //   変数
    //--------------------------------------
    Text    text;
    int     stageLevel;

    //--------------------------------------
    //   初期化
    //--------------------------------------
    //各種コンポーネントの取得、セーブデータの読み込み
    void Start()
    {
        text = GetComponent<Text>();
        highScore = PlayerPrefs.GetFloat("HighScore");
    }
    //スコアを0にする
    public void Init()
    {
        score = 0.0f;
    }

    //--------------------------------------
    //   セーブ
    //--------------------------------------
    //ハイスコアのセーブを行う
    public void Save()
    {
        PlayerPrefs.SetFloat( "HighScore" ,highScore);
        PlayerPrefs.Save();
    }

    //--------------------------------------
    //   イベント
    //--------------------------------------
    bool isGameOver = false;
    //ゲームオーバーになった時の処理
    public void GameOverEvent()
    {
        isGameOver = true;
    }

    //--------------------------------------
    //   情報更新
    //--------------------------------------
    //スコアを加算する
    public void AddScore(float add)
    {
        if (isGameOver) return; //ゲームオーバー中も、ゲームは動いているため、スコアが変わらないようにしないといけない
        score += add;
    }
    //ステージのレベル情報を更新する
    public void SetStageLevel( int level ) {
        stageLevel = level;
    }

    //--------------------------------------
    //   情報取得
    //--------------------------------------
    //現在設定されてるステージのレベルでの、次のレベルへ進むために必要なスコアのノルマを返す
    float GetQuota_toNextStage()
    {
        return Mathf.Pow(1.6f, stageLevel) * 5000.0f;
    }
    //現在のスコアで、次のレベルに進めるかどうか返す
    public bool HaveAchievedQuota_toNextStage()
    {
        return score >= GetQuota_toNextStage() ;
    }

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    //表示する項目を、現在のシーンによって変更し、スコア描画情報を更新する。また、スコアがハイスコアより大きい場合、ハイスコアの更新をする。
    void Update()
    {
        if (isBattleScene)
        {
            if (score > highScore)
            {
                highScore = score;
            }
            text.text = highScore.ToString(" HighScore : #########0\n\n");
            text.text += score.ToString(" Score : #########0 / ") + GetQuota_toNextStage().ToString(" Quota : ########0");
        }
        else
        {
            text.text = highScore.ToString(" HighScore : #########0\n");
            text.text += score.ToString(" LastScore : #########0");
        }
    }
}
