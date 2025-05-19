using UnityEngine;

public class BananaMovement : MonoBehaviour
{
    public bool isCollected = false;

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.player == null) return;

        float distance = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

        if (distance < 1.5f && !isCollected)
        {
            isCollected = true;
            GameManager.Instance.ColetarBanana(); // ✅ Aplica a lógica de banana
            Destroy(gameObject);
        }
        else if (!isCollected)
        {
            // Movimento suave em direção ao player
            Vector3 direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.player.transform.position, 0.05f);
        }
    }
}
