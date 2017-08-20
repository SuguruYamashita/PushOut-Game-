using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/*--------------
 * 
 * タイトル等のメニューボタンのコールバック関数に、SceneLoad関数をぶちこむと、そのボタンを押したときにBattleSceneに移行する
 * 
 ---------------*/
public class StartBattle : MonoBehaviour {

    //--------------------------------------
    //   シーン遷移
    //--------------------------------------
    //BattleSceneに遷移する
    public void SceneLoad()
    {
        SceneManager.LoadScene("Battle");
    }
}
