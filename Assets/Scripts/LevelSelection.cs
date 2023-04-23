using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{

    [SerializeField] private bool unlocked;//Default value is false;Inspector上可以调整，但是在其他地方或者脚本里无法被访问
    public Image unlockImage;
    public GameObject[] stars;

    public Sprite starSprite;
    public Sprite starSprite2;

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
    }

    private void Update()
    {
        UpdateLevelImage();//TODO MOve this method later
        UpdateLevelStatus();//TODO MOve this method later
    }

    private void UpdateLevelStatus()
    {
        //if the current lv is 5, the pre should be 4
        int previousLevelNum = int.Parse(gameObject.name) - 1;
        if (PlayerPrefs.GetInt("Lv" + previousLevelNum.ToString()) > 0)//If the firts level star is bigger than 0, second level can play
        {
            unlocked = true;
        }
    }

    private void UpdateLevelImage()
    {
        if(!unlocked)//MARKER if unclock is false means This level is clocked!
        {
            unlockImage.gameObject.SetActive(true);
            for(int i = 0; i < stars.Length; i++)
            {
                stars[i].gameObject.SetActive(false);
            }
        }
        else//if unlock is true means This level can play !
        {
            unlockImage.gameObject.SetActive(false);
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].gameObject.SetActive(true);
            }

            if (PlayerPrefs.GetInt("Lv" + gameObject.name) == 3)
            {
                for(int i = 0; i < 3; i++)
                {
                    stars[i].gameObject.GetComponent<Image>().sprite = starSprite;
                }
            }
            else if (PlayerPrefs.GetInt("Lv" + gameObject.name) == 4)
            {
                for (int i = 0; i < 3; i++)
                {
                    stars[i].gameObject.GetComponent<Image>().sprite = starSprite2;
                }
            }

        }
    }

    public void PressSelection(string _LevelName)//When we press this level, we can move to the corresponding Scene to play
    {
        if (unlocked)
        {
            SceneManager.LoadScene(_LevelName);
        }
    }

}
