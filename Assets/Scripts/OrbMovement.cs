using UnityEngine;

public class OrbMovement : MonoBehaviour
{
    public float moveDuration = 1f;
    public Transform player;
    public bool isCollected = false;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float journeyLength;
    private float startTime;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            // Posições inicial e alvo
            startPosition = transform.position;
            targetPosition = new Vector3(player.position.x, player.position.y + 1f, player.position.z);

            // Calcula a distância total entre a orb e o jogador
            journeyLength = Vector3.Distance(startPosition, targetPosition);

            // Armazena o tempo inicial
            startTime = Time.time;
        }
        else
        {
            Debug.LogWarning("Player não encontrado pela tag no OrbMovement.");
        }
    }

    private void Update()
    {
        if (!isCollected && player != null)
        {
            float distanceCovered = (Time.time - startTime) * (journeyLength / moveDuration);

            if (distanceCovered < journeyLength)
            {
                float fractionOfJourney = distanceCovered / journeyLength;
                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            }
            else
            {
                transform.position = targetPosition;
                isCollected = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Orb coletada!");
            GameManager gameManager = FindObjectOfType<GameManager>();  // Acessa o GameManager
            gameManager.IncrementOrbCount();  // Incrementa a contagem de orbs no GameManager
            Destroy(gameObject);  // Destrói a orb quando coletada
        }
    }
}
