using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        //Si no se ha destruido aún, destruir la bomba después de su tiempo de vida
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        //Recoge los colliders en una esfera desde la posición de la bomba con el radio máximo.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        //Recorro los colliders
        for (int i = 0; i < colliders.Length; i++)
        {
            //Selecciono su Rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            //Si no tienen, paso al siguiente
            if (!targetRigidbody)
                continue;

            //Añado la fuerza de la explosion
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            //Busco el script TankHealth asociado con el Rigidbody
            TankHealth tankHealth = targetRigidbody.GetComponent<TankHealth>();

            //Si no hay script TankHealt, paso al siguiente
            if (!targetRigidbody)
                continue;

            //Calculo el daño a aplicar en funcion de la distancia a la bomba

            float damage = CalculateDamage(targetRigidbody.position);

            //Aplico el daño al tanque
            tankHealth.TakeDamage(damage);
        }

        //Desanclo el sistema de particulas de la bomba
        m_ExplosionParticles.transform.parent = null;

        //Repropduzco el sistema de particulas
        m_ExplosionParticles.Play();

        //Reproduzco el audio
        m_ExplosionAudio.Play();

        //Cuando las particulas han terminado, destruyo su objeto asociado
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);

        //Destruyo la bomba
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        //Creo un vector desde la bomba al objetivo
        Vector3 explosionToTarget = targetPosition - transform.position;

        //Calculo la distancia desde la bomba al objetivo
        float explosionDistance = explosionToTarget.magnitude;

        //Calculo la proporcion de maxima distancia (radio maximo) desde la explosion al tanque
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        //Calculo el daño a esa proporcion
        float damage = relativeDistance * m_MaxDamage;

        //Me aseguro de que el minimo daño es 0.
        damage = Mathf.Max(0f, damage);

        //Devuelvo el daño
        return damage;
    }
}