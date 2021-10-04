using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class StarCanvasManager : MonoBehaviour
{

    private readonly List<GameObject> _buttons = new List<GameObject>();
    private int _knownChildren = 0;

    private Camera _mainCamera = null;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        
        
        // Here we check for a difference in how the canvas hierachy.
        int childCount = gameObject.transform.hierarchyCount;
        
        if ( childCount == _knownChildren)
            return;

        FindAndAddAllChildren();
        _knownChildren = childCount;
    }

    private void FindAndAddAllChildren()
    {
        _buttons.Clear();
        foreach (var button in GetComponentsInChildren<Button>())
        {
            _buttons.Add(button.gameObject);
        }
    }
    
    public bool PointMightClickButton(Vector2 point)
    {
        /* Here we will check to see if a certain point is contained within
           a list of all of our child button objects.
        */

        foreach (GameObject button in _buttons)
        {
            RectTransform rt = button.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(rt, point, _mainCamera))
                return true;
        }

        return false;
    }
}
