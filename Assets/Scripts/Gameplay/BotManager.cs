
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
{
    #region public and private fields

    public static BotManager instance;

    [SerializeField] 
    private Transform[] pockets;

    [SerializeField] 
    private int layerMask = 6, maxBotPosTries = 5;

    [SerializeField] 
    private float botStrikerForceMultiplier = 5f;


    private event System.Action botTurn = delegate { };
    private float strikerMinX, strikerMaxX, strikerY, pieceRadius, strikerRadius, strikerForce;
    private Vector2 strikerShootDirection;
    private List<float> botPositions = new List<float>();
    private ContactFilter2D filter;
    private float sliderValue;
    private bool strikerBeingAdjusted = false;
    private int playerTargetPieceColour;
    private float prevPosition = 0.5f;
    private Coroutine strikerAdjustingCoroutine;

    public enum BotType
    {
        FAIRPLAY,
        MUSTWIN
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region set references, initialize bot with max and min striker positions, layer masks for raycasts and the function to call on bot turn depending on game mode

    private void Awake()
    { 
        instance = this;
    }


    public void InitializeBot(BotType botType, float strikerMinX, float strikerMaxX, float strikerY, float strikerRadius, int playerTargetPieceColour = 0)
    {
        Debug.Log("Initializing bot");
        TimerScript.timerOver += () => this.StopAllCoroutines();
        this.strikerMinX = strikerMinX;
        this.strikerMaxX = strikerMaxX;
        this.strikerY = strikerY;
        this.pieceRadius = GameManager.instance.GetPieceRadius();
        this.strikerRadius = strikerRadius;
        this.playerTargetPieceColour = playerTargetPieceColour;
        filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("Piece");
        filter.useTriggers = true;
        for(int i = 0; i < maxBotPosTries; i++)
        {
            botPositions.Add((float)i / (maxBotPosTries - 1f));
        }
        if (botType == BotType.FAIRPLAY)
        {
            if (GameManager.instance.gameMode != CommonValues.GameMode.BLACK_AND_WHITE)
            {
                botTurn = delegate { StartCoroutine(FreeStyleFairplayBot()); }; 
            }
            else
            {
                botTurn = delegate { StartCoroutine(BlackAndWhiteFairplayBot()); };
            }
        }
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region turn for bot, bot calculations for striker placement and shoot depending on game mode

    public void BotTurn()
    {
        if (GameManager.instance.gameStarted && !GameManager.instance.gameOver)
        {
            prevPosition = 0.5f;
            botTurn();
        }
    }

    private void SendStrikerForceAndDirection()
    {
        this.StopAllCoroutines();
        if (StrikerController.instance.ValidSpot)
        {
            StrikerController.instance.BotShootStriker(strikerForce, strikerShootDirection);
        }
        else
        {
            botTurn();
        }
    }

    private IEnumerator FreeStyleFairplayBot()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        List<Vector2> piecePos = new List<Vector2>(GameManager.instance.GetEnabledPiecePos());
        Vector2 intersection;
        Debug.Log("Turn for freestyle fairplay bot");
        List<float> positions = new List<float>(botPositions);
        yield return new WaitForSeconds(1f);
        int randPos;
        StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged, prevPosition);
        while(strikerBeingAdjusted)
        {
            yield return null;
        }

        for (int i = 0; i < pockets.Length; i++)
        {
            for (int j = 0; j < piecePos.Count; j++)
            {
                if (Physics2D.CircleCast(piecePos[j], pieceRadius, ((Vector2)pockets[i].position - piecePos[j]).normalized, filter, hits, Vector2.Distance((Vector2)pockets[i].position, piecePos[j])) <= 2)
                {
                    if (LineSegmentsIntersection(piecePos[j], (Vector2)pockets[i].position, new Vector2(strikerMinX, strikerY), new Vector2(strikerMaxX, strikerY), out intersection))
                    {
                        if (intersection.x <= strikerMaxX && intersection.x >= strikerMinX)
                        {
                            if (Physics2D.CircleCast(intersection, strikerRadius, (piecePos[j] - intersection).normalized, filter, hits, Vector2.Distance(intersection, piecePos[j])) <= 1)
                            {
                                StartCoroutine(StrikerMovementCoroutine((intersection.x - strikerMinX) / (strikerMaxX - strikerMinX)));
                                yield return new WaitForSeconds(1f);
                                StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged, prevPosition);
                                while (strikerBeingAdjusted)
                                {
                                    yield return null;
                                }
                                yield return new WaitForSeconds(0.5f);
                                strikerForce = Vector2.Distance(piecePos[j], pockets[i].position) * botStrikerForceMultiplier + Vector2.Distance(piecePos[j], transform.position) * botStrikerForceMultiplier;
                                strikerShootDirection = ((Vector2)transform.position - piecePos[j]).normalized;
                                SendStrikerForceAndDirection();
                            }
                        }
                    }
                }
            }
        }
        
       
        for (int pos = 0; pos < maxBotPosTries; pos++)
        {
            randPos = Random.Range(0, positions.Count);
            StartCoroutine(StrikerMovementCoroutine(Mathf.Clamp(positions[randPos] + Random.Range(-0.02f, 0.02f), 0f, 1f)));

            yield return new WaitForSeconds(1f);
            StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged, prevPosition);
            while (strikerBeingAdjusted)
            {
                yield return null;
            }
            
            for (int i = 0; i < pockets.Length; i++)
            {
                hits[0] = Physics2D.Linecast((Vector2)transform.position, (Vector2)pockets[i].position, 1 << layerMask);
                if (hits[0])
                {
                    if (hits[0].collider.CompareTag("Piece"))
                    {
                        strikerForce = Vector2.Distance((Vector2)hits[0].transform.position, (Vector2)transform.position) * botStrikerForceMultiplier + Vector2.Distance((Vector2)hits[0].transform.position, (Vector2)pockets[i].position) * botStrikerForceMultiplier;
                        strikerShootDirection = ((Vector2)transform.position - (Vector2)hits[0].transform.position).normalized;
                        yield return new WaitForSeconds(1f);
                        SendStrikerForceAndDirection();
                    }
                }
            }
        }
        StartCoroutine(StrikerMovementCoroutine(Random.value));
        
        yield return new WaitForSeconds(1.1f);
        StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged,prevPosition);
        while (strikerBeingAdjusted)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        int randPiece = Random.Range(0, piecePos.Count);
        strikerForce = Vector2.Distance(piecePos[randPiece], (Vector2)transform.position) * botStrikerForceMultiplier + 4f;
        strikerShootDirection = ((Vector2)transform.position - piecePos[randPiece]).normalized;
        SendStrikerForceAndDirection();
    }

    public static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }


    private IEnumerator BlackAndWhiteFairplayBot()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        List<(Vector2, int)> piecePosAndColour = new List<(Vector2, int)>(GameManager.instance.GetEnabledPiecePosAndColour());

        Vector2 intersection;
        Debug.Log("Turn for black and white fairplay bot");
        List<float> positions = new List<float>(botPositions);
        yield return new WaitForSeconds(1f);
        int randPos;

        StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged, prevPosition);
        while (strikerBeingAdjusted)
        {
            yield return null;
        }

        for (int i = 0; i < pockets.Length; i++)
        {
            for (int j = 0; j < piecePosAndColour.Count; j++)
            {
                if (piecePosAndColour[j].Item2 == playerTargetPieceColour)
                {
                    continue;
                }
                if (Physics2D.CircleCast(piecePosAndColour[j].Item1, pieceRadius, ((Vector2)pockets[i].position - piecePosAndColour[j].Item1).normalized, filter, hits, Vector2.Distance((Vector2)pockets[i].position, piecePosAndColour[j].Item1)) <= 2)
                {
                    if (LineSegmentsIntersection(piecePosAndColour[j].Item1, (Vector2)pockets[i].position, new Vector2(strikerMinX, strikerY), new Vector2(strikerMaxX, strikerY), out intersection))
                    {
                        if (intersection.x <= strikerMaxX && intersection.x >= strikerMinX)
                        {
                            if (Physics2D.CircleCast(intersection, strikerRadius, (piecePosAndColour[j].Item1 - intersection).normalized, filter, hits, Vector2.Distance(intersection, piecePosAndColour[j].Item1)) <= 1)
                            {
                                StartCoroutine(StrikerMovementCoroutine((intersection.x - strikerMinX) / (strikerMaxX - strikerMinX)));
                                yield return new WaitForSeconds(1f);
                                StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged, prevPosition);
                                while (strikerBeingAdjusted)
                                {
                                    yield return null;
                                }
                                yield return new WaitForSeconds(0.5f);
                                strikerForce = Vector2.Distance(piecePosAndColour[j].Item1, pockets[i].position) * botStrikerForceMultiplier + Vector2.Distance(piecePosAndColour[j].Item1, transform.position) * botStrikerForceMultiplier;
                                strikerShootDirection = ((Vector2)transform.position - piecePosAndColour[j].Item1).normalized;
                                SendStrikerForceAndDirection();
                            }
                        }
                    }
                }
            }
        }


        for (int pos = 0; pos < maxBotPosTries; pos++)
        {
            randPos = Random.Range(0, positions.Count);
            StartCoroutine(StrikerMovementCoroutine(Mathf.Clamp(positions[randPos] + Random.Range(-0.02f, 0.02f), 0f, 1f)));

            yield return new WaitForSeconds(1f);
            StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged, prevPosition);
            while (strikerBeingAdjusted)
            {
                yield return null;
            }

            for (int i = 0; i < pockets.Length; i++)
            {
                hits[0] = Physics2D.Linecast((Vector2)transform.position, (Vector2)pockets[i].position, 1 << layerMask);
                if (hits[0])
                {
                    if (hits[0].collider.CompareTag("Piece"))
                    {
                        if (FindPieceColour(piecePosAndColour, (Vector2)hits[0].collider.transform.position) != playerTargetPieceColour)
                        {
                            strikerForce = Vector2.Distance((Vector2)hits[0].transform.position, (Vector2)transform.position) * botStrikerForceMultiplier + Vector2.Distance((Vector2)hits[0].transform.position, (Vector2)pockets[i].position) * botStrikerForceMultiplier;
                            strikerShootDirection = ((Vector2)transform.position - (Vector2)hits[0].transform.position).normalized;
                            yield return new WaitForSeconds(1f);
                            SendStrikerForceAndDirection();
                        }
                    }
                }
            }
        }
        StartCoroutine(StrikerMovementCoroutine(Random.value));

        yield return new WaitForSeconds(1.1f);
        StartAdjustingStrikerPosition(StrikerController.instance.StrikerOpponentSliderValueChanged, prevPosition);
        while (strikerBeingAdjusted)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        int randPiece = Random.Range(0, piecePosAndColour.Count);
        strikerForce = Vector2.Distance(piecePosAndColour[randPiece].Item1, (Vector2)transform.position) * botStrikerForceMultiplier + 4f;
        strikerShootDirection = ((Vector2)transform.position - piecePosAndColour[randPiece].Item1).normalized;
        SendStrikerForceAndDirection();
    }

    private int FindPieceColour(List<(Vector2, int)> piecePos, Vector2 pos)
    {
        for (int i = 0; i < piecePos.Count; i++)
        {
            if (FastApproximately(piecePos[i].Item1.x, pos.x, 0.001f) && FastApproximately(piecePos[i].Item1.y, pos.y, 0.001f))
            {
                return piecePos[i].Item2;
            }
        }
        return -1;
    }

    private static bool FastApproximately(float a, float b, float threshold)
    {
        return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
    }

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------

    #region striker movement in line and adjustment of striker


    private IEnumerator StrikerMovementCoroutine(float target)
    {
        float currentPos = prevPosition;
        if(!Mathf.Approximately(target,currentPos))
        {
            float difference = Mathf.Abs(target - currentPos);
            float t = 0;
            Debug.Log("Difference = " + difference);
            if (target > currentPos)
            {
                while (currentPos < target)
                {
                    t += Time.smoothDeltaTime;
                    //Debug.Log(t);
                    currentPos += Time.smoothDeltaTime;
                    StrikerController.instance.StrikerOpponentSliderValueChanged(currentPos);
                    yield return new WaitForSeconds(Time.smoothDeltaTime);
                }
            }
            else
            {
                while (currentPos > target)
                {
                    t += Time.smoothDeltaTime;
                    //Debug.Log(t);
                    currentPos -= Time.smoothDeltaTime;
                    StrikerController.instance.StrikerOpponentSliderValueChanged(currentPos);
                    yield return new WaitForSeconds(Time.smoothDeltaTime);
                }
            }
            prevPosition = currentPos;
        }
        
    }
    public void StartAdjustingStrikerPosition(System.Action<float> sliderMovementMethod, float sliderInitialValue, float maxTime = 0)
    {
        if (!strikerBeingAdjusted)
        {
            strikerBeingAdjusted = true;
            strikerAdjustingCoroutine = StartCoroutine(AdjustStrikerPosition(sliderMovementMethod, sliderInitialValue, maxTime));
        }
    }

    public void EndAdjustingStrikerPosition()
    {
        if (strikerAdjustingCoroutine != null)
        {
            StopCoroutine(strikerAdjustingCoroutine);
            strikerBeingAdjusted = false;
        }
    }

    private IEnumerator AdjustStrikerPosition(System.Action<float> sliderMovementMethod, float sliderInitialValue, float maxTime = 0)
    {
        sliderValue = sliderInitialValue;
        float _totalTime = 0f;
        while (!StrikerController.instance.ValidSpot && sliderValue < 1f)
        {
            
            yield return new WaitForSeconds(0.025f);
            _totalTime += 0.025f;
            sliderValue += 0.025f;
            sliderMovementMethod(sliderValue);
            if(maxTime > 0 && _totalTime >= maxTime)
            {
                break;
            }
        }
        
        if (sliderValue > 1f)
        {
            sliderValue = 1f;
            sliderMovementMethod(sliderValue);
        }
        if(!StrikerController.instance.ValidSpot)
        {
            while (!StrikerController.instance.ValidSpot && sliderValue > sliderInitialValue)
            {
                yield return new WaitForSeconds(0.02f);
                _totalTime += 0.02f;
                sliderValue -= 0.08f;
                sliderMovementMethod(sliderValue);
                if (maxTime > 0 && _totalTime >= maxTime)
                {
                    break;
                }
            }
        }
        while (!StrikerController.instance.ValidSpot && sliderValue > 0f)
        {
            yield return new WaitForSeconds(0.025f);
            _totalTime += 0.025f;
            sliderValue -= 0.025f;
            sliderMovementMethod(sliderValue);
            if (maxTime > 0 && _totalTime >= maxTime)
            {
                break;
            }
        }
        if (sliderValue < 0f)
        {
            sliderValue = 0f;
            sliderMovementMethod(sliderValue);
        }
        prevPosition = sliderValue;
        strikerBeingAdjusted = false;
    }
    

    #endregion

    //--------------------------------------------------------------------------------------------------------------------------------------
}
