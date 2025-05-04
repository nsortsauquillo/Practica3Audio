using UnityEngine;
using System.Collections;

public class AullidoLobo : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float volumenInicial = 1.0f;
    [SerializeField] private float volumenMinimo = 0.0f;

    [Header("Configuración de Tiempo")]
    [SerializeField] private float tiempoMinimoEntreAullidos = 30.0f;
    [SerializeField] private float tiempoMaximoEntreAullidos = 40.0f;

    [Header("Configuración de Desvanecimiento")]
    [SerializeField] private float duracionDesvanecimiento = 2.0f;
    [SerializeField] private AnimationCurve curvaDesvanecimiento = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private float tiempoSiguienteAullido;
    private bool desvaneciendose = false;

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
                audioSource.loop = false;
                audioSource.spatialBlend = 1.0f; // Audio 3D
            }
        }

        // Aseguramos que el volumen inicial sea correcto
        audioSource.volume = volumenInicial;

        // Calculamos el tiempo para el primer aullido
        EstablecerProximoTiempoAullido();
    }

    private void Update()
    {
        // Si no estamos en proceso de desvanecimiento y es tiempo de aullar
        if (!desvaneciendose && Time.time >= tiempoSiguienteAullido)
        {
            ReproducirAullido();
        }
    }

    private void ReproducirAullido()
    {
        if (audioSource != null)
        {
            // Restauramos el volumen inicial
            audioSource.volume = volumenInicial;

            // Reproducimos el sonido
            audioSource.Play();

            // Comenzamos el desvanecimiento al final del sonido
            StartCoroutine(DesvanecerAlFinal());

            // Programamos el próximo aullido
            EstablecerProximoTiempoAullido();
        }
    }

    private IEnumerator DesvanecerAlFinal()
    {
        desvaneciendose = true;

        // Esperamos hasta que casi termine de reproducirse el sonido
        float duracionAudio = audioSource.clip.length;
        float tiempoEspera = duracionAudio - duracionDesvanecimiento;

        if (tiempoEspera > 0)
        {
            yield return new WaitForSeconds(tiempoEspera);
        }

        // Iniciamos el desvanecimiento gradual
        float tiempoTranscurrido = 0;

        while (tiempoTranscurrido < duracionDesvanecimiento && audioSource.isPlaying)
        {
            float t = tiempoTranscurrido / duracionDesvanecimiento;
            float factorVolumen = curvaDesvanecimiento.Evaluate(t);

            // Aplicamos el desvanecimiento
            audioSource.volume = Mathf.Lerp(volumenInicial, volumenMinimo, 1 - factorVolumen);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        desvaneciendose = false;
    }

    private void EstablecerProximoTiempoAullido()
    {
        // Tiempo aleatorio entre el mínimo y máximo establecido
        float tiempoEspera = Random.Range(tiempoMinimoEntreAullidos, tiempoMaximoEntreAullidos);
        tiempoSiguienteAullido = Time.time + tiempoEspera;
    }
}
