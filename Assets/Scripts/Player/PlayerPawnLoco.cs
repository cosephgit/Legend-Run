using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// locomotion component for the player pawn
// created 18/8/23
// last modified 18/8/23

public class PlayerPawnLoco : MonoBehaviour
{
    private const string ANIM_RUN = "Run Blend"; // the keyword used in the animator for running speed
    [field: SerializeField] public Animator pawnAnim { get; private set; }
    [field: SerializeField] public float moveRate { get; private set; } = 4f;
    private float targetX;
    private float speed;

    public void SetMoveTarget(float targetXNew)
    {
        targetX = targetXNew;
    }

    public void SetSpeed(float speedNew)
    {
        speed = speedNew;
        pawnAnim.SetFloat(ANIM_RUN, speed);
    }

    private void Update()
    {
        if (!Mathf.Approximately(transform.position.x, targetX))
        {
            Vector3 pos = transform.position;

            if (targetX > pos.x)
            {
                pos.x = Mathf.Min(targetX, pos.x + Time.deltaTime * moveRate * (1f + speed) * 0.5f);
            }
            else if (targetX < pos.x)
            {
                pos.x = Mathf.Max(targetX, pos.x - Time.deltaTime * moveRate * (1f + speed) * 0.5f);
            }

            transform.position = pos;
        }
    }
}
