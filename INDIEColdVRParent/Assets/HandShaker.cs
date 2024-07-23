using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class HandShaker : MonoBehaviour
{
    // Public variables
    public GameObject handModel;
    [Header("Jitter Types")]
    [Tooltip("Enables rotational hand jitter (turning along axis of arm)")] public bool enableAxisJitter;
    [Tooltip("Enables positional hand jitter (moving around)")] public bool enableShakeJitter;
    [Tooltip("Enables jittering when close to a cold object")] public bool enableProximityJitterStudy;
    [Tooltip("In-code toggle for turning prox jitter on and off")] public bool enableProximityJitter;

    [Header("Axis Jitter Parameters")]
    [Tooltip("Maximum angle from 0 that the hand will rotate to")] public float axisUpperLimit = 5;
    [Tooltip("How fast the hand will rotate")] public float axisSpeed = 200;
    public bool enableAxisJitterX;
    public bool enableAxisJitterY;
    public bool enableAxisJitterZ;

    private int xDirAxis = -1;
    private int yDirAxis = -1;
    private int zDirAxis = -1;

    public float currentAxisLimit;
    public float currentAxisRotationX;
    public float currentAxisRotationY;
    public float currentAxisRotationZ;
    private Quaternion currentQuatRotation;

    [Header("Shake Jitter Parameters")]
    [Tooltip("Maximum distance from the origin the hand will shake to")] public float shakeUpperLimit = .1f;
    [Tooltip("Maximum distance the hand will move each tick")] public float shakeUpperTravelLimit = .5f;
    [Tooltip("How fast the hand will move")] public float shakeSpeed = 120;
    public float currShakeDistanceX;
    public float currShakeDistanceY;
    public float currShakeDistanceZ;

    [Header("Proximity Jitter Parameters")]
    [Tooltip("How close the hand needs to be to start shaking")] public double farProximity = 4;
    [Tooltip("How close the hand needs to be to be at max shaking")] public double closeProximity = .5;
    public float proxMultiplier = 0;

    [Header("Held Jitter Parameters")]
    [Tooltip("% reduction of shaking when an object is held (0-1)")] public float weightReduction = .5f;
    private float currentWeightReduction;

    private Vector3 currentPosition;
    private int xDirShake = 1;
    private int yDirShake = 1;
    private int zDirShake = 1;

    private CapsuleCollider proxCollider;
    
    private List<Collider> nearbyColdObj;
    private Grabber grabber;

    void Start()
    {
        nearbyColdObj = new List<Collider>();
        proxCollider = GetComponent<CapsuleCollider>();
        proxCollider.radius = (float)farProximity;
        grabber = GetComponent<Grabber>();
    }

    // Update is called once per frame
    void Update()
    {
        enableAxisJitter = GameManager.Instance.IsAxisJitter;
        enableShakeJitter = GameManager.Instance.IsShakeJitter;
        enableProximityJitterStudy = GameManager.Instance.IsProxJitter;

        if (grabber.locked)
        {
            currentWeightReduction = weightReduction;
        }
        else
        {
            currentWeightReduction = 1;
        }

        if (enableProximityJitterStudy)
        {
            if (enableProximityJitter)
            {
                proxMultiplier = 0;
                // For each nearby cold object, map its distance to a value between 0 and 1
                foreach (Collider col in nearbyColdObj)
                {
                    double currDist = Vector3.Distance(col.transform.position, this.transform.position);
                    proxMultiplier += (float)math.remap(farProximity, closeProximity, 0, 1, currDist);
                }
                if (proxMultiplier < 0)
                    proxMultiplier = 0;
                if (proxMultiplier > 1) // Cap it at 1
                    proxMultiplier = 1;

            }
            else // If we aren't close to anything, we shouldn't be cold
            {
                proxMultiplier = 0;
            }
        }
        else // If we don't have prox jitter on, make it irrelevant in calcs
        {
            proxMultiplier = 1;
        }


        if (enableAxisJitter)
        {
            Vector3 tempRot = new Vector3(currentAxisRotationX, currentAxisRotationY, currentAxisRotationZ);
            currentQuatRotation = Quaternion.Euler(tempRot);

            currentAxisLimit = axisUpperLimit * proxMultiplier;
            if (enableAxisJitterX)
            {
                currentAxisRotationX += axisSpeed * xDirAxis * Time.deltaTime * proxMultiplier * currentWeightReduction;
                if (Mathf.Abs(currentAxisRotationX) > currentAxisLimit)
                {
                    currentAxisRotationX = xDirAxis * currentAxisLimit;
                    xDirAxis *= -1;
                }
            }
            if (enableAxisJitterY)
            {
                currentAxisRotationY += axisSpeed * yDirAxis * Time.deltaTime * proxMultiplier * currentWeightReduction;
                if (Mathf.Abs(currentAxisRotationY) > currentAxisLimit)
                {
                    currentAxisRotationY = yDirAxis * currentAxisLimit;
                    yDirAxis *= -1;
                }
            }
            if (enableAxisJitterZ)
            {
                currentAxisRotationZ += axisSpeed * zDirAxis * Time.deltaTime * proxMultiplier * currentWeightReduction;
                if (Mathf.Abs(currentAxisRotationZ) > currentAxisLimit)
                {
                    currentAxisRotationZ = zDirAxis * currentAxisLimit;
                    zDirAxis *= -1;
                }
            }

        }
        if (enableShakeJitter)
        {
            currentPosition = new Vector3(currShakeDistanceX, currShakeDistanceY, currShakeDistanceZ);

            // X increment
            currShakeDistanceX += shakeSpeed * xDirShake * Time.deltaTime * UnityEngine.Random.Range(0, shakeUpperTravelLimit) * proxMultiplier * currentWeightReduction;
            if (Mathf.Abs(currShakeDistanceX) > shakeUpperLimit * proxMultiplier)
            {
                currShakeDistanceX = shakeUpperLimit * proxMultiplier * xDirShake;
                xDirShake *= -1;
            }

            // Y increment
            currShakeDistanceY += shakeSpeed * yDirShake * Time.deltaTime * UnityEngine.Random.Range(0, shakeUpperTravelLimit) * proxMultiplier * currentWeightReduction;
            if (Mathf.Abs(currShakeDistanceY) > shakeUpperLimit * proxMultiplier)
            {
                currShakeDistanceY = shakeUpperLimit * proxMultiplier * yDirShake;
                yDirShake *= -1;
            }

            // Z increment
            currShakeDistanceZ += shakeSpeed * zDirShake * Time.deltaTime * UnityEngine.Random.Range(0, shakeUpperTravelLimit) * proxMultiplier * currentWeightReduction;
            if (Mathf.Abs(currShakeDistanceZ) > shakeUpperLimit * proxMultiplier)
            {
                currShakeDistanceZ = shakeUpperLimit * proxMultiplier * zDirShake;
                zDirShake *= -1;
            }

        }
        handModel.transform.SetLocalPositionAndRotation(currentPosition, currentQuatRotation);
    }

    // Using a collider to keep track of the 'cold' objects nearby
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cold"))
        {
            nearbyColdObj.Add(other);
            enableProximityJitter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cold"))
        {
            nearbyColdObj.Remove(other);

            if (nearbyColdObj.Count == 0)
            {
                enableProximityJitter = false;
            }
        }
    }
}
