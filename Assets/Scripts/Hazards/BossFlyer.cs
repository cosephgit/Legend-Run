using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BossFlyer
// controls flying-type bosses like dragons
// created 20/8/24
// modified 20/8/24

public enum BossFlyerState
{
    Hidden, // inactive
    Approach, // approaching orbit (could be from hiding place or from end of attack)
    Orbit, // orbiting
    PreAttack, // moving to attack position
    Attack // in attack run
}

public class BossFlyer : MonoBehaviour
{
    [SerializeField] private float attackStartDistance = 100f; // how far forward from the player should an attack run start?
    [SerializeField] private Transform posHide; // the position which the boss uses to hide
    [SerializeField] private Transform posOrbit; // the position which the boss uses to orbit around
    [SerializeField] private float posOrbitRadius = 20f; // how far to orbit around posOrbit
    [SerializeField] private float moveSpeed = 20f; // average speed for the boss
    private float speedCurrent;
    public BossFlyerState state { get; private set; }
    private Vector3 orbitForward;
    private Vector3 orbitRight;
    private Vector3 orbitUp;
    private float orbitRadiansPerSecond;
    private float orbitAngle;

    private void Awake()
    {
        BossHide();
    }

    private void Start()
    {
    }


    private bool ManageOrbitMove()
    {
        float frameMove = Time.deltaTime * moveSpeed;

        if (frameMove > 0)
        {
            bool stable = true;

            Vector3 offset = transform.position - posOrbit.position;
            float heightOffset = Vector3.Dot(orbitUp, offset);
            offset -= heightOffset * orbitUp; // now get the offset on the forward/right plane
            float distance = offset.magnitude;

            if (distance < posOrbitRadius)
            {
                distance = Mathf.Min(distance + frameMove, posOrbitRadius);
                stable = false;
            }
            else if (distance > posOrbitRadius)
            {
                distance = Mathf.Max(distance - frameMove, posOrbitRadius);
                stable = false;
            }

            Debug.Log("heightOffset " + heightOffset);

            if (heightOffset > 0)
            {
                heightOffset = Mathf.Max(heightOffset - frameMove, 0);
                stable = false;
            }
            else if (heightOffset < 0)
            {
                heightOffset = Mathf.Min(heightOffset + frameMove, 0);
                stable = false;
            }

            Debug.Log("heightOffset (adjusted) " + heightOffset);

            Vector3 pos = posOrbit.position;
            orbitAngle += Time.deltaTime * orbitRadiansPerSecond;
            pos += orbitForward * Mathf.Cos(orbitAngle) * distance;
            pos += orbitRight * Mathf.Sin(orbitAngle) * distance;
            pos += orbitUp * heightOffset;

            transform.rotation = Quaternion.LookRotation(pos - transform.position);
            transform.position = pos;

            Debug.Log("orbit is stable? " + stable);

            return stable;
        }
        return false;
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case BossFlyerState.Hidden:
                break;
            case BossFlyerState.Approach:
                if (ManageOrbitMove())
                    state = BossFlyerState.Orbit;
                break;
            case BossFlyerState.Orbit:
                ManageOrbitMove();
                break;
        }
    }

    // start the attack! (make it fly into view and go into idle state)
    // timeToReady - the maximum time before the boss can be in a ready position
    public void BossBegin(float timeToReady)
    {
        gameObject.SetActive(true);
        CalculateOrbit();
        state = BossFlyerState.Approach;
    }

    // start an attack run for the boss
    // timeOnBase - the time before the boss MUST reach the attack run start facing the right away
    // timeOnTarget - the time before the boss MUST reach the end of the attack run
    public void BossAttack(float timeOnBase, float timeOnTarget)
    {

    }

    // stop the attack! (make it fly away)
    public void BossEnd()
    {

    }

    // turn the boss off
    public void BossHide()
    {
        state = BossFlyerState.Hidden;
        gameObject.SetActive(false);
    }


    // work out the Vector3s to define the orbit plane around the orbit position
    private void CalculateOrbit()
    {
        orbitForward = posOrbit.forward;
        orbitRight = posOrbit.right;
        orbitUp = posOrbit.up;

        Vector3 offset = transform.position - posOrbit.position;
        float offsetForward = Vector3.Dot(orbitForward, offset);
        float offsetRight = Vector3.Dot(orbitRight, offset);
        float offsetUp = Vector3.Dot(orbitUp, offset);

        Debug.Log("offset " + offset);
        Debug.Log("offsetForward " + offsetForward + " offsetRight " + offsetRight + " offsetUp " + offsetUp);
        Debug.Log("offset recombined " + (orbitForward * offsetForward + orbitRight * offsetRight + orbitUp * offsetUp));

        orbitAngle = Mathf.Atan2(offsetRight, offsetForward);

        Debug.Log("orbitAngle " + orbitAngle);

        /*
         * reducing
         */



        orbitRadiansPerSecond = moveSpeed / posOrbitRadius;
        if (CoSephUtils.RandomBool()) orbitRadiansPerSecond = -orbitRadiansPerSecond;
    }
}
