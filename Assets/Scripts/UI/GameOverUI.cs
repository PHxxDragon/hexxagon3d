using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Text winText;

    private void Start()
    {
        winText.text = GameOverConfig.wonTeam.ToString() + " Won";
    }
    public void ToTitle()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
