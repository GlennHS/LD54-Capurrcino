using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float score;

    [HideInInspector]
    public int catsPlaced = 0;

    [HideInInspector]
    public int turnsUntilNewCustomer = 8;
    
    [HideInInspector]
    public int turnsCustomerWillWait = 10;

    public bool gameOver;

    public TMP_Text earnedText;
    public AudioSource bgm;
    public TMP_Text muteText, customerRateText, customerWaitText, newCustomerInText, catsPlacedText;
    public GameObject gameOverText;
    public GameObject audioManager;

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
            _turnsUntilNewCustomer = turnsUntilNewCustomer;
        }

        UpdateGameVars();
    }

    public void UpdateGameVars()
    {
        turnsUntilNewCustomer = 8 - (int)Mathf.Floor(catsPlaced / 20);
        turnsCustomerWillWait = 10 - (int)Mathf.Floor(catsPlaced / 30);

        customerWaitText.text = $"{turnsCustomerWillWait} minutes";
        customerRateText.text = $"{turnsUntilNewCustomer} minutes";
        newCustomerInText.text = $"{_turnsUntilNewCustomer} minutes";
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
        score += Random.Range(10f, 20f) + Random.Range(0.5f, 2f) * turnsRemaining + (2 / turnsCustomerWillWait) * Random.Range(0.25f, 1f);
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverText.SetActive(true);
        audioManager.GetComponent<AudioSource>().Play();
    }
}
