using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;           // O jogador
    public Vector3 offset = new Vector3(0, 3, -6); // Posição relativa
    public float followSpeed = 5f;     // Suavidade do follow
    public float rotateSpeed = 5f;     // Velocidade de rotação com o rato

    private float yaw = 0f;            // Rotação horizontal

    void LateUpdate()
    {
        if (!target) return;

        // Input do rato para rotação horizontal
        yaw += Input.GetAxis("Mouse X") * rotateSpeed;

        // Calcula a nova posição da câmara com rotação aplicada
        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        // Suaviza o movimento
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        // Olha para o jogador
        transform.LookAt(target.position + Vector3.up * 1.5f); // olhar para o tronco, não pés
    }
}
