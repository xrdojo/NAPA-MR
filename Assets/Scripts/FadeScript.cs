using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public CanvasGroup introObj;

    private bool fadeIn = false;
    private bool fadeOut = false;

    void Start()
    {
        introObj.alpha = 0;
    }

    public void ToggleFadeIn()
    {
        fadeIn = true;
    }

    public void ToggleFadeOut()
    {
        fadeOut = true;
    }

    void Update()
    {
        if(fadeIn)
        {
            if(introObj.alpha < 1)
            {
                introObj.alpha += Time.deltaTime;
                if(introObj.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }

        if(fadeOut)
        {
            if(introObj.alpha >= 0)
            {
                introObj.alpha -= Time.deltaTime;
                if(introObj.alpha == 0)
                {
                    fadeOut = false;
                }
            }
        }
    }
}
