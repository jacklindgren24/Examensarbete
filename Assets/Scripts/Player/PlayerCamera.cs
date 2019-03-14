using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public bool invertedX = false;
    public bool invertedY = false;
    public float sensitivity = 1.0f;
    public float maxAngle = 90;
    public float minAngle = -90;
    public float shakeIntensity = 6;
    public float shakeDuration = 0.4f;

    float x = 0;
    float y = 0;
    Rigidbody playerRB;

	void Start ()
    {
        playerRB = transform.parent.GetComponent<Rigidbody>();
	}

    void LateUpdate ()
    {
        if (!GameManager.instance.isPaused)
        {
            Vector2 dir = new Vector2(Input.GetAxis("Camera X"), Input.GetAxis("Camera Y"));

            x += invertedX ? -(dir.x * sensitivity) : dir.x * sensitivity;
            y += invertedY ? -(dir.y * sensitivity) : dir.y * sensitivity;
            y = Mathf.Clamp(y, minAngle, maxAngle);

            playerRB.rotation = Quaternion.Euler(new Vector2(0, x)); // Rotate player horizontally.
            transform.localEulerAngles = new Vector2(-y, 0); // Rotate camera vertically.
        }
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ScreenShake(shakeDuration));
    }

    public IEnumerator ScreenShake(float duration)
    {
        float intensity = shakeIntensity;
        Vector3 originalPosition = transform.localPosition;
        System.Random rand = new System.Random();

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;

            bool invertX = rand.Next(2) == 0;
            bool invertY = rand.Next(2) == 0;
            float x = invertX ? -intensity : intensity;
            float y = invertY ? -intensity : intensity;
            Vector3 rot = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(new Vector3(rot.x + x, rot.y + y, rot.z));

            intensity = Mathf.Lerp(intensity, 0, t);

            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

    public void SetInvertX(bool invert)
    {
        invertedX = invert;
    }

    public void SetInvertY(bool invert)
    {
        invertedY = invert;
    }
}
