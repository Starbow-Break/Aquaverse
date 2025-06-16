using UnityEngine;

[CreateAssetMenu(fileName = "ConnectionData", menuName = "ScriptableObject/ConnectionData", order = 0)]
public class ConnectionData : ScriptableObject
{
    public enum ConnectionTarget { Lobby, Game, }
    public enum ConnectionID { MainLobby, FirstGame, SecondGame }
    
    public string Name;
    [Space]
    public ConnectionID ID;
    public ConnectionTarget Target;
    [Space]
    public int MaxClients = 20;
    public int SceneIndex;
}
