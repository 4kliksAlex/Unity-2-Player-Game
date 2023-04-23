using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager2 : MonoBehaviour
{
    public Text starsText;

    private void Update()
    {
        UpdateStarsUI();//TODO Not put inside the Update method later
    }

    public void UpdateStarsUI()
    {
        int sum = 0;

        for (int i = 1; i < 14; i++)
        {
            if (PlayerPrefs.GetInt("Lv" + i.ToString()) == 4)
            {
                sum += PlayerPrefs.GetInt("Lv" + i.ToString()) * 10 / 4;//Add the level 1 stars number, level 2 stars number.....
            }
        }

        starsText.text = "Player 2 Account ：" + sum + "฿";
    }
}
