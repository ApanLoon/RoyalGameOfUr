using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;

    public Text CurrentRollText;
    public Text StateText;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of UI.");
            gameObject.SetActive(false);
            return;
        }
        Instance = this;

        RNG.OnRolled -= OnRolled; // Prevent multiple registrations if the component is reset.
        RNG.OnRolled += OnRolled;

        GameController.OnStateChanged -= OnStateChanged;  // Prevent multiple registrations if the component is reset.
        GameController.OnStateChanged += OnStateChanged;

        CurrentRollText.text = "";
        OnStateChanged(GameController.GameState.Player1Roll, GameController.Instance.State);
    }

    protected void OnRolled()
    {
        CurrentRollText.fontSize = 72;
        CurrentRollText.text = RNG.CurrentRoll.ToString();
    }

    protected void OnStateChanged(GameController.GameState oldState, GameController.GameState newState)
    {
        switch (newState)
        {
            case GameController.GameState.Player1Roll:
                StateText.text = "White: Roll";
                break;
            case GameController.GameState.Player2Roll:
                StateText.text = "Black: Roll";
                break;
            case GameController.GameState.Player1Select:
                StateText.text = "White: Move";
                break;
            case GameController.GameState.Player2Select:
                StateText.text = "Black: Move";
                break;
        }

        if (newState == GameController.GameState.Player1Roll || newState == GameController.GameState.Player2Roll)
        {
            CurrentRollText.fontSize = 14;
            CurrentRollText.text = "Roll";
        }
    }

}
