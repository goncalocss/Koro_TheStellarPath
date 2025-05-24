using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    private GameObject interactionUIInstance;
    private bool jogadorDentro = false;
    private CollectiblesManager collectiblesManager;

    [Header("UI")]
    public Canvas canvasHUD; // Canvas principal em Screen Space - Overlay
    public TMP_FontAsset lemonMilkFont; // Fonte Lemon Milk (arrastar no Inspector)

    void Start()
    {
        collectiblesManager = FindObjectOfType<CollectiblesManager>();

        if (canvasHUD == null || lemonMilkFont == null)
        {
            Debug.LogError("❌ Chest.cs precisa do Canvas HUD e da fonte Lemon Milk atribuídos.");
            return;
        }

        CriarTextoHUD();
    }

    void Update()
    {
        if (jogadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            if (collectiblesManager != null)
            {
                Collider col = GetComponent<Collider>();
                collectiblesManager.HitBox(col);
            }

            if (interactionUIInstance != null)
                interactionUIInstance.SetActive(false);

            Invoke(nameof(DestruirBau), 0.1f);
        }
    }

    private void DestruirBau()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorDentro = true;
            if (interactionUIInstance != null)
                interactionUIInstance.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorDentro = false;
            if (interactionUIInstance != null)
                interactionUIInstance.SetActive(false);
        }
    }

    private void CriarTextoHUD()
    {
        GameObject textObj = new GameObject("TextoInteracaoChest");
        textObj.transform.SetParent(canvasHUD.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "E para abrir";
        tmp.font = lemonMilkFont;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        RectTransform rect = tmp.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 80);
        rect.anchorMin = new Vector2(0.5f, 0f); 
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0, 80);

        textObj.SetActive(false);
        interactionUIInstance = textObj;
    }
}
