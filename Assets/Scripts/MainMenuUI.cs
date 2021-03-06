﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {

    public GameObject panel1;
    public GameObject panel2;
    public CanvasGroup transition_image;
    public AudioSource button_audio;

	public void PlayButton()
    {
        panel1.SetActive(true);
        button_audio.Play();
    }

    public void NextButton()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
        button_audio.Play();
    }

    public void LaunchButton()
    {
        StartCoroutine(StartGame());
        button_audio.Play();
    }

    private IEnumerator StartGame()
    {
        while (transition_image.alpha < 1)
        {
            transition_image.alpha += Time.deltaTime * 2;
            yield return null;
        }
        SceneManager.LoadScene("Main");
        yield return null;
    }
}
