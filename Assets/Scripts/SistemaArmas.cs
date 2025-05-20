using UnityEngine;

public class SistemaArmas : MonoBehaviour
{
    public GameObject[] armasPorNivel; // Ex: 3 armas = nível 0, 1 e 2
    private int nivelAtual = 0;

    public int NivelMaximo => armasPorNivel.Length - 1;

    private void Start()
    {
        AtualizarArma();
    }





    public void MelhorarArma()
    {
        if (nivelAtual < armasPorNivel.Length - 1)
        {
            nivelAtual++;
            AtualizarArma();
            Debug.Log($"🛠️ Arma melhorada para nível {nivelAtual + 1}");
        }
    }

    public int ObterNivelAtual() => nivelAtual;

    private void AtualizarArma()
    {
        for (int i = 0; i < armasPorNivel.Length; i++)
        {
            if (armasPorNivel[i] != null)
                armasPorNivel[i].SetActive(i == nivelAtual);
        }
    }

    public void DefinirNivel(int novoNivel)
    {
        nivelAtual = Mathf.Clamp(novoNivel, 0, armasPorNivel.Length - 1);
        AtualizarArma();
    }
}
