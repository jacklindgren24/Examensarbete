using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

	void Awake ()
    {
        if (instance == null) instance = this; else Destroy(gameObject);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) ToggleCursorState();
    }

    void ToggleCursorState()
    {
        Cursor.visible = Cursor.visible == true ? false : true;
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
