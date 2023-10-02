using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> meows = new List<AudioClip>();

    [SerializeField]
    AudioClip loseGameAudio, paidAudio;

    public void PlayMeow()
    {
        gameObject.GetComponent<AudioSource>().loop = false;
        gameObject.GetComponent<AudioSource>().Stop();
        gameObject.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 1.25f);
        gameObject.GetComponent<AudioSource>().clip = meows[Random.Range(0, meows.Count)];
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void PlayLose()
    {
        gameObject.GetComponent<AudioSource>().loop = false;
        gameObject.GetComponent<AudioSource>().Stop();
        gameObject.GetComponent<AudioSource>().clip = loseGameAudio;
        gameObject.GetComponent<AudioSource>().Play();
    }
    public void PlayCustomerPaid()
    {
        gameObject.GetComponent<AudioSource>().loop = false;
        gameObject.GetComponent<AudioSource>().Stop();
        gameObject.GetComponent<AudioSource>().clip = paidAudio;
        gameObject.GetComponent<AudioSource>().Play();
    }
}
