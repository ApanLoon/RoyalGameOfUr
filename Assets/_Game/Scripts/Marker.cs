
using UnityEngine;

public class Marker : MonoBehaviour
{
    public bool IsPlayer1;
    public bool IsAtGoal;
    public Tile Tile;

    public float MaxDistanceFromTile = 0.5f;

    protected new Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    private void OnMouseDown()
    {
        if (!IsAtGoal)
        {
            GameController.Instance.Move(this);
        }
    }

    private void FixedUpdate()
    {
        if (Tile == null)
        {
            return;
        }
        if (rigidbody.IsSleeping() && (Vector3.Distance(transform.position, Tile.transform.position) > MaxDistanceFromTile))
        {
            transform.position = Tile.transform.position + (Vector3.up * 0.33f);
            Debug.Log("Marker: Trying to correct position.");
        }
    }
}
