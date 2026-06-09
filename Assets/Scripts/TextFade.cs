using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFade : MonoBehaviour
{

    public Camera mainCamera;
    public void TriggerFadeSequence(TextMeshProUGUI textMeshProUGUI, float fadeInDuration = 0.2f, float stayDuration= 0.3f, float fadeOutDuration=0.3f)
    {
        StartCoroutine(FadeSequence(fadeInDuration, stayDuration, fadeOutDuration, textMeshProUGUI));
        StartCoroutine(GrowSequence(fadeInDuration, stayDuration, fadeOutDuration, textMeshProUGUI));
    }

    private IEnumerator FadeSequence(float fadeIn, float stay, float fadeOut, TextMeshProUGUI textMeshProUGUI)
    {
        // fade in
        float elapsed = 0f;

        while (elapsed < fadeIn)
        {
            elapsed += Time.deltaTime;
            textMeshProUGUI.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeIn);
            yield return null;
        }
        textMeshProUGUI.alpha = 1f;

        // stay on
        yield return new WaitForSeconds(stay);

        // fade out
        elapsed = 0f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.deltaTime;
            textMeshProUGUI.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOut);
            yield return null;
        }
        textMeshProUGUI.alpha = 0f;

        yield return new WaitForSeconds(1f);
        Destroy(textMeshProUGUI.gameObject);
    }

    private IEnumerator GrowSequence(float fadeIn, float stay, float fadeOut, TextMeshProUGUI textMeshProUGUI)
    {
        float yPos = textMeshProUGUI.transform.position.y;
        // fade in
        float elapsed = 0f;
        while (elapsed < fadeIn)
        {
            elapsed += Time.deltaTime;
            textMeshProUGUI.fontSize = Mathf.Lerp(0f, 10f, elapsed / fadeIn);
            textMeshProUGUI.transform.localPosition = new Vector3(textMeshProUGUI.transform.localPosition.x,  + Mathf.Lerp(0f, 50f, elapsed / fadeIn), textMeshProUGUI.transform.position.z); 
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < stay)
        {
            elapsed += Time.deltaTime;
            textMeshProUGUI.fontSize = Mathf.Lerp(10f, 13f, elapsed / stay);
            textMeshProUGUI.transform.localPosition = new Vector3(textMeshProUGUI.transform.localPosition.x, Mathf.Lerp(50f, 100f, elapsed / stay), textMeshProUGUI.transform.position.z);
            yield return null;
        }

        // fade out
        elapsed = 0f;
        while (elapsed < fadeOut)
        {
            elapsed += Time.deltaTime;
            textMeshProUGUI.fontSize = Mathf.Lerp(13f, 16f, elapsed / fadeOut);
            textMeshProUGUI.transform.localPosition = new Vector3(textMeshProUGUI.transform.localPosition.x, Mathf.Lerp(100f,120f, elapsed / fadeOut), textMeshProUGUI.transform.position.z);
            yield return null;
        }
    }
}
