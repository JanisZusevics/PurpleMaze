using UnityEngine;
using System.Collections;


public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f; // Duration of the shake in seconds
    public float shakeMagnitude = 0.7f; // Magnitude of the shake

    // Function to shake the camera
    public void Shake()
    {
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        //Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            //set game speed to half
            Time.timeScale = 0.5f;
            
            // generate random offset
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            float z = Random.Range(-1f, 1f) * shakeMagnitude;

            // create offset vector
            Vector3 offset = new Vector3(x, y, z);

            // apply offset to camera position
            transform.localPosition = transform.localPosition + offset;

            elapsed += Time.deltaTime;

            yield return null;
        }
        //set time back to nrmal
        Time.timeScale = 1f;
        //transform.localPosition = originalPos;
    }
}