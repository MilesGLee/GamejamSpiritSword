using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Assigned outside of code
    public Transform orientation;
    public Transform playerCamera;

    //Other
    private Rigidbody playerRigidbody;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensitivityMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500f;
    public float maxSpeed = 20f;
    public bool grounded;
    public LayerMask whatIsGround;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch and Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400f;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 500;

    //Input
    float x, y;
    bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        //Set the base player scale and lock the cursor
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
    }

    //Move this to its own class eventually
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        //Set players size to the crouch size and move them closer to the ground to avoid height falling
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        //If the player was moving before crouching, begind sliding
        if (playerRigidbody.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                playerRigidbody.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        //Harder gravity
        playerRigidbody.AddForce(Vector3.down * Time.deltaTime * 10);

        //The magnitude of the relative velocity to where the player is looking
        Vector2 mag = FindVelocityRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counter movement for fluid strafing
        CounterMovement(x, y, mag);

        if (readyToJump && jumping) Jump();

        float maxSpeed = this.maxSpeed;

        //No jumping while crouched
        if (crouching && grounded && readyToJump)
        {
            playerRigidbody.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //Checking if the x or y exceed the max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Movement multipliers
        float multiplier = 1f, multiplierV = 1f;

        //Half the speed of the player if they are crouched
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        if (grounded && crouching) multiplierV = 0f;

        //Apply the calculated forces in relation to the orientation
        playerRigidbody.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        playerRigidbody.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Apply jump forces
            playerRigidbody.AddForce(Vector2.up * jumpForce * 1.5f);
            playerRigidbody.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while in air, reset y velocity
            Vector3 vel = playerRigidbody.velocity;
            if (playerRigidbody.velocity.y > 0.5f)
                playerRigidbody.velocity = new Vector3(vel.x, 0, vel.z);
            else if (playerRigidbody.velocity.y > 0)
                playerRigidbody.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensitivityMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensitivityMultiplier;

        //Find the current direction the player is looking
        Vector3 rotation = playerCamera.transform.localRotation.eulerAngles;
        desiredX = rotation.y + mouseX;

        //Rotate with maximums and minimums
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Apply the rotations
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        //Slow down sliding
        if (crouching)
        {
            playerRigidbody.AddForce(moveSpeed * Time.deltaTime * -playerRigidbody.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter Movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05 || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            playerRigidbody.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05 || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            playerRigidbody.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running
        if (Mathf.Sqrt((Mathf.Pow(playerRigidbody.velocity.x, 2) + Mathf.Pow(playerRigidbody.velocity.z, 2))) > maxSpeed)
        {
            float fallSpeed = playerRigidbody.velocity.y;
            Vector3 n = playerRigidbody.velocity.normalized * maxSpeed;
            playerRigidbody.velocity = new Vector3(n.x, fallSpeed, n.z);
        }
    }

    public Vector2 FindVelocityRelativeToLook()
    {
        //The players look direction and move direction
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(playerRigidbody.velocity.x, playerRigidbody.velocity.z) * Mathf.Rad2Deg;

        //Get the compared angle between the two
        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        //Calculate the magnitude between the new angles
        float magnitue = playerRigidbody.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        //Return the relative velocity
        return new Vector2(xMag, yMag);
    }

    private bool isFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    private void OnCollisionStay(Collision other)
    {
        //If on collision, if the collided object is the ground layer
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Loop through every contact the collided object has
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;

            //If one of the normals of the contacts is a floor
            if (isFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
}
