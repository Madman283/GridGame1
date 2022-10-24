using Assets.Scripts;
using Assets.Scripts.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    GridGameCubePlayer inputActions = null;
    public Vector3 latestMove = Vector3.zero;

    private Vector3 startingPosition = Vector3.zero;

    private void Awake()
    {
        inputActions = new GridGameCubePlayer();
        inputActions.Player.Move.performed += Move_performed;

        GridGameEventBus.Subscribe(MovementEventType.RESTART, ResetPosition);
    }

    private void ResetPosition()
    {
        this.transform.position = startingPosition;
    }

    private void Start()
    {
        startingPosition = this.transform.position;
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }


   
    /// <summary>
    /// Remap the X-Y input to the X, Z in a Vector3
    /// The context is negated (flipped) since the 
    /// camera is placed ahead of the grid. 
    /// So Left appears to Right, Down is up. Negating the vector
    /// solves this problem. 
    /// </summary>
    /// <param name="context"></param>
    private void Move_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        Marker currentMarker = GameManager.Instance.CurrentMarker;
        Command currentCommand = null;

        Direction[] directions = new[] { Direction.LEFT, Direction.RIGHT, Direction.UP, Direction.DOWN };
        Dictionary<Vector2, Direction> Vector2ToDirection =
            new Dictionary<Vector2, Direction>
            {
                { Vector2.left, Direction.LEFT },
                { Vector2.right, Direction.RIGHT },
                { Vector2.up, Direction.UP },
                { Vector2.down, Direction.DOWN }

            };


        if (Vector2ToDirection.TryGetValue(direction, out Direction current))
        {
            switch (current)
            {
                // STEP 2:
                // UNCOMMENT this area after the MoveLeft, MoveRight, MoveUp and MoveDown classes
                // are created in the Moves.cs file.
                // Remember that all of these classes inherit from Command and they must
                // implement the CanExecute and Execute methods with the same signature 
                // as what is in the Command class.
                
                case Direction.LEFT:
                    currentCommand = new MoveLeft();
                    break;
                case Direction.RIGHT:
                    currentCommand = new MoveRight();
                    break;
                case Direction.UP:
                    currentCommand = new MoveUp();
                    break;
                case Direction.DOWN:
                    currentCommand = new MoveDown();
                    break;
                
                default:
                    break;
            }

            if (currentCommand != null && currentCommand.CanExecute(currentMarker.neighbors))
            {
                latestMove = currentCommand.Execute(currentMarker.neighbors[current], transform.position);
                transform.Translate(latestMove);
            }
        }


        }
    }
