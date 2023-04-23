using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleLevel : MonoBehaviour
{
    private int currentStarsNum = 0;
    public int levelIndex;
    public GameObjectMatrix gameObjectMatrix;

    /*public void SingleLevelGameObject(GameObjectMatrix gameObjectMatrix)
    {
        this.gameObjectMatrix = gameObjectMatrix;
    }*/

    public void BackButton()
    {
        SceneManager.LoadScene(0);
    }

    public void PressStarsButton(int _starsNum)
    {
        currentStarsNum = _starsNum;

        if(currentStarsNum > PlayerPrefs.GetInt("Lv" + levelIndex) && gameObjectMatrix.isWin == true)//不等于0的判断使得在关卡星星数量不为零后不能再修改
        {
            PlayerPrefs.SetInt("Lv" + levelIndex, _starsNum);
            BackButton();
        }

        //BackButton();
        //MARKER Each level has saved their own stars number
        //CORE PLayerPrefs.getInt("KEY", "VALUE"); We can use the KEY to find Our VALUE
        Debug.Log(PlayerPrefs.GetInt("Lv" + levelIndex, _starsNum));
    }

}
