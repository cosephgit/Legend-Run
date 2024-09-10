using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MenuCompanion
// simple script just to make the companion fly in a circle

public class MenuCompanion : MonoBehaviour
{
    [SerializeField] private Animator companionAnim;
    [SerializeField] private float rotationRate = 1f;

    private void Start()
    {
        companionAnim.SetBool("Run", true);
    }

    private void Update()
    {
        Vector3 angles = transform.eulerAngles;

        angles.y += Time.deltaTime * 360f * rotationRate;

        transform.eulerAngles = angles;
    }
}
