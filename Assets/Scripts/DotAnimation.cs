using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DotAnimator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private float interval = 0.5f;

    private bool _isRunning = false;

    private void Start()
    {
        _isRunning = true;
        AnimateDotsAsync().Forget(); // async 메서드를 백그라운드로 실행
    }

    private async UniTaskVoid AnimateDotsAsync()
    {
        int dotCount = 0;

        while (_isRunning)
        {
            dotCount = (dotCount % 4) + 1;
            tmpText.text = new string('.', dotCount);

            await UniTask.Delay(System.TimeSpan.FromSeconds(interval), cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }

    private void OnDestroy()
    {
        _isRunning = false; // 중지 플래그 설정 (필수는 아니지만 명확하게)
    }
}