using UnityEngine;
using Unity.Netcode;

public class TankController2D : NetworkBehaviour
{
    [Header("Movimiento del tanque")]
    public float moveSpeed = 5f;        // Velocidad de avance / retroceso
    public float rotationSpeed = 180f;  // Velocidad de giro del tanque (cuerpo) en grados por segundo

    [Header("Torreta")]
    public Transform turret;           // Referencia al punto de pivote de la torreta
    public float turretRotationSpeed = 200f;  // Velocidad de giro de la torreta

    private Rigidbody2D _rb;             // Referencia al Rigidbody2D del tanque
    private float _moveInput;            // Input de movimiento (W/S)
    private float _rotationInput;        // Input de rotación del cuerpo (Q/E)
    private float _turretRotationInput;  // Input de rotación de la torreta (I/P)

    void Awake()
    {
        // Obtenemos el componente Rigidbody2D
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // MUY IMPORTANTE: sólo el dueño procesa el input
        if (!IsOwner) return;
        // --- MOVIMIENTO ADELANTE / ATRÁS (CUERPO) ---
        _moveInput = Input.GetAxisRaw("Vertical");

        // --- ROTACIÓN DEL CUERPO (Q / E) ---
        _rotationInput = 0f;

        // Q = girar cuerpo a la izquierda (antihorario)
        if (Input.GetKey(KeyCode.Q))
            _rotationInput = 1f;

        // E = girar cuerpo a la derecha (horario)
        if (Input.GetKey(KeyCode.E))
            _rotationInput = -1f;

        // --- ROTACIÓN DE LA TORRETA (I / P) ---
        _turretRotationInput = 0f;

        // I = girar torreta a la izquierda
        if (Input.GetKey(KeyCode.I))
            _turretRotationInput = 1f;

        // P = girar torreta a la derecha
        if (Input.GetKey(KeyCode.P))
            _turretRotationInput = -1f;
    }

    void FixedUpdate()
    {
        // Sólo el dueño mueve físicamente el tanque
        if (!IsOwner) return;
        // --- MOVIMIENTO DEL CUERPO SEGÚN DONDE MIRA ---
        // La "punta" del tanque (el Sprite) en este caso mira hacia arriba
        // Cambiaríamos a transform.right si el sprite mira a la derecha
        Vector2 forward = transform.up;

        Vector2 newPosition = _rb.position + forward * _moveInput * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(newPosition);

        // --- ROTACIÓN DEL CUERPO ---
        float newRotation = _rb.rotation + _rotationInput * rotationSpeed * Time.fixedDeltaTime;
        _rb.MoveRotation(newRotation);

        // --- ROTACIÓN DE LA TORRETA ---
        if (turret != null)
        {
            // Obtenemos la rotación actual en Z
            float currentZ = turret.eulerAngles.z;

            // Calculamos la nueva rotación
            float newTurretZ = currentZ + _turretRotationInput * turretRotationSpeed * Time.fixedDeltaTime;

            // Aplicamos la rotación solo en Z
            turret.rotation = Quaternion.Euler(0f, 0f, newTurretZ);
        }
    }
}

