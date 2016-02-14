using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{

    private Vector3 lastPosition;
    float mouseSensitivity = 0.05f;
    
    public float orthographicZoomSpeed = .4f;

    GameObject[] staff;

    Controller mainController;

    // Use this for initialization
    void Start()
    {
        staff = GameObject.FindGameObjectsWithTag("Staff");
        mainController = GameObject.Find("Central Controller").GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize++;
        }

        if (Camera.main.orthographicSize < 4f)
        {
            Camera.main.orthographicSize = 4f;
        }

        if (Camera.main.orthographicSize > 18f)
        {
            Camera.main.orthographicSize = 18f;
        }


        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = Input.mousePosition;
        }

        if (mainController.statusCode != 5 && mainController.statusCode != 1 && mainController.statusCode != 2)
        {
            /// pinch zoom controls
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                Vector3 prevTouch1 = touch1.position - touch1.deltaPosition;
                Vector3 prevTouch2 = touch2.position - touch2.deltaPosition;

                float prevTouchDeltaMagnitude = (prevTouch1 - prevTouch2).magnitude;
                float touchDeltaMagnitude = (touch1.position - touch2.position).magnitude;

                // check if fingers are getting further apart (zoom in), or close together (zoom out)
                float deltaDiff = prevTouchDeltaMagnitude - touchDeltaMagnitude;

                Camera.main.orthographicSize += deltaDiff * orthographicZoomSpeed;

                if (Camera.main.orthographicSize > 18f)
                {
                    Camera.main.orthographicSize = 18f;
                }
                if (Camera.main.orthographicSize < 4f)
                {
                    Camera.main.orthographicSize = 4f;
                }


            }

            //mainController.statusCode = 4;

            if (Input.GetMouseButton(0))
            {
                bool overlapping = false;

                for (int i = 0; i < staff.Length; i++)
                {
                    Bounds bounds1 = staff[i].GetComponent<Renderer>().bounds;
                    bounds1.extents = new Vector3(2, 3, 1);

                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mousePos.z = 0;

                    if (bounds1.Contains(mousePos))
                    {
                        overlapping = true;
                        break;
                    }
                }

                if (!overlapping)
                {
                    Vector3 delta = Input.mousePosition - lastPosition;
                    Camera.main.transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, 0);
                    lastPosition = Input.mousePosition;
                }
            }
            //else
            //{
            //    mainController.statusCode = 0;
            //}
        }

        

    }
}
