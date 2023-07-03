using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using DG.Tweening;

public class StrikerController : MonoBehaviour
{
    #region references and initialization

    public static StrikerController instance;
    public static event System.Action shoot = delegate { };
    public static event System.Action<bool> strikerIsMoving = delegate { };
    public static event System.Action<Collider2D, Vector2, CircleCollider2D, SpriteRenderer, Transform, int> strikerInPocket = delegate { };

    public float strikerMinX;
    public float strikerMaxX;
    public float[] strikerYPos;
    public Slider[] sliders;
    public bool isOverlay = false;

    [SerializeField] private Transform strikerForceBG;
    [SerializeField] private GameObject shadow;
    [SerializeField] private SpriteRenderer strikerForceBGSprite;
    [SerializeField] private Transform strikerDirectionArrowParent;
    [SerializeField] private Transform strikerDirectionArrow;
    [SerializeField] private bool strikerForce = false;
    [SerializeField] private bool playerTurn;
    [SerializeField] private float strikerForceMax;
    [SerializeField] private Sprite[] glowCircleSprites;
    [SerializeField] private SpriteRenderer glowCircle;
    [SerializeField] private Material ArrowMat;
    [SerializeField] private Gradient gradient;

    private RaycastHit2D hit;
    private float strikerForceMagnitude;
    private Vector2 strikerForceDirection;
    private Rigidbody2D rigidbody;
    private bool shotFired = false;
    private bool playerClickShoot = false;
    private bool validSpot = true;
    private bool canMoveStriker = false;
    private SpriteRenderer strikerColourSprite;
    private CircleCollider2D collider;
    private float strikerScale;
    private float strikerForceBGScale;
    private float screenMinY, screenMaxY;
    private Color currentColour1;
    private Color currentColour2;
    private Coroutine arrowAnimation;
    private bool strikerFell = false;
    private bool sliderDragging = false;


    private void Awake()
    {
        instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
        strikerColourSprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();
        strikerScale = transform.localScale.x;
        strikerForceBGScale = transform.GetChild(0).localScale.x;
        rigidbody.useFullKinematicContacts = true;
        
    }
    private void OnEnable()
    {
        GameManager.setKinematicForPieces += SetKinematic;
    }

    private void Start()
    {
        screenMinY = Camera.main.ScreenToWorldPoint(Vector2.zero).y - collider.radius;
        screenMaxY = -screenMinY;
        enabled = false;
    }

    private void OnDisable()
    {
        GameManager.setKinematicForPieces -= SetKinematic;
    }

    private void OnDestroy()
    {
        GameManager.setKinematicForPieces -= SetKinematic;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region getters and setters, disable striker

    public void FlipScreenMaxMinPoints()
    {
        float temp = screenMinY;
        screenMinY = screenMaxY;
        screenMaxY = temp;
    }

    public bool PlayerTurn
    {
        get
        {
            return playerTurn;
        }
        set
        {
            playerTurn = value;
            StartCoroutine(ResetStriker());
        }
    }

    public void DisableStriker()
    {
        strikerColourSprite.enabled = false;
        DottedLine.DottedLine.Instance.DestroyAllDots();
        sliders[0].gameObject.SetActive(false);
        sliders[1].gameObject.SetActive(false);
        rigidbody.DOKill();
        transform.DOKill();
        glowCircle.gameObject.SetActive(false);
        glowCircle.transform.DOKill();
        glowCircle.transform.localEulerAngles = Vector3.zero;
        shotFired = false;
        strikerForce = false;
        strikerForceDirection = Vector2.zero;
        strikerForceMagnitude = 0f;
        //transform.localEulerAngles = Vector3.zero;
        strikerForceBG.localScale = Vector3.one * strikerForceBGScale;
        //strikerDirectionArrowParent.localPosition = Vector3.zero;
        strikerDirectionArrowParent.gameObject.SetActive(false);
        strikerForceBG.localEulerAngles = Vector3.zero;
        strikerDirectionArrowParent.localEulerAngles = Vector3.zero;
        strikerForceBG.gameObject.SetActive(false);
        strikerColourSprite.DOKill();
        playerClickShoot = false;
        transform.localScale = Vector3.one * strikerScale;
        rigidbody.rotation = 0f;
        strikerFell = false;
        sliderDragging = false;
    }

    public bool ValidSpot
    {
        get
        {
            return validSpot;
        }
        set
        {
            validSpot = value;
            if (validSpot)
            {
                strikerColourSprite.color = new Color(1f, 1f, 1f, 1f);
                glowCircle.sprite = glowCircleSprites[0];
            }
            else
            {
                strikerColourSprite.color = new Color(1f, 0f, 0f, 0.3f);
                glowCircle.sprite = glowCircleSprites[1];
            }
        }
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
    public float StrikerForceMagnitude
    {
        get
        {
            return strikerForceMagnitude;
        }
        set
        {
            strikerForceMagnitude = Mathf.Clamp(value, 0.2f, 5.5f);
        }
    }

    public bool IsKinematic
    {
        get
        {
            return rigidbody.isKinematic;
        }
        set
        {
            if (value)
            {
                rigidbody.isKinematic = value;
                ResetRigidbodyVelocities();
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }
            else
            {
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
                rigidbody.isKinematic = value;
            }

        }
    }

    private IEnumerator ResetStriker()
    {
        transform.localEulerAngles = Vector3.zero;
        
        sliders[0].interactable = playerTurn;
        if (GameManager.instance.playerNumberOnline == 1)
        {
            strikerColourSprite.flipX = true;
            strikerColourSprite.flipY = true;
        }

        if (GameManager.instance.gameMode == CommonValues.GameMode.LOCAL_MULTIPLAYER)
        {
            sliders[1].interactable = !playerTurn;
        }
        if (playerTurn)
        {
            transform.DOMove(new Vector2(0, strikerYPos[0]), 0.6f).From(new Vector2(0, screenMinY)).OnStart(()=> strikerColourSprite.enabled = true);
        }
        else
        {
            
            transform.DOMove(new Vector2(0, strikerYPos[1]), 0.6f).From(new Vector2(0, screenMaxY)).OnStart(() => strikerColourSprite.enabled = true);
        }
        ValidSpot = true;
        AudioManager.instance.Play("StrikerPlace");
        yield return new WaitForSeconds(0.4f);
        IsSimulated = true;
        strikerColourSprite.enabled = true;

        if (playerTurn)
        {
            sliders[0].gameObject.SetActive(true);
            sliders[0].value = 0.5f;
            //StrikerPlayerSliderValueChanged(0.5f);
            glowCircle.gameObject.SetActive(true);
            glowCircle.transform.DORotate(180f * Vector3.forward, 1f,RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
            TimerScript.instance.StartTimer(GameManager.instance.timerForTurn, 0);
        }
        else 
        {
            sliders[1].gameObject.SetActive(GameManager.instance.gameMode == CommonValues.GameMode.LOCAL_MULTIPLAYER && !GameManager.instance.hasBot);
            sliders[1].value = 0.5f;
            //StrikerOpponentSliderValueChanged(0.5f);
            TimerScript.instance.StartTimer(GameManager.instance.timerForTurn, 1);
        }
        canMoveStriker = true;

        if (GameManager.instance.hasBot && !playerTurn)
        {
            BotManager.instance.BotTurn();
        }
    }

    
    
    public void ResetRigidbodyVelocities()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.angularVelocity = 0f;
    }

    private void SetKinematic(bool value)
    {
        IsKinematic = value;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region hand over control of other slider to server to simulate movement of opponent

    public void PlayerSliderAddListenerAndCorrecGlowCirclePosition(bool value)
    {
        sliders[0].onValueChanged.AddListener(delegate
        { 
            NetworkClient.instance.SendStrikerPositionChangeSignal(sliders[0].value); 
        });
        if (!value)
        {
            float temp = strikerYPos[0];
            strikerYPos[0] = strikerYPos[1];
            strikerYPos[1] = temp;
            glowCircle.transform.localPosition -= Vector3.up * 0.1f;
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region collision detection logic

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canMoveStriker)
        {
            if (collision.CompareTag("Piece"))
            {
                // striker collided with piece before shooting
                ValidSpot = false;
                if(!sliderDragging && playerTurn)
                {
                    // automatic striker position correction from red to valid spot
                    BotManager.instance.StartAdjustingStrikerPosition(StrikerPlayerSliderValueChanged, sliders[0].value);
                }
            }
        }
        else if (collision.CompareTag("Pocket") && !strikerFell && shotFired)
        {
            // striker in pocket after shooting
            strikerFell = true;
            shotFired = false;
            Vector2 velocity = rigidbody.velocity;
            rigidbody.velocity = Vector2.zero;
            if (velocity.magnitude < 1f)
            {
                velocity = velocity.normalized * 1f;
            }
            IsSimulated = false;
            strikerInPocket(collision, velocity, collider, strikerColourSprite, transform, -1);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (canMoveStriker)
        {
            if (other.CompareTag("Piece"))
            {
                // striker collided with piece before shooting
                if (ValidSpot)
                {
                    ValidSpot = false;
                }
                if (!sliderDragging && playerTurn)
                {
                    // automatic striker position correction from red to valid spot
                    BotManager.instance.StartAdjustingStrikerPosition(StrikerPlayerSliderValueChanged, sliders[0].value);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (canMoveStriker)
        {
            if (collision.CompareTag("Piece"))
            {
                // striker not colliding with piece anymore before shooting
                ValidSpot = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Piece"))
        {
            // striker collided with piece after shooting
            if(Random.value > 0.5f)
            {
                AudioManager.instance.Play("PieceCollide1",volumeMultiplier:rigidbody.velocity.magnitude/2f);
            }
            else
            {
                AudioManager.instance.Play("PieceCollide2",volumeMultiplier: rigidbody.velocity.magnitude);
            }
        }
        else
        {
            // striker collided with board after shooting
            AudioManager.instance.Play("PieceHitBoard", volumeMultiplier:rigidbody.velocity.magnitude/2f);
        }
        if (rigidbody.velocity.magnitude >= GameManager.instance.velocityThresholdForStoppingMovement)
        {
            if (!shotFired)
            {
                // striker had stopped on its own but a piece somehow collided with it and moved it again
                shotFired = true;
                strikerIsMoving(true);
            }
        }
    }


    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region striker movement through slider

    public void SliderBeingDragged(bool value)
    {
        sliderDragging = value;
        if (value && playerTurn)
        {
            BotManager.instance.EndAdjustingStrikerPosition();
        }
    }

    public void StrikerPlayerSliderValueChanged(float value)
    {
        if (canMoveStriker)
        {
            
            if (GameManager.instance.flipped)
            {
                value = 1f - value;
            }
            //sliders[0].value = value;
            rigidbody.position = new Vector2(strikerMinX + ((strikerMaxX - strikerMinX) * value), strikerYPos[0]);
        }
    }

    public void StrikerOpponentSliderValueChanged(float value)
    {
        if (canMoveStriker)
        {
            if (GameManager.instance.flipped)
            {
                value = 1f - value;
            }
            //sliders[1].value = value;
            rigidbody.position = new Vector2(strikerMinX + ((strikerMaxX - strikerMinX) * value), strikerYPos[1]);
        }
    }

    public void MakeSlidersUninteractable()
    {
        sliders[0].interactable = false;
        sliders[1].interactable = false;
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region user interation with striker to set force direction and magnitude

    private void Update()
    {
        shadow.SetActive(gameObject.activeInHierarchy);
        shadow.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        shadow.transform.position = GetComponent<CircleCollider2D>().bounds.center;

        if (!GameManager.instance.gameOver && GameManager.instance.gameStarted)
        {
            /*if(canMoveStriker)
            {
                if(Physics2D.OverlapCircle(rigidbody.position, collider.radius) != None;
            }*/
            if (validSpot && Input.GetMouseButtonDown(0) && !playerClickShoot && !strikerForce && canMoveStriker && !isOverlay)
            {
                if (playerTurn || (GameManager.instance.gameMode == CommonValues.GameMode.LOCAL_MULTIPLAYER && !GameManager.instance.hasBot))
                {
                    //hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
                    //(hit.collider && hit.collider.CompareTag(gameObject.tag)) ||

                    // player clicked on striker or near striker to set force and direction of shot
                    if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), rigidbody.position) < 0.5f)
                    {
                        if (collider.tag == "Player")
                        {
                            Debug.Log("Clicked on striker");
                            strikerForce = true;
                            canMoveStriker = false;
                            sliders[0].interactable = false;
                            sliders[1].interactable = false;
                            arrowAnimation = StartCoroutine(ArrowColourAnimation());
                            glowCircle.transform.DOKill();
                            glowCircle.gameObject.SetActive(false);
                            glowCircle.transform.localEulerAngles = Vector3.zero;
                            strikerDirectionArrowParent.gameObject.SetActive(true);
                            strikerForceBG.gameObject.SetActive(true);
                        }
                    }
                }
            }
            if (strikerForce)
            {
                // setting force and direction of shot and showing on screen
                strikerForceDirection = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - rigidbody.position;
                StrikerForceMagnitude = strikerForceDirection.magnitude * 6f;
                strikerForceDirection = strikerForceDirection.normalized;
                strikerForceBG.localScale = Vector3.one * StrikerForceMagnitude * 0.35f;
                DottedLine.DottedLine.Instance.DrawDottedLine(rigidbody.position, (Vector2)rigidbody.position + (-strikerForceDirection * (Vector2)strikerForceBG.localScale * 1.5f));
                strikerDirectionArrow.localPosition = new Vector3(0f, strikerForceBGSprite.sprite.bounds.max.y * strikerForceBG.localScale.y, 0f);
                //strikerDirectionArrow.localPosition = (-strikerForceDirection * (StrikerForceMagnitude / 5f));
                strikerDirectionArrowParent.localEulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.down, strikerForceDirection));
            }
            if (Input.GetMouseButtonUp(0) && !playerClickShoot && strikerForce)
            {
                // player has shot the striker
                if (arrowAnimation != null)
                {
                    StopCoroutine(arrowAnimation);
                }

                DottedLine.DottedLine.Instance.DestroyAllDots();
                if (StrikerForceMagnitude < 2.5f)
                {
                    // when striker force is too low, reset the striker and give player another chance to set force and direction of shot
                    strikerForce = false;
                    strikerDirectionArrowParent.gameObject.SetActive(false);
                    strikerForceBG.gameObject.SetActive(false);
                    glowCircle.gameObject.SetActive(true);

                    canMoveStriker = true;
                    if (playerTurn)
                    {
                        sliders[0].interactable = true;
                    }
                    else
                    {
                        sliders[1].interactable = true;
                    }
                    glowCircle.transform.DORotate(180f * Vector3.forward, 1f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
                }
                else
                {
                    // get final shot force and direction and send to server
                    strikerForceDirection = new Vector2(Mathf.Round(strikerForceDirection.x * 10000f) / 10000f, Mathf.Round(strikerForceDirection.y * 10000f) / 10000f);
                    strikerForceMagnitude = Mathf.Round(strikerForceMagnitude * 10000f) / 10000f;
                    Debug.Log("MouseButtonUp");

                    if (GameManager.instance.gameMode == CommonValues.GameMode.LOCAL_MULTIPLAYER || GameManager.instance.hasBot)
                    {
                        StrikerShoot();
                    }
                    else
                    {
                        strikerForce = false;
                        strikerForceBG.localScale = Vector3.one * strikerForceBGScale;
                        //strikerDirectionArrowParent.localPosition = Vector3.zero;
                        sliders[0].interactable = false;
                        sliders[1].interactable = false;
                        canMoveStriker = false;
                        strikerDirectionArrowParent.gameObject.SetActive(false);
                        strikerForceBG.gameObject.SetActive(false);
                        validSpot = false;
                        NetworkClient.instance.SendStrikerShootSignal(strikerForceDirection * StrikerForceMagnitude, rigidbody.position, rigidbody.rotation);

                    }
                }
            }
        }

    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region striker shoot logic 

    private void StrikerShoot()
    {
        // striker shot from the server at the same time on both clients

        /*if(!ValidSpot)
        {
            Debug.LogError("Striker not in valid spot and still shot");
        }*/
        sliders[0].gameObject.SetActive(false);
        sliders[1].gameObject.SetActive(false);
        TimerScript.instance.StopTimer();
        shoot();
        strikerForce = false;
        strikerForceBG.localScale = Vector3.one * strikerForceBGScale;
        //strikerDirectionArrowParent.localPosition = Vector3.zero;

        Debug.Log("Striker Shot");
        playerClickShoot = true;
        sliders[0].interactable = false;
        sliders[1].interactable = false;
        canMoveStriker = false;
        strikerDirectionArrowParent.gameObject.SetActive(false);
        strikerForceBG.gameObject.SetActive(false);
        validSpot = false;

        AudioManager.instance.Play("StrikerShoot");
        AudioManager.instance.Play("StrickerShootVoiceOver");
    }

    public void StrikerShootFromServer(Vector2 force, Vector2 position, float rotation)
    {
        strikerForceMagnitude = force.magnitude;
        strikerForceDirection = force.normalized;
        rigidbody.position = new Vector2(position.x, position.y);
        rigidbody.rotation = rotation;
        StrikerShoot();
    }
    

    private void FixedUpdate()
    {
        if (!GameManager.instance.gameOver && GameManager.instance.gameStarted)
        {
            if (rigidbody.velocity.magnitude >= GameManager.instance.velocityThresholdForStoppingMovement)
            {
                // restart striker if velocity greater than threshold
                if (!shotFired && !strikerFell)
                {
                    shotFired = true;
                    strikerIsMoving(true);
                }
            }

            if (shotFired)
            {
                if (rigidbody.velocity.magnitude < GameManager.instance.velocityThresholdForStoppingMovement)
                {
                    // stop striker if velocity too low
                    Debug.Log("Velocity became 0");

                    shotFired = false;
                    strikerIsMoving(false);
                    ResetRigidbodyVelocities();
                }
            }
            if (playerClickShoot)
            {
                // Add force to striker for shooting
                rigidbody.AddForceAtPosition(-strikerForceDirection * StrikerForceMagnitude * strikerForceMax, rigidbody.position + (strikerForceDirection * collider.radius), ForceMode2D.Impulse);
                strikerIsMoving(true);
                shotFired = true;
                playerClickShoot = false;
            }
        }
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region bot

    public void SetBotValues(BotManager.BotType botType)
    {
        BotManager.instance.InitializeBot(botType, strikerMinX, strikerMaxX, strikerYPos[1], collider.radius, GameManager.instance.pieceTargetColour);
    }

    public void BotShootStriker(float magnitude, Vector2 direction)
    {
        strikerForceMagnitude = magnitude;
        strikerForceDirection = direction;
        StrikerShoot();
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    #region arrow colour animation

    private IEnumerator ArrowColourAnimation()
    {
        float t = 0f;
        while(true)
        {
            if(Mathf.Approximately(t,1f) || t > 1f)
            {
                t = 0f;
            }
            ArrowMat.SetColor("_Color", ColorFromGradient(t));
            if (t > 0.5f)
            {
                ArrowMat.SetColor("_Color2", ColorFromGradient((t + 0.5f) - 1f));
            }
            else
            {
                ArrowMat.SetColor("_Color2", ColorFromGradient(t + 0.5f));
            }
            
            t += Time.smoothDeltaTime;
            yield return new WaitForSeconds(Time.smoothDeltaTime);
        }
    }

    Color ColorFromGradient(float value)  // float between 0-1
    {
        return gradient.Evaluate(value);
    }

    #endregion

    //------------------------------------------------------------------------------------------------------------------------------------------

    

}
