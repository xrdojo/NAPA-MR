using System.Collections;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    public float fadeDuration = 1f;
    public bool fadeInOnStart = false;

    private Renderer[] renderers;
    private Material[] materials;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        materials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            // Use a separate instance of the material (don’t affect shared materials)
            materials[i] = renderers[i].material;
            SetMaterialAlpha(materials[i], fadeInOnStart ? 0f : 1f);
        }

        if (fadeInOnStart)
            StartCoroutine(FadeToAlpha(1f));
    }

    public void TriggerFadeIn()
    {
        StartCoroutine(FadeToAlpha(1f));
    }

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 public void TriggerFadeOut()
    {
        StartCoroutine(FadeToAlpha(0f));
    }

    IEnumerator FadeToAlpha(float targetAlpha)
    {
        float time = 0f;

        float[] startAlphas = new float[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            startAlphas[i] = materials[i].color.a;
        }

        while (time < fadeDuration)
        {
            float t = time / fadeDuration;
            for (int i = 0; i < materials.Length; i++)
            {
                float newAlpha = Mathf.Lerp(startAlphas[i], targetAlpha, t);
                SetMaterialAlpha(materials[i], newAlpha);
            }

            time += Time.deltaTime;
            yield return null;
        }

        // Final pass
        for (int i = 0; i < materials.Length; i++)
        {
            SetMaterialAlpha(materials[i], targetAlpha);
        }
    }

    void SetMaterialAlpha(Material mat, float alpha)
    {
        if (mat.HasProperty("_Color"))
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;

            // If using Standard Shader, make sure it's in Transparent mode
            if (mat.shader.name == "Standard")
            {
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}
