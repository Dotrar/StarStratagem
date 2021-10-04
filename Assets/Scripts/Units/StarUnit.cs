using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;




public struct StarAction
{ 
  /*
   A Representation of an action that the player
   can take with this particular ship.
   */
  public enum ActionType
  {
    Move,
    SimpleAttack,
  }
  
  public string Name;
  public UnityAction Callback;
  public ActionType Type;
}

public struct StarCommand
{ 
  /*
   Links the StarAction to a target, to act as a "Command" that will be 
   executed during the resolver. 
   */
  public StarAction Action;
  public Vector2 Target; 
}
public class StarUnit : MonoBehaviour
{
  /* Star unit is the core game object
   
   it can move, be issued commands, and etc.
   */

  // A list of actions that we are able to use.
  public List<StarAction> actions = new List<StarAction>();
  
  // Link to the root game object.
  public UnitSelectionAndCommands gameManager;
 
  // Private -----------------------------------------------------------------
  // A Queue of actions that this unit will do during the resolver.
  private readonly Queue<StarCommand> _nextCommandsToDo = new Queue<StarCommand>();
  // A link to the _interface (draw special effects, healthbar, interfaces etc)
  private StarUnitInterface _interface;
  
  void Awake()
  {
    //awake is called first, in all cases, where start is only called if item is enabled.
    _interface = GetComponent<StarUnitInterface>();

  }
  void Start()
  {
    // We will add a list of actions that we can perform.
    actions.Add( new StarAction
    {
      Name = "Move Ship",
      Callback = MoveCommand,
      Type = StarAction.ActionType.Move,
    });
    
    actions.Add(new StarAction
    {
      Name = "Standard Attack",
      Callback = StandardAttack,
      Type = StarAction.ActionType.SimpleAttack,
    });
    
  }

  public void PrepareForResolver()
  {
    // When the "Resolver" turn-based calculator sends us this command, we get ready for 
    // all sorts of things.
    _interface.ClearLines();
  }

  public void PrepareForInteraction()
  {
    // This is after the turn has ended and we're now awaiting commands again.
    
  }

  void IssueCommand(StarAction action, Vector3 targetPoint)
  {
    // Draw lines and do needed pre-calcs
    
    if (action.Callback == MoveCommand)
    {
      _interface.DrawMovementAction(targetPoint);
    }
    
    
    // add it to the list of "next commands to do"
      _nextCommandsToDo.Enqueue(new StarCommand
      {
        Action = action,
        Target = targetPoint
      });
  }

  public StarCommand? GetNextCommand()
  {
    Debug.Log("Trying to get from pile: "+ _nextCommandsToDo.Count );
    if (_nextCommandsToDo.Count > 0)
      return _nextCommandsToDo.Dequeue();
    return null;
  }
  
  /* ---------------------------------------------------------------------------------------
   Defining commands that the star-unit can perform.
   in this case we have:
   
   MoveCommand: - move the ship to the location.
   */
  
  async void MoveCommand()
  {
    PreActionCommands();
    // Request a target Point
    var targetPoint = await gameManager.RequestTargetPoint();

    if (targetPoint is null)
    {
      Debug.Log("Cancelled!");
      // we still want to show the menu; so re-enable that if possible
      return;
    }
    // We've got a point, let's draw a line on the interafce.
    _interface.DrawMovementAction(targetPoint.Value);
    
    //add to the list 
    _nextCommandsToDo.Enqueue(new StarCommand
    {
      Action = actions[0], //TODO better this.
      Target = targetPoint.Value
    });
    //Order is issued so let's just deselect ourselves now
    DeSelect();
  }

  void PreActionCommands()
  {
    Debug.Assert(!(gameManager is null), "Unable to find game manager from within StarUnit when trying" +
                                         "to create a command");
    
    //TODO remove this to allow for "chained" commands (shift clicks)
    _nextCommandsToDo.Clear();
  }

  void CommandCancelled()
  {
      Debug.Log("Cancelled!");
      // we still want to show the menu; so re-enable that if possible
  }
  
  async void StandardAttack()
  {
    PreActionCommands();
    
    // Request a target Point
    var targetPoint = await gameManager.RequestTargetPoint();
    if (targetPoint is null)
    {
      CommandCancelled();
      return;
    }
    // TODO find a better way to do this, we shouldn't have to find it each time.
    var action = actions.Find(a => a.Type == StarAction.ActionType.SimpleAttack);
    var command = new StarCommand
    {
      Action = action,
      Target = targetPoint.Value
    };
    
    _interface.DrawCommand(command);
    _nextCommandsToDo.Enqueue(command);
    
    //Order is issued so let's just deselect ourselves now
    DeSelect();
  }

  public void DeSelect()
  {
    //the game manager will give us time to 
    // PerformDeSelectActions
    gameManager.DeSelect();
  }

  public StarUnit Select()
  {
    _interface.SetSelected(true);
    return this;
  }

  public void PerformDeSelectActions()
  {
    _interface.SetSelected(false);
  }

}

