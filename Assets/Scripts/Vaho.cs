using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Vaho : MonoBehaviour
{
    public Image fogImage;      // Arrastra aquí la imagen del vaho
    public float cleanTime = 0.5f;
    private bool isCleaning = false;
    


    void Update()
    {
        // Si se pulsa la tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            CleanFog();
        }
    }

    public void CleanFog()
    {
        if (!isCleaning)
            StartCoroutine(CleanRoutine());
    }

    private IEnumerator CleanRoutine()
    {
        isCleaning = true;

        float timer = 0f;
        Color startColor = fogImage.color;

        while (timer < cleanTime)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(startColor.a, 0f, timer / cleanTime);
            fogImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // Asegurar que queda totalmente transparente
        fogImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        isCleaning = false;
    }
}