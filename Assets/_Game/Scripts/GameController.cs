
using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public static event Action<GameState, GameState> OnStateChanged;

    public Tile Player1FirstTile;
    public Tile Player2FirstTile;
    public Transform Player1Home;
    public Transform Player2Home;

    public int nMarkers = 6;
    public GameObject Player1MarkerPrefab;
    public GameObject Player2MarkerPrefab;

    public enum GameState
    {
        Player1Roll,
        Player2Roll,
        Player1Select,
        Player2Select
    }

    public GameState State
    {
        get { return _State; }
        protected set
        {
            GameState oldState = _State;
            _State = value;
            if (OnStateChanged != null)
            {
                OnStateChanged(oldState, _State);
            }
        }
    }

    protected GameState _State;
    protected Marker[] Player1Markers;
    protected Marker[] Player2Markers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of GameController.");
            gameObject.SetActive(false);
            return;
        }
        Instance = this;

        State = GameState.Player1Roll;
    }

    private void Start()
    {
        StartGame();
    }
	
    public void StartGame()
    {
        if (Player1Markers != null)
        {
            for (int i = 0; i < nMarkers; i++)
            {
                Destroy(Player1Markers[i].gameObject);
                Destroy(Player2Markers[i].gameObject);
            }
        }
        Player1Markers = new Marker[nMarkers];
        Player2Markers = new Marker[nMarkers];
        for (int i = 0; i < nMarkers; i++)
        {
            GameObject go = Instantiate(Player1MarkerPrefab, Player1Home.position + (Vector3.up * (i * 0.33f)), Player1Home.rotation);
            Player1Markers[i] = go.GetComponent<Marker>();
            Player1Markers[i].IsPlayer1 = true;
            go = Instantiate(Player2MarkerPrefab, Player2Home.position + (Vector3.up * (i * 0.33f)), Player2Home.rotation);
            Player2Markers[i] = go.GetComponent<Marker>();            
        }
    }

    public void Roll()
    {
        if (State != GameState.Player1Roll && State != GameState.Player2Roll)
        {
            return;
        }
        RNG.Roll();
        State = State == GameState.Player1Roll ? GameState.Player1Select : GameState.Player2Select;
    }

    public void Pass()
    {
        if (State != GameState.Player1Select && State != GameState.Player2Select)
        {
            return;
        }
        State = State == GameState.Player1Select ? GameState.Player2Roll : GameState.Player1Roll;
    }

    public void Move(Marker marker)
    {
        if (RNG.CurrentRoll == 0)
        {
            return;
        }

        GameState nextStateMove;
        GameState nextStateRollAgain;
        if (marker.IsPlayer1 && State == GameState.Player1Select)
        {
            nextStateMove = GameState.Player2Roll;
            nextStateRollAgain = GameState.Player1Roll;
        }
        else if (!marker.IsPlayer1 && State == GameState.Player2Select)
        {
            nextStateMove = GameState.Player1Roll;
            nextStateRollAgain = GameState.Player2Roll;
        }
        else
        {
            return;
        }

        Tile EndTile = TileAfterMove(marker, RNG.CurrentRoll);
        if (EndTile == null)
        {
            return;
        }

        if (EndTile.Marker != null && EndTile.Marker.IsPlayer1 == marker.IsPlayer1) // I can't move to a tile occupied by my own marker
        {
            return;
        }

        if (EndTile.Marker != null && EndTile.Marker.IsPlayer1 != marker.IsPlayer1 && EndTile.isSafe) // I can't move to a tile occupied by opponent's marker if the tile is safe
        {
            return;
        }

        // Move the marker
        StartTile.Marker = null;
        marker.transform.position = EndTile.transform.position + (Vector3.up * 0.33f);

        if (EndTile.Marker != null)
        {
            // Send opponent marker home
            EndTile.Marker.transform.position = EndTile.Marker.IsPlayer1 ? Player1Home.position : Player2Home.position;
            EndTile.Marker.Tile = null;
        }


        marker.Tile = EndTile;
        EndTile.Marker = marker;

        if (EndTile.IsGoal)
        {
            marker.IsAtGoal = true;
            marker.Tile = null;
            EndTile.Marker = null;
            int i = 0;
            while (i < nMarkers && (marker.IsPlayer1 ? Player1Markers[i] : Player2Markers[i]).IsAtGoal)
            {
                i++;
            }
            if (i == nMarkers)
            {
                Debug.Log("Game over. Player " + (marker.IsPlayer1 ? "1" : "2") + " won.");
                StartGame();
            }
        }

        State = EndTile.IsRollAgain ? nextStateRollAgain : nextStateMove;


        //if (CanMove(marker, RNG.CurrentRoll))
        //{
        //    Debug.Log("Can move");
        //    StartMove(marker, EndTile.transform.position);

        //    //TODO: Wait for the Tile to inform us that the animation is complete

        //    MoveComplete(marker);
        //    // TODO: Check if we ended up on a "roll-again" tile
        //}
        //else
        //{
        //    Debug.Log("Can not move");
        //}

        //State = nextState;
    }


    protected Tile StartTile;
    protected Tile EndTile;


    public Tile TileAfterMove (Marker marker, int distance)
    {
        StartTile = marker.Tile;

        if (StartTile == null)
        {
            StartTile = marker.IsPlayer1 ? Player1FirstTile : Player2FirstTile;
            distance--; // Jumping onto the board costs one.
        }
        EndTile = StartTile;
        for (int i = 0; i < distance && EndTile != null; i++)
        {
            EndTile = marker.IsPlayer1 ? EndTile.P1NextTile : EndTile.P2NextTile;
        }

        return EndTile;
    }




    protected void MoveComplete(Marker marker)
    {
        if (EndTile.Marker != null)
        {
            // Knock off opponent marker
            // TODO: Animate knocking opponent to home
            EndTile.Marker.transform.position = EndTile.Marker.IsPlayer1 ? Player1Home.position : Player2Home.position;
            EndTile.Marker.Tile = null;
        }
        marker.Tile = EndTile;
        EndTile.Marker = marker;

        if (EndTile.IsGoal)
        {
            marker.IsAtGoal = true;
            EndTile.Marker = null;
            int i = 0;
            while (i < nMarkers && (marker.IsPlayer1 ? Player1Markers[i] : Player2Markers[i]).IsAtGoal)
            {
                i++;
            }
            if (i == nMarkers)
            {
                Debug.Log("Game over. Player " + (marker.IsPlayer1 ? "1" : "2") + " won.");
                StartGame();
            }
        }

        StartTile = null;
        EndTile = null;
    }

}
