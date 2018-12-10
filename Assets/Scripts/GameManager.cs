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
        if (Input.GetKeyDown(KeyCode.M)) ToggleMute();
    }

    void ToggleCursorState()
    {
        Cursor.visible = Cursor.visible == true ? false : true;
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void ToggleMute()
    {
        // Get master channel group.
        FMOD.ChannelGroup master;
        FMODUnity.RuntimeManager.LowlevelSystem.getMasterChannelGroup(out master);

        // Get mute status.
        bool isMuted = false;
        master.getMute(out isMuted);

        master.setMute(!isMuted); // Invert mute status.
    }
}
