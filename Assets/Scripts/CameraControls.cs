using UnityEngine;
using System.Collections;
using System;

public class CameraControls : MonoBehaviour
{

    private Vector3 lastPosition;
    float mouseSensitivity = 0.05f;

    //float minLeft;

    public Vector3 endPos = new Vector3(0, 0, -10);

    public float orthographicZoomSpeed = .2f;

    GameObject[] staff;

    public Controller mainController;
    public Popup_Controller popupController;

    float vertExtent;
    float horizExtent;

    public GameObject bottomBar;

    float bottomBarHeight;

    float minX, maxX, minY, maxY;

    // Use this for initialization
    void Start()
    {
        staff = GameObject.FindGameObjectsWithTag("Staff");
        endPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // if it is allowable to 
        if (mainController.statusCode < 3 && mainController.statusCode != 1 || mainController.statusCode == 50)
        {
            vertExtent = Camera.main.orthographicSize;
            horizExtent = vertExtent * Screen.width / Screen.height;
            bottomBarHeight = vertExtent / bottomBar.transform.localScale.y;

            minX = horizExtent;
            maxX = 80f - (Camera.main.orthographicSize * 1.79f);
            minY = vertExtent;
            maxY = (40f - (Camera.main.orthographicSize * 1.25f)) * 0.8f;

            // moving to a staff members position
            if (transform.position != endPos)
            {
                transform.position = Vector3.Lerp(transform.position, endPos, Time.deltaTime * 5);
            }
            else if (mainController.statusCode == 50 && !popupController.staffList.gameObject.active)
            {
                mainController.statusCode = 6;
            }


            // zooming
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

            if (Camera.main.orthographicSize > 15f)
            {
                Camera.main.orthographicSize = 15f;
            }

            if (Input.GetMouseButtonDown(0))
            {
                lastPosition = Input.mousePosition;
            }


            popupController.objectInfo.SetActive(false);
            popupController.closeInfo.SetActive(false);

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
                    endPos = transform.position;
                    lastPosition = Input.mousePosition;
                }
            }
        }


        // check if out of bounds
        Vector3 currPos = (Camera.main.transform.position);
        float x = currPos.x;
        float y = currPos.y;

        if (mainController.statusCode != 10 && mainController.statusCode != 8 && mainController.statusCode != 73)
        {
            if (x < minX)
            {
                currPos.x = minX;
            }
            if (y < minY - bottomBarHeight)
            {
                currPos.y = minY - bottomBarHeight;
            }
            if (x > maxX)
            {
                currPos.x = maxX;
            }
            if (y > maxY)
            {
                currPos.y = maxY;
            }

        }
        currPos.z = -10;
        Camera.main.transform.position = (currPos);

    }
}
