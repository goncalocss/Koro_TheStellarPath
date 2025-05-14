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
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Posições inicial e alvo
        startPosition = transform.position;
        targetPosition = new Vector3(player.position.x, player.position.y + 1f, player.position.z);

        // Calcula a distância total entre a orb e o jogador
        journeyLength = Vector3.Distance(startPosition, targetPosition);

        // Armazena o tempo inicial
        startTime = Time.time;
    }

    private void Update()
    {
        if (!isCollected)
        {
            // O tempo que passou desde o início do movimento
            float distanceCovered = (Time.time - startTime) * (journeyLength / moveDuration);

            // Se a orb ainda não chegou ao jogador, mova ela
            if (distanceCovered < journeyLength)
            {
                // Move a orb de forma linear entre a posição inicial e o alvo
                float fractionOfJourney = distanceCovered / journeyLength;
                transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            }
            else
            {
                // Se a orb chegou ao destino, define a posição final exata
                transform.position = targetPosition;
                isCollected = true;  // Marque a orb como coletada
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
