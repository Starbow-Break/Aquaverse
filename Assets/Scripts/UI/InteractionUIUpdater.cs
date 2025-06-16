using UnityEngine;

public class InteractionUIUpdater : MonoBehaviour
{
    [field: SerializeField] public Camera camera { get; private set; }

    [Header("UI Elements")] 
    [SerializeField] private GameObject _grabInteractionUI;
    [SerializeField] private GameObject _triggerInteractionUI;
    [SerializeField] private GameObject _grabEndInteractionUI;
    [SerializeField] private GameObject _triggerEndInteractionUI;

    public void SetActiveGrabEndInteractionUI(bool value)
    {
        _grabEndInteractionUI.SetActive(value);
    }
    
    public void SetActiveTriggerEndInteractionUI(bool value)
    {
        _triggerEndInteractionUI.SetActive(value);
    }

    public void SetActiveGrabInteractionUI(bool value)
    {
        _grabInteractionUI.SetActive(value);
    }
    
    public void SetGrabInteractionUIPositionWorld(Vector3 world)
    {
        var screenPos = camera.WorldToScreenPoint(world);
        
        var parentCanvas = _grabInteractionUI.GetComponentInParent<Canvas>();
        var canvasRect = parentCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out Vector2 rectPos
        );
        
        _grabInteractionUI.GetComponent<RectTransform>().anchoredPosition = rectPos;
    }
    
    public void SetActiveTriggerInteractionUI(bool value)
    {
        _triggerInteractionUI.SetActive(value);
    }
    
    public void SetTriggerInteractionUIPositionWorld(Vector3 world)
    {
        var screenPos = camera.WorldToScreenPoint(world);
        
        var parentCanvas = _triggerInteractionUI.GetComponentInParent<Canvas>();
        var canvasRect = parentCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out Vector2 rectPos
        );
        
        _triggerInteractionUI.GetComponent<RectTransform>().anchoredPosition = rectPos;
    }

    public void SetCamera(Camera camera)
    {
        this.camera = camera;
    }
}
