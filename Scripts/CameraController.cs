using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 15f;

    private Vector3 touchStart;

    public float initialOrthographicSize = 10f; 

    void Start()
    {
        // Set the initial orthographic size of the camera
        Camera.main.orthographicSize = initialOrthographicSize;
    }

    void Update()
    {
        // Camera movement with WSAD keys
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // Camera zoom with mouse scroll wheel
        float scrollInput = Input.GetAxisRaw("Mouse ScrollWheel");
        float zoomAmount = scrollInput * zoomSpeed;
        float newSize = Mathf.Clamp(Camera.main.orthographicSize - zoomAmount, minZoom, maxZoom);
        Camera.main.orthographicSize = newSize;

        
        if (Input.GetMouseButtonDown(1))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        
        if (Input.GetMouseButton(1))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += direction;
        }
    }
}
