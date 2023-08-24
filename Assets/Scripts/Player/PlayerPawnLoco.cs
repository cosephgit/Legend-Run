using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// locomotion component for the player pawn
// created 18/8/23
// last modified 24/8/23

public class PlayerPawnLoco : MonoBehaviour
{
    private const string ANIM_RUN = "Run Blend"; // the keyword used in the animator for running speed
    private const string ANIM_FLY = "Flying"; // keyword for the flying animator boolean
    private const string ANIM_AIRSPEED = "Air Speed"; // keyword for the flying animator boolean
    [Header("Jump settings")]
    [SerializeField] private float jumpHeight = 4f; // desired jump height
    [SerializeField] private float jumpGravity = -10f; // fake gravity
    [field: SerializeField] public Animator pawnAnim { get; private set; }
    [field: SerializeField] public float moveRate { get; private set; } = 4f;
    private float targetX;
    private float speed;
    private float jumpStartSpeed;
    private float speedY; // current vertical speed
    private float height; // current fake height (> 0  means off the ground and invulnerable)
    private float heightFloor;
    public bool jumping { get; private set; }

    private void Awake()
    {
        jumpStartSpeed = Mathf.Sqrt(-2 * jumpHeight * jumpGravity);
        heightFloor = transform.position.y;
        speedY = 0;
        speed = 0;
        /* working out the math:
         * s = ut + 1/2at^2
         * find the peak:
         * ds/dt = u + at
         * 0 = jumpstartspeed + jumpGravity * t
         * t = -jumpstartspeed / jumpgravity
         * substituting:
         * jumpheight = jumpStartSpeed * -jumpstartspeed / jumpgravity + 1/2 * jumpgravity * jumpstartspeed^2 / jumpgravity^2
         * multiply by jumpgravity
         * jumpheight * jumpgravity = -jumpstartspeed ^ 2 + 1/2 * jumpstartspeed ^ 2
         * -0.5 * jumpstartspeed ^ 2 = jumpheight * jumpgravity
         * jumpstartspeed = sqrt(-2 * jumpheight * jumpgravity)
         * EXAMPLE
         * jumpstartspeed = sqrt(-2 * 4 * -10)
         * =sqrt(80)
         * =8.9
         */
    }

    public void SetMoveTarget(float targetXNew)
    {
        targetX = targetXNew;
    }

    public void SetSpeed(float speedNew)
    {
        speed = speedNew;
        pawnAnim.SetFloat(ANIM_RUN, speed);
    }

    private void FixedUpdate()
    {
        bool moving = !Mathf.Approximately(transform.position.x, targetX);
        if (jumping || moving)
        {
            Vector3 pos = transform.position;

            if (moving)
            {
                if (targetX > pos.x)
                {
                    pos.x = Mathf.Min(targetX, pos.x + Time.fixedDeltaTime * moveRate * (1f + speed) * 0.5f);
                }
                else if (targetX < pos.x)
                {
                    pos.x = Mathf.Max(targetX, pos.x - Time.fixedDeltaTime * moveRate * (1f + speed) * 0.5f);
                }
            }

            if (jumping)
            {
                height += Time.fixedDeltaTime * speedY;
                if (height < 0)
                {
                    // jump has finished
                    jumping = false;
                    pawnAnim.SetBool(ANIM_FLY, false);
                    pawnAnim.SetFloat(ANIM_AIRSPEED, 0f);
                    height = 0;
                    speedY = 0;
                    pos.y = heightFloor;
                }
                else
                {
                    speedY += Time.fixedDeltaTime * jumpGravity;
                    pawnAnim.SetFloat(ANIM_AIRSPEED, speedY);
                    pos.y = heightFloor + height;
                }


                transform.position = pos;
            }

            transform.position = pos;
        }
    }

    // called by the player pawn to start a jump sequence
    // returns false if already in the middle of jumping
    public bool StartJump()
    {
        if (jumping)
        {
            return false;
        }
        else
        {
            jumping = true;
            speedY = jumpStartSpeed;
            pawnAnim.SetBool(ANIM_FLY, true);
            return true;
        }
    }
}
