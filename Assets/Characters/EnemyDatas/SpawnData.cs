using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*------------------------------------
 * 
 *実質構造体。敵のスポーン管理に必要な情報
 *
 -----------------------------------*/

public class SpawnData : MonoBehaviour {
    public float spawnRate; //出現率。100を基準として、初期値が100よりマイナスだと、ステージ進行ごとに確率が加算されていき、最終的に100+絶対値まで上昇する。つまり、後半になるほど序盤の敵は出てきにくく、後半の敵が出やすくなっていく
    public float spawnCost;
}
