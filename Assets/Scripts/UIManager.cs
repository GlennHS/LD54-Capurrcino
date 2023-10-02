using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text infoText;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("App quitted.");
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    public void ShowStartGamePanel()
    {
        infoPanel.SetActive(true);
        infoText.text = "Not available in Ludum Dare version";
    }
    
    public void ShowEndlessPanel()
    {
        infoPanel.SetActive(true);
        infoText.text = "Customers will start off few and calm but as you become more popular you'll be swarmed with thirsty customers eager for kitty companionship!";
    }
    
    public void ShowQuitPanel()
    {
        infoPanel.SetActive(true);
        infoText.text = "Leave your cats to tear up your cafe?";
    }
}
