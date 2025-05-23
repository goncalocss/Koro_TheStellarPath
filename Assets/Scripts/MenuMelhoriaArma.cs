using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuMelhoriaArma : MonoBehaviour
{
    [Header("Referências de UI")]
    public TextMeshProUGUI textoOrbs;
    public TextMeshProUGUI textoBananas;
    public TextMeshProUGUI textoNivel;
    public TextMeshProUGUI textoFaltaOrbs;
    public Button botaoMelhorar;

    [Header("Referência à arma")]
    public SistemaArmas sistemaArmas; // Pode ser referenciado via Inspector ou automaticamente

    [Header("Configuração")]
    public int custoMelhoria = 10;

    private void Start()
    {
        // Busca SistemaArmas via GameManager se não estiver setado no Inspector
        if (sistemaArmas == null && GameManager.Instance != null)
        {
            sistemaArmas = GameManager.Instance.sistemaArmas;
        }

        if (sistemaArmas == null)
        {
            Debug.LogWarning("⚠️ Sistema de Armas não encontrado.");
            return;
        }

        AtualizarUI();
    }

    private void OnEnable()
    {
        if (sistemaArmas != null)
            AtualizarUI();
    }

    public void TentarMelhorarArma()
    {
        if (sistemaArmas == null || GameManager.Instance == null)
            return;

        int orbs = GameManager.Instance.ObterOrbs();
        int nivelAtual = sistemaArmas.ObterNivelAtual();

        if (orbs >= custoMelhoria && nivelAtual < sistemaArmas.NivelMaximo)
        {
            GameManager.Instance.DiminuirOrbs(custoMelhoria);
            sistemaArmas.MelhorarArma();
            AtualizarUI();
            Debug.Log("🛠️ Arma melhorada via menu!");
        }
        else
        {
            Debug.Log("❌ Orbs insuficientes ou arma já está no nível máximo.");
        }
    }

    private void AtualizarUI()
    {
        if (GameManager.Instance == null || sistemaArmas == null)
            return;

        int orbs = GameManager.Instance.ObterOrbs();
        int bananas = GameManager.Instance.ObterBananaCount();
        int nivelAtual = sistemaArmas.ObterNivelAtual();
        int orbsFaltam = Mathf.Max(0, custoMelhoria - orbs);

        textoOrbs.text = orbs.ToString();
        textoBananas.text = bananas.ToString();
        textoNivel.text = $"Nível da Arma: {nivelAtual + 1}";

        // Se a arma está no nível máximo, muda o texto
        if (nivelAtual == sistemaArmas.NivelMaximo)
        {
            textoFaltaOrbs.text = "NÍVEL MÁXIMO";
        }
        else
        {
            // Se não está no nível máximo, mostra a quantidade de orbs faltando
            textoFaltaOrbs.text = orbsFaltam == 0
                ? "PRONTO PARA MELHORAR"
                : $"NECESSITAS DE MAIS {orbsFaltam} ORBS PARA MELHORARES A ARMA";
        }

        // Habilitar ou desabilitar o botão de melhorar com base no nível máximo
        botaoMelhorar.interactable = (orbsFaltam == 0 && nivelAtual < sistemaArmas.NivelMaximo);
    }

    public void VoltarAoCheckpoint()
    {
        // Descarrega a cena do menu de melhorias e respawna o jogador no checkpoint
        SceneManager.UnloadSceneAsync("MenuMelhorias");
        GameManager.Instance.Respawn();
        Time.timeScale = 1f; // Garantir que o jogo não fique pausado
        Debug.Log("↩️ Voltou ao checkpoint e fechou o menu.");
    }
}
