using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraMovement : MonoBehaviour
{
    [SerializeField] GameObject center_of_titles;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    public float camera_zoom_out_speed = 0.5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, center_of_titles.transform.position, ref velocity, smoothTime);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, center_of_titles.transform.localScale.x, camera_zoom_out_speed * Time.deltaTime);
    }
}
