using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{


    public Renderer cameraPV;
    private bool isQuadActive = false;
    private float flashInterval = 10.0f;
    private float visibleDuration = 5.0f;
    private Coroutine flashCoroutine;
    private Renderer renderer;

    void Start()
    {
        renderer = cameraPV.GetComponent<Renderer>();
        flashCoroutine = null;
    }
    public void DestroyQuad()
    {
        Debug.Log("Toggler method called!");
        isQuadActive = !isQuadActive; // Toggle the state
        renderer.enabled = isQuadActive;
    }

    public void StartFlashing()
    {
        if (flashCoroutine == null)
        {
            Debug.Log("Flash Started");
            flashCoroutine = StartCoroutine(FlashQuad());
        }
    }

    public void StopFlashing()
    {
        if (flashCoroutine != null)
        {
            Debug.Log("Flash Stopped");
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
            isQuadActive = true;
            renderer.enabled = isQuadActive;
        }
    }

    private IEnumerator FlashQuad()
    {
        renderer.enabled = true;        
        while (true)
        {
            renderer.enabled = true;
            yield return new WaitForSeconds(visibleDuration);

            renderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);    


        }
    }
}
