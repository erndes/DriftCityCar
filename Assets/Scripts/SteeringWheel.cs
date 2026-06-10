using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SteeringWheel : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform rectDelta;
    private float wheelAngle = 0f;
    private float lastWheelAngle = 0f;
    private Vector2 centerPoint;

    [Header("Impostazioni")]
    public float maxSteerAngle = 200f; // Quanto può girare il volante
    public float releaseSpeed = 300f; // Velocità con cui torna dritto

    [HideInInspector] public float outValue = 0f; // Questo è il valore che leggerà l'auto (-1 a 1)
    private bool isHolding = false;

    void Start()
    {
        rectDelta = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Se non tocchiamo il volante, torna gradualmente al centro
        if (!isHolding && wheelAngle != 0f)
        {
            float deltaAngle = releaseSpeed * Time.deltaTime;
            if (Mathf.Abs(wheelAngle) <= deltaAngle) wheelAngle = 0f;
            else if (wheelAngle > 0f) wheelAngle -= deltaAngle;
            else wheelAngle += deltaAngle;
        }

        // Applica la rotazione visiva allo sprite
        rectDelta.localRotation = Quaternion.Euler(0, 0, -wheelAngle);
        
        // Calcola il valore finale tra -1 (tutto a sinistra) e 1 (tutto a destra)
        outValue = wheelAngle / maxSteerAngle;
    }

    public void OnPointerDown(PointerEventData data)
    {
        isHolding = true;
        centerPoint = RectTransformUtility.WorldToScreenPoint(data.pressEventCamera, rectDelta.position);
        lastWheelAngle = Vector2.Angle(Vector2.up, data.position - centerPoint);
    }

    public void OnDrag(PointerEventData data)
    {
        float newAngle = Vector2.Angle(Vector2.up, data.position - centerPoint);
        
        if ((data.position - centerPoint).sqrMagnitude > 400)
        {
            if (data.position.x > centerPoint.x) wheelAngle += newAngle - lastWheelAngle;
            else wheelAngle -= newAngle - lastWheelAngle;
        }
        
        wheelAngle = Mathf.Clamp(wheelAngle, -maxSteerAngle, maxSteerAngle);
        lastWheelAngle = newAngle;
    }

    public void OnPointerUp(PointerEventData data)
    {
        OnDrag(data);
        isHolding = false;
    }
}