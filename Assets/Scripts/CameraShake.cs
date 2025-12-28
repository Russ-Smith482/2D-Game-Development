using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _cameraStartingPos;
    // Start is called before the first frame update
    void Start()
    {
        _cameraStartingPos = transform.position;
    }
    public IEnumerator CameraShakeCoroutine(float duration, float magnitude)
    {
        float _elapsedTime = 0.0f;

        while (_elapsedTime < duration)
        {
            float xOffSet = Random.Range(-1, 1) * magnitude;
            float yOffSet = Random.Range(-1, 1) * magnitude;

            transform.position = new Vector3(xOffSet, yOffSet, _cameraStartingPos.z);
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = _cameraStartingPos;
    }
}
