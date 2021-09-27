using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.U2D;

public class StarUnitInterface : MonoBehaviour
{
    /*
     This class is simply used to group together Interface concerns on the star unit
     such as reticle activation, animation, and otherwise.
     */
    
    public SpriteRenderer selectionReticle;
    public GameObject contextMenuPrefab;
    public Canvas parentCanvas;

    private UnitMenuManager _menu = null;

    private Vector3 _offset;
    
    private bool _isSelected = false;
    private Camera _mainCamera;
    private StarUnit _starUnit;
    void Start()
    { 
        //get sibling starunit controller.
        _starUnit = GetComponent<StarUnit>();
        _mainCamera = Camera.main;
        _offset = transform.Find("MenuPoint").position;
        
    }

    // Update is called once per frame
    void Update()
    {
        // update interfaces for all cases.
        
        //update reticle
        if (selectionReticle.gameObject.activeInHierarchy != _isSelected)
        {
            selectionReticle.gameObject.SetActive(_isSelected);
        }

        LazyLoadMenu();
        ShowHideMenu();
    }

    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
    }

    void ShowHideMenu()
    {
        //check that we have the menu loaded yet
        // if we dont, we don't have to be here
        if (!_menu)
            return;
        
        _menu.SetActive(_isSelected);

        if (_isSelected)
        {
            Vector3 screenPoint = _mainCamera.WorldToViewportPoint(transform.position);
            _menu.transform.position = screenPoint + _offset;
        }
    }
    void LazyLoadMenu()
    {
        if (!_isSelected)
            return;
        // Load the menu if we don't have it already
        if (_menu)
            return;
        // we create our own copy of a menu
        GameObject contextMenu = Instantiate(contextMenuPrefab, parentCanvas.transform,false);
        _menu = contextMenu.GetComponent<UnitMenuManager>();
        _menu.CreateMenuItems(_starUnit);
        Debug.Log("Loaded Menu");
    }
}
