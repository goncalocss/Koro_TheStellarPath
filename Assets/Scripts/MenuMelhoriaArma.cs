using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuMelhoriaArma : MonoBehaviour
{
    public SistemaArmas sistemaArmas; // Referência ao script de armas
    public TextMeshProUGUI textoOrbs;
    public TextMeshProUGUI textoNivel;
    public Button botaoMelhorar;

    public int custoMelhoria = 5;

    private void OnEnable()
    {
        AtualizarUI();
    }

    public void TentarMelhorarArma()
    {
        int orbs = GameManager.Instance.ObterOrbs();
        int nivelAtual = sistemaArmas.ObterNivelAtual();

        if (orbs >= custoMelhoria && nivelAtual < sistemaArmas.NivelMaximo)
        {
            GameManager.Instance.DiminuirOrbs(custoMelhoria);
            sistemaArmas.MelhorarArma();
            AtualizarUI();
        }
    }

    private void AtualizarUI()
    {
        textoOrbs.text = $"Orbs: {GameManager.Instance.ObterOrbs()}";
        textoNivel.text = $"Nível da Arma: {sistemaArmas.ObterNivelAtual() + 1}";

        bool podeMelhorar = GameManager.Instance.ObterOrbs() >= custoMelhoria && sistemaArmas.ObterNivelAtual() < sistemaArmas.NivelMaximo;
        botaoMelhorar.interactable = podeMelhorar;
    }
}
