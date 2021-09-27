using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UnitSelectionAndCommands : MonoBehaviour
{

    /*
     * This  script handles the actions related to Unit selection and issuing commands.
     * such as:
     * - Dragging boxes
     * - Showing command menu
     * - Giving that command to starunits
     */
    private const float DragMagnitudeDelta = 0.001f;


    public Texture2D targetCursor;
    
    [HideInInspector] 
    public const int LeftMouseButton = 0;
    [HideInInspector] 
    public const int RightMouseButton = 1;
    [HideInInspector] 
    public const int MiddleMouseButton =2;

    private StarUnit _selectedUnit = null;

    private Camera _mainCamera;

    //track dragging state
    private bool _userIsDragging = false;
    private Vector2? _dragStartPosition = null;
    //-----------------------------------------
    //Deselection work
    private bool _shouldDeSelect = false;
    // targeting mode?
    private bool _isInTargetMode = false;
    private Vector2? _targetPoint = null;
    void Start()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPositionThisFrame = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mouseWorldPositionThisFrame.x, mouseWorldPositionThisFrame.y);

        if (Input.GetMouseButtonUp(LeftMouseButton) && ! _userIsDragging){
            if (_isInTargetMode)
            {
                _targetPoint = mousePosition2D;
            }
            else
            {
                SelectObjectUnderPoint(mousePosition2D);
            }
        }
    }

    private void LateUpdate()
    {
        //this must be called last, to be used in next frame, n+1
        CheckForDrag();
    }

    void CheckForDrag()
    {
        //as this is just a delta comparison, we can use raw mouse position
        Vector2 position = Input.mousePosition;
        // This will set userIsDragging during times which the user is dragging.
        if (Input.GetMouseButton(LeftMouseButton))
        {
            if (_dragStartPosition is null)
            {
                _dragStartPosition = position;
                return;
            }
            
            _userIsDragging = (_dragStartPosition.Value - position).magnitude > DragMagnitudeDelta;
        }
        else if ( Input.GetMouseButtonUp(LeftMouseButton))
        {
            _dragStartPosition = null;
            _userIsDragging = false;
        }
    }

    public async Task<Vector2> RequestTargetPoint()
    {
        // One of the star units have requested a target, so we give this target mode
        // to the user
        EnableTargetMode();

        //wait until we have a value
        //TODO is there a better way to do this?
        while (_targetPoint is null)
        {
            await Task.Delay(100);
        }

        Vector2 value = _targetPoint.Value;
        
        DisableTargetMode();
        
        Debug.Log("waited 2 second");
        return value;
    }

    void EnableTargetMode()
    {
        _isInTargetMode = true;
        _targetPoint = null;
        if(targetCursor)
            // TODO change offset
            Cursor.SetCursor(targetCursor, Vector2.zero, CursorMode.Auto);
        else
            Debug.LogWarning("No TargetCursor Set");
        
    }

    void DisableTargetMode()
    {
        _isInTargetMode = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    void SelectObjectUnderPoint(Vector2 worldPoint)
    {
        Collider2D obj = Physics2D.OverlapPoint(worldPoint);
        if (obj)
        {
            //perhaps we try to see if the game unit is selected, and de-select that?
            // TODO remove if this is annoying
            if (GameObjectIsSelected(obj.gameObject))
            {
                DeSelect();
                return;
            }
            
            MaybeSelectUnit(obj.gameObject);
        }
        else
        {
            // we didn't select anything with a collider, but this could be a "deselect" action
            // TODO but we could also be pressing menu items
            // so let's deselect after a short delay, and we will let other code cancel our delay if needed.
            //DeSelect();
            
            
        }
    }

    bool GameObjectIsSelected(GameObject obj)
    {
        if (_selectedUnit && _selectedUnit.gameObject == obj)
            return true;
        return false;
    }
    
    IEnumerator DeSelectAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        if (_shouldDeSelect && _selectedUnit)
        {
            _selectedUnit.DeSelect();
            _selectedUnit = null;
        }

        _shouldDeSelect = false;
    }

    void DeSelect()
    {
        _shouldDeSelect = true;
        StartCoroutine(DeSelectAfterFrame());
    }

    public void CancelDeSelect()
    {
        _shouldDeSelect = false;
    }

    void MaybeSelectUnit(GameObject otherObject)
    {
        StarUnit unit = otherObject.GetComponent<StarUnit>();

        if (!unit)
            
        {
            // if it's not a proper StarUnit, we want to abort and do nothing.
            return;
        }

        if (_selectedUnit)
        {
            _selectedUnit.DeSelect();
        }
        _selectedUnit = unit.Select();
    }
}
