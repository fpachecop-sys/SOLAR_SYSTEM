using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    public float movementSpeed = 15f;
    private bool gyroSupported;
    private Quaternion baseRotation = Quaternion.Euler(90, 0, 0);

    void Start()
    {
        gyroSupported = SystemInfo.supportsGyroscope;
        if (gyroSupported)
        {
            Input.gyro.enabled = true;
        }
    }

    void Update()
    {
        // 1. ROTACIÓN CON EL GIROSCOPIO
        if (gyroSupported)
        {
            transform.localRotation = GyroToQuaternion(Input.gyro.attitude);
        }
        else if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * 2f;
            float mouseY = Input.GetAxis("Mouse Y") * 2f;
            transform.Rotate(Vector3.up, mouseX, Space.World);
            transform.Rotate(Vector3.left, mouseY, Space.Self);
        }

        // 2. MOVIMIENTO DIVIDIDO (Izquierda = Retroceder, Derecha = Avanzar)
        float direccionMovimiento = 0f;

        // Soporte para Celular (Touch)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Si toca el lado derecho de la pantalla
            if (touch.position.x > Screen.width / 2)
            {
                direccionMovimiento = 1f; // Avanzar
            }
            // Si toca el lado izquierdo
            else
            {
                direccionMovimiento = -1f; // Retroceder
            }
        }
        // Soporte para PC (Clic Izquierdo Avanza, Clic Central/Rueda o Teclas S/W retrocede)
        else if (Input.GetMouseButton(0))
        {
            // En PC, si presionas Clic y el mouse está a la derecha del centro de la pantalla de juego
            if (Input.mousePosition.x > Screen.width / 2)
                direccionMovimiento = 1f;
            else
                direccionMovimiento = -1f;
        }

        // Soporte extra por teclado (W/S de respaldo para máxima comodidad en PC)
        if (Input.GetKey(KeyCode.W)) direccionMovimiento = 1f;
        if (Input.GetKey(KeyCode.S)) direccionMovimiento = -1f;

        // Aplicamos el movimiento físico en base a la dirección calculada
        if (direccionMovimiento != 0f)
        {
            transform.position += transform.forward * direccionMovimiento * movementSpeed * Time.deltaTime;
        }
    }

    private Quaternion GyroToQuaternion(Quaternion q)
    {
        return baseRotation * new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}