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
    
    /*
     * - Make a state machine to hold the command "mode"
     * 
     */


    public Texture2D targetCursor;
    public StarCanvasManager canvasHudGameObject;
    
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
    private bool _hasCancelledCommand = false;
    //-----------------------------------------
    private Vector2 _displayCursorOffset;
    
    
    void Start()
    {
        _mainCamera = Camera.main;
        
        // calculate cursor offset
        _displayCursorOffset = new Vector2(targetCursor.width / 2.0f, targetCursor.height / 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Check - is the mouse in a region that would possibly be handled by someone else? mainly
        // handling buttons here. 
        if (CheckForUiElements(Input.mousePosition))
        {
            ShowNormalCursor();
            return;
        }

        if (_isInTargetMode)
        {
            ShowTargetCursor();
        }
        
        Vector3 mouseWorldPositionThisFrame = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mouseWorldPositionThisFrame.x, mouseWorldPositionThisFrame.y);
        
        // Are we trying to click on a unit? or send a command somewhere?
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

    private bool CheckForUiElements(Vector3 mousePosition)
    {
        if (!canvasHudGameObject) return false;

        return canvasHudGameObject.PointMightClickButton(new Vector2(mousePosition.x, mousePosition.y));

    }

    private void LateUpdate()
    {
        CheckForCommandCancel();
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

    void CheckForCommandCancel()
    {
        if (Input.GetKeyUp(KeyCode.Escape)  && _isInTargetMode)
        {
            _hasCancelledCommand = true;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public async Task<Vector2?> RequestTargetPoint()
    {
        // await 1 to clear the current mouseUp callback
        await Task.Delay(1);
        
        // One of the star units have requested a target, so we give this target mode
        // to the user
        EnableTargetMode();

        //wait until we have a value
        while (_targetPoint is null && !_hasCancelledCommand)
        {
            await Task.Delay(100);
        }
        // We have either cancelled or received a point,
        // either way, we will disable target mode now
        
        DisableTargetMode();
        
        _hasCancelledCommand = false;
        // return whatever _targetPoint might be, null is interpreted as a cancel.
        return _targetPoint;
    }

    void ShowTargetCursor()
    {
        if(targetCursor)
            Cursor.SetCursor(targetCursor, _displayCursorOffset, CursorMode.Auto);
        else
            Debug.LogWarning("No TargetCursor Set");
    }

    void ShowNormalCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void EnableTargetMode()
    {
        //Enable target mode, which is to show the cursor, and etc.
        _isInTargetMode = true;
        _targetPoint = null;
        _hasCancelledCommand = false;
        ShowTargetCursor();
    }

    public bool IsInTargetMode()
    {
        return _isInTargetMode;
    }

    void DisableTargetMode()
    {
        // Leave target mode, undoing all actions by EnableTargetMode
        // Should have class create/destroy manage these things to ensure
        // that resources are correctly managed TODO in future.
        _isInTargetMode = false;
        _hasCancelledCommand = false;
        ShowNormalCursor();
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
            _selectedUnit.PerformDeSelectActions();
            _selectedUnit = null;
        }

        _shouldDeSelect = false;
    }

    public void DeSelect()
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
            _selectedUnit.PerformDeSelectActions();
        }
        _selectedUnit = unit.Select();
    }
}
