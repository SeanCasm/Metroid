using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Player.PowerUps;
using System;
using Player;
using UnityEngine.Events;
/// <summary>
/// 
/// </summary>
public class PlayerController : MonoBehaviour
{
    #region Properties
    public static PlayerController current;
    [SerializeField] CapsuleCollider2D capsule;
    [SerializeField] InputManager inputManager;
    [SerializeField] BoxCollider2D edgeCollider, hurtBox;
    [SerializeField, Range(.01f, 1f)] float jumpTime = 0.35f;
    [SerializeField] Transform camHandle;
    [SerializeField] AnimatorHandler animatorHandler;
    [Header("Floor config")]
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float overHeadCheck = .01f;
    [SerializeField] float wallDistance, wallEdgeOffset, edgesOffset, spinOffset;
    [Header("Running and Speed Booster config")]
    [SerializeField] SpeedBooster speedBoosterComp;
    [SerializeField] Shinespark shinespark;
    [Tooltip("Standard running without any boost.")]
    [SerializeField, Range(0, 115)] float runningSpeed = 100;
    [Tooltip("Speed Booster max speed")]
    [SerializeField, Range(100, 130)] float speedBooster = 115;
    [SerializeField, Range(0, 200)] float maxSpeed;
    [SerializeField, Range(0.15f, 3)] float speedIncreaseOverTime = 1.5f;
    [Header("Misc events")]
    [SerializeField] UnityEvent<bool> onScrew;
    [SerializeField] UnityEvent onMorphball, onEnable;
    private Vector2 direction;
    private float jumpForce = 88, speed = 88, curSpinOffset = 1, slow2Gravity = 1;
    private float yInput = 0, xVelocity, jumpTimeCounter, currentSpeed, angleAim, spriteCenter;
    public float XVelocity { get => xVelocity; }
    public float Slow2Gravity
    {
        get
        {
            return slow2Gravity;
        }
        set
        {
            slow2Gravity = value;
            rb.gravityScale /= slow2Gravity;
        }
    }
    public float xInput { get; private set; } = 0;
    public Animator anim { get; set; }
    private PlayerFXHandler playerFX;
    private SpriteRenderer spriteRenderer;
    private Gun gun;
    private bool fall, isJumping, firstAir, airShoot, morphSpin,
       onJumpingState, _onSpin, _shootOnWalk, wallInFront;

    private System.Action OnJump = delegate { };
    private GroundState _groundState = GroundState.Stand;
    public JumpType jumpType { get; set; } = JumpType.Default;
    private RunningState runningState = RunningState.None;
    public AngleAim aimState { get; private set; } = AngleAim.None;
    public Status status { get; private set; } = Status.Normal;
    private int aimUpDown = 0;
    public float currentJumpForce { get; set; }
    private bool isGrounded { get; set; }
    public bool groundOverHead { get; private set; }
    public bool canMorph { get; set; } = true;
    public float slow2Forces = 1;
    public AnimatorHandler AnimatorHandler{get=>animatorHandler;}
    public bool OnSpin
    {
        get => _onSpin;
        set
        {
            _onSpin = value;
            if (value)
            {
                playerFX.RollJump(jumpType);
                if (jumpType == JumpType.Screw && _groundState == GroundState.Stand)
                {
                    onScrew?.Invoke(true);
                    status = Status.Powered;
                }
                curSpinOffset = spinOffset;
            }
            else
            {
                curSpinOffset = 1;
                playerFX.StopAudio(jumpType);
                if (jumpType == JumpType.Screw)
                {
                    onScrew?.Invoke(false);
                    if (status != Status.Damaged) status = Status.Normal;
                }
            }
            animatorHandler.FixSpritePivot(
                _onSpin && jumpType == JumpType.Default,
                jumpType == JumpType.Screw && _onSpin && _groundState == GroundState.Stand,
                jumpType == JumpType.Space && _onSpin
            );
        }
    }
    public bool leftLook { get; set; }
    public bool IsJumping
    {
        get => isJumping;
        set
        {
            isJumping = value;
            if (isJumping) { groundChecker.OnJumping(); isGrounded = false; }
            else jumpTimeCounter = 0;
        }
    }
    public bool ShootOnWalk
    {
        get => _shootOnWalk;
        set
        {
            _shootOnWalk = value;
            if (_shootOnWalk) CancelAndInvoke(nameof(shootingClocking), 2f);
            else CancelInvoke(nameof(shootingClocking));
        }
    }
    public GroundState GroundState
    {
        get => _groundState;
        set
        {
            _groundState = value;
            if (_groundState == GroundState.Balled)
            {
                onMorphball?.Invoke();
                OnSpin = false;
                angleAim = 0;
                aimUpDown = 0;
            }
            else
            {
                gun.OnStand?.Invoke();
            }
        }
    }
    private Rigidbody2D rb;
    #endregion
    #region Unity methods
    private void OnEnable()
    {
        onEnable.Invoke();
        groundChecker.OnGroundLanding += FirstOnGround;
        if (inputManager.lockFireInput) inputManager.DisableFireInput();
        if (inputManager.HorizontalMovement == null)
        {
            inputManager.HorizontalMovement += MoveHorStarted;
            inputManager.HorizontalMovementCanceled += MoveHorCanceled;
            inputManager.VerticalMovement += MoveVerStarted;
            inputManager.VerticalMovementCanceled += MoveVerCanceled;
            inputManager.Run += Run;
            inputManager.RunCanceled += RunCanceled;
            inputManager.Morph += QuickMorphball;
            inputManager.Jump += Jump;
            inputManager.JumpCanceled += JumpCanceled;
            inputManager.AimAngleUp += AngleAimUp;
            inputManager.AimAngleUpCanceled += AngleAimUpCanceled;
            inputManager.AimAngleDown += AngleAimDown;
            inputManager.AimAngleDownCanceled += AngleAimDownCanceled;
        }
    }
    private void OnDisable()
    {
        groundChecker.OnGroundLanding -= FirstOnGround;
        inputManager.DisablePlayerControls();
    }
    private void OnDestroy()
    {
        inputManager.HorizontalMovement -= MoveHorStarted;
        inputManager.HorizontalMovementCanceled -= MoveHorCanceled;
        inputManager.VerticalMovement -= MoveVerStarted;
        inputManager.VerticalMovementCanceled -= MoveVerCanceled;
        inputManager.Run -= Run;
        inputManager.RunCanceled -= RunCanceled;
        inputManager.Morph -= QuickMorphball;
        inputManager.Jump -= Jump;
        inputManager.JumpCanceled -= JumpCanceled;
        inputManager.AimAngleUp -= AngleAimUp;
        inputManager.AimAngleUpCanceled -= AngleAimUpCanceled;
        inputManager.AimAngleDown -= AngleAimDown;
        inputManager.AimAngleDownCanceled -= AngleAimDownCanceled;
        current = null;
    }
    void Awake()
    {
        current = this;
        slow2Gravity = 1;
        OnJump += OnNormalJump;
        jumpTimeCounter = jumpTime;
        currentJumpForce = jumpForce;
    }
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerFX = GetComponentInChildren<PlayerFXHandler>();
        gun = GetComponentInChildren<Gun>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        currentSpeed = speed;
    }

    void Update()
    {
        if (status != Status.Damaged)
        {
            spriteCenter = spriteRenderer.bounds.size.y / 2;
            camHandle.position = TransformCenter();
            hurtBox.transform.position = capsule.bounds.center;
            hurtBox.size = new Vector2(capsule.bounds.size.x + .01f, capsule.bounds.size.y + .01f);

            isGrounded = groundChecker.SetFloorChecking();

            if (isGrounded) OnGround();
            else OnAir();

        }
    }
    void FixedUpdate()
    {
        if (status != Status.Damaged)
        {
            if (groundOverHead && _groundState != GroundState.Balled)
                xInput = 0;

            xVelocity = xInput * (currentSpeed / slow2Forces) * Time.deltaTime;
            if (!isGrounded)
            {
                if (isJumping && jumpTimeCounter > 0f) rb.velocity = Vector2.up * (currentJumpForce / slow2Forces) * Time.deltaTime;

                if (onJumpingState) SetVelocity(new Vector2(xVelocity / 2, rb.velocity.y));
                else if (_onSpin || morphSpin) SetVelocity(new Vector2(xVelocity, rb.velocity.y));
            }
            else if (isGrounded && xInput != 0 && !wallInFront) groundChecker.FixedUpdateOnGround();
        }
    }
    void LateUpdate()
    {
        if (Time.timeScale > 0 && shinespark.ShinesparkState != ShinesparkState.Full) AnimStates();
    }
    #endregion
    public void SetAllInput(bool active) => inputManager.SetAllInput(active);
    public Vector3 TransformCenter()
    {
        return _onSpin ? transform.position : new Vector3(transform.position.x, transform.position.y + spriteCenter);
    }
    private void CancelAndInvoke(string method, float time)
    {
        CancelInvoke(method);
        Invoke(method, time);
    }

    #region On ground methods
    void OnGround()
    {
        groundChecker.CheckSlopesAndEdges();
        CheckWallInFront();
        CheckGroundUp();
        if (xInput != 0f)
        {
            if (runningState == RunningState.Running && slow2Forces == 1)
            {
                currentSpeed += speedIncreaseOverTime;
                if (currentSpeed >= maxSpeed) currentSpeed = maxSpeed;
                if (currentSpeed >= speedBooster)
                {
                    runningState = RunningState.MaxSpeed;
                    speedBoosterComp.InvokeGhost();
                }
            }
        }
        else
        {
            if (runningState == RunningState.MaxSpeed && shinespark.ShinesparkState != ShinesparkState.Charged)
            {
                speedBoosterComp.CancelGhost();
            }
            rb.velocity = Vector2.zero;
            currentSpeed = speed;
        }
    }
    void CheckWallInFront()
    {
        RaycastHit2D wallHit = Physics2D.BoxCast(new Vector2(capsule.bounds.center.x + (capsule.size.x / 2) * direction.x,
        capsule.bounds.center.y),
        new Vector2(wallDistance, capsule.bounds.size.y - wallEdgeOffset), 0f, direction, wallDistance, groundLayer);
        if (wallHit && Vector2.Angle(wallHit.normal, Vector2.up) == 90)
        {
            wallInFront = true;
            xInput = 0;
        }
        else wallInFront = false;
    }
    private void FirstOnGround(float y)
    {
        aimUpDown = 0;
        if (_onSpin && !morphSpin)
        {
            StartCoroutine(FixPivot(y));
            if (groundOverHead) GroundState = GroundState.Crouched;
        }
        morphSpin = IsJumping = airShoot = OnSpin = firstAir = onJumpingState = fall = false;
        playerFX.StopAudio(StopAction.All);
    }
    /// <summary>
    /// Fix the transform.position pivot center when player arrives to the ground at spin jump.
    /// </summary>
    /// <param name="y">point.y of the raycast collision with the ground</param>
    /// <returns></returns>
    IEnumerator FixPivot(float y)
    {
        yield return new WaitWhile(() => animatorHandler.AnyJump());
        transform.position = new Vector3(transform.position.x, y, 0);
    }

    #endregion
    #region On air methods 
    void OnAir()
    {
        FirstAir();
        if (aimUpDown == 0 && !isJumping && status == Status.Normal && shinespark.ShinesparkState != ShinesparkState.Full
        && gun.fireType == FireType.Normal && angleAim == 0 && !airShoot && !onJumpingState && !_onSpin)
        {
            fall = true;
        }
        else
        {
            fall = false;
            if (isJumping)
            {
                if (Physics2D.Raycast(transform.position + new Vector3(0, spriteCenter), Vector2.up, spriteCenter, groundLayer))
                {
                    IsJumping = false;
                    jumpTimeCounter = 0;
                }
                if (jumpTimeCounter > 0) jumpTimeCounter -= Time.deltaTime;
                else IsJumping = false;
            }
        }
    }
    private bool CheckWallJump()
    {
        if (Physics2D.Raycast(transform.position, Vector2.left, 0.28f, groundLayer) && xInput > 0) return true;
        else if (Physics2D.Raycast(transform.position, Vector2.right, 0.28f, groundLayer) && xInput < 0) return true;
        return false;
    }
    private void FirstAir()
    {
        if (!firstAir)
        {
            if (_onSpin) transform.position = new Vector3(transform.position.x, transform.position.y + spriteCenter, 0);
            edgeCollider.enabled = false; rb.gravityScale = 1 / slow2Gravity; firstAir = true;
            wallInFront = ShootOnWalk = groundChecker.OnSlope = false;
        }
    }
    #endregion
    private void AnimStates()
    {
        bool[] values =
        {
            angleAim < 0,angleAim > 0,  _groundState == GroundState.Balled,   isGrounded && xInput != 0 && !wallInFront && !groundOverHead,
            (_shootOnWalk || gun.fireType != FireType.Normal) && isGrounded && xInput != 0,     _groundState == GroundState.Crouched,
            leftLook,   /*idle*/isGrounded && xInput == 0,  isGrounded,/*spin jump*/_onSpin && jumpType == JumpType.Default,
            /*airshoot*/(gun.fireType != FireType.Normal || airShoot) && !isGrounded && !_onSpin,
            /*screw*/jumpType == JumpType.Screw && _onSpin && _groundState == GroundState.Stand,
            /*jump state*/onJumpingState,fall,  /*gravity jump*/jumpType == JumpType.Space && _onSpin
        };
        animatorHandler.SetAnimations(values);

        anim.SetFloat("VerticalVelocity", rb.velocity.y);
        //anim.SetFloat(animatorHash[16], 1 / slow2Forces);
        anim.SetInteger("upDown", aimUpDown);
        anim.SetInteger("leftRight", (int)xInput);
    }
    #region Delayed Methods
    void shootingClocking() => ShootOnWalk = false;
    void HyperJumpTimeAction()
    {
        runningState = RunningState.None;
        shinespark.ShinesparkState = ShinesparkState.None;
        speedBoosterComp.CancelGhost();
        if (shinespark.ShinesparkState != ShinesparkState.Full) speedBoosterComp.SetSpeedBooster(false);
    }
    #endregion
    #region Public methods
    public void OnShinesparkFull()
    {
        rb.gravityScale = 0;
        runningState = RunningState.None;
        status = Status.Powered;
        anim.Rebind();
        airShoot = onJumpingState = this.enabled = false;
    }
    public void OnShinesparkNone()
    {
        if (status != Status.Damaged)
            status = Status.Normal;

        rb.gravityScale = isGrounded ? 8 : 1;
        this.enabled = groundChecker.checkFloor = true;
    }
    public void ShootOnAir()
    {
        airShoot = true;
        OnSpin = onJumpingState = false;
    }
    #region Rigidbody
    public void SetStatus(Status newStatus) => status = newStatus;
    public void SetConstraints(RigidbodyConstraints2D constraints) => rb.constraints = constraints;
    public void SetVelocity(Vector2 velocity) => rb.velocity = velocity;
    #endregion
    #region Animator
    #endregion
    public void SetSpeedToDefault() => maxSpeed = runningSpeed;
    public void SetSpeedToBooster() => maxSpeed = speedBooster;
    //Called in PlayerKnockBack UnityEvent
    public void SetDamageState()
    {
        ResetState();
        status = Status.Damaged;
        if (GroundState != GroundState.Balled) { anim.SetTrigger("Hitted"); }
    }
    public void ResetState()
    {
        CancelInvoke();
        groundChecker.ResetState();
        fall = IsJumping = onJumpingState = isGrounded = OnSpin = ShootOnWalk = airShoot = false;
        xInput = yInput = 0;
        angleAim = 0;
        aimUpDown = 0;
        HyperJumpTimeAction();
        gun.fireType = FireType.Normal;
        if (GroundState != GroundState.Balled) GroundState = GroundState.Stand;
        runningState = RunningState.None;
        shinespark.ShinesparkState = ShinesparkState.None;
        anim.Rebind();
        AnimStates();
        anim.SetFloat("AnimSpeed", 1);
    }
    public void SetTransformCenter(Vector3 vector) => transform.position = vector;
    public void RestoreValuesAfterHit()
    {
        groundChecker.checkFloor = true;
        status = Status.Normal;
        if (groundOverHead) GroundState = GroundState.Crouched;
        inputManager.EnablePlayerInput();
    }
    public void Freeze(bool freeze)
    {
        if (freeze)
        {
            xInput = yInput = 0;
            inputManager.DisablePlayerInput();
            SetConstraints(RigidbodyConstraints2D.FreezeAll);
            anim.enabled = this.enabled = false;
        }
        else
        {
            anim.enabled = this.enabled = true;
            inputManager.EnablePlayerInput();
            SetConstraints(RigidbodyConstraints2D.FreezeRotation);
        }
    }

    #region Aim methods
    private void AngleAimUpCanceled(InputAction.CallbackContext context)
    {
        switch (aimState)
        {
            case AngleAim.Both:
                aimState = AngleAim.Down;
                angleAim = -1;
                break;
            case AngleAim.Up:
                aimState = AngleAim.None;
                angleAim = 0;
                break;
        }
        aimUpDown = 0;
    }
    private void AngleAimUp(InputAction.CallbackContext context)
    {
        if (aimState != AngleAim.Down) AimUp();
        else
        {
            aimState = AngleAim.Both;
            angleAim = 0;
            aimUpDown = 1;
            CheckAimUp();
        }
        ShootOnWalk = false;
    }
    private void AngleAimDownCanceled(InputAction.CallbackContext context)
    {
        switch (aimState)
        {
            case AngleAim.Both:
                aimState = AngleAim.Up;
                angleAim = 1;
                break;
            case AngleAim.Down:
                aimState = AngleAim.None;
                angleAim = 0;
                break;
        }
        aimUpDown = 0;
    }
    public void AngleAimDown(InputAction.CallbackContext context)
    {
        if (aimState != AngleAim.Up) AimDown();
        else
        {
            aimState = AngleAim.Both;
            angleAim = 0;
            aimUpDown = 1;
            CheckAimUp();
        }
        ShootOnWalk = false;
    }
    private void CheckAimUp()
    {
        if (isGrounded)
        {
            switch (_groundState)
            {
                case GroundState.Stand:
                    if (xInput == 0 && angleAim == 0) aimUpDown = 1;
                    break;
                case GroundState.Crouched:
                    if (!groundOverHead && aimState != AngleAim.Both) GroundState = GroundState.Stand;
                    break;
                case GroundState.Balled:
                    if (!groundOverHead) GroundState = GroundState.Crouched;
                    break;
            }
        }
        else
        {
            switch (_groundState)
            {
                case GroundState.Stand:
                    if (angleAim == 0) aimUpDown = 1;
                    break;
                case GroundState.Balled:
                    if (!groundOverHead)
                    {
                        GroundState = GroundState.Stand;
                        aimUpDown = 1;
                        OnSpin = false;
                    }
                    break;
            }
        }
    }
    private void AimUp()
    {
        angleAim = 1;
        aimState = AngleAim.Up;
        aimUpDown = 0;
        if (shinespark.ShinesparkState == ShinesparkState.Prepared)
        {
            shinespark.SetJumpAngleUp(leftLook);
            anim.SetTrigger("HyperJump LR");
        }
    }
    private void AimDown()
    {
        aimState = AngleAim.Down;
        angleAim = -1;
        aimUpDown = 0;
    }
    public void OnLeft(bool value)
    {
        leftLook = value;
        if (value)
        {
            transform.eulerAngles = new Vector2(0, 180);
            direction = Vector2.left;
        }
        else
        {
            transform.eulerAngles = new Vector2(0, 0);
            direction = Vector2.right;
        }
    }
    #endregion

    private void MoveHorStartedAction()
    {
        aimUpDown = 0;

        if (xInput < 0 && !leftLook) OnLeft(true);
        else if (xInput > 0 && leftLook) OnLeft(false);
        if (shinespark.ShinesparkState != ShinesparkState.Prepared)
        {
            if (_groundState == GroundState.Crouched && !groundOverHead) GroundState = GroundState.Stand;
        }
        else
        {
            shinespark.SetJumpToDirection(direction);
            anim.SetTrigger("HyperJump LR");
            CancelInvoke("HyperJumpTimeAction");
        }
    }
    private void MoveHorCanceledAction()
    {
        if (angleAim > 0) AimUp();
        else if (angleAim < 0) AimDown();
        if (isGrounded && speedBoosterComp.isInvoking) speedBoosterComp.CancelGhost();
        ShootOnWalk = false;
    }
    private void MoveVerStartedAction()
    {
        if (shinespark.ShinesparkState != ShinesparkState.Prepared)
        {
            if (yInput > 0f) CheckAimUp();
            else if (yInput < 0f)
            {
                if (runningState == RunningState.MaxSpeed && isGrounded && !IsInvoking("HyperJumpTimeAction"))
                {
                    shinespark.ShinesparkState = ShinesparkState.Charged;
                    speedBoosterComp?.SetSpeedBooster(true);
                    Invoke("HyperJumpTimeAction", 2f);
                }
                if (_groundState != GroundState.Balled && xInput == 0f && aimState == AngleAim.None && !isGrounded) { aimUpDown = -1; angleAim = 0; }
                else if (canMorph && _groundState == GroundState.Crouched) GroundState = GroundState.Balled;
                else if (_groundState == GroundState.Stand && isGrounded && xInput == 0) GroundState = GroundState.Crouched;
            }
        }
        else
        {
            if (yInput > 0)
            {
                CancelInvoke("HyperJumpTimeAction");
                shinespark.SetJumpUp();
                anim.SetTrigger("HyperJump up");
                onJumpingState = false;
            }
        }
    }
    private void MoveVerCanceledAction()
    {
        if (aimState == AngleAim.None && aimUpDown > 0) aimUpDown = 0;
    }
    private void MoveHorStarted(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<float>() > 0 ? 1 : -1;
        if (Time.deltaTime > 0) MoveHorStartedAction();
        else inputManager.AddToUnscaledActions(2, MoveHorStartedAction);
    }
    private void MoveHorCanceled(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<float>();
        if (Time.deltaTime > 0) MoveHorCanceledAction();
        else inputManager.AddToUnscaledActions(0, MoveHorCanceledAction);
    }
    private void MoveVerStarted(InputAction.CallbackContext context)
    {
        yInput = context.ReadValue<float>();
        if (Time.deltaTime > 0) MoveVerStartedAction();
        else inputManager.AddToUnscaledActions(3, MoveVerStartedAction);
    }
    private void MoveVerCanceled(InputAction.CallbackContext context)
    {
        yInput = context.ReadValue<float>();

        if (Time.deltaTime > 0) MoveVerCanceledAction();
        else inputManager.AddToUnscaledActions(1, MoveVerCanceledAction);
    }
    private void CheckGroundUp()
    {
        groundOverHead = (Physics2D.Raycast(capsule.bounds.max, Vector2.up, overHeadCheck, groundLayer)
                  && _groundState != GroundState.Stand
        );
    }
    private void QuickMorphball(InputAction.CallbackContext context)
    {
        if (canMorph)
        {
            ShootOnWalk = false;
            if (_groundState != GroundState.Balled) GroundState = GroundState.Balled;
        }
    }
    private void Run(InputAction.CallbackContext context)
    {
        if (isGrounded && _groundState != GroundState.Balled && xInput != 0)
            runningState = RunningState.Running;
    }
    private void RunCanceled(InputAction.CallbackContext context)
    {
        if (isGrounded && _groundState != GroundState.Balled) { currentSpeed = speed; runningState = RunningState.None; }
        if (isGrounded && speedBoosterComp.isInvoking) speedBoosterComp.CancelGhost();
    }

    #region Player jumping Methods
    public void SetGravityJump() => OnJump = OnGravityJump;
    public void SetNormalJump() => OnJump = OnNormalJump;
    private void OnGravityJump()
    {
        JumpManager(() =>
        {
            if (isGrounded) IsJumping = onJumpingState = true;
            else if (_groundState == GroundState.Stand) { airShoot = false; OnSpin = true; }
        },
        () =>
        {
            if (_groundState == GroundState.Stand) OnSpin = IsJumping = true;
            else if (_groundState == GroundState.Balled && isGrounded) morphSpin = IsJumping = true;
        });
        jumpTimeCounter = jumpTime;
    }
    private void OnNormalJump()
    {
        if (isGrounded && !isJumping)
        {
            jumpTimeCounter = jumpTime;
            JumpManager(() => IsJumping = onJumpingState = true,
            () =>
            {
                if (_groundState == GroundState.Balled) morphSpin = IsJumping = true;
                else IsJumping = OnSpin = true;
            });
        }
    }
    private void JumpManager(Action idleJump, Action moveJump)
    {
        if (_groundState == GroundState.Balled && isGrounded) playerFX.BallJump();
        if (xInput == 0)
        {
            if (shinespark.ShinesparkState == ShinesparkState.Charged && _groundState == GroundState.Stand)
            {
                IsJumping = onJumpingState = true; shinespark.ShinesparkState = ShinesparkState.Prepared;
            }
            else idleJump.Invoke();
        }
        else moveJump.Invoke();
    }
    private void JumpCanceled(InputAction.CallbackContext context) => IsJumping = false;
    public void Jump(InputAction.CallbackContext context)
    {
        if (_groundState != GroundState.Crouched)
        {
            OnJump?.Invoke();
            if (_onSpin && CheckWallJump())
                IsJumping = OnSpin = true;
        }
    }
    #endregion
    #endregion
}
public enum AngleAim
{
    Up, Down, Both, None
}
public enum GroundState
{
    Crouched, Balled, Stand
}

public enum JumpType
{
    Space = 0, Screw = 1, Default = 4
}
public enum Status
{
    Normal = 0, Powered = 1, Damaged = 2
}