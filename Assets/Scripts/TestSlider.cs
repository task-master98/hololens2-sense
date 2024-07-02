using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class TestSlider : MonoBehaviour
{
    public PinchSlider pinchSlider;
    public Renderer pv_image;
    private float intialXscale;
    private float InitialYscale;
    private float lowerXlimit;
    private float upperXlimit;
    private float lowerYlimit;
    private float upperYlimit;
    [SerializeField]
    public float scaleFactorX = 0.2f;
    [SerializeField]
    public float scaleFactorZ = 0.4f;
    void Awake()
    {
        if ((pinchSlider == null) || (pv_image == null))
        {
            Debug.LogError("Objects not intialized");
            return;
        }

        SetSliderValue(0.5f);
        intialXscale = pv_image.transform.localScale.x;
        InitialYscale = pv_image.transform.localScale.y;

        DisplayValues(intialXscale, InitialYscale);
        SetupLimits();
        // Subscribe to slider events
        pinchSlider.OnValueUpdated.AddListener(OnSliderValueUpdated);        
    }

    private void OnDestroy()
    {
        // Unsubscribe from slider events
        if (pinchSlider != null)
        {
            pinchSlider.OnValueUpdated.RemoveListener(OnSliderValueUpdated);            
        }
    }

    // Event handler for when the slider value is updated
    private void OnSliderValueUpdated(SliderEventData eventData)
    {
        Debug.Log("Slider Value Updated: " + eventData.NewValue);
        AdjustQuadSize(eventData.NewValue);
    }

   
    private void AdjustQuadSize(float value)
    {
        if (pv_image != null)
        {
           Vector3 currentScale = pv_image.transform.localScale;
           float newSizeX = Mathf.Lerp(lowerXlimit, upperXlimit, value);
           float newSizeY = Mathf.Lerp(lowerYlimit, upperYlimit, value);      
           DisplayValues(newSizeX, newSizeY);     
           Vector3 newScale = new Vector3(newSizeX, currentScale.y, newSizeY);
           pv_image.transform.localScale = newScale;
        }
    }
    
    public void SetSliderValue(float value)
    {
        if (pinchSlider != null)
        {
            pinchSlider.SliderValue = value;
        }
    }

    private void DisplayValues(float valueX, float valueY)
    {
        Debug.Log("X scale value: " + valueX);
        Debug.Log("Y scale value: " + valueY);
    }

    private void SetupLimits()
    {
        lowerXlimit = (float)(1.0 - scaleFactorX) * intialXscale;
        upperXlimit = (float)(1.0 + scaleFactorX) * intialXscale;
        lowerYlimit = (float)(1.0 - scaleFactorZ) * InitialYscale;
        upperYlimit = (float)(1.0 + scaleFactorZ) * InitialYscale;

        DisplayValues(lowerXlimit, lowerYlimit);
        DisplayValues(upperXlimit, upperYlimit);
        Debug.Log("Limits set!");
    }
}
