using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerInfo : MonoBehaviour
{
    [SerializeField]
    private Text nameText, pingText, scoreText;


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowInfo(string name, int ping, int score)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        nameText.text = name;
        pingText.text = ping + "ms";
        scoreText.text = score.ToString();
    }
}
