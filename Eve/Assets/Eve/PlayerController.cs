using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 moveDirection;
    Vector2 lookDirection;
    float jumpDirection;

    public float maxForwardSpeed = 8f;
    public float moveSpeed = 2f;
    public float turnSpeed = 100f;
    public float jumpSpeed = 30000f;


    float desiredSpeed;
    float forwardSpeed;

    bool escapPressed = false;

    const float groundAccel = 5f;
    const float groundDecel = 25f;
    //public Joystick JS;
    private Animator anim;
    Rigidbody rb;

    bool onGround = true;

    public Transform hip;
    public Transform hand;
    public Transform weapon;

    public bool isDead = false;
    public bool again = false;

    public void PickupGun()
    {
        weapon.SetParent(hand);
        weapon.localPosition = new Vector3(-0.003f, 0.044f, 0.012f);
        weapon.localRotation = Quaternion.Euler(-74.25201f, -171.096f, -70.96101f);
        weapon.localScale = new Vector3(1,1,1);

    }
    public void PutdownGun()
    {
        weapon.SetParent(hip);
        weapon.localPosition = new Vector3(-0.1114954f, -0.05937604f, -0.07853895f);
        weapon.localRotation = Quaternion.Euler(-96.031f, -64.80399f, -129.898f);
        weapon.localScale = new Vector3(1, 1, 1);

    }
    bool IsMoveInput { get { return !Mathf.Approximately(moveDirection.sqrMagnitude, 0f); } }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
    public void Onlook(InputAction.CallbackContext context)
    {
       lookDirection = context.ReadValue<Vector2>();
    }  
    public void OnJump(InputAction.CallbackContext context)
    {
        jumpDirection = context.ReadValue<float>();
    }

    bool firing = false;

    int health = 100;

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            health -= 10;
            anim.SetBool("Hit", true);
         //   anim.SetBool("Hit", false);

            if (health <= 0)
            {
                isDead = true;
                anim.SetLayerWeight(1, 0);
                anim.SetBool("Dead", true);
            }
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        firing = false;
        if ((int)context.ReadValue<float>() == 1 && anim.GetBool("Armed"))
        {
            anim.SetTrigger("Fire");
            firing = true;
        }

    }
    public void OnESC(InputAction.CallbackContext context)
    {
        if ((int)context.ReadValue<float>() == 1)
            escapPressed = true;
        else
            escapPressed = false;
    }
    //public void OnAgain(InputAction.CallbackContext context)
    //{
    //    if ((int)context.ReadValue<float>() == 1)
    //        again = true;
    //    else
    //        again = false;
    //}
    public void OnArmed(InputAction.CallbackContext context)
    {
        anim.SetBool("Armed", !anim.GetBool("Armed"));
    }

    public void Move(Vector2 direction)
    {
        float turnAmount = direction.x;
        float fDirecion = direction.y;
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();
        desiredSpeed = direction.sqrMagnitude * maxForwardSpeed * Mathf.Sign(fDirecion);
        float acceleration = IsMoveInput ? groundAccel : groundDecel;
        forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, acceleration * Time.deltaTime);
        anim.SetFloat("ForwardSpeed", forwardSpeed);
        transform.Rotate(0f, turnAmount * turnSpeed * Time.deltaTime, 0f);
        //transform.Translate(direction.x * moveSpeed * Time.deltaTime, 0, direction.y * moveSpeed * Time.deltaTime);

    }

    bool jumpReady = false;
    float jumpEffort = 0f;
    public void Jump(float direction)
    { 
        if(direction > 0f && onGround)
        {
            anim.SetBool("ReadyJump", true);
            jumpReady = true;
            jumpEffort += Time.deltaTime;
        }
        else if(jumpReady)
        {
            anim.SetBool("Launch", true);
            anim.SetBool("ReadyJump",false);
            jumpReady = false;
        }
        //Debug.Log("Jump Effort:" + jumpEffort);
    }
    public void Launch()
    {
        anim.SetBool("Launch", false);
        rb.AddForce(0f, jumpSpeed * Mathf.Clamp(jumpEffort,1,3), 0);
        rb.AddForce(this.transform.forward * forwardSpeed * 1000);
        anim.applyRootMotion = false;
        onGround = false;
    }
    public void Land()
    {
        anim.SetBool("Land", false);
        anim.applyRootMotion = true;
        anim.SetBool("Launch", false);
        jumpEffort = 0;
    }

        // Start is called before the first frame update
    void Start()
    {
        //JS = GameObject.FindWithTag("joystick").GetComponent<Joystick>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    float groundRayDist = 2f;

    public Transform spine;
    public float xSensitivity = 0.5f; 
    public float ySensitivity = 0.5f;

    Vector2 lastlookdirecion;
    public void LateUpdate()
    {
        if (isDead) return;
        if (anim.GetBool("Armed") && !isDead)
        {
            lastlookdirecion += new Vector2(-lookDirection.y * ySensitivity, lookDirection.x * xSensitivity);
            lastlookdirecion.x = Mathf.Clamp(lastlookdirecion.x, -30, 30);
            lastlookdirecion.y = Mathf.Clamp(lastlookdirecion.y, -30, 60);
            spine.localEulerAngles = lastlookdirecion;
        }
    }

    public LineRenderer laser;
    //public GameObject crossHair;
    public GameObject crossLight;

    bool cursorIsLocked = true;
    public void UpdateCursorLock()
    {
        if (escapPressed)
        {
            cursorIsLocked = false;
        }
        if (cursorIsLocked && !isDead)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
   //     UpdateCursorLock();
        Move(moveDirection);
        Jump(jumpDirection);
        if (anim.GetBool("Armed"))
        {
            laser.gameObject.SetActive(true);
            //crossHair.gameObject.SetActive(true);
            crossLight.gameObject.SetActive(true);

            RaycastHit laserHit;
            Ray laserRay = new Ray(laser.transform.position, laser.transform.forward);
            if (Physics.Raycast(laserRay, out laserHit))
            {
                laser.SetPosition(1, laser.transform.InverseTransformPoint(laserHit.point));
                Vector3 cameraLocation = Camera.main.WorldToScreenPoint(laserHit.point);
                // crossHair.transform.position = cameraLocation;
                crossLight.transform.localPosition = new Vector3(0, 0, laser.GetPosition(1).z * 0.9f);

                if (firing == true && laserHit.collider.gameObject.tag == "Orb")
                {
                    laserHit.collider.gameObject.GetComponent<AIController>().BlowUp();
                }
            }
            else
            {
                //crossHair.gameObject.SetActive(false);
                crossLight.gameObject.SetActive(false);
            }

        }
        else
        {
            laser.gameObject.SetActive(false);
            //crossHair.gameObject.SetActive(false);
            crossLight.gameObject.SetActive(false);
        }

        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up);
        if (Physics.Raycast(ray, out hit, groundRayDist))
        {
            if (!onGround)
            {
                anim.SetFloat("LandingVelocity", rb.velocity.magnitude);
                anim.SetBool("Land", true);
                anim.SetBool("Falling", false);
                onGround = true;
            }
        }
        else
        {
            //rb.AddForce(this.transform.forward * forwardSpeed * 25);
            anim.SetBool("Land", false);
            anim.SetBool("Falling", true);
            anim.applyRootMotion = false;
            onGround = false;
        }
        Debug.DrawRay(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up * groundRayDist, Color.red);
        //if (isDead)
        //{
        //    Debug.Log("Player is Dead for Play Again Type: ");
        //}
        
    }
}
