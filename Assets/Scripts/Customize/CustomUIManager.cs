using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class CustomUIManager : MonoBehaviour
{
    [SerializeField] private List<PartLineUI> partLines;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private SaveLoadPopupUI popupUI;

    private readonly string sceneName = "StartScene";
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Start()
    {
        foreach (var line in partLines)
        {
            if (line == null) continue;

            var partType = line.partType;
            if (!CustomizationManager.Instance.PartOptions.TryGetValue(partType, out var options))
            {
                Debug.LogWarning($"[CustomizationUI] 옵션 없음: {partType}");
                continue;
            }

            line.Init(partType, options);
        }
        
        if (CustomizationManager.Instance.HasSavedData())
        {
            // CustomizationManager.Instance.LoadCustomization();
            foreach (var line in partLines)
            {
                line.RefreshUI();
            }
        }
    }
    
    public void OnSaveClicked()
    {
        if (!CustomizationManager.Instance.IsCurrentSelectionEqualToSavedData())
        {
            popupUI.Show("Already have data\nWant to Overwrite?", () =>
            {
                CustomizationManager.Instance.SaveCustomization();
            });
        }
        else
        {
            CustomizationManager.Instance.SaveCustomization();
        }
    }

    public void OnLoadClicked()
    {
        if (!CustomizationManager.Instance.IsCurrentSelectionEqualToSavedData())
        {
            popupUI.Show("Load saved data?\nIt will Reset Data", () =>
            {
                CustomizationManager.Instance.LoadCustomization();
                foreach (var line in partLines)
                {
                    line.RefreshUI();
                }

                Debug.Log("[CustomizationUI] 저장된 커스터마이징 로드 및 UI 반영 완료");
            });
        }
        else
        {
            Debug.LogWarning("[CustomizationUI] 저장된 데이터랑 같음");
        }
    }

    public async void OnBackClicked()
    {
        AudioManager.Instance.StopBGM();
        await new WaitUntil(() => AudioManager.Instance.StopBGMCoroutine == null);
        
        if (!CustomizationManager.Instance.IsCurrentSelectionEqualToSavedData())
        {
            popupUI.Show("You didn't save data\nWant to Save?", 
                async () =>
                {
                    await CustomizationManager.Instance.SyncCurrentSelectionsFromSavedData();
                    SceneManager.LoadScene(sceneName);
                },
                () =>
                {
                    SceneManager.LoadScene(sceneName);
                }
            );
            
            Debug.Log("[CustomizationUI] 커스터마이징 갱신 완료");
        }
        else
        {
            SceneManager.LoadScene(sceneName);
            Debug.LogWarning("[CustomizationUI] 저장된 데이터랑 같음");
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sceneIndex = scene.buildIndex;
        AudioManager.Instance.PlayBGM((BGMType)sceneIndex);
    }
}
