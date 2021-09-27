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
  
  // Zoom constants
  private const int ZoomInLevel = 2;
  private const int ZoomOutLevel = 40;

  private Camera _mainCamera;

  private Vector3? _dragStartPosition;
  
  void Awake()
  {
  }
  void Start()
  {
    _dragStartPosition = null;
    _mainCamera = Camera.main;
  }

  // Update is called once per frame

  void Update()
  {
    // Zoom In and out of screen
    ScrollWheelZoomCamera();
    // Pan world
    HoldAndPanWorld();
  }

  void ScrollWheelZoomCamera()
  {
    /*
     Allows the player to scroll in and out of the main
     viewport. 
     */
    if (!_mainCamera)
    {
      return;
    }
    if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
    {
      _mainCamera.orthographicSize--;
    }
    else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
    {
      _mainCamera.orthographicSize++;

    }

    if (_mainCamera.orthographicSize <= ZoomInLevel)
      _mainCamera.orthographicSize = ZoomInLevel;

    if (_mainCamera.orthographicSize >= ZoomOutLevel)
      _mainCamera.orthographicSize = ZoomOutLevel;
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
