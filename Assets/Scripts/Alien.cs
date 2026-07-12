using UnityEngine;
using System.Collections;

public class AlienMovement : MonoBehaviour
{
    public float speed = 2.0f; // Velocidad de movimiento
    private Vector3 targetDirection;

    void Start()
    {
        // Iniciamos el ciclo de cambio de dirección
        StartCoroutine(ChangeDirectionRoutine());
    }

    void Update()
    {
        // Mueve al alien en la dirección elegida
        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);
    }

    IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            // Crea una dirección aleatoria en el plano XZ (suelo)
            float randomX = Random.Range(-1.0f, 1.0f);
            float randomZ = Random.Range(-1.0f, 1.0f);
            targetDirection = new Vector3(randomX, 0, randomZ).normalized;

            // Opcional: Hace que el alien mire hacia donde camina
            if (targetDirection != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(targetDirection);

            // Espera 3 segundos antes de cambiar de nuevo
            yield return new WaitForSeconds(9.0f);
        }
    }
}