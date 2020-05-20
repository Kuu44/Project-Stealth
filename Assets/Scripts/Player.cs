using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;

    public float moveSpeed=7;
    public float smoothMoveTime = 0.1f;
    public float turnSpeed = 8;

    float smoothInputMagnitude;
    float smoothMoveVelocity;
    float currentAngle;
    Vector3 velocity;

    Rigidbody rigidBody;
    bool disabled=false;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
    }

    void Update()
    {
        Vector3 input = Vector3.zero; 
        if(!disabled) input= new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        float inputMagnitude = input.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(input.x, input.z)*Mathf.Rad2Deg;
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * turnSpeed*inputMagnitude);
        //transform.eulerAngles = Vector3.up * currentAngle;

        //transform.Translate(transform.forward * moveSpeed * Time.deltaTime*smoothInputMagnitude, Space.World);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }
    private void FixedUpdate()
    {
        rigidBody.MoveRotation(Quaternion.Euler(Vector3.up * currentAngle));
        rigidBody.MovePosition(rigidBody.position + velocity * Time.deltaTime);

    }
    private void Disable()
    {
        disabled = true;
    }
    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
    private void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null) OnReachedEndOfLevel();
        }
    }
}
