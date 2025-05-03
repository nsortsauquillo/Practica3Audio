using UnityEngine;

public class FlickeringLamp : MonoBehaviour
{
    public Light lampLight;
    public AudioSource flickerSound;

    public float steadyDuration = 10f;
    public float flickerDuration = 3f;

    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.3f;

    [Range(0f, 1f)]
    public float volumeOn = 1f;
    [Range(0f, 1f)]
    public float volumeOff = 0.2f;
    public float fadeInDuration = 0.5f;

    private float cycleTimer = 0f;
    private bool isFlickering = false;

    private float flickerTimer = 0f;
    private float nextFlickerTime = 0f;

    private bool isFadingIn = false;
    private float fadeTimer = 0f;

    private void Start()
    {
        if (lampLight != null)
            lampLight.enabled = true;

        if (flickerSound != null)
        {
            flickerSound.playOnAwake = false;
            flickerSound.Stop();
            flickerSound.volume = 0f; // Empieza en 0 para evitar pico
            Invoke(nameof(EnableAudio), 5f);
        }
    }

    void EnableAudio()
    {
        if (flickerSound != null)
        {
            flickerSound.Play();
            isFadingIn = true;
            fadeTimer = 0f;
        }
    }

    private void Update()
    {
        // Volumen sube gradualmente al inicio
        if (isFadingIn && flickerSound != null)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeInDuration);
            flickerSound.volume = Mathf.Lerp(0f, volumeOn, t);

            if (t >= 1f)
                isFadingIn = false;
        }

        cycleTimer += Time.deltaTime;

        if (!isFlickering)
        {
            if (cycleTimer >= steadyDuration)
            {
                isFlickering = true;
                flickerTimer = 0f;
                nextFlickerTime = 0f;
            }
            else
            {
                if (lampLight != null)
                    lampLight.enabled = true;

                if (flickerSound != null && flickerSound.isPlaying && !isFadingIn)
                    flickerSound.volume = volumeOn;
            }
        }
        else
        {
            flickerTimer += Time.deltaTime;

            if (flickerTimer >= flickerDuration)
            {
                isFlickering = false;
                cycleTimer = 0f;

                if (lampLight != null)
                    lampLight.enabled = true;

                if (flickerSound != null && flickerSound.isPlaying && !isFadingIn)
                    flickerSound.volume = volumeOn;
            }
            else
            {
                nextFlickerTime -= Time.deltaTime;

                if (nextFlickerTime <= 0f)
                {
                    if (lampLight != null)
                        lampLight.enabled = !lampLight.enabled;

                    if (flickerSound != null && flickerSound.isPlaying && !isFadingIn)
                        flickerSound.volume = lampLight.enabled ? volumeOn : volumeOff;

                    nextFlickerTime = Random.Range(minFlickerTime, maxFlickerTime);
                }
            }
        }
    }
}
