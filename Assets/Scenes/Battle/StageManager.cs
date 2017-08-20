using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*---------------
 *  
 *  現段階では、ステージのレベルを管理しているだけ。ステージを次のレベルに進めるか判定し、実際に進める処理を行う
 * 
 ---------------*/
public class StageManager : MonoBehaviour {

    //--------------------------------------
    //   変数
    //--------------------------------------
    Score           score;
    Enemy_Spawner   enemy_Spawner;
    TimeLimit       timeLimit;
    int             stageLevel = 1;
    Text            StageLevel_Text;

    //--------------------------------------
    //   初期化
    //--------------------------------------
    //各種プレハブの取得、コンポーネントの取得を行い、それらを初期ステージレベルで更新する
    void Start() {
        score = GameObject.Find("Score").GetComponent<Score>(); //あるかわからないやつを無理やりとってくるの怖いから、もっとましな方法探したい。方法はいくつかある気がするけど、Unityのコンポーネント指向に合っているのだろうか、時間のある時に模索したい
        enemy_Spawner = GameObject.Find("EnemySpawner").GetComponent<Enemy_Spawner>();
        timeLimit = GameObject.Find("TimeLimit").GetComponent<TimeLimit>();
        StageLevel_Text = GetComponent<Text>();
        score.SetStageLevel(stageLevel);
        enemy_Spawner.Init(stageLevel);
    }

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    //現在のスコアが、ノルマを満たしていた場合、次のレベルに移行するための更新処理
    void Update () {
        if (score.HaveAchievedQuota_toNextStage())
        {
            stageLevel++;
            enemy_Spawner.Init( stageLevel );
            score.SetStageLevel( stageLevel );
            timeLimit.AddRemainingTime( 30.0f );
            StageLevel_Text.text = stageLevel.ToString("Level ###");
        }
	}
}
