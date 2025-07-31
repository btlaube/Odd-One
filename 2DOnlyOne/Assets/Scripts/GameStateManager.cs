using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState gameState;
    public static GameStateManager Instance;

    void Start()
    {
        ResetGameState();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        if (gameState.level >= gameState.levelsToWin)
        {
            WinGame();
        }
    }

    public void IncrementLevel()
    {
        gameState.level++;
    }

    public void ResetGameState()
    {
        gameState.level = 1;
    }

    public void PassLevel()
    {
        IncrementLevel();
        if (gameState.level < gameState.levelsToWin) LevelLoader.Instance.ReloadScene();    
    }

    public void FailLevel()
    {
        ResetGameState();
        LevelLoader.Instance.ReloadScene();
    }

    public void WinGame()
    {
        ResetGameState();
        LevelLoader.Instance.LoadScene(2);
        GetComponent<AudioHandler>().Play("Music");
    }

}
