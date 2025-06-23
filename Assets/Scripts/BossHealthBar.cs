using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BossHealthBar : MonoBehaviour
{
    public Image fillImage;
    public TextMeshProUGUI bossNameText;

    private string nomeOriginalBoss;

    void OnEnable()
    {
        string cenaAtual = SceneManager.GetActiveScene().name;

        if (cenaAtual == "Verdalya")
        {
            nomeOriginalBoss = "MOLGRIN";
        }
        else if (cenaAtual == "Drexan")
        {
            nomeOriginalBoss = "SKORVAL";
        }
        else if (cenaAtual == "Nebelya")
        {
            nomeOriginalBoss = "NYXORA";
        }
        else
        {
            nomeOriginalBoss = bossNameText != null ? bossNameText.text : "???";
            Debug.LogWarning("Cena não reconhecida, nome do boss não definido explicitamente.");
        }

        if (bossNameText != null)
        {
            bossNameText.text = nomeOriginalBoss;
        }
    }


    public void SetMaxHealth(int maxHealth)
    {
        fillImage.fillAmount = 1f;

        if (bossNameText != null)
        {
            bossNameText.text = nomeOriginalBoss;
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        fillImage.fillAmount = (float)currentHealth / maxHealth;
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (bossNameText != null)
        {
            bossNameText.text = nomeOriginalBoss;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
