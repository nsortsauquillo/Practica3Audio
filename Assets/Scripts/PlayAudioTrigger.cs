using UnityEngine;
using System.Collections;

public class PlayAudioTrigger : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioClip audioClip;
    public bool playOnce = false;
    [Range(0, 60)]
    public float cooldownTime = 15f;

    private AudioSource audioSource;
    private bool canPlay = true;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = audioClip;
        audioSource.spatialBlend = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canPlay)
        {
            audioSource.Play();
            canPlay = false;
            StartCoroutine(CooldownRoutine());

            if (playOnce) Destroy(this);
        }
    }

    IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        if (!playOnce) canPlay = true;
    }
}