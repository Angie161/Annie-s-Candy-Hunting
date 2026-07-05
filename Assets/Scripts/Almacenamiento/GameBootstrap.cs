using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    public static GameBootstrap Instance;

    void Awake()
    {
        // si ya existe uno, destruye este duplicado
        if (Instance != null)
        {
            Debug.LogWarning("Bootstrap duplicado detectado, se procede a borrar");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // o hace persistente entre escenas
        DontDestroyOnLoad(gameObject);

        // carga save solo una vez
        SaveData.Data = SaveSystem.Load();
    }
}