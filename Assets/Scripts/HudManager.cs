using UnityEngine;
using TMPro;  // Necessário para manipular o TextMeshPro

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI orbCountText; // Referência ao componente TextMeshPro no HUD (no Canvas)
    private CollectiblesManager collectiblesManager; // Referência ao CollectiblesManager

    void Start()
    {
        // Encontre o CollectiblesManager na cena
        collectiblesManager = FindFirstObjectByType<CollectiblesManager>();
    }

    void Update()
    {
        // Atualiza o texto do HUD com o número de orbs coletadas
        if (collectiblesManager != null && orbCountText != null)
        {
            orbCountText.text = "Orbs: " + collectiblesManager.GetOrbCount().ToString();
        }
    }
}
