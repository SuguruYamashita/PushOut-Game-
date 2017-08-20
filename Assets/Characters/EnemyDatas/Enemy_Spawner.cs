using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*-------------------------------
 * 
 * 実質構造体。（そういえばC++では構造体もクラスも違いが無いからクラスにしたけどC#どうだったんだろう・・・余裕あれば調べる）
 * 敵と、そのコストを記録しておく（敵は死亡＝Destroyで、敵の情報が残らないから、別変数にコスト情報のみ避難させるため）
 *
 -------------------------------*/
class SpawningEnemyData {
    public GameObject enemyObject;
    public float enemyCost;
}



/*--------------------------------
 * 
 * そのステージレベルで出現できる敵の総コストに達するまで、ひたすら敵をスポーンさせ続ける。
 * 敵が死ねば、すぐにそのコストを埋めるように敵が生成される。（すぐに生成していると、コストの高い敵を意図的に出さないようにできてしまうかもしれない。この辺は後で調整する）
 * 敵のリストから、コスト、出現率をステージレベルに合わせて自動で計算し、敵を選んで出現させることができる。
 * 敵が出現する高さは、このスクリプトを所有するオブジェクトの高さになる。
 * 
 --------------------------------*/
public class Enemy_Spawner : SwitchActivation {

    //--------------------------------------
    //   設定用変数
    //--------------------------------------
    public GameObject[] allEnemyTypeDatas;

    //--------------------------------------
    //   変数
    //--------------------------------------
    float   stageCost;                      //そのステージで同時に出現できる最大コスト
    float   sum_ofNowCost;                  //現在出現している敵の総コスト
    float   allEnemyType_Sum_ofSpawnRate;   //全ての敵の出現率の合計
    int     stageLevel;
    List< SpawningEnemyData >   spawningEnemyDataList   = new List<SpawningEnemyData>();
    List< SpawnData >           allEnemyType_spawnDatas = new List<SpawnData>();
    Score score;

    //--------------------------------------
    //   情報取得
    //--------------------------------------
    const float BE_INCREASED_SPAWN_RATE_byLevel = 60.0f;
    // 現在のステージレベルによる影響を受けたスポーン率を返す
    float get_SpawnRate_atThisLevel(SpawnData spawnData)
    {
        float add_SpawnRate = stageLevel * BE_INCREASED_SPAWN_RATE_byLevel;
        float spawnRate = spawnData.spawnRate + add_SpawnRate;
        float Max = 100.0f - spawnData.spawnRate + 100.0f;  //100なら100、70なら130、30なら170、-30なら230まで上昇する。こうすることで、終盤ほど、強い敵の割り合いが増える
        return Mathf.Clamp(spawnRate, 0.0f, Max);    //マイナスは返さない
    }

    //--------------------------------------
    //   初期化
    //--------------------------------------
    // ステージレベル情報を更新し、それに合わせて出現管理のための情報を更新する
    public void Init(int level)
    {
        stageLevel = level;
        foreach (SpawnData spawnData in allEnemyType_spawnDatas)
        {
            allEnemyType_Sum_ofSpawnRate += get_SpawnRate_atThisLevel(spawnData);
        }
        stageCost = stageLevel + 10; //+10するのは、ステージ1とステージ2の差が大きくなりすぎるから。
    }
    // 敵リストから、スポーン情報のリストを生成し、ステージレベルを１として初期化、各種コンポーネントの取得
    protected override void ChildStart()
    {
        foreach (GameObject gameObject in allEnemyTypeDatas)
        {
            allEnemyType_spawnDatas.Add(gameObject.GetComponent<SpawnData>());
        }
        Init(1);
        score = GameObject.Find("Score").GetComponent<Score>();
    }

    //--------------------------------------
    //   情報取得
    //--------------------------------------
    // 現在の情報に基づいて選択された、ランダムな敵の要素番号を返す
    int GetRandomEnemyTypeData()
    {
        float sum_ofSpawnRate_whenChooseEnemy = Random.Range( 0, allEnemyType_Sum_ofSpawnRate );

        float now_Sum_ofSpawnRate = 0.0f;

        for(int i = 0; i < allEnemyType_spawnDatas.Count ; i++)
        {
           now_Sum_ofSpawnRate += get_SpawnRate_atThisLevel(allEnemyType_spawnDatas[i]);
            if ( now_Sum_ofSpawnRate > sum_ofSpawnRate_whenChooseEnemy ) {
                return i;
            }
        }
        return 0;//てきとうな値を返す
    }
    //敵がスポーンできる領域の、ランダムな座標を返す。y座標はこのゲームオブジェクトのy座標を使用する
    Vector3 MakeRandomXZPosition_withinTheStage()
    {
        float x = Random.Range( -17.0f, 17.0f );
        float z = Random.Range( -17.0f, 17.0f );
        return new Vector3( x, transform.position.y, z );
    }

    //--------------------------------------
    //   更新処理（可変）
    //--------------------------------------
    // アクティブの間のみ、敵が死んでいたらリストから消す処理、総コストに余裕があれば敵をスポーンさせりう処理を行う
    protected override void Update_WhileActive ()
    {
        base.Update_WhileActive();

        //死んだ敵をリストから除外
        for (int i = spawningEnemyDataList.Count - 1; i >= 0; i--)
        {
            if (spawningEnemyDataList[i].enemyObject == null) //一応、Destroy後は"自称null"になるらしく、この判定なら正しく行える
            {
                sum_ofNowCost -= spawningEnemyDataList[i].enemyCost;
                score.AddScore(spawningEnemyDataList[i].enemyCost * 100);
                spawningEnemyDataList.Remove( spawningEnemyDataList[i] );
            }
        }

        //総コストに余裕があれば敵をスポーン
        while (sum_ofNowCost < stageCost) {
            int wasChosen_EnemyNumber           =  GetRandomEnemyTypeData();
            sum_ofNowCost                       += allEnemyType_spawnDatas[wasChosen_EnemyNumber].spawnCost;
            SpawningEnemyData spawningEnemyData =  new SpawningEnemyData();
            spawningEnemyData.enemyObject       =  Instantiate( allEnemyTypeDatas[ wasChosen_EnemyNumber ], MakeRandomXZPosition_withinTheStage(), allEnemyTypeDatas[wasChosen_EnemyNumber].transform.rotation); //C#、クラスが参照で渡される仕様が怖いよう、transformもなんかコピーコンストラクタないし、C++に帰りたいよう
            spawningEnemyData.enemyCost         =  allEnemyType_spawnDatas[wasChosen_EnemyNumber].spawnCost;
            spawningEnemyDataList.Add( spawningEnemyData );
        }
	}
}
