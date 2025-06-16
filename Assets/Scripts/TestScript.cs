using System.Collections;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
