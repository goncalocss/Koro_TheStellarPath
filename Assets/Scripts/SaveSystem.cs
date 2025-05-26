using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveSystem
{
    private static string folderStandalone = Application.persistentDataPath + "/Saves/";
    private static string fileName = "save.json";
    private static string webGLKey = "SaveGameJson";

    // Chave e IV para encriptação (16 bytes = 128 bits)
    private static readonly byte[] key = Encoding.UTF8.GetBytes("1234567890123456");
    private static readonly byte[] iv = Encoding.UTF8.GetBytes("abcdefghijklmnop");

    private static string GetSavePath()
    {
        return Path.Combine(folderStandalone, fileName);
    }

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string encryptedJson = EncryptString(json);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            PlayerPrefs.SetString(webGLKey, encryptedJson);
            PlayerPrefs.Save();
            Debug.Log("💾 Jogo guardado encriptado no PlayerPrefs (WebGL)");
        }
        else
        {
            if (!Directory.Exists(folderStandalone))
                Directory.CreateDirectory(folderStandalone);

            string savePath = GetSavePath();
            File.WriteAllText(savePath, encryptedJson);
            Debug.Log("💾 Jogo guardado encriptado em: " + savePath);
        }
    }

    public static SaveData LoadGame()
    {
        string encryptedJson;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (!PlayerPrefs.HasKey(webGLKey))
            {
                Debug.LogWarning("❌ Nenhum save encontrado no PlayerPrefs (WebGL).");
                return null;
            }
            encryptedJson = PlayerPrefs.GetString(webGLKey);
        }
        else
        {
            string savePath = GetSavePath();
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("❌ Nenhum ficheiro de save encontrado.");
                return null;
            }
            encryptedJson = File.ReadAllText(savePath);
        }

        string json = DecryptString(encryptedJson);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool SaveExists()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return PlayerPrefs.HasKey(webGLKey);
        }
        else
        {
            return File.Exists(GetSavePath());
        }
    }

    public static void DeleteSave()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (PlayerPrefs.HasKey(webGLKey))
            {
                PlayerPrefs.DeleteKey(webGLKey);
                Debug.Log("🗑 Save apagado do PlayerPrefs (WebGL).");
            }
        }
        else
        {
            string savePath = GetSavePath();
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("🗑 Save apagado do disco.");
            }
        }
    }

    // Função para encriptar string com AES
    private static string EncryptString(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            byte[] encrypted;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                encrypted = ms.ToArray();
            }
            return Convert.ToBase64String(encrypted);
        }
    }


    // Função para desencriptar string com AES
    private static string DecryptString(string cipherText)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(buffer))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

}
