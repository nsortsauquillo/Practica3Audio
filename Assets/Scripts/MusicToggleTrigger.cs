using UnityEngine;

public class MusicToggleTrigger : MonoBehaviour
{
    [Header("Configuración de Música")]
    public AudioClip musicaPisoArriba;
    public AudioClip musicaPisoAbajo;
    public float fadeDuration = 1f;

    private AudioSource audioSource;
    private bool isPisoArriba = true;
    private string playerTag = "Player";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.spatialBlend = 0;
        }

        audioSource.clip = musicaPisoArriba;
        audioSource.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            AudioClip nuevaMusica = isPisoArriba ? musicaPisoAbajo : musicaPisoArriba;
            StartCoroutine(FadeMusic(nuevaMusica));
            isPisoArriba = !isPisoArriba;
        }
    }

    System.Collections.IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();

        while (audioSource.volume < startVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }
}