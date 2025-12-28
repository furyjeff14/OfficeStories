using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI")]
    public RectTransform background;
    public RectTransform handle;

    [Header("Settings")]
    public float radius = 100f;

    [Header("Input")]
    public InputReader inputReader; // Optional

    private Canvas canvas;
    private Camera uiCamera;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // Hide joystick initially
        background.gameObject.SetActive(false);
        handle.localPosition = Vector2.zero;
    }

    private void Start()
    {
        if (inputReader == null)
            inputReader = InputReader.Instance;
    }

    // --- Called when screen is touched anywhere ---
    public void OnPointerDown(PointerEventData eventData)
    {

        background.gameObject.SetActive(true);

        // Convert screen point to canvas local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            uiCamera,
            out Vector2 localPos
        );

        background.localPosition = localPos;
        handle.localPosition = Vector2.zero;

        inputReader?.SetMoveFromJoystick(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            uiCamera,
            out Vector2 pos
        );

        Vector2 clamped = Vector2.ClampMagnitude(pos, radius);
        handle.localPosition = clamped;

        inputReader?.SetMoveFromJoystick(clamped / radius);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        handle.localPosition = Vector2.zero;

        inputReader?.SetMoveFromJoystick(Vector2.zero);
    }
}
