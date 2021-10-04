using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugHelper : MonoBehaviour
{
    // Start is called before the first frame update

    private Text _text;
    private Camera _mainCamera;
    void Start()
    {
        _text = GetComponent<Text>();
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 mouseViewport = _mainCamera.ScreenToViewportPoint(mousePosition);
        
        // Update mouse position in debug text.
        _text.text =
            $"Mouse Position: ({Input.mousePosition.x},{Input.mousePosition.y})\n" +
            $"World Position: ({mouseWorld.x:0.0},{mouseWorld.y:0.0})\n" + 
            $"Viewport Position: ({mouseViewport.x:0.0},{mouseViewport.y:0.0})\n" + 
            $"OrthoSize: {_mainCamera.orthographicSize:0.00}";
        

    }
}
