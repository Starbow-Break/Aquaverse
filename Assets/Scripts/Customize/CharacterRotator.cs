using UnityEngine;

public class CharacterRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f;

    private Transform _characterRoot; // 회전시킬 캐릭터 루트 오브젝트
    private bool _isRotating = false;
    private Vector3 _lastMousePosition;

    private void Awake()
    {
        _characterRoot = transform;
    }

    private void Update()
    {
        // 우클릭 시작
        if (Input.GetMouseButtonDown(1))
        {
            _isRotating = true;
            _lastMousePosition = Input.mousePosition;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 우클릭 해제
        if (Input.GetMouseButtonUp(1))
        {
            _isRotating = false;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 우클릭 중 마우스 이동
        if (_isRotating)
        {
            float mouseX = Input.GetAxis("Mouse X");
            _characterRoot.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.World);
        }
    }
}
