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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (orbCountText != null)
        {
            UpdateOrbCountText();
        }

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        // ✅ Carregar dados de save se existirem
        if (TempSaveData.Instance != null && TempSaveData.Instance.saveData != null)
        {
            SaveData data = TempSaveData.Instance.saveData;

            if (player != null)
            {
                player.transform.position = data.playerPosition;
                DefinirVidaAtual(data.playerHealth);
                DefinirNumeroOrbs(data.orbs);

                Debug.Log("✅ Jogador reposicionado com dados do save.");
            }

            Destroy(TempSaveData.Instance.gameObject); // limpa após usar
        }
        else
        {
            Debug.Log("ℹ️ Nenhum TempSaveData encontrado — provavelmente novo jogo.");
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("🔁 Cena carregada: " + scene.name);

        // Reencontra o Player
        player = FindObjectOfType<Player>();

        // Reencontra o texto de orbs
        orbCountText = FindObjectOfType<TextMeshProUGUI>();
        if (orbCountText != null)
            UpdateOrbCountText();

        // Recriar lista de vidas (a maior estrutura com imagens)
        GameObject[] candidatos = GameObject.FindGameObjectsWithTag("PontosDeVida");
        GameObject escolhido = null;
        int maxCount = 0;

        foreach (var candidato in candidatos)
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

            Debug.Log($"✅ {pontosDeVida.Count} pontos de vida carregados de: {escolhido.name}");
        }
        else
        {
            Debug.LogWarning("⚠️ Nenhum objeto com tag 'PontosDeVida' e imagens foi encontrado.");
        }

        // Atualiza os inimigos (importante se a cena foi recarregada)
        foreach (var inimigo in FindObjectsOfType<LizardAI_Test>())
        {
            inimigo.ReiniciarDeteccao();
            Debug.Log("🎯 Inimigo atualizado no OnSceneLoaded.");
        }

        foreach (var boss in FindObjectsOfType<BossAI>())
        {
            boss.ReeniciarDetatacaoBoos();
            Debug.Log("🎯 Boss atualizado no OnSceneLoaded.");
        }

        // Por padrão, o jogador está vivo após carregar cena
        playerVivo = true;
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

    public int ObterOrbs()
    {
        return orbCount;
    }

    public void ResetOrbCount()
    {
        orbCount = 0;
        UpdateOrbCountText();
    }

    public void ReceberDano()
    {
        if (player == null)
        {
            Debug.LogWarning("⚠️ Tentativa de aplicar dano, mas o player está null.");
            return;
        }

        // Verifica o estado real do player, não só o GameManager
        if (!player.estaVivo)
        {
            Debug.Log("⛔ Player ainda não está ativo (estaVivo = false), dano ignorado.");
            return;
        }

        acertosRecebidos++;
        Debug.Log($"💥 Jogador levou dano ({acertosRecebidos})");

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
            if (vidaAtual < pontosDeVida.Count)
                pontosDeVida[vidaAtual].enabled = false;
        }

        if (vidaAtual == 0)
        {
            playerVivo = false;
            if (player != null)
                player.Morrer();
        }
    }

    public void DefinirCheckpoint(Vector3 posicao)
    {
        ultimoCheckpoint = posicao;
        temCheckpoint = true;
        Debug.Log("✅ Checkpoint guardado em: " + posicao);
    }

    public Vector3 ObterCheckpoint()
    {
        return ultimoCheckpoint;
    }

    public bool TemCheckpoint()
    {
        return temCheckpoint;
    }

    public void Respawn()
    {
        if (player == null)
        {
            Debug.LogError("❗ Não há referência ao Player no respawn.");
            return;
        }

        // Reset do estado do jogador
        player.ResetarEstado();

        // Move o jogador para o último checkpoint
        player.transform.position = ultimoCheckpoint;
        player.gameObject.tag = "Player";

        // Reativa os componentes essenciais
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        var rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        var col = player.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        var movimento = player.GetComponent<PlayerMovement>();
        if (movimento != null) movimento.enabled = true;

        // Repor vida e marcar como vivo
        ReporVida();
        playerVivo = true;

        // Atualiza os inimigos
        foreach (var inimigo in FindObjectsOfType<LizardAI_Test>())
        {
            inimigo.ReiniciarDeteccao();
            Debug.Log("🎯 Inimigo atualizado após respawn.");
        }

        //logica para reset do boos
        foreach (var boss in FindObjectsOfType<BossAI>())
        {
            boss.ReeniciarDetatacaoBoos();
            Debug.Log("🎯 Boss atualizado após respawn.");
        }

        Debug.Log("✅ Respawn concluído com sucesso.");
    }





    public void ReporVida()
    {
        if (pontosDeVida == null || pontosDeVida.Count == 0)
        {
            Debug.LogWarning("⚠️ Lista de pontos de vida está vazia no ReporVida!");
            return;
        }

        vidaAtual = pontosDeVida.Count;
        acertosRecebidos = 0;

        for (int i = 0; i < pontosDeVida.Count; i++)
        {
            if (pontosDeVida[i] != null)
            {
                pontosDeVida[i].enabled = true; // Ativa imagem
                pontosDeVida[i].gameObject.SetActive(true); // Ativa GameObject se estiver desativado
            }
        }

        Debug.Log($"❤️ Vidas repostas: {vidaAtual}");
    }

    public int ObterVidaAtual()
    {
        return vidaAtual;
    }

    public int ObterVidaMaxima()
    {
        return pontosDeVida.Count;
    }

    public void DefinirVidaAtual(int novaVida)
    {
        vidaAtual = novaVida;
    }

    public void DefinirNumeroOrbs (int novoNumero)
    {
        orbCount = novoNumero;
    }


}
