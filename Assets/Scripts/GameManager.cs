using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    List<Spawner> spawners = new List<Spawner>();

	void Awake ()
    {
        if (instance == null) instance = this; else Destroy(gameObject);
	}

    void Start()
    {
        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawners.Add(spawner.GetComponent<Spawner>());
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) ToggleCursorState();
        if (Input.GetKeyDown(KeyCode.M)) ToggleMute();
        if (Input.GetKeyDown(KeyCode.P)) ToggleSpawners();
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

    void ToggleSpawners()
    {
        foreach (Spawner spawner in spawners)
        {
            spawner.isPaused = !spawner.isPaused;
        }
    }
}
