using UnityEngine;

public class InteractionUIHolder : MonoBehaviour
{
    [SerializeField] private InteractionUIUpdater _player1Updater;
    [SerializeField] private InteractionUIUpdater _player2Updater;

    public static InteractionUIHolder Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    
    public InteractionUIUpdater GetInteractionUIUpdater(int playerId)
    {
        if (playerId == 1)
        {
            return _player1Updater;
        }

        if (playerId == 2)
        {
            return _player2Updater;
        }

        return null;
    }
}
