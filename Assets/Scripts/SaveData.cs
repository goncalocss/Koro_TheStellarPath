using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public int playerHealth;
    public string currentScene;
    public Vector3 checkpointPosition; // Adiciona a posição do checkpoint
    public int orbs; // Adiciona o número de orbs
}
