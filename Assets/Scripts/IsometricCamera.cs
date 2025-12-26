using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target;

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 6.5f, -3.5f); // Default isometric offset
    public float followSpeed = 5f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // Lock rotation for true isometric view
        transform.rotation = Quaternion.Euler(60f, 0, 0f); // Classic isometric angle
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Smooth follow
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        }

        HandleZoom();
    }

    public void Initialize(Transform _target)
    {
        target = _target;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            // Adjust offset based on zoom
            offset += offset.normalized * -scroll * zoomSpeed;
            offset = Vector3.ClampMagnitude(offset, maxZoom);
            if (offset.magnitude < minZoom)
                offset = offset.normalized * minZoom;
        }
    }
}
