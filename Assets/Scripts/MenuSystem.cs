using Assets.Scripts;
using Assets.Scripts.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// See the reference below for help on scripting the UI Toolkit
/// 
/// </summary>
/// <see cref="https://docs.unity3d.com/2022.2/Documentation/Manual/UIE-get-started-with-runtime-ui.html"/>
public class MenuSystem : MonoBehaviour
{
    
    /// <summary>
    /// Display fields for the game status, Playing, Won, 
    /// </summary>
    private TextField _numMovesTextField;
    private Label _gameStatus;

    private int numMoves = 0;

    /// <summary>
    /// Used to start the game over
    /// </summary>
    private Button _startOver;


    /// <summary>
    /// Buttons to indicate available directions.
    /// </summary>
    private Button _left, _right, _up, _down;
    

    /// <summary>
    /// Initialize the variables to the parts of the UI XML document.
    /// This code is similar to other UI systems including Android.
    /// The main purpose of initializing the variables is to register 
    /// methods (Callbacks) that will handle user actions with each 
    /// control.
    /// </summary>
    private void OnEnable()
    {
        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // Start Over has a click event handler
        _startOver = uiDocument.rootVisualElement.Q("btnPlayAgain") as Button;
        _startOver.RegisterCallback<ClickEvent>(RestartGame);

        // These are display fields
        _numMovesTextField = uiDocument.rootVisualElement.Q("txtNumMoves") as TextField;
        _gameStatus = uiDocument.rootVisualElement.Q("txtStatus") as Label;


        // These are used to display if the direction is valid
        _left = uiDocument.rootVisualElement.Q("btnLeft") as Button;
        _right = uiDocument.rootVisualElement.Q("btnRight") as Button;
        _up = uiDocument.rootVisualElement.Q("btnUp") as Button;
        _down = uiDocument.rootVisualElement.Q("btnDown") as Button;

        // MenuSystem needs to know about these events.
        GridGameEventBus.Subscribe(MovementEventType.ARRIVED_AT_DESTINATION, GameOver);
        GridGameEventBus.Subscribe(MovementEventType.NEXT_MOVE, UpdateNavButtons);
        GridGameEventBus.Subscribe(MovementEventType.NEXT_MOVE, UpdateFields);
    }

    /// <summary>
    /// Disconnect the Click Events when the Menu System is being destroyed at the end of a scene.
    /// </summary>
    private void OnDisable()
    {
        _startOver.UnregisterCallback<ClickEvent>(RestartGame);
        GridGameEventBus.Unsubscribe(MovementEventType.ARRIVED_AT_DESTINATION, GameOver);
        GridGameEventBus.Unsubscribe(MovementEventType.NEXT_MOVE, UpdateNavButtons);
        GridGameEventBus.Unsubscribe(MovementEventType.NEXT_MOVE, UpdateFields);
    }

    /// <summary>
    /// Increment the move and update the _numMovesTextField field
    /// </summary>
    private void UpdateFields()
    {
        numMoves++;
        _numMovesTextField.value = numMoves.ToString();

    }

    private void RestartGame(ClickEvent evt)
    {
        GridGameEventBus.Publish(MovementEventType.RESTART);
        numMoves = 0;

    }

    private void GameOver()
    {
        _gameStatus.text = "Nice, now try again.";
    }

    /// <summary>
    /// Use the information from the neighbors dictionary in the 
    /// CurrentMarker to enable/disable the directional menu items
    /// No LONGER being used See UpdateNavButtons after this method
    /// </summary>
    private void UpdateNavButtonsOrig()
    {
        // Only process when there is a CurrentMarker
        if (GameManager.Instance.CurrentMarker == null)
        {
            return;
        }
         var neighbors = GameManager.Instance.CurrentMarker.neighbors;

        
        if (neighbors[Direction.LEFT] != null && !neighbors[Direction.LEFT].HasBeenVisited)
        {
            Marker current = neighbors[Direction.LEFT];
            if (GameManager.Instance.IsBarrier(current))
            {
                _left.SetEnabled(false);
            }
            else
            {
                _left.SetEnabled(true);
            }
        }
        else
        {
            _left.SetEnabled(false);
        }

        if (neighbors[Direction.RIGHT] != null && !neighbors[Direction.RIGHT].HasBeenVisited)
        {
            Marker current = neighbors[Direction.RIGHT];
            if (GameManager.Instance.IsBarrier(current))
            {
                _right.SetEnabled(false);
            }
            else
            {
                _right.SetEnabled(true);
            }
        }
        else
        {
            _right.SetEnabled(false);
        }
        if (neighbors[Direction.UP] != null && !neighbors[Direction.UP].HasBeenVisited)
        {
            Marker current = neighbors[Direction.UP];
            if (GameManager.Instance.IsBarrier(current))
            {
                _up.SetEnabled(false);
            }
            else
            {
                _up.SetEnabled(true);
            }
        }
        else
        {
            _up.SetEnabled(false);
        }

        if (neighbors[Direction.DOWN] != null && !neighbors[Direction.DOWN].HasBeenVisited)
        {
            Marker current = neighbors[Direction.DOWN];
            if (GameManager.Instance.IsBarrier(current))
            {
                _down.SetEnabled(false);
            }
            else
            {
                _down.SetEnabled(true);
            }
        }
        else
        {
            _down.SetEnabled(false);
        }

    }

    private void UpdateNavButtons()
    {
        // Only process when there is a CurrentMarker
        if (GameManager.Instance.CurrentMarker == null)
        {
            return;
        }
        var neighbors = GameManager.Instance.CurrentMarker.neighbors;

        if (neighbors == null)
        {
            return;
        }


        Direction[] directions = new[] { Direction.LEFT, Direction.RIGHT, Direction.UP, Direction.DOWN };
        Dictionary<Direction, Button> DirectionsToButtons = new Dictionary<Direction, Button>
        {
            {Direction.LEFT, _left },
            {Direction.RIGHT, _right },
            {Direction.UP, _up },
            {Direction.DOWN,_down }
        };

        Command currentCommand = null;

        
        foreach (Direction direction in directions)
        {
            // STEP 3: Uncomment the code below, Lines 222-238, after you have
            // create the derived Command classes in the Moves.cs 
            // file.
            
                switch(direction)
                {
                    case Direction.LEFT:
                        currentCommand = new MoveLeft();
                        break;
                    case Direction.RIGHT:
                        currentCommand= new MoveRight();
                        break;
                    case Direction.UP:
                        currentCommand = new MoveUp();
                        break;
                    case Direction.DOWN:
                        currentCommand = new MoveDown();
                        break;
                }
            
            Button currentButton = DirectionsToButtons[direction];
            if (currentButton != null && currentCommand != null)
            {
                bool IsAnOption= currentCommand.CanExecute(neighbors);
                currentButton.SetEnabled(IsAnOption);
            }

        }

       
    }

  
    

    
    


  
    

}
