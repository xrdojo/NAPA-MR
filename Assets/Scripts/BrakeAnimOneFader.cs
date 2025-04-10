using System.Collections;
using UnityEngine;

public class BrakeAnimOneFader : MonoBehaviour
{
  [Header("Objects to Fade (must have Renderer)")]
    public Renderer[] renderers;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;
    public bool fadeOutOnStart = false;

    public bool fadeInOnStart = false;

    void Start()
    {
        if (fadeInOnStart)
            FadeIn();
            
        if (fadeOutOnStart)
            FadeOut();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(false));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(true));
    }

    IEnumerator FadeRoutine(bool fadeOut)
    {
        foreach (var rend in renderers)
        {
            rend.material = new Material(rend.material); // ensure instance
            SetupMaterialForFade(rend.material);
        }

        float time = 0f;
        while (time < fadeDuration)
        {
            float alpha = fadeOut
                ? Mathf.Lerp(1f, 0f, time / fadeDuration)
                : Mathf.Lerp(0f, 1f, time / fadeDuration);

            foreach (var rend in renderers)
            {
                Color col = rend.material.color;
                rend.material.color = new Color(col.r, col.g, col.b, alpha);
            }

            time += Time.deltaTime;
            yield return null;
        }

        float finalAlpha = fadeOut ? 0f : 1f;
        foreach (var rend in renderers)
        {
            Color col = rend.material.color;
            rend.material.color = new Color(col.r, col.g, col.b, finalAlpha);
        }
    }

    void SetupMaterialForFade(Material mat)
    {
        mat.SetFloat("_Mode", 2f); // Fade
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}
