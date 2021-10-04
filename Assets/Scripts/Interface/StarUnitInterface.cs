using UnityEngine;

public class StarUnitInterface : MonoBehaviour
{
    /*
     This class is simply used to group together Interface concerns on the star unit
     such as reticle activation, animation, and otherwise.
     
     
     SELECTION COLOURS = ff8100; orange reticle
     
     */
    
    public SpriteRenderer selectionReticle;
    public GameObject contextMenuPrefab;
    public Canvas parentCanvas;
    
    
    // ================================
    // Star Stratagem controllers
    private UnitMenuManager _menu ;
    private UnitSelectionAndCommands _commandManager ;
    private StarUnitInterfaceLines _lineRenderingObject ;
    private StarUnit _starUnit ;
    
    // Unity objects
    private Vector3 _viewPointOffset;
    private bool _isSelected = false;
    private bool _shouldShowMenu = false;
    private Camera _mainCamera;

    void Start()
    { 
        // Get Sibling StarUnit Controller
        _starUnit = GetComponent<StarUnit>();
        _lineRenderingObject = GetComponentInChildren<StarUnitInterfaceLines>();
        _commandManager = _starUnit.gameManager;
        // _menu will be lazy loaded as it must load after StarUnits have finished initalising
        
        _mainCamera = Camera.main;
        Debug.Assert(_mainCamera != null, nameof(_mainCamera) + " != null");
        // var ourpos = transform.position;
        // _offset = _mainCamera.WorldToViewportPoint(
        //     transform.Find("MenuPoint").position - ourpos);
        _viewPointOffset = new Vector3(0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        // update interfaces for all cases.
       
        // Reticle should always be selected if this star unit is selected.
        if (selectionReticle.gameObject.activeInHierarchy != _isSelected)
        {
            selectionReticle.gameObject.SetActive(_isSelected);
        }

        _shouldShowMenu = UpdateStateFromCommandManager();

        LazyLoadMenu();
        ShowHideMenu();
    }

    public void ClearLines()
    {
        _lineRenderingObject.ClearAllLines();
    }

    bool UpdateStateFromCommandManager()
    {
        // we want to check to see if we should be showing the menu or not
        
        
        if (!_isSelected)
            return false;
        
        if (_commandManager.IsInTargetMode())
            return false;
        
        //we are selected, not in target mode, we should show
        return true;
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

        
        _menu.SetActive(_shouldShowMenu);
        if (_shouldShowMenu)
        {   // then we should also update the menu position
            //Vector3 screenPoint = _mainCamera.WorldToViewportPoint(transform.position);
            
            // We find the menu point by applying the point in "viewport space" - 
            // if we were to do in worldspace, it would be different depending on 
            // how far we are zoomed in. 

            _menu.transform.position = _mainCamera.ViewportToWorldPoint(
                _mainCamera.WorldToViewportPoint(transform.position) + _viewPointOffset);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void LazyLoadMenu()
    {
        if (!_shouldShowMenu)
            return;
        // Load the menu if we don't have it already
        if (_menu)
            return;
        // we create our own copy of a menu
        GameObject contextMenu = Instantiate(contextMenuPrefab, parentCanvas.transform,false);
        contextMenu.name = $"{gameObject.name}'s Popup Menu";
        _menu = contextMenu.GetComponent<UnitMenuManager>();
        _menu.CreateMenuItems(_starUnit);
    }

    public void DrawCommand(StarCommand theCommand)
    {
        if (theCommand.Action.Type == StarAction.ActionType.Move)
        {
            _lineRenderingObject.AddMovementLine(new []
            {
                theCommand.Target
            });
        }
        
    }

    public void DrawMovementAction(Vector2 thePoint)
    {
        //TODO add more items to array
        _lineRenderingObject.AddMovementLine(new []{thePoint});
    }
}
