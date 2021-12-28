using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using DG.Tweening;

public class PieceScript : MonoBehaviour
{
    #region references and initialization

    public static event System.Action<bool> pieceIsMoving = delegate { };
    public static event System.Action<Collider2D, Vector2, CircleCollider2D, SpriteRenderer, Transform, int> pieceInPocket = delegate { };



    public int pieceIndex;
    public SpriteRenderer pieceColourSprite;
    public CircleCollider2D collider;
    public int points;
    public float scale;
    public Rigidbody2D rigidbody;


    [SerializeField] private bool contact;
    [SerializeField] private CommonValues.Colour colour;
    [SerializeField] private bool isKinematic;


    
    private bool pieceMovingFired;

    private void Awake()
    {
        pieceColourSprite = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        contact = false;
        scale = transform.localScale.x;
        rigidbody.useFullKinematicContacts = true;
    }

    private void OnEnable()
    { 
        GameManager.setKinematicForPieces += SetKinematic;

        IsSimulated = true;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region getters and setters

    public bool IsKinematic
    {
        get
        {
            return isKinematic;
        }
        set
        {
            isKinematic = value;
            if (isKinematic)
            {
                rigidbody.isKinematic = true;
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
                ResetRigidbodyVelocities();
            }
            else
            {
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                rigidbody.isKinematic = false;
            }
        }
    }

    public void ResetRigidbodyVelocities()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
    }

    public bool IsSimulated
    {
        get
        {
            return rigidbody.simulated;
        }
        set
        {
            rigidbody.simulated = value;
        }
    }

    public CommonValues.Colour Colour
    {
        get
        {
            return colour;
        }
        set
        {
            colour = value;
            if (colour == CommonValues.Colour.BLACK)
            {
                pieceColourSprite.sprite = GameManager.instance.pieceSprites[0];
                points = 10;
            }
            else if (colour == CommonValues.Colour.WHITE)
            {
                pieceColourSprite.sprite = GameManager.instance.pieceSprites[1];
                points = 20;
            }
            else if (colour == CommonValues.Colour.RED)
            {
                pieceColourSprite.sprite = GameManager.instance.pieceSprites[2];
                points = 50;
            }
            
        }
    }
    private void SetKinematic(bool value)
    {
        IsKinematic = value;
        pieceMovingFired = false;
        collider.isTrigger = value;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region collision detection logic 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Piece"))
        {
            if (Random.value > 0.5f)
            {
                AudioManager.instance.Play("PieceCollide1", volumeMultiplier: rigidbody.velocity.magnitude / 2f);
            }
            else
            {
                AudioManager.instance.Play("PieceCollide2", volumeMultiplier: rigidbody.velocity.magnitude / 2f);
            }
        }
        else if (!collision.collider.CompareTag("Player"))
        {
            AudioManager.instance.Play("PieceHitBoard", volumeMultiplier: rigidbody.velocity.magnitude);
        }

        if (rigidbody.velocity.magnitude >= GameManager.instance.velocityThresholdForStoppingMovement)
        {
            if (!pieceMovingFired)
            {
                contact = true;
                pieceIsMoving(true);
                pieceMovingFired = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collider.isTrigger)
        {
            if (other.CompareTag("Pocket"))
            {
                if (rigidbody.velocity.magnitude < GameManager.instance.velocityThresholdForPieceFallInPocket)
                {
                    Debug.Log("Piece fell in pocket");
                    pieceMovingFired = false;
                    Vector2 velocity = rigidbody.velocity;
                    IsKinematic = true;
                    IsSimulated = false;
                    if (velocity.magnitude < 1f)
                    {
                        velocity = velocity.normalized * 1f;
                    }
                    contact = false;
                    collider.isTrigger = true;
                    pieceInPocket(other, velocity, collider, pieceColourSprite, transform, pieceIndex);
                    Debug.Log(other.name);
                }
                else
                {
                    Debug.Log("Piece too fast and got rebounded");
                }
            }
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region check if piece stopped moving so that when all pieces stop moving turn will end

    private void FixedUpdate()
    {
        if (contact)
        {
            if (rigidbody.velocity.magnitude < GameManager.instance.velocityThresholdForStoppingMovement)
            {
                ResetRigidbodyVelocities();
                contact = false;
                pieceMovingFired = false;
                pieceIsMoving(false);
            }
        }
        if (rigidbody.velocity.magnitude >= GameManager.instance.velocityThresholdForStoppingMovement)
        {
            if (!pieceMovingFired)
            {
                contact = true;
                pieceIsMoving(true);
                pieceMovingFired = true;
            }
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region reset piece on disable

    private void OnDisable()
    {
        GameManager.setKinematicForPieces -= SetKinematic;
        IsSimulated = false;
        transform.localScale = Vector3.one * scale;
        rigidbody.rotation = 0f;
        pieceColourSprite.DOFade(1, 0f);
    }

    private void OnDestroy()
    {
        GameManager.setKinematicForPieces -= SetKinematic;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------
}
