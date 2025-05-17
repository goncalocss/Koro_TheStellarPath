using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image fillImage; // Associa aqui a imagem interna da barra

    // Define o valor inicial (vida cheia)
    public void SetMaxHealth(int maxHealth)
    {
        fillImage.fillAmount = 1f;
    }

    // Atualiza o valor da barra com base na vida atual
    public void SetHealth(int currentHealth, int maxHealth)
    {
        fillImage.fillAmount = (float)currentHealth / maxHealth;
    }

    // Mostra a barra na UI
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // Esconde a barra da UI
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
