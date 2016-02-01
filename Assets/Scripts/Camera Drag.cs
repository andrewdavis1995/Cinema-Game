using UnityEngine;
using System.Collections;

public class CameraDrag : MonoBehaviour {
    
    private Vector3 lastPosition;
    float mouseSensitivity = 1.0f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize++;
        }

        if (Camera.main.orthographicSize < 2f)
        {
            Camera.main.orthographicSize = 2f;
        }

        if (Camera.main.orthographicSize > 23f)
        {
            Camera.main.orthographicSize = 23f;
        }

        if (Input.GetMouseButton(0))
        {
            lastPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Translate(delta.x * mouseSensitivity, delta.y * mouseSensitivity, 0);
            lastPosition = Input.mousePosition;
        }
    }
}
