using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLimiter : MonoBehaviour
{
    private float z_thresh;
    public Renderer vlc_image;
    public Renderer pv_image;
    private Vector3 pv_position;
    private Vector3 vlc_position;
    private float z_position_diff;

    void Start()
    {
        pv_position = pv_image.transform.position;
        vlc_position = vlc_image.transform.position;
        z_position_diff = Math.Abs(pv_position.z - vlc_position.z);

    }
    void Update()
    {
        
        float current_pos_diff = Math.Abs(pv_image.transform.position.z - vlc_image.transform.position.z);
        Vector3 current_pv_position = pv_image.transform.position;
        if (current_pos_diff < z_position_diff)
        {
            current_pv_position.z = pv_position.z;
            pv_image.transform.position = current_pv_position;
        }
    }
}
