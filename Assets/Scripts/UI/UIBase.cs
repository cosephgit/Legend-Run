using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// base UI class for all UI elements
// created 23/8/23
// last modified 1/9/23

public class UIBase : MonoBehaviour
{
    [SerializeField] private float shakeDrag = 0.4f;
    [SerializeField] private float shakeDamping = 1f;
    protected Vector2 posOriginal;
    protected float shakeIntensity;
    private float shakeX;
    private float shakeY;
    private float freqX;
    private float freqY;
    private float shakeTime;

    protected virtual void Start()
    {
        posOriginal = transform.position;
    }

    // make the UI element shake (normally called when it changes)
    protected void AddShake(float intensity)
    {
        if (shakeIntensity == 0f)
        {
            if (CoSephUtils.RandomBool())
            {
                shakeX = Random.Range(1f, 2f);
                shakeY = Random.Range(0f, 1f);
            }
            else
            {
                shakeX = Random.Range(0, 1f);
                shakeY = Random.Range(1f, 2f);
            }
            freqX = Random.Range(10f, 20f);
            freqY = Random.Range(10f, 20f);
            shakeTime = 0f;
        }
        shakeIntensity += intensity;
    }

    protected virtual void Update()
    {
        if (shakeIntensity > 0)
        {
            Vector2 pos = posOriginal;

            shakeIntensity = Mathf.Max((shakeIntensity * (1f - shakeDrag * Time.unscaledDeltaTime)) - Time.unscaledDeltaTime * shakeDamping, 0f);
            shakeTime += Time.unscaledDeltaTime;

            if (shakeIntensity > 0)
            {
                pos.x += Mathf.Sin(Mathf.PI * freqX * shakeTime) * shakeX * shakeIntensity;
                pos.y += Mathf.Sin(Mathf.PI * freqY * shakeTime) * shakeY * shakeIntensity;
            }

            transform.position = pos;
        }
    }

    // make the provided element pop out then return to normal
    protected void PopElement(Transform element, float scale)
    {
        StartCoroutine(MakeElementPop(element, scale));
    }

    private IEnumerator MakeElementPop(Transform element, float scale)
    {
        Vector3 scaleOriginal = element.localScale;
        float timeLeft = 0.5f;

        while (timeLeft > 0)
        {
            element.localScale = scaleOriginal * Mathf.Lerp(scale, 1f, (0.5f - timeLeft) / 0.5f);
            timeLeft -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        element.localScale = scaleOriginal;
    }
}
