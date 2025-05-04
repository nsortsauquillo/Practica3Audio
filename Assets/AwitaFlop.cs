using UnityEngine;

public class DuchaRota : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float volumenMinimo = 0.8f;
    [SerializeField] private float volumenMaximo = 1.0f;
    [SerializeField] private float pitchMinimo = 0.9f;
    [SerializeField] private float pitchMaximo = 1.1f;

    [Header("Configuración de Partícula")]
    [SerializeField] private ParticleSystem sistemaParticulas;

    // Contador para rastrear cuántas partículas hay activas
    private int contadorParticulas = 0;
    private ParticleSystem.Particle[] particulasArray;

    private void Start()
    {
        // Verificamos que tenemos los componentes necesarios
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();

            // Si aún no hay AudioSource, lo añadimos
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1.0f; // Audio 3D
            }
        }

        if (sistemaParticulas == null)
        {
            sistemaParticulas = GetComponent<ParticleSystem>();

            if (sistemaParticulas == null)
            {
                Debug.LogError("No se encontró un sistema de partículas en el objeto. Por favor, asigna uno.");
            }
        }

        // Inicializamos el array de partículas
        particulasArray = new ParticleSystem.Particle[1];
    }

    private void Update()
    {
        if (sistemaParticulas != null)
        {
            // Obtenemos el número actual de partículas
            int particulas = sistemaParticulas.GetParticles(particulasArray, 1);

            // Si anteriormente no había partículas y ahora hay una, reproducimos el sonido
            if (contadorParticulas == 0 && particulas > 0)
            {
                ReproducirSonidoGota();
            }

            // Actualizamos el contador
            contadorParticulas = particulas;
        }
    }

    private void ReproducirSonidoGota()
    {
        if (audioSource != null)
        {
            // Variamos ligeramente el volumen y el pitch para que no suene siempre igual
            audioSource.pitch = Random.Range(pitchMinimo, pitchMaximo);
            audioSource.volume = Random.Range(volumenMinimo, volumenMaximo);

            // Reproducimos el sonido
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("El AudioSource no está configurado.");
        }
    }
}
