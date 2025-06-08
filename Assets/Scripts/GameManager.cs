using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TextMeshProUGUI orbCountText;
    private int orbCount = 0;

    public List<Image> pontosDeVida;
    private int vidaAtual = 6;
    private int acertosRecebidos = 0;

    private int bananaCount = 0;
    private const int bananasPorVidaExtra = 1;

    private List<string> caixasDestruidas = new List<string>();
    private List<string> lareirasUsadas = new List<string>();

    [Header("Sistema de Armas")]
    public SistemaArmas sistemaArmas;

    //PAUSAR JOGO
    private bool jogoPausado = false;
    private Vector3 posicaoAntesDaPausa;

    [Header("Referência ao Player")]
    public Player player;
    public bool playerVivo = true;


    private Vector3 ultimoCheckpoint;
    private bool temCheckpoint = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (sistemaArmas == null)
        {
            sistemaArmas = FindFirstObjectByType<SistemaArmas>();
            if (sistemaArmas == null)
                Debug.LogWarning("⚠️ SistemaArmas não encontrado no GameManager.");
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !jogoPausado)
        {
            PausarJogo();
        }
        if (Input.GetKeyDown(KeyCode.M) && !jogoPausado)
        {
            AbrirMenuMelhorias();
        }

    }



    private void PausarJogo()
    {
        if (player == null) return;

        jogoPausado = true;
        posicaoAntesDaPausa = player.transform.position;

        Time.timeScale = 0f; // PAUSA TODAS AS ATUALIZAÇÕES DEPENDENTES DO TEMPO
        SceneManager.LoadScene("MenuPausa", LoadSceneMode.Additive);
        SoundManager.Instance.StopMusic(); // para a música atual
        SoundManager.Instance.PlayMusic("mainmenu-song");
    }

    private void AbrirMenuMelhorias()
    {
        if (player == null) return;

        jogoPausado = true;
        posicaoAntesDaPausa = player.transform.position;

        Time.timeScale = 0f; // PAUSA TODAS AS ATUALIZAÇÕES DEPENDENTES DO TEMPO
        SceneManager.LoadScene("MenuMelhorias", LoadSceneMode.Additive);
        SoundManager.Instance.StopMusic(); // para a música atual
        SoundManager.Instance.PlayMusic("mainmenu-song");
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

   

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = FindFirstObjectByType<Player>();
        orbCountText = FindFirstObjectByType<TextMeshProUGUI>();
        sistemaArmas = FindFirstObjectByType<SistemaArmas>();

        // HUD - Orb count
        if (orbCountText != null)
            UpdateOrbCountText();

        // Pontos de vida (corações)
        GameObject escolhido = null;
        int maxCount = 0;
        foreach (var candidato in GameObject.FindGameObjectsWithTag("PontosDeVida"))
        {
            int count = candidato.GetComponentsInChildren<Image>(true).Length;
            if (count > maxCount)
            {
                maxCount = count;
                escolhido = candidato;
            }
        }

        if (escolhido != null)
        {
            pontosDeVida = new List<Image>();
            foreach (var img in escolhido.GetComponentsInChildren<Image>(true))
            {
                if (img.gameObject != escolhido)
                    pontosDeVida.Add(img);
            }
        }

        // 👉 CARREGAR SAVE DEPOIS da cena estar carregada
        if (TempSaveData.Instance != null && TempSaveData.Instance.saveData != null)
        {
            SaveData data = TempSaveData.Instance.saveData;

            if (player != null)
                player.transform.position = data.playerPosition;

            ultimoCheckpoint = data.checkpointPosition;
            temCheckpoint = true;

            DefinirNumeroOrbs(data.orbs);
            DefinirBananaCount(data.bananaCount);
            AplicarVidaMaxima(data.vidaMaxima);
            DefinirVidaAtual(data.playerHealth);

            if (sistemaArmas != null)
            {
                sistemaArmas.DefinirNivel(data.nivelArma);
                Debug.Log($"🔫 Nível da arma carregado: {data.nivelArma}");
            }

            if (data.caixasDestruidas != null)
            {
                CarregarCaixasDestruidas(data.caixasDestruidas);
                RemoverCaixasDestruidas();
            }

            if (data.lareirasUsadas != null)
            {
                CarregarLareirasUsadas(data.lareirasUsadas);
            }

            Destroy(TempSaveData.Instance.gameObject);
        }

        AtualizarVidaVisual();
        playerVivo = true;

        if (scene.name == "Verdalya")
            SoundManager.Instance.PlayMusic("World1");
        if (scene.name == "Drexan")
            SoundManager.Instance.PlayMusic("World2");
    }


    public void IncrementOrbCount()
    {
        orbCount++;
        UpdateOrbCountText();
    }

    private void UpdateOrbCountText()
    {
        if (orbCountText != null)
            orbCountText.text = "x" + orbCount;
    }

    public void ResetOrbCount()
    {
        orbCount = 0;
        UpdateOrbCountText();
    }

    public void ReceberDano()
    {
        if (player == null || !player.estaVivo) return;

        acertosRecebidos++;
        if (acertosRecebidos >= 3)
        {
            PerderPontoDeVida();
            acertosRecebidos = 0;
        }
    }

    public void ReceberDanoBoss()
    {
        acertosRecebidos++;
        if (acertosRecebidos == 1)
        {
            PerderPontoDeVida();
            acertosRecebidos = 0;
        }
    }

    void PerderPontoDeVida()
    {
        if (vidaAtual > 0)
        {
            vidaAtual--;
            AtualizarVidaVisual();
            SoundManager.Instance.PlaySFX("lost-live");
        }

        if (vidaAtual == 0 && player != null)
        {
            playerVivo = false;
            player.Morrer();
        }
    }

    public void DefinirCheckpoint(Vector3 posicao)
    {
        ultimoCheckpoint = posicao;
        temCheckpoint = true;
    }

    public Vector3 ObterCheckpoint() => ultimoCheckpoint;
    public bool TemCheckpoint() => temCheckpoint;

    public void Respawn()
    {
        if (player == null) return;

        player.ResetarEstado();
        player.transform.position = ultimoCheckpoint;
        player.gameObject.tag = "Player";

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        if (player.TryGetComponent(out Rigidbody rb)) rb.isKinematic = false;
        if (player.TryGetComponent(out Collider col)) col.enabled = true;
        if (player.TryGetComponent(out PlayerMovement move)) move.enabled = true;

        if (!jogoPausado)
        {
            ReporVida();
        }
        playerVivo = true;
        jogoPausado = false; // Limpa o estado de pausa

    }

    public void ReporVida()
    {
        if (pontosDeVida == null || pontosDeVida.Count == 0) return;

        acertosRecebidos = 0;
        if (vidaAtual == 0)
        {
            vidaAtual = 3;
        }
        else
        {
            vidaAtual++;
        }
        AtualizarVidaVisual();
    }

    public void DefinirVidaAtual(int novaVida) => vidaAtual = Mathf.Clamp(novaVida, 0, pontosDeVida != null ? pontosDeVida.Count : 6);
    public int ObterVidaAtual() => vidaAtual;
    public int ObterVidaMaxima() => pontosDeVida.Count;

    public void DefinirNumeroOrbs(int novoNumero)
    {
        orbCount = novoNumero;
        UpdateOrbCountText();
    }

    public void DiminuirOrbs(int quantidade)
    {
        orbCount -= quantidade;
        if (orbCount < 0) orbCount = 0;
        UpdateOrbCountText();
    }

    public int ObterOrbs() => orbCount;

    public void DefinirBananaCount(int novasBananas) { bananaCount = novasBananas; }
    public int ObterBananaCount() => bananaCount;



    public void ColetarBanana()
    {
        bananaCount++;
        Debug.Log($"🍌 Banana coletada! Total: {bananaCount}");

        if (bananaCount % bananasPorVidaExtra == 0)
        {
            if (vidaAtual < pontosDeVida.Count)
            {
                vidaAtual++;
                AtualizarVidaVisual();
                Debug.Log($"❤️ Vida recuperada com bananas! Vida atual: {vidaAtual}/{pontosDeVida.Count}");
            }
            else
            {
                Debug.Log("🧍 Vida já está cheia — nada a recuperar.");
            }
        }
    }


    public void AplicarVidaMaxima(int novaVidaMaxima)
    {
        novaVidaMaxima = Mathf.Clamp(novaVidaMaxima, 1, 6);
        int coracoesFaltam = novaVidaMaxima - pontosDeVida.Count;

        if (coracoesFaltam > 0 && pontosDeVida.Count > 0)
        {
            Image referencia = pontosDeVida[0];
            for (int i = 0; i < coracoesFaltam; i++)
            {
                Image novo = Instantiate(referencia, referencia.transform.parent);
                pontosDeVida.Add(novo);
            }
            Debug.Log($"⚙️ Aplicada vida máxima estrutural: {novaVidaMaxima}");
        }

        AtualizarVidaVisual();
    }


    private void AtualizarVidaVisual()
    {
        if (pontosDeVida == null) return;

        for (int i = 0; i < pontosDeVida.Count; i++)
        {
            bool ativo = i < vidaAtual;
            pontosDeVida[i].enabled = ativo;
            pontosDeVida[i].gameObject.SetActive(true);
        }
    }
    public void RegistarCaixaDestruida(string id)
    {
        if (!caixasDestruidas.Contains(id))
        {
            caixasDestruidas.Add(id);
        }
    }
    public bool CaixaJaFoiDestruida(string id)
    {
        return caixasDestruidas.Contains(id);
    }
    public List<string> ObterCaixasDestruidas()
    {
        return new List<string>(caixasDestruidas);
    }
    public void CarregarCaixasDestruidas(List<string> ids)
    {
        caixasDestruidas = new List<string>(ids);
    }

    private void RemoverCaixasDestruidas()
    {
        var all = Object.FindObjectsByType<IdentificadorPersistente>(FindObjectsSortMode.None);

        foreach (var obj in all)
        {
            if ((obj.CompareTag("Box") || obj.CompareTag("Bau")) && CaixaJaFoiDestruida(obj.idUnico))
            {
                Destroy(obj.gameObject);
            }
        }
    }



    public void RegistarLareiraUsada(string id)
    {
        if (!lareirasUsadas.Contains(id))
        {
            lareirasUsadas.Add(id);
        }
    }

    public bool LareiraJaFoiUsada(string id)
    {
        return lareirasUsadas.Contains(id);
    }

    public List<string> ObterLareirasUsadas()
    {
        return new List<string>(lareirasUsadas);
    }

    public void CarregarLareirasUsadas(List<string> lista)
    {
        lareirasUsadas = new List<string>(lista);
    }

}
