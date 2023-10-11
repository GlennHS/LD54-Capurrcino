using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float score;

    [HideInInspector]
    public int catsPlaced = 0;

    [HideInInspector]
    public int turnsUntilNewCustomer = 6;
    
    [HideInInspector]
    public int turnsCustomerWillWait = 12;

    [HideInInspector]
    public int turnsSinceCustomerLastPaid = 0;

    public bool gameOver;

    public TMP_Text earnedText;
    public AudioSource bgm;
    public TMP_Text muteText, customerRateText, customerWaitText, newCustomerInText, catsPlacedText;
    public GameObject gameOverText, gameOverMMButton;
    public GameObject audioManager;
    public GameObject instructionsPanel;

    private bool _bgmPlaying = true;
    private int _turnsUntilNewCustomer;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        // Variable initialisation
        score = 0;
        _turnsUntilNewCustomer = turnsUntilNewCustomer;

        UpdateGameVars();
    }

    // Update is called once per frame
    void Update()
    {
        earnedText.text = $"Earned: {score.ToString("C", new CultureInfo("en-US"))}";
    }

    public void IncreaeCatsPlaced()
    {
        catsPlaced++;
        _turnsUntilNewCustomer--;
        if(_turnsUntilNewCustomer == 0)
        {
            BoardManager.instance.AddCustomer();
            ResetCustomerWaitCounter();
        }

        UpdateGameVars();
    }

    public void ResetCustomerWaitCounter()
    {
        _turnsUntilNewCustomer = turnsUntilNewCustomer;
        UpdateGameVars();
    }

    public void UpdateGameVars()
    {
        turnsUntilNewCustomer = 6 - (int)Mathf.Floor(catsPlaced / 20);
        turnsCustomerWillWait = 12 - (int)Mathf.Floor(catsPlaced / 30);

        customerWaitText.text = $"{turnsCustomerWillWait} turns";
        customerRateText.text = $"{turnsUntilNewCustomer} turns";
        newCustomerInText.text = $"{_turnsUntilNewCustomer} turns";
        catsPlacedText.text = $"Cats placed: {catsPlaced}";
    }

    public void ToggleMusic()
    {
        _bgmPlaying = !_bgmPlaying;
        if(_bgmPlaying)
        {
            bgm.UnPause();
            muteText.text = "Mute BGM";
        }
        else
        {
            bgm.Pause();
            muteText.text = "Unmute BGM";
        }
    }

    public void CustomerPaid(int turnsRemaining)
    {
        float customerPayment = Random.Range(10f, 20f) + Random.Range(0.5f, 2f) * turnsRemaining + (2 / turnsCustomerWillWait) * Random.Range(0.25f, 1f);

        // Customer satisfied in same turn as another
        if(turnsSinceCustomerLastPaid == 0)
        {
            customerPayment *= 2;
        }
        else if(turnsSinceCustomerLastPaid == 1)
        {
            customerPayment *= 1.5f;
        }

        score += customerPayment;

        turnsSinceCustomerLastPaid = 0;
}

    public void GameOver()
    {
        gameOver = true;
        gameOverText.SetActive(true);
        gameOverMMButton.SetActive(true);
        audioManager.GetComponent<AudioManager>().PlayLose();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void RestartScene()
    {
        SceneManager.LoadScene(1);
    }

    public void HideInstructions()
    {
        instructionsPanel.SetActive(false);
    }
    
    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
    }
}
