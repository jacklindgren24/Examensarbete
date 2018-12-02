using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float sensitivityX = 1.0f;
    public float sensitivityY = 1.0f;
    public float maxAngle = 90;
    public float minAngle = -90;

    float x = 0;
    float y = 0;
    Transform player;

	void Start ()
    {
        player = transform.parent;

	}

    void Update ()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        x += dir.x * sensitivityX;
        y += dir.y * sensitivityY;
        y = Mathf.Clamp(y, minAngle, maxAngle);

        transform.localEulerAngles = new Vector2(-y, 0); // Rotate camera vertically.
        player.localEulerAngles = new Vector2(0, x); // Rotate player horizontally.
    }
}
