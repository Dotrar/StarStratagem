using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct StarAction
{
  public string name;
  public UnityAction callback;
}
public class StarUnit : MonoBehaviour
{
  /* Star unit is the core game object
   
   it can move, be issued commands, and etc.
   */

  public List<StarAction> actions = new List<StarAction>();
  public GameObject GameManager;
  

  private StarUnitInterface _interface;
  void Awake()
  {
    //awake is called first, in all cases, where start is only called if item is enabled.
    _interface = GetComponent<StarUnitInterface>();

  }
  void Start()
  {
    var action = new StarAction
    {
      callback = MoveCommand,
      name = "Move Ship"
    };
    Debug.Log("Created Action:" + action.ToString());
    actions.Add(action);
    
  }
  async void MoveCommand()
  {

    Debug.Log("Move Command Issued!");
    UnitSelectionAndCommands manager = GameManager.GetComponent<UnitSelectionAndCommands>();

    if (manager is null)
    {
      Debug.Log("Unable to find manager!");
      return;
    }

    var targetPoint = await manager.RequestTargetPoint();
    
    Debug.Log("Got the target in StarUnit! " + targetPoint.ToString() );
    //manager.RequestTargetCommand( raw = true, );
  }


  // Update is called once per frame
  void Update()
  {

  }


  public StarUnit Select()
  {
    _interface.SetSelected(true);
    return this;
  }

  public void DeSelect()
  {
    _interface.SetSelected(false);
  }

}
