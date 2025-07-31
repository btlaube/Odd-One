using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState", order = 1)]
public class GameState : ScriptableObject
{
    public int level = 1;
    public int levelsToWin = 6;
}
