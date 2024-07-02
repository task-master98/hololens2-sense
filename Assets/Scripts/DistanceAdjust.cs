using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class DistanceAdjust : MonoBehaviour
{
    public PinchSlider distanceSlider; // Reference to the PinchSlider
    public GameObject targetObject; // Reference to the GameObject with the RadialView solver
    public float minDistanceRange = 0.5f; // Minimum distance slider can set
    public float maxDistanceRange = 3.0f; // Maximum distance slider can set

    private RadialView radialView;

    void Start()
    {
        if (distanceSlider == null || targetObject == null)
        {
            Debug.LogError("DistanceSlider or targetObject is not assigned.");
            return;
        }

        // Get the RadialView component from the target object
        radialView = targetObject.GetComponent<RadialView>();

        if (radialView == null)
        {
            Debug.LogError("RadialView component not found on targetObject.");
            return;
        }

        // Set the slider's initial value to match the current MinDistance
        distanceSlider.SliderValue = (radialView.MinDistance - minDistanceRange) / (maxDistanceRange - minDistanceRange);

        // Add a listener to the slider's OnValueUpdated event
        distanceSlider.OnValueUpdated.AddListener(OnSliderUpdated);
    }

    private void OnSliderUpdated(SliderEventData eventData)
    {
        // Map the slider value to the desired distance range
        float distance = Mathf.Lerp(minDistanceRange, maxDistanceRange, eventData.NewValue);

        // Set both MinDistance and MaxDistance to the same value to keep a fixed distance
        radialView.MinDistance = distance;
        radialView.MaxDistance = distance;
    }

    void OnDestroy()
    {
        // Remove the listener when the object is destroyed to prevent memory leaks
        if (distanceSlider != null)
        {
            distanceSlider.OnValueUpdated.RemoveListener(OnSliderUpdated);
        }
    }
}
