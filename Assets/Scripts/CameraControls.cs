using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{

    private Vector3 lastPosition;
    float mouseSensitivity = 0.1f;

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

        if (Camera.main.orthographicSize > 23f)
        {
            Camera.main.orthographicSize = 23f;
        }


        if (Input.GetMouseButtonDown(0))
        {
            lastPosition = Input.mousePosition;
        }

        if (mainController.statusCode != 5)
        {
            //mainController.statusCode = 4;

            if (Input.GetMouseButton(0))
            {
                bool overlapping = false;

                for (int i = 0; i < staff.Length; i++)
                {
                    Bounds bounds1 = staff[i].GetComponent<Renderer>().bounds;
                    bounds1.extents = new Vector3(3, 4, 2);

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
