using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LoadingCurtain : MonoBehaviour
{
    [Header("Refs")]
    public CanvasGroup Curtain;

    [Header("Timings")]
    public float FadeDuration = 0.5f;
    public float DelayBeforeHide = 2f; 
    public float DelayAfterShow  = 2f;

    private void Start()
    {
        Curtain.alpha = 1f;
    }

    public IEnumerator ShowRoutine()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);

       yield return new WaitForEndOfFrame();
       yield return FadeRoutine(1);
    }

    public IEnumerator HideRoutine()
    {
        if (!gameObject.activeSelf) yield break;

        yield return new WaitForSeconds(DelayBeforeHide);   
        yield return FadeRoutine(0f);                       

        if (Mathf.Approximately(Curtain.alpha, 0f))
            gameObject.SetActive(false);
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float start = Curtain.alpha;
        float t = 0f;

        targetAlpha = Mathf.Clamp01(targetAlpha);

        while (t < FadeDuration)
        {
            t += Time.deltaTime;
            Curtain.alpha = Mathf.Lerp(start, targetAlpha, t / FadeDuration);
            yield return null;
        }

        Curtain.alpha = targetAlpha;
    }

    private void Awake()
    {
        if (Curtain != null) Curtain.alpha = 0f;
        gameObject.SetActive(false);
    }
}