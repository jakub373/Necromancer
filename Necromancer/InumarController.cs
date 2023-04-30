using UnityEngine;
using UnityEngine.InputSystem;

public class InumarController : MonoBehaviour
{
    [SerializeField] private float targetSpeed;
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float baseMoveSpeed = 2.0f;
    [SerializeField] private float MoveSpeed;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float SpeedChangeRate = 10.0f;

    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField] private float Gravity = -15.0f;
    [Space(10)]
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] private bool grounded = true;
    [Tooltip("Useful for rough ground")]
    [SerializeField] private float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    [SerializeField] private float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -30.0f;

    private float cinemachineTargetYaw, cinemachineTargetPitch;

    private float animationBlend, rotationVelocity, verticalVelocity, fallTimeoutDelta;
    private float terminalVelocity = 53.0f;
    private float targetRotation = 0.0f;

    public Vector2 move, look;
    public bool jump, sprint;
    [SerializeField] private float speed;
    private const float threshold = 0.01f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;

    private int animIDSpeed, animIDGrounded, animIDFreeFall, animIDMotionSpeed;

    public Animator animator;
    private StatusShadow status;
    private Mouse mouse;
    private Keyboard key;
    private CharacterController controller;
    private GameObject mainCamera;
    private HeroInputs controls;

    private void Awake()
    {
        if (mainCamera == null) { mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); }
        controls = new HeroInputs();
        status = GetComponent<StatusShadow>();
    }

    private void Start()
    {
        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        controller = GetComponent<CharacterController>();
        
        AssignAnimationIDs();

        fallTimeoutDelta = FallTimeout;
        MoveSpeed = baseMoveSpeed;

        mouse = Mouse.current;
        key = Keyboard.current;
    }

    private void Update()
    {
        if (status.setupState || status.gameMenu.activeSelf) return;
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate() 
    {
        if (status.setupState || status.gameMenu.activeSelf) return;
        CameraRotation(); 
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (animator != null) { animator.SetBool(animIDGrounded, grounded); }
    }

    private void CameraRotation()
    {
        // Obrót kamery w kierunku kursora!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        TryGetComponent(out CementaryGround_Shoot cementary);
        if (mouse.leftButton.isPressed && !cementary.cementaryBool) { transform.rotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0); }

       if (look.sqrMagnitude >= threshold)
        {
            cinemachineTargetYaw += look.x;
            cinemachineTargetPitch += look.y;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
    }

    private void Move()
    {
        if (MoveSpeed <= 0)
        {
            MoveSpeed = 0;
            transform.rotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);
        }

        targetSpeed = MoveSpeed;

        TryGetComponent(out CementaryGround_Shoot cementary);
        if ((cementary.cementary && key.eKey.isPressed) || mouse.leftButton.isPressed) { targetSpeed = 0; }

        if (move == Vector2.zero) targetSpeed = 0.0f;
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = 0;
        if (cementary.cementaryBool) { inputMagnitude = 0; } else { inputMagnitude = move.magnitude; }

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * move.magnitude, Time.deltaTime * SpeedChangeRate);

            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else { speed = targetSpeed; }

        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

        if (move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            // Obracaj na input.look, gdy nie jest wciœniêta myszka !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (!mouse.leftButton.isPressed && !(cementary.cementary && key.eKey.isPressed))
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat(animIDSpeed, animationBlend);
            animator.SetFloat(animIDMotionSpeed, inputMagnitude);
        }
    }

    private void JumpAndGravity()
    {
        jump = false;

        if (!grounded)
        {
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool(animIDFreeFall, true);
                }
            }

            jump = false;
        }
        else { animator.SetBool(animIDFreeFall, false); }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
        }
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();
    public void OnMove(InputValue value) => MoveInput(value.Get<Vector2>());
    public void OnLook(InputValue value) => LookInput(value.Get<Vector2>());
    public void OnJump(InputValue value) => JumpInput(value.isPressed);
    public void OnSprint(InputValue value) => SprintInput(value.isPressed);
    public void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;
    public void LookInput(Vector2 newLookDirection) => look = newLookDirection;
    public void JumpInput(bool newJumpState) => jump = newJumpState;
    public void SprintInput(bool newSprintState) => sprint = newSprintState;
}