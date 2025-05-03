using UnityEngine;

// GameMode
// manages variables between different game versions
// created 3/5/25
// modified 3/5/25

[CreateAssetMenu(fileName = "GameMode", menuName = "ScriptableObjects/GameMode", order = 0)]
public class GameMode : ScriptableObject
{
    public string modeSubtitle = "";
    public bool debugMode = false;
}
