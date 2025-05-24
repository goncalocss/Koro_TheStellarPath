using UnityEngine;

public class CollectiblesMovement : MonoBehaviour
{
    public float moveDuration = 1f;  
    public float moveSpeed = 2f;     
    public float verticalOffset = 1f;

    public enum TipoColetavel { Orb, Banana }
    public TipoColetavel tipo;

    private Transform player;
    private bool isMoving = false;
    public bool IsCollected => isMoving;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startTime;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("❌ Player não encontrado no CollectiblesMovement.");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMoving && other.CompareTag("Player"))
        {
            isMoving = true;
            startPosition = transform.position;
            targetPosition = player.position + Vector3.up * verticalOffset;
            startTime = Time.time;
        }
    }

    private void Update()
    {
        if (!isMoving || player == null) return;

        if (moveSpeed > 0f)
        {
            targetPosition = player.position + Vector3.up * verticalOffset;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                Coletar();
            }
        }
        else // Usa Lerp baseado em tempo
        {
            float t = (Time.time - startTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (t >= 1f)
            {
                Coletar();
            }
        }
    }

    private void Coletar()
    {
        switch (tipo)
        {
            case TipoColetavel.Orb:
                GameManager.Instance?.IncrementOrbCount();
                break;

            case TipoColetavel.Banana:
                GameManager.Instance?.ColetarBanana();
                break;
        }

        Destroy(gameObject);
    }
}
