using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    public static PieceGenerator instance;
    [SerializeField] private GameObject piecePrefab;
    [SerializeField] private Transform piecesOnBoardParent;

    private void Awake()
    {
        instance = this;
    }

    public void GeneratePieces(bool flipColours)
    {
        List<PieceScript> piecesOnBoard = new List<PieceScript>();

        // Setting red piece in center of board
        piecesOnBoard.Add(Instantiate(piecePrefab, Vector3.zero, Quaternion.identity, piecesOnBoardParent).GetComponent<PieceScript>());
        piecesOnBoard[0].Colour = CommonValues.Colour.RED;
        piecesOnBoard[0].pieceIndex = 0;

        // Setting radius for proper placement of pieces
        float radius = (piecePrefab.GetComponent<CircleCollider2D>().radius * piecePrefab.transform.localScale.x) + 0.001f;

        // Divided by 6 because 6 pieces in inner circle
        float increments = 360f / 6f;

        // Change this if you want to rotate the pattern
        float startingDegree = 15f;


        Debug.Log("Radius = " + radius);
        float displacement = 2 * radius;

        // Making inner circle
        for (int i = 0; i < 6; i++)
        {
            piecesOnBoard.Add(Instantiate(piecePrefab, displacement * new Vector3(Mathf.Sin((i * increments + startingDegree) * Mathf.Deg2Rad ), Mathf.Cos((i * increments + startingDegree) * Mathf.Deg2Rad), 0f), Quaternion.identity, piecesOnBoardParent).GetComponent<PieceScript>());;
            if (flipColours)
            {
                piecesOnBoard[piecesOnBoard.Count - 1].Colour = (CommonValues.Colour)((i + 1) % 2);
            }
            else
            {
                piecesOnBoard[piecesOnBoard.Count - 1].Colour = (CommonValues.Colour)(i % 2);
            }
            piecesOnBoard[piecesOnBoard.Count - 1].pieceIndex = piecesOnBoard.Count - 1;
        }

        // Setting the outer circle of pieces, divided by 12 because 12 pieces in outer circle
        increments = 360f / 12f;

        // This value is used upon trial and error
        displacement *= Mathf.Sqrt(3f);

        for (int i = 0; i < 12; i++)
        {
            piecesOnBoard.Add(Instantiate(piecePrefab, displacement * new Vector3(Mathf.Sin((i * increments + startingDegree) * Mathf.Deg2Rad), Mathf.Cos((i * increments + startingDegree) * Mathf.Deg2Rad), 0f), Quaternion.identity, piecesOnBoardParent).GetComponent<PieceScript>());
            if (flipColours)
            {
                piecesOnBoard[piecesOnBoard.Count - 1].Colour = (CommonValues.Colour)(i % 2);
            }
            else
            {
                piecesOnBoard[piecesOnBoard.Count - 1].Colour = (CommonValues.Colour)((i + 1) % 2);
            }
            piecesOnBoard[piecesOnBoard.Count - 1].pieceIndex = piecesOnBoard.Count - 1;
        }

        GameManager.instance.SetPiecesOnBoard(piecesOnBoard);

        piecesOnBoardParent.transform.position = new Vector3(piecesOnBoardParent.transform.position.x, piecesOnBoardParent.transform.position.y + -0.067f, piecesOnBoardParent.transform.position.z);
    }
}
