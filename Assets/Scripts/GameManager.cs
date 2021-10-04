using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
0 - left mouse button
1 - right mouse button
*/
public class GameManager : MonoBehaviour
{
  //public settable fields
  public Image backdropImage;

  [Range(0.01f, 0.1f)] public float backdropParallax = 0.05f;
  [Range(1f, 10f)] public float mouseScrollAcceleration = 4.0f;
  [Range(3, 10)] public int startingOrthoSize = 5;
  
  // Zoom constants
  private const int MinZoomLevel = 2;
  private const int MaxZoomLevel = 40;

  private Camera _mainCamera;

  private Vector3? _dragStartPosition;
  private float _targetOrthoSize;
  
  void Awake()
  {
  }
  void Start()
  {
    _dragStartPosition = null;
    _mainCamera = Camera.main;
    _mainCamera.orthographicSize = startingOrthoSize;
    _targetOrthoSize = startingOrthoSize;
  }
  

  // Update is called once per frame

  void Update()
  {
    // Zoom In and out of screen
    ScrollWheelZoomCamera();
    // Pan world on drag
    HoldAndPanWorld();
  }

  void ScrollWheelZoomCamera()
  {
    /*
     Allows the player to scroll in and out of the main
     viewport. also lerps .
     */
    if (!_mainCamera)
    {
      return;
    }
    if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
    {
      _targetOrthoSize--;
    }
    else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
    {
      _targetOrthoSize++;

    }

    if (_targetOrthoSize <= MinZoomLevel)
      _targetOrthoSize = MinZoomLevel; 
    
    if (_targetOrthoSize >= MaxZoomLevel)
      _targetOrthoSize = MaxZoomLevel;
    
    _mainCamera.orthographicSize += (
      _targetOrthoSize - _mainCamera.orthographicSize
      ) * mouseScrollAcceleration * Time.deltaTime;
  }
  void HoldAndPanWorld()
  {
    // TODO smooth panning
    if (Input.GetMouseButton(UnitSelectionAndCommands.LeftMouseButton))
    {
      Vector3 mousePositionThisFrame = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
      if (_dragStartPosition is null)
      {
        _dragStartPosition = mousePositionThisFrame;
      }
      else
      {
        Vector3 deltaPosition = _dragStartPosition.Value - mousePositionThisFrame;
        _mainCamera.transform.Translate(deltaPosition.x , deltaPosition.y, 0);
        // move the background ever so slightly to give sense of parallax.
        
        backdropImage.transform.Translate(deltaPosition.x * -backdropParallax, deltaPosition.y * -backdropParallax, 0);
      }
    }
    else
    {
      //finish dragging
      _dragStartPosition = null;
    }
  }

}
