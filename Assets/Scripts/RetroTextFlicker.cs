using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RetroTextFlicker : MonoBehaviour
{
    public Color colorA = Color.green;
    public Color colorB = Color.white;
    public float flickerSpeed = 0.15f;

    private Text text;
    private bool toggle;

    void Awake()
    {
        text = GetComponent<Text>();
        text.color = colorA;
    }

    void Start()
    {
        StartCoroutine(FlickerRoutine());
    }

    System.Collections.IEnumerator FlickerRoutine()
    {
        while (true)
        {
            text.color = colorA;
            yield return new WaitForSeconds(0.12f);

            text.color = colorB;
            yield return new WaitForSeconds(0.08f);
        }
    }
}
