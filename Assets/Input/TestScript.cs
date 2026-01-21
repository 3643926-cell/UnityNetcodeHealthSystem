//using UnityEngine;

//public class TestScript : MonoBehaviour
//{
//    [SerializeField] private InputReader inputReader;
//    private void Start()
//    {
//        inputReader.MoveEvent += HandleMove;
//        inputReader.PrimaryFireEvent += HandlePrimaryFire;
//    }
//    private void OnDestroy()
//    {
//        inputReader.MoveEvent -= HandleMove;
//        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
//    }
//    private void HandleMove(Vector2 movement)
//    {
//        Debug.Log("Movimiento: " + movement);
//    }
//    private void HandlePrimaryFire(bool isFiring)
//    {
//        Debug.Log("Disparando: " + isFiring);
//    }



//}
using UnityEngine;
using Unity.Netcode;

public class TestScript : NetworkBehaviour
{
    [Header("Input")]
    [SerializeField] private InputReader inputReader;

    [Header("Movimiento del tanque")]
    [SerializeField] private float moveSpeed = 5f;        // Velocidad de avance / retroceso
    [SerializeField] private float rotationSpeed = 180f;  // Velocidad de giro del cuerpo (grados/segundo)

    [Header("Torreta")]
    [SerializeField] private Transform turret;            // Pivote de la torreta
    [SerializeField] private float turretRotationSpeed = 200f; // Velocidad de giro de la torreta

    private Rigidbody2D _rb;

    // Variables de input actuales
    private float _moveInput;            // Adelante / atrás
    private float _rotationInput;        // Rotación del cuerpo
    private float _turretRotationInput;  // Rotación de la torreta

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (inputReader == null) return;

        // Nos suscribimos a los eventos del InputReader
        inputReader.MoveEvent += HandleMove;
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
        inputReader.RotateBodyEvent += HandleRotateBody;
        inputReader.RotateTurretEvent += HandleRotateTurret;
    }

    private void OnDisable()
    {
        if (inputReader == null) return;

        // Nos desuscribimos para evitar referencias colgantes
        inputReader.MoveEvent -= HandleMove;
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
        inputReader.RotateBodyEvent -= HandleRotateBody;
        inputReader.RotateTurretEvent -= HandleRotateTurret;
    }

    // ------------------ Handlers de input ------------------

    private void HandleMove(Vector2 movement)
    {
        if (!IsOwner) return;

        // Suponemos que movement.y = adelante/atrás (W/S o stick vertical)
        // y movement.x = rotación del cuerpo (A/D o stick horizontal)
        _moveInput = movement.y;

        // Si quieres que el giro del cuerpo dependa SOLO de RotateBodyEvent,
        // puedes comentar la siguiente línea.
        _rotationInput = movement.x;
    }

    private void HandleRotateBody(float value)
    {
        if (!IsOwner) return;

        // -1 a 1 (Q/E, flechas, stick, etc.)
        _rotationInput = value;
    }

    private void HandleRotateTurret(float value)
    {
        if (!IsOwner) return;

        // -1 a 1: izquierda / derecha
        _turretRotationInput = value;
    }

    private void HandlePrimaryFire(bool isFiring)
    {
        if (!IsOwner) return;

        // De momento solo mostramos por consola
        Debug.Log("Disparando: " + isFiring);

        // Aquí en el futuro:
        // if (isFiring) ShootServerRpc();
    }

    // ------------------ Movimiento físico ------------------

    private void FixedUpdate()
    {
        // Solo el dueño mueve el tanque
        if (!IsOwner) return;

        // --- MOVIMIENTO DEL CUERPO ---
        // El sprite mira hacia arriba (transform.up)
        Vector2 forward = transform.up;
        Vector2 newPosition =
            _rb.position + forward * _moveInput * moveSpeed * Time.fixedDeltaTime;

        _rb.MovePosition(newPosition);

        // --- ROTACIÓN DEL CUERPO ---
        float newRotation =
            _rb.rotation + _rotationInput * rotationSpeed * Time.fixedDeltaTime;

        _rb.MoveRotation(newRotation);

        // --- ROTACIÓN DE LA TORRETA ---
        if (turret != null)
        {
            float currentZ = turret.eulerAngles.z;
            float newTurretZ =
                currentZ + _turretRotationInput * turretRotationSpeed * Time.fixedDeltaTime;

            turret.rotation = Quaternion.Euler(0f, 0f, newTurretZ);
        }
    }
}
