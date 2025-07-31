using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSceneLevelLoader : MonoBehaviour
{
    private LevelLoader levelLoader;

    void Start()
    {
        levelLoader = LevelLoader.Instance;
        levelLoader.EndTransition();
    }

    public void LoadScene(int sceneToLoad)
    {
        levelLoader.LoadScene(sceneToLoad);
    }
}
