using Fusion;
using TMPro;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private GameObject _startButton;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private TMP_Text _dot;
    [SerializeField] private ConnectionData _initialConnection;

    public void Launch()
    {
            _startButton.SetActive(false);

            _text.text = "Connecting to Lobby";
            _text.gameObject.SetActive(true);
            _dot.gameObject.SetActive(true);
            
            _ = ConnectionManager.Instance.ConnectToRunner(_initialConnection, onFailed: OnConnectionFailed);
    }

    private void OnConnectionFailed(ShutdownReason reason)
    {
        _text.text = $"Failed: {reason}";
        _startButton.SetActive(true);
    }
}
