using UnityEngine;

[CreateAssetMenu(fileName = "Locations",menuName = "PatrolLocations", order = 1)]
public class PatrolLocations : ScriptableObject
{
    public PatrolObjects[] PatrolObjects;
}
[System.Serializable]
public struct PatrolObjects
{
    public Transform[] gameObjects;
}
