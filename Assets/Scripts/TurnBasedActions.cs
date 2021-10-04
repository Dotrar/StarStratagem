using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class TurnBasedActions : MonoBehaviour
{
    // Start is called before the first frame update
    private const float WorldPositionCloseEnough = 0.002f;

    public bool playerBeginsTurn = true;
    [Range(1f,10f)]
    public float shipMovementActionSpeed = 2.0f;

    public Text gameRoundText;

    
    // Private StarUnit Pointers:
    private readonly List<StarUnit> _gameList = new List<StarUnit>();
    UnitSelectionAndCommands _commandManager;
    

    
    // variables
    private int _starUnitTurnIndex = 0;
    private int _roundNumber = 0;
    private bool _timeForAction = false;
    private StarUnit _currentUnit;
    private StarCommand? _currentCommand;
    private bool _finishedRound = false;

    void Start()
    {
        _commandManager = GetComponent<UnitSelectionAndCommands>();
        Debug.Assert(_commandManager, "We need a command manger within TurnBasedActions");
        

        //get units

        foreach (var unit in GetComponentsInChildren<StarUnit>())
        {
            _gameList.Add(unit);
        }

        Debug.Log("We have " + _gameList.Count + " star units");
    }

    public void PlayerClicksEndTurn()
    {
        Debug.Log("Time for action!");
        // TODO: Trigger AI from other players.
        
        // Signal to other components that we are now in the "action mode"

        foreach (var unit in _gameList)
        {
            unit.PrepareForResolver();
        }    
        _timeForAction = true;
    }

    public void WaitForMoreCommands()
    {
        // Signal to other components that we are now back to "commands" mode 
        foreach (var unit in _gameList)
        {
            unit.PrepareForResolver();
        }    
    }
    

    void EnsureUnitThisTurn()
    {
        /* Ensure that we have a unit that we're currently dealing with
         */

        if (_currentUnit is null)
        {
            _currentUnit = GetCurrentTurnStarUnit();
        }
    }

    public StarUnit GetCurrentTurnStarUnit()
    {
        if (_starUnitTurnIndex >= _gameList.Count)
        {
            Debug.Log("Out of bound index, " + _starUnitTurnIndex + " where length is only " + _gameList.Count);
            _starUnitTurnIndex = 0;
        }

        return _gameList[_starUnitTurnIndex];
    }

    StarCommand? GetNextStarCommand()
    {
        /* Main "who's turn next" logic.
         We will attempt to set the next command from 
         */
        EnsureUnitThisTurn();

        StarCommand? command = null;

        int failedAttempts = 0;

        while (command is null && failedAttempts < _gameList.Count)
        {
            command = _currentUnit.GetNextCommand();

            //if no more commands for this unit, go on to the next one
            // or finish alltogether;
            if (command is null)
            {
                ChangeToNextUnit();
                failedAttempts += 1;
            }
        }

        // this can be the command with the _current unit stored; 
        // OR it could be nulled;
        return command;
    }

    void ChangeToNextUnit()
    {
        // Increments the pointer to next unit and returns True if we've reached the end of the list
        _starUnitTurnIndex += 1;
        _starUnitTurnIndex %= _gameList.Count;
        _currentUnit = GetCurrentTurnStarUnit();
    }



    // Update is called once per frame
    void Update()
    {
        if (!_timeForAction)
            return;

        // We're doing things, 
        UpdateRoundText();
        if (_currentCommand is null)
        {
            Debug.Log("Getting next available action ");
            _currentCommand = GetNextStarCommand();

            if (_currentCommand is null)
            {
                Debug.Log("No further actions, finishing round.");
                // We're still null, so we've finished the round of units, and nothing is left to do.
                FinishRound();
                return;
            }

        }

        // We should have a _currentCommand, and a _currentUnit. 
        // We might either be in the middle of processing, or not.
        
        if (ProcessCommandHasFinished())
        {
            Debug.Log("Finished processing command");
            _currentCommand = null;
        }
    }

    void FinishRound()
    {
        _roundNumber += 1;
        UpdateRoundText();
        _timeForAction = false;
    }

    void UpdateRoundText()
    {
        if (gameRoundText)
        {
            gameRoundText.text = $"Round {_roundNumber}";
        }
    }

    bool ProcessCommandHasFinished()
    {
        // We have a unit and a command,

        // we will perform actions that are needed
        // and check to see that the command is complete.
        // almost in a poor-man's async way. 

        return ProcessMoveCommand();
    }

    bool ProcessMoveCommand()
    {
        if (_currentCommand is null) return true;
        StarCommand command = _currentCommand.Value;
        
        Vector3 currentPosition = _currentUnit.transform.position;
        Vector3 desiredPosition = new Vector3(command.Target.x, command.Target.y, currentPosition.z);
        Vector3 deltaPosition = desiredPosition - currentPosition;

        if (deltaPosition.magnitude < WorldPositionCloseEnough)
            return true;

        deltaPosition.Normalize();
        deltaPosition *= shipMovementActionSpeed;

        _currentUnit.transform.Translate(deltaPosition * Time.deltaTime);
        return false;
    }
}