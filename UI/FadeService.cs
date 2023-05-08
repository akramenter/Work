using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeService : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float speed = 1.0f;
    public State state;
    public enum State
    {
        None,
        FadeIn,
        FadeOut
    }

    //void Start()
    //{

    //}


    void Update()
    {
        switch (state)
        {
            case State.FadeIn:
                UpdateFadeIn();
                break;
            case State.FadeOut:
                UpdateFadeOut();
                break;
        }
    }



    public void FadeIn()
    {
        state = State.FadeIn;
        fadeImage.gameObject.SetActive(true);
    }
    public void FadeOut()
    {
        state = State.FadeOut;
        fadeImage.gameObject.SetActive(true);
    }
    public bool IsFading()
    {
        return state != State.None;
    }
    private void UpdateFadeIn()
    {
        Color color = fadeImage.color;
        color.a -= speed * Time.deltaTime;
        if (color.a <= 0f)
        {
            color.a = 0f;
            state = State.None;
            fadeImage.gameObject.SetActive(false);
        }
        fadeImage.color = color;
    }
    private void UpdateFadeOut()
    {
        Color color = fadeImage.color;
        color.a += speed * Time.deltaTime;
        if (color.a >= 1f)
        {
            color.a = 1f;
            state = State.None;
        }
        fadeImage.color = color;
    }

}
