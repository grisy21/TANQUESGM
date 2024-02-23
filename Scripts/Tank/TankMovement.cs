using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1; //Se utiliza para identificar que tanque es de cada jugador.
    public float m_Speed = 12f; // Rapidez con la que 1 tanque se mueve alante y atras.
    public float m_TurnSpeed = 180f;       //Rapidez de giro del tanque en grados por segundo.
    public AudioSource m_MovementAudio;    //Referencia de audio del motor del tanque (diferente del de disparo).
    public AudioClip m_EngineIdling;       //Audio a reproducir cuando el tanque no se mueve.
    public AudioClip m_EngineDriving;      //Audio a reproducir cuando el tanque se mueve.
    public float m_PitchRange = 0.2f;     //Cantidad de variacion de afinacion del audio del motor (para que sea mas real).



    private string m_MovementAxisName;      // Nombre del eje para moverse alante y atrás.
    private string m_TurnAxisName;         //Nombre del eje para girar.
    private Rigidbody m_Rigidbody;         //Referencia del componente para mover el tanque.
    private float m_MovementInputValue;    //Valor actual de entrada para el movimiento.
    private float m_TurnInputValue;        //Valor actual de entrada para el giro.
    private float m_OriginalPitch;         //Valor del pitch de la fuente de audio al inicio.


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }


    private void OnEnable()
    {
        // Al arrancar/habilitar el tanque, deshabilitamos la kinemática del tanque para que se pueda mover.
        m_Rigidbody.isKinematic = false;

        // Reseteamos los valores de entrada.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable()
    {
        // Al parar/deshabilitar el tanque, habilitamos la kinemática del tanque para que se pare. 
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        //Nombres de los ejes segun el numero de jugador.
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        //Almaceno la afinacion original del audio del motor.
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update()
    {
        // Almaceno los valores de entrada. 
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        //Llamo a la función que gestiona el audio del motor
        EngineAudio();
    }


    private void EngineAudio()
    {
        //Si no hay entrada, es que esta quieto...
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f)
        {
            // ... y si estaba reproduciendo el audio de moverse...
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // ... cambio el aduio al de estar parado y lo reproduzco.

                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {

            // Si hay entrada es que se esta moviendo. Si estaba reproduciendo el esta parado...
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                //... cambio el aduio al de moverse y lo reproduzco.
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }

    }


    private void FixedUpdate()
    {
        // Ajusto la posicion y rotacion
        Move();
        Turn();
    }


    private void Move()
    {
        // Creo un vector en la dirección en la que apunta el tanque,
        // con una magnitud basada en la entrada, la velocidad y el tiempo entre frames. 
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Aplico ese vector de movimiento al rigidbody del tanque

        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        // Calculo el número de grados de rotación basandome en la entrada, la velocidad y el tiempo entre frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Convirto ese numero en una rotacion en el eje Y.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Aplico esa rotacion al rigidbody del tanque.
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}