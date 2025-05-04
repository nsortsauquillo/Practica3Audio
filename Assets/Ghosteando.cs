using UnityEngine;

public class FantasmaAnimacion : MonoBehaviour
{
    [Header("Configuración de Animator")]
    [SerializeField] private Animator animator;

    [Header("Configuración de Tiempos")]
    [SerializeField] private float tiempoEnIdle = 5.0f;

    [Header("Nombres de Parámetros y Estados")]
    [SerializeField] private string parameterTiempoCumplido = "Has5SecPas";
    [SerializeField] private string parameterAnimacionTerminada = "HasAnimationFinish";
    [SerializeField] private string estadoIdle = "Idle";
    [SerializeField] private string estadoSurprised = "Surprised";

    // Variables para controlar el tiempo
    private float tiempoTranscurridoEnIdle = 0f;

    // Estado actual
    private enum Estado { Idle, Surprised }
    private Estado estadoActual = Estado.Idle;

    private void Start()
    {
        // Verificamos que tenemos el componente Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("No se encontró un Animator en el objeto. Por favor, asigna uno.");
                enabled = false;
                return;
            }
        }

        // Reiniciamos todos los parámetros a falso
        ResetearParametros();

        // Imprimimos los parámetros disponibles para verificar que existen
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            Debug.Log("Parámetro en Animator: " + param.name + " (Tipo: " + param.type + ")");
        }

        Debug.Log("Fantasma inicializado. Esperando " + tiempoEnIdle + " segundos antes de surprised.");
    }

    private void Update()
    {
        // Obtenemos información del estado actual
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Debug para ver el estado actual
        Debug.Log("Estado actual: " + (stateInfo.IsName(estadoIdle) ? "IDLE" :
                  (stateInfo.IsName(estadoSurprised) ? "SURPRISED" : "DESCONOCIDO")) +
                  " (Tiempo normalizado: " + stateInfo.normalizedTime.ToString("F2") + ")");

        // Actualizamos nuestro seguimiento del estado basado en el Animator
        ActualizarEstadoActual(stateInfo);

        // Manejamos la lógica según el estado
        switch (estadoActual)
        {
            case Estado.Idle:
                ManejarEstadoIdle();
                break;

            case Estado.Surprised:
                ManejarEstadoSurprised(stateInfo);
                break;
        }
    }

    private void ActualizarEstadoActual(AnimatorStateInfo stateInfo)
    {
        // Actualizamos nuestro estado interno basado en el Animator
        if (stateInfo.IsName(estadoIdle))
        {
            if (estadoActual != Estado.Idle)
            {
                estadoActual = Estado.Idle;
                // Cuando entramos a Idle, reiniciamos el temporizador
                tiempoTranscurridoEnIdle = 0f;
                Debug.Log("Entrando en estado IDLE");
            }
        }
        else if (stateInfo.IsName(estadoSurprised))
        {
            if (estadoActual != Estado.Surprised)
            {
                estadoActual = Estado.Surprised;
                Debug.Log("Entrando en estado SURPRISED");
            }
        }
    }

    private void ManejarEstadoIdle()
    {
        // Aumentamos el contador de tiempo en idle
        tiempoTranscurridoEnIdle += Time.deltaTime;

        // Si pasaron los segundos configurados, activamos la transición
        if (tiempoTranscurridoEnIdle >= tiempoEnIdle)
        {
            Debug.Log("¡Tiempo cumplido! (" + tiempoTranscurridoEnIdle.ToString("F2") + " segundos)");

            // IMPORTANTE: Reseteamos ambos parámetros para estar seguros
            ResetearParametros();

            // Y luego activamos el que necesitamos
            animator.SetBool(parameterTiempoCumplido, true);

            // Verificamos que se haya activado correctamente
            Debug.Log("Parámetro " + parameterTiempoCumplido + " establecido a: " +
                     animator.GetBool(parameterTiempoCumplido));
        }
    }

    private void ManejarEstadoSurprised(AnimatorStateInfo stateInfo)
    {
        // Si la animación surprised está casi terminada o ha terminado
        if (stateInfo.normalizedTime >= 0.9f)
        {
            // IMPORTANTE: Reseteamos ambos parámetros para estar seguros
            ResetearParametros();

            // Y luego activamos el que necesitamos para volver a idle
            animator.SetBool(parameterAnimacionTerminada, true);

            Debug.Log("Animación surprised al " + (stateInfo.normalizedTime * 100).ToString("F0") +
                     "%. Parámetro " + parameterAnimacionTerminada + " establecido a: " +
                     animator.GetBool(parameterAnimacionTerminada));

            // Para forzar el cambio si la transición no funciona automáticamente
            if (stateInfo.normalizedTime >= 0.95f)
            {
                animator.Play(estadoIdle);
                Debug.Log("¡FORZANDO cambio a Idle!");
            }
        }
    }

    private void ResetearParametros()
    {
        // Reiniciamos todos los parámetros booleanos
        animator.SetBool(parameterTiempoCumplido, false);
        animator.SetBool(parameterAnimacionTerminada, false);
    }

    // Función para forzar el cambio a un estado específico (útil para depuración)
    public void ForzarCambioAIdle()
    {
        ResetearParametros();
        animator.Play(estadoIdle);
        estadoActual = Estado.Idle;
        tiempoTranscurridoEnIdle = 0f;
    }

    public void ForzarCambioASurprised()
    {
        ResetearParametros();
        animator.Play(estadoSurprised);
        estadoActual = Estado.Surprised;
    }
}