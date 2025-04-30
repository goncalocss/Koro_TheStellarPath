using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;           // O jogador
    public Vector3 offset = new Vector3(0, 2, -6); // Posição relativa
    public float followSpeed = 5f;     // Suavidade do follow
    public float rotateSpeed = 5f; 

    private float yaw = 0f;

    void LateUpdate()
    {
        if (!target) return;

        yaw += Input.GetAxis("Mouse X") * rotateSpeed;

        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        transform.LookAt(target.position + Vector3.up * 1.5f); 
    }
}
