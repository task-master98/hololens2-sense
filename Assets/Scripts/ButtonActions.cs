using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{


    public Renderer cameraPV;
    private bool isQuadActive = true;
    public void DestroyQuad()
    {
        isQuadActive = !isQuadActive; // Toggle the state
        cameraPV.gameObject.SetActive(isQuadActive);
    }
}
