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
    [Header("Floor config")]
    [SerializeField] GroundChecker groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float spinOffset;
    [Header("Running and Speed Booster config")]
    [SerializeField] SpeedBooster speedBoosterComp;
    [SerializeField] Shinespark shinespark;
    [Tooltip("Standard running without any boost.")]
    [SerializeField, Range(0, 14)] float runningSpeed = 100;
    [Tooltip("Speed Booster max speed")]
    [SerializeField, Range(100, 130)] float speedBooster = 14;
    [SerializeField, Range(0, 200)] float maxSpeed;
    [SerializeField, Range(0.14f, 3)] float speedIncreaseOverTime = 1.5f;
    [Header("Misc events")]
    [SerializeField] UnityEvent<bool> onScrew;
    [SerializeField] UnityEvent onMorphball, onEnable;
    private float jumpForce = 88, speed = 88, curSpinOffset = 1, slow2Gravity = 1;
    private float yInput = 0, xVelocity, jumpTimeCounter, currentSpeed, angleAim, spriteCenter;
    public float xInput { get; set; } = 0;
    public Animator anim { get; set; }
    private PlayerFXHandler playerFX;
    private SpriteRenderer spriteRenderer;
    private Gun gun;
    private bool fall, isJumping, airShoot, morphSpin, onJumpingState, _onSpin, isGrounded;
    private System.Action OnJump = delegate { };
    private GroundState _groundState = GroundState.Stand;
    public JumpType jumpType { get; set; } = JumpType.Default;
    private RunningState runningState = RunningState.None;
    public AngleAim aimState { get; private set; } = AngleAim.None;
    public Status status { get; private set; } = Status.Normal;
    private int aimUpDown = 0;
    int[] animatorHash = new int[27];
    public float currentJumpForce { get; set; }
    public float slow{get;set;}=1;
    public float morphballSlow{get;set;}=1;
    public bool groundOverHead { get; private set; }
    public bool canMorph { get; set; } = true;
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
            anim.SetBool(animatorHash[9], _onSpin && jumpType == JumpType.Default);//spin jump
            anim.SetBool(animatorHash[12], jumpType == JumpType.Screw && _onSpin && _groundState == GroundState.Stand);//screw
            anim.SetBool(animatorHash[14], jumpType == JumpType.Space && _onSpin);//gravity jump
        }
    }
    public GroundChecker GroundChecker { get => groundChecker; }
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
            else gun.OnStand?.Invoke();
        }
    }
    public Rigidbody2D rb { get; private set; }
    #endregion
    #region Unity methods
    private void OnEnable()
    {
        onEnable.Invoke();
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
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerFX = GetComponentInChildren<PlayerFXHandler>();
        gun = GetComponentInChildren<Gun>();
        anim = GetComponentInChildren<Animator>();
        for (int i = 0; i < anim.parameterCount; i++) animatorHash[i] = Animator.StringToHash(anim.parameters[i].name);
        currentSpeed = speed;
    }

    void Update()
    {
        camHandle.position = TransformCenter();
        if (status != Status.Damaged)
        {
            spriteCenter = spriteRenderer.bounds.size.y / 2;
            hurtBox.transform.position = capsule.bounds.center;
            hurtBox.size = new Vector2(capsule.bounds.size.x + .01f, capsule.bounds.size.y + .01f);

            isGrounded = groundChecker.SetFloorChecking();

            if (isGrounded) OnGround();
            else OnAir();

        }
    }
    public void SetAnimation(int index, bool enable) => anim.SetBool(animatorHash[index], enable);

    void FixedUpdate()
    {
        if (status != Status.Damaged)
        {
            if (groundOverHead && _groundState != GroundState.Balled)
                xInput = 0;

            xVelocity = xInput * (currentSpeed * slow) * Time.deltaTime;
            if (!isGrounded)
            {
                if (isJumping && jumpTimeCounter > 0f) rb.velocity = Vector2.up * (currentJumpForce * slow) * Time.deltaTime;

                if (onJumpingState) SetVelocity(new Vector2(xVelocity / 2, rb.velocity.y));
                else if (_onSpin || morphSpin) SetVelocity(new Vector2(xVelocity, rb.velocity.y));
            }

            groundChecker.FixedUpdateOnGround(xInput, xVelocity);
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
    #region On ground methods
    void OnGround()
    {
        xInput=groundChecker.WallAndSlopeCheck(xInput);
        groundOverHead = groundChecker.GroundOverHead;
        if (xInput != 0f)
        {
            if (runningState == RunningState.Running && slow == 1)
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

    internal void FirstOnGround(float y)
    {
        aimUpDown = 0;
        if (_onSpin && !morphSpin)
        {
            StartCoroutine(FixPivot(y));
            if (groundOverHead) GroundState = GroundState.Crouched;
        }
        morphSpin = IsJumping = airShoot = OnSpin = onJumpingState = fall = false;
        playerFX.StopAudio(StopAction.All);
    }
    /// <summary>
    /// Fix the transform.position pivot center when player arrives to the ground at spin jump.
    /// </summary>
    /// <param name="y">point.y of the raycast collision with the ground</param>
    /// <returns></returns>
    IEnumerator FixPivot(float y)
    {
        yield return new WaitWhile(() =>
            anim.GetBool(animatorHash[9]) ||
            anim.GetBool(animatorHash[12]) ||
            anim.GetBool(animatorHash[14])
        );
        transform.position = new Vector3(transform.position.x, y, 0);
    }

    #endregion
    #region On air methods 
    void OnAir()
    {
        groundChecker.FirstAir(_onSpin, slow2Gravity, spriteCenter);
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
    #endregion
    private void AnimStates()
    {
        anim.SetBool(animatorHash[0], angleAim < 0);
        anim.SetBool(animatorHash[1], angleAim > 0);
        anim.SetBool(animatorHash[2], _groundState == GroundState.Balled);
        anim.SetBool(animatorHash[3], isGrounded && xInput != 0 && !groundChecker.wallInFront && !groundOverHead); //walk
        anim.SetBool(animatorHash[4], (groundChecker.ShootOnWalk || gun.fireType != FireType.Normal) && isGrounded && xInput != 0);
        anim.SetBool(animatorHash[5], _groundState == GroundState.Crouched);
        anim.SetBool(animatorHash[6], leftLook);
        anim.SetBool(animatorHash[7],/*idle*/isGrounded && xInput == 0);
        anim.SetBool(animatorHash[8], isGrounded);
        anim.SetBool(animatorHash[9],/*spin jump*/_onSpin && jumpType == JumpType.Default);
        anim.SetBool(animatorHash[10],/*airshoot*/(gun.fireType != FireType.Normal || airShoot) && !isGrounded && !_onSpin);
        anim.SetBool(animatorHash[11],/*screw*/jumpType == JumpType.Screw && _onSpin && _groundState == GroundState.Stand);
        anim.SetBool(animatorHash[12],/*jump state*/onJumpingState);
        anim.SetBool(animatorHash[13], fall);
        anim.SetBool(animatorHash[14],/*gravity jump*/jumpType == JumpType.Space && _onSpin);

        anim.SetFloat("VerticalVelocity", rb.velocity.y);
        anim.SetFloat("AnimSpeed", slow>morphballSlow ? morphballSlow : slow);
        anim.SetInteger("upDown", aimUpDown);
        anim.SetInteger("leftRight", (int)xInput );
    }
    #region Delayed Methods
    void HyperJumpTimeAction()
    {
        runningState = RunningState.None;
        shinespark.ShinesparkState = ShinesparkState.None;
        speedBoosterComp.CancelGhost();
        if (shinespark.ShinesparkState != ShinesparkState.Full) speedBoosterComp.SetSpeedBooster(false);
    }
    #endregion
    #region Public methods
    public void OnSaveStation(Vector2 position)
    {
        ResetState();
        SetAnimation(15, true);
        inputManager.DisableAll();
        SetTransformCenter(position);
        SetConstraints(RigidbodyConstraints2D.FreezeAll);
        enabled = false;
    }
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
        inputManager.DisablePlayerInput();
        fall = IsJumping = onJumpingState = isGrounded = OnSpin = airShoot = false;
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
        if (!inputManager.lockFireInput) inputManager.EnablePlayerInput();
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
        groundChecker.ShootOnWalk = false;
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
        groundChecker.ShootOnWalk = false;
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
        transform.eulerAngles = value ? new Vector2(0, 180) : new Vector2(0, 0);
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
            shinespark.SetJumpToDirection(new Vector2(xInput, 0));
            anim.SetTrigger("HyperJump LR");
            CancelInvoke("HyperJumpTimeAction");
        }
    }
    private void MoveHorCanceledAction()
    {
        if (angleAim > 0) AimUp();
        else if (angleAim < 0) AimDown();
        if (isGrounded && speedBoosterComp.isInvoking) speedBoosterComp.CancelGhost();
        groundChecker.ShootOnWalk = false;
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
    private void QuickMorphball(InputAction.CallbackContext context)
    {
        if (canMorph)
        {
            groundChecker.ShootOnWalk = false;
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
            StartCoroutine(nameof(WaitToWallJump));
        }
    }
    #endregion
    /*
        Called at the end of the current frame to solve a issue with the pivot.
    */
    IEnumerator WaitToWallJump()
    {
        yield return new WaitForEndOfFrame();
        if (_onSpin && groundChecker.CheckWallJump(xInput))
        {
            IsJumping = OnSpin = true;
            jumpTimeCounter = jumpTime/2;
        }
    }
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