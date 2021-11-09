using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private Text winText;

    private void Start()
    {
        if (GameOverConfig.wonTeam != Team.Empty)
        {
            winText.text = GameOverConfig.wonTeam.ToString() + " Won";
        } else
        {
            winText.text = "Draw";
        }
       
    }
    public void ToTitle()
    {
        SceneManager.LoadScene("MenuScene");
    }

    void OnDestroy()
    {
        Config.Reset();
        File.Delete(Application.persistentDataPath
                     + "/board.dat");
    }
}
