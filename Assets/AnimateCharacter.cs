using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateCharacter : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Range(0, 100f)] public float limbSpeed = 0.2f;
    [Range(0, 20f)] public float moveSpeed = 0.5f;
    [Range(0, 100f)] public float rotationSpeed = 10f;
    [Range(0, 10f)] public float rotationCooldown = 1.5f;

    [Header("Body Parts")]
    public Transform TopRightLeg;
    public Transform RightLeg;
    public Transform RightLegMid;
    [Space(10)]
    public Transform TopLeftLeg;
    public Transform LeftLeg;
    public Transform LeftLegMid;

    [Header("Clamp Angle Values")]
    [Range(-360, 360)] public float RightLegAngleRotateMin;
    [Range(-360, 360)] public float LeftLegAngleRotateMin;
    [Range(-360, 360)] public float RightLegMidAngleRotateMin;
    [Range(-360, 360)] public float LeftLegMidAngleRotateMin;
    [Range(-360, 360)] public float RightLegAngleRotateMax;
    [Range(-360, 360)] public float LeftLegAngleRotateMax;
    [Range(-360, 360)] public float RightLegMidAngleRotateMax;
    [Range(-360, 360)] public float LeftLegMidAngleRotateMax;

    [Header("Update Transform State")]
    public bool UpdateRightMid;
    public bool UpdateRight;
    public bool UpdateLeft;
    public bool UpdateLeftMid;

    [Header("TestingLeft")]
    public Transform FinalPoint;
    public Transform destinationTopLeg;
    public Transform originalTopLeg;

    [Header("TestingRight")]
    public Transform FinalPointRight;
    public Transform destinationTopLegRight;
    public Transform originalTopLegRight;
    public Transform originalRightLower;

    [Header("Body Shake Movements")]
    public Transform BodyTransform;
    public Transform PingForward;
    public Transform PingBackward;
    [SerializeField, Range(0.0f, 4.0f)] private float BodyChangeTime = 2.0f;

    private int currentIndex = 0;
    private float initialCoolDown = 0.0f;
    private float Timewait = 0.25f;
    private float SecondSwitch = 0.5f;
    private float secondInitial = 0.0f;
    private float initialTimeLeg = 0.0f;
    private int state = 0;
    private float bodyMoveTiming = 0.0f;
    private bool canTurn = false;
    private bool originalTurn = true;

    private void Start()
    {
        StartCoroutine(MoveLegs());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("obstacle"))
        {
            Debug.LogWarning("Obstacle on the way");
            canTurn = true;
            originalTurn = false;
        }
    }

    private void Update()
    {
        transform.localPosition += -transform.right * Time.deltaTime * moveSpeed;

        if (originalTurn)
        {
            initialCoolDown = 0.0f;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);
        }

        if (canTurn)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * rotationSpeed);

            if (initialCoolDown > rotationCooldown)
            {
                canTurn = false;
                originalTurn = true;
            }

            initialCoolDown += Time.deltaTime;
        }

        if (secondInitial > SecondSwitch)
        {
            state = 1 - state;
            secondInitial = 0;
            initialTimeLeg = 0;
        }

        secondInitial += Time.deltaTime;

        switch (state)
        {
            case 0:
                LegMoveSequenceUp(LeftLeg, LeftLegAngleRotateMin, LeftLegAngleRotateMax, Time.deltaTime * limbSpeed);
                LegMoveSequence(LeftLegMid, LeftLegMidAngleRotateMin, LeftLegMidAngleRotateMax, Time.deltaTime * limbSpeed);

                RightMoveSequenceUpReverse(RightLegMid, LeftLegAngleRotateMax, LeftLegAngleRotateMin, Time.deltaTime * limbSpeed);
                RightMoveSequenceReverse(RightLeg, LeftLegAngleRotateMax, LeftLegAngleRotateMin, Time.deltaTime * limbSpeed);

                PingPongBodyMovementsF(Time.deltaTime * limbSpeed);

                if (initialTimeLeg > Timewait)
                {
                    // TopLegMoveSequence(TopLeftLeg, LeftLegMidAngleRotateMin, LeftLegMidAngleRotateMin, Time.deltaTime * limbSpeed);
                }

                initialTimeLeg += Time.deltaTime;
                break;
            case 1:
                LegMoveSequence(LeftLegMid, LeftLegMidAngleRotateMax, LeftLegMidAngleRotateMin, Time.deltaTime * limbSpeed);
                TopLegMoveSequenceReverse(TopLeftLeg, LeftLegAngleRotateMax, LeftLegAngleRotateMin, Time.deltaTime * limbSpeed);

                RightMoveSequenceUp(RightLegMid, LeftLegAngleRotateMax, LeftLegAngleRotateMin, Time.deltaTime * limbSpeed);
                RightMoveSequence(RightLeg, LeftLegAngleRotateMax, LeftLegAngleRotateMin, Time.deltaTime * limbSpeed);

                PingPongBodyMovementsB(Time.deltaTime * limbSpeed);

                initialTimeLeg += Time.deltaTime;
                break;
        }

        LegUpdateTimer();
    }

    private IEnumerator MoveLegs()
    {
        while (true)
        {
            yield return new WaitForSeconds(6);
        }
    }

    private void LegUpdateTimer() { }

    public void RightMoveSequenceUp(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, FinalPointRight.localRotation, deltaTime);
    }

    public void RightMoveSequence(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, destinationTopLegRight.localRotation, deltaTime);
    }

    public void RightMoveSequenceUpReverse(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, originalRightLower.localRotation, deltaTime);
    }

    public void RightMoveSequenceReverse(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, originalTopLeg.localRotation, deltaTime);
    }

    public void LegMoveSequenceUp(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, FinalPoint.localRotation, deltaTime);
    }

    public void LegMoveSequence(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, Quaternion.Euler(AngleMax, Leg.localRotation.y, Leg.localRotation.z), deltaTime);
    }

    public void TopLegMoveSequence(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, destinationTopLeg.localRotation, deltaTime);
    }

    public void TopLegMoveSequenceReverse(Transform Leg, float AngleMin, float AngleMax, float deltaTime)
    {
        Leg.transform.localRotation = Quaternion.Lerp(Leg.transform.localRotation, originalTopLeg.localRotation, deltaTime);
    }

    private void PingPongBodyMovementsF(float deltaTime)
    {
        BodyTransform.localRotation = Quaternion.Lerp(BodyTransform.localRotation, PingForward.localRotation, deltaTime);
    }

    private void PingPongBodyMovementsB(float deltaTime)
    {
        BodyTransform.localRotation = Quaternion.Lerp(BodyTransform.localRotation, PingBackward.localRotation, deltaTime);
    }
}
