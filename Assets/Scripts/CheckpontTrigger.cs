using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CheckpointTrigger : MonoBehaviour
{
    public GameObject fogo; // Referência ao GameObject do fogo (Particle System, etc.)
    public Canvas canvasHUD; // arrastar no Inspector
    public TMP_FontAsset lemonMilkFont; // arrastar no Inspector
    public string checkpointID; // ID único da lareira — definir no Inspector

    private bool ativado = false;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.LareiraJaFoiUsada(checkpointID))
        {
            ativado = true;

            if (fogo != null)
            {
                fogo.SetActive(true);
                Debug.Log($"🔥 Fogueira '{checkpointID}' ativada automaticamente via save.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ativado) return;

        if (other.CompareTag("Player"))
        {
            // ⚠️ Se já estiver usada, não faz nada
            if (GameManager.Instance.LareiraJaFoiUsada(checkpointID))
            {
                ativado = true;
                fogo?.SetActive(true); // Ativa o fogo se existir
                Debug.Log($"ℹ️ Lareira '{checkpointID}' já foi usada anteriormente. Ignorado.");
                return;
            }

            ativado = true;

            if (fogo != null)
            {
                fogo.SetActive(true);
                Debug.Log("🔥 Fogo aceso no checkpoint: " + gameObject.name);
            }

            Vector3 offset = new Vector3(2f, 0f, 0f);
            Vector3 posicaoCheckpoint = transform.position + offset;

            GameManager.Instance.DefinirCheckpoint(posicaoCheckpoint);
            GameManager.Instance.RegistarLareiraUsada(checkpointID); // ✅ novo

            Debug.Log("✅ Checkpoint ativado em posição: " + posicaoCheckpoint);

            SoundManager.Instance.PlaySFX("checkpoint");

            SaveData data = new SaveData();
            data.playerPosition = other.transform.position;
            data.playerHealth = GameManager.Instance.ObterVidaAtual();
            data.currentScene = SceneManager.GetActiveScene().name;
            data.checkpointPosition = posicaoCheckpoint;
            data.orbs = GameManager.Instance.ObterOrbs();
            data.vidaMaxima = GameManager.Instance.ObterVidaMaxima();
            data.bananaCount = GameManager.Instance.ObterBananaCount();
            data.caixasDestruidas = GameManager.Instance.ObterCaixasDestruidas();
            data.lareirasUsadas = GameManager.Instance.ObterLareirasUsadas(); 
            data.nivelArma = GameManager.Instance.sistemaArmas.ObterNivelAtual();

            SaveSystem.SaveGame(data);

            Debug.Log("💾 Progresso gravado ao ativar o checkpoint.");

            MostrarTextoGuardado();
        }
    }

    private void MostrarTextoGuardado()
    {
        GameObject textObj = new GameObject("TextoGuardado");
        textObj.transform.SetParent(canvasHUD.transform, false);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "Jogo Guardado!";
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

        Destroy(textObj, 3f);
    }
}
