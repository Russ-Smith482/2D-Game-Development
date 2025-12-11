using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenFlash : MonoBehaviour

{
    public Image flashImage;
    public float flashDuration = 0.1f;
    public Color flashColor = new Color(0.35f, 0f, 0f, 0.4f);

    private Color originalColor;

    void Start()
    {
        originalColor = flashImage.color;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        flashImage.color = originalColor;
    }
}
