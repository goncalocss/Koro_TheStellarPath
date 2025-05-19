using UnityEngine;

public class TempSaveData : MonoBehaviour
{
    public static TempSaveData Instance;
    public SaveData saveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Garante que só há um
        }
    }
}
