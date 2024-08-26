using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class DistanceAdjust : MonoBehaviour
{
    public PinchSlider distanceSlider; // Reference to the PinchSlider
    public GameObject targetObject; // Reference to the GameObject with the RadialView solver
    public float minDistanceRange = 0.5f; // Minimum distance slider can set
    public float maxDistanceRange = 3.0f; // Maximum distance slider can set
    public float initialDistance = 1.5f;

    private Orbital orbital;    

    void Start()
    {
        if (distanceSlider == null || targetObject == null)
        {
            Debug.LogError("DistanceSlider or targetObject is not assigned.");
            return;
        }

        // Get the RadialView component from the target object
        orbital = targetObject.GetComponent<Orbital>();

        if (orbital == null)
        {
            Debug.LogError("Orbital component not found on targetObject.");
            return;
        }

        // Set the slider's initial value to match the current MinDistance
        distanceSlider.SliderValue = (initialDistance - minDistanceRange) / (maxDistanceRange - minDistanceRange);;   
        
        // Add a listener to the slider's OnValueUpdated event
        distanceSlider.OnValueUpdated.AddListener(OnSliderUpdated);
        SetSliderValue(0.5f);
    }

    private void OnSliderUpdated(SliderEventData eventData)
    {
        // Map the slider value to the desired distance range
        float distance = Mathf.Lerp(minDistanceRange, maxDistanceRange, eventData.NewValue);

        // Set both MinDistance and MaxDistance to the same value to keep a fixed distance
        Vector3 localOffset = orbital.LocalOffset;
        localOffset.z = distance;
        orbital.LocalOffset = localOffset;
    }

    // private void LateUpdate()
    // {
    //     MaintainHeadLevel();
    // }

    // private void MaintainHeadLevel()
    // {
    //     Vector3 headPosition = Camera.main.transform.position;
    //     Vector3 headForward = Camera.main.transform.forward;

    //     Vector3 desiredPosition = headPosition + headForward * radialView.MinDistance;
    //     Vector3 smoothedPosition = Vector3.SmoothDamp(targetObject.transform.position, desiredPosition, ref velocity, smoothTime);
    //     targetObject.transform.position = smoothedPosition;

    //     Quaternion targetRotation = Quaternion.LookRotation(targetObject.transform.position - headPosition);
    //     targetObject.transform.rotation = Quaternion.Slerp(targetObject.transform.rotation, targetRotation, Time.deltaTime / smoothTime);
    // }

    void OnDestroy()
    {
        // Remove the listener when the object is destroyed to prevent memory leaks
        if (distanceSlider != null)
        {
            distanceSlider.OnValueUpdated.RemoveListener(OnSliderUpdated);
        }
    }
    public void SetSliderValue(float value)
    {
        if (distanceSlider != null)
        {
            distanceSlider.SliderValue = value;
        }
    }
}
