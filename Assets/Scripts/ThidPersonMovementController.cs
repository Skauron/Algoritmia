using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class ThidPersonMovementController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform normalCam;
    [SerializeField] [Range(1f, 7f)] private float speed = 2f;
    [SerializeField] [Range(0f, 1f)] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Variable Powers")]
    [SerializeField] private List<Power> listPowers;
    [SerializeField] private bool telekinesisActive = false;
    [SerializeField] private GameObject objTelekines = null;

    [Header("UI")]
    [SerializeField] private Canvas canvas;

    //Animations
    private Animator animator;
    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;
    private float acceleration = 2.0f;
    private float deceleration = 2.0f;
    private float maximumWalkVelocity = 0.5f;
    private float maximumRunVelocity = 2.0f;
    private int VelocityZHash;
    private int VelocityXHash;

    [Header("Gravity")]
    [SerializeField] private float gravity;
    [SerializeField] private float currentGravity;
    [SerializeField] private float constantGravity;
    [SerializeField] private float MaxGravity;
    private Vector3 gravityDirection = Vector3.down;
    private Vector3 gravityMovement;

    //Keys
    private bool forwardPressed;
    private bool leftPressed;
    private bool rightPressed;
    private bool backwardPressed;
    private bool runPressed;
    private bool AimPressed;
    private bool ShootPressed;

    [Header("CineMachine Aim Camera")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform CameraLookAt;
    [SerializeField] private Cinemachine.AxisState xAxis;
    [SerializeField] private Cinemachine.AxisState yAxis;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;

    // holds lock values to manage the Windows cursor
    CursorLockMode lockMode;
    void Awake()
    {
        lockMode = CursorLockMode.Locked;
        Cursor.lockState = lockMode;
    }

    void Start()
    {
        //Get the animator from the gameObject

        animator = GetComponent<Animator>();

        //Set the Hash for the animator
        VelocityZHash = Animator.StringToHash("Velocity Z");
        VelocityXHash = Animator.StringToHash("Velocity X");

        canvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GeyKeys();
        CalculateGravity();
        if (AimPressed)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            Aim();
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            canvas.gameObject.SetActive(false);
            if(objTelekines != null){
                objTelekines.GetComponent<Target>().gravity = true;
            }
            objTelekines = null;
            telekinesisActive = false;
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            Animation();
            Movement();
        }
    }

    void FixedUpdate()
    {
        if (AimPressed)
        {
            xAxis.Update(Time.deltaTime);
            yAxis.Update(Time.deltaTime);

            CameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value - 180f, 0f);

            if (telekinesisActive)
            {
                Telekinesis();
            }
        }
    }

    #region Aim and Shoot
    private void Aim()
    {
        canvas.gameObject.SetActive(true);

        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            //debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        Shoot(hitTransform);

        //Rotation of the player
        // Vector3 worldAimTarget = mouseWorldPosition;
        // worldAimTarget.y = transform.position.y;
        // Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        // transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }

    public void Shoot(Transform hitTransform)
    {
        if (ShootPressed)
        {
            if (hitTransform != null)
            {
                StartCoroutine(ShootAnimation());
                if (hitTransform.GetComponent<Target>() != null)
                {
                    hitTransform.GetComponent<Target>().Hit();
                    if(listPowers[0].IsActive){
                       Instantiate(vfxHitGreen, hitTransform.position, Quaternion.identity); 
                    }
                }
                else
                {
                    //Instantiate(vfxHitRed, hitTransform.position, Quaternion.identity);
                }
            }
        }
    }

    IEnumerator ShootAnimation()
    {
        animator.SetBool("Shoot", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("Shoot", false);
    }
    #endregion

    #region VariablePowers
    public void SetVariablePower(int index, bool state)
    {
        listPowers[index].IsActive = state;
    }

    public bool GetVariablePower(int index)
    {
        return listPowers[index].IsActive;
    }

    public void SetTelekinesis(bool newState, GameObject obj)
    {
        telekinesisActive = newState;
        objTelekines = obj;
    }

    public bool GetTelekinesis()
    {
        return telekinesisActive;
    }

    public void Telekinesis()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        
        //objTelekines.transform.Translate(new Vector3(horizontal, 0.5f, vertical) * Time.deltaTime);
        objTelekines.GetComponent<Rigidbody>().AddForce(new Vector3(horizontal, 0.05f, vertical) * Time.deltaTime * 5f, ForceMode.Impulse);
    }
    #endregion

    #region Inputs
    private void GeyKeys()
    {
        forwardPressed = Input.GetKey(KeyCode.W);
        leftPressed = Input.GetKey(KeyCode.A);
        rightPressed = Input.GetKey(KeyCode.D);
        backwardPressed = Input.GetKey(KeyCode.S);
        runPressed = Input.GetKey(KeyCode.LeftShift);
        AimPressed = Input.GetMouseButton(1);
        ShootPressed = Input.GetMouseButtonDown(0);
    }
    #endregion

    #region Gravity
    private void CalculateGravity()
    {
        if (IsGrounded())
        {
            currentGravity = constantGravity;
        }
        else
        {
            if (currentGravity > MaxGravity)
            {
                currentGravity -= gravity * Time.deltaTime;
            }
        }
        gravityMovement = gravityDirection * -currentGravity * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        return controller.isGrounded;
    }
    #endregion

    #region Movement
    void Movement()
    {
        //Getting the axis for the movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //Get the direction and normalized
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + normalCam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (runPressed)
                speed = 5f;
            else
                speed = 2f;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move((moveDir.normalized + gravityMovement) * speed * Time.deltaTime);
        }
        else
        {
            controller.Move(gravityMovement);
        }
    }
    #endregion

    #region Animations
    void Animation()
    {
        //Set current maxVelocity
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        ChangeVelocity(currentMaxVelocity);
        LockOrResetVelocity(currentMaxVelocity);

        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
    }

    void ChangeVelocity(float currentMaxVelocity)
    {
        //If player press forward, increase velocity in z direction
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        //If player press backward, increase velocity in z direction
        if (backwardPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        //Increase velocity in left direction
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        //Increase velocity in right direction
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        //Decrease velocityZ
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        //Decrease velocityZ
        if (!backwardPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        //Decrease velocityX if left is not pressed and velocityX < 0
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        //Decrease velocityX if right is not pressed and velocityX > 0
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    void LockOrResetVelocity(float currentMaxVelocity)
    {
        //Reset VelocityZ
        if (!forwardPressed && !backwardPressed && velocityZ != 0.0f && (velocityZ > -0.5f && velocityZ < 0.5f))
        {
            velocityZ = 0.0f;
        }

        //Reset VelocityX
        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.5f && velocityX < 0.5f))
        {
            velocityX = 0.0f;
        }

        //Lock forward
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        //Decelerate to the maximum walk velocity
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        //Round to the currentMaxVelocity if wathin offset
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }

        //Lock backward
        if (backwardPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        //Decelerate to the maximum walk velocity
        else if (backwardPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            if (velocityZ < currentMaxVelocity && velocityZ > (-currentMaxVelocity + 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        //Round to the currentMaxVelocity if wathin offset
        else if (backwardPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }

        //Lock left
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        //Decelerate to the maximum walk velocity
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            //Round to the currenMaxVelocity if within offset
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity + 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        //Round to the currentMaxVelocity if within offset
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        //Lock right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        //Decelerate to the maximum walk velocity
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            //Round to the currenMaxvelocity if within offset
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        //Round to the currentMaxVelocity if within offset
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }
    #endregion
}
