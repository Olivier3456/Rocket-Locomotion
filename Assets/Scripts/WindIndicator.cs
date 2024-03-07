using TMPro;
using UnityEngine;

public class WindIndicator : MonoBehaviour
{
    [SerializeField] private Transform windIndicatorVisual;
    [SerializeField] private TextMeshProUGUI speedText;
    [Space(20)]
    [SerializeField] private bool changeScale;
    [SerializeField, Tooltip("The length of the arrow when the wind is at its maximum value.")] private float maximumLocalScale = 2f;
    [Space(20)]
    [SerializeField] private float indicatorYAngleOffset = 270f;

    private Wind wind;

    private float refreshDelay = 1f;
    private float refreshTimer = 0;


    void Start()
    {
        wind = FindObjectOfType<Wind>();

        if (wind == null)
        {
            Debug.Log("No wind in the scene. Destroying all objects related to wind indications");
            Destroy(windIndicatorVisual.gameObject);
            Destroy(speedText.gameObject);
            Destroy(this);
            return;
        }
    }


    void Update()
    {
        if (wind == null)
        {
            return;
        }

        Vector3 direction = wind.Direction;
        Quaternion YAngleOffset = Quaternion.Euler(0, indicatorYAngleOffset, 0);
        float speed = wind.Magnitude;

        UpdateIndicatorDirection(direction, YAngleOffset);
        DisplayWindSpeed(speed);
    }


    private void UpdateIndicatorDirection(Vector3 direction, Quaternion YAngleOffset)
    {
        windIndicatorVisual.rotation = Quaternion.LookRotation(YAngleOffset * -direction);
    }


    private void DisplayWindSpeed(float speed)
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer > refreshDelay)
        {
            refreshTimer = 0f;
            float speedKmH = speed * 3.6f;
            speedText.text = speedKmH.ToString("0");
        }
    }
}
