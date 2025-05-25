using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public int playerHealth;
    public string currentScene;
    public Vector3 checkpointPosition; // Adiciona a posição do checkpoint
    public int orbs; // Adiciona o número de orbs
    public int vidaMaxima;
    public int bananaCount;
    public int nivelArma;
    public List<string> lareirasUsadas = new List<string>();


    public List<string> caixasDestruidas = new List<string>();
}
