using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenuManager : MonoBehaviour
{
    private void Start()
    {
        DataManager.Instance.LoadSettings();
    }
    public void StartTutoriel()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}