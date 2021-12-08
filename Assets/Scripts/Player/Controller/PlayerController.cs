using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Metroid;
using Player.PowerUps;
using System;
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
    [SerializeField] BoxCollider2D edgeCollider,hurtBox;
    [SerializeField, Range(.01f, 1f)] float jumpTime = 0.35f;
    [SerializeField] Transform camHandle;
    [Header("Floor config")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float overHeadCheck=.01f;
    [SerializeField] float wallDistance, wallEdgeOffset, edgesOffset, spinOffset;
    [SerializeField, Range(.001f, .88f)] float groundDistance = 0.18f,airGroundDistance = 0.18f;
    [SerializeField, Range(.001f, .22f)] float slopeFrontRay = 0.08f,slopeBackRay = 0.08f,groundHitSlope;
    [SerializeField, Range(-1, 1.5f)] float slopeEdgesOffset;
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
    private Vector2 posFrontRay, posBackRay, slopePerp,direction;
    private RaycastHit2D frontHit, backHit;
    private float jumpForce = 88, speed = 88, frontAngle, backAngle, curSpinOffset = 1,slow2Gravity = 1,curGroundDis;
    private float yInput = 0, xVelocity, jumpTimeCounter, currentSpeed, slopeAngle, aim,spriteCenter;
    public float xInput{get;private set;}=0;
    private Animator anim;
    private PlayerFXHandler playerFX;
    private SpriteRenderer spriteRenderer;
    private Gun gun;
    private bool fall, isJumping, firstLand, firstAir, airShoot,morphSpin,rbStatic,
       onJumpingState, _onSpin, onSlope, checkFloor = true, _shootOnWalk,wallInFront;
    private System.Action OnJump=delegate{};
    private GroundState _groundState = GroundState.Stand;
    public JumpType jumpType { get; set; } = JumpType.Default;
    private RunningState runningState = RunningState.None;
    public AngleAim aimState{get;private set;} = AngleAim.None;
    public Status status { get; private set; } = Status.Normal;
    private int aimUpDown = 0;
    int[] animatorHash = new int[27];
    public float currentJumpForce { get; set; }
    public bool groundOverHead{get;private set;}
    public bool canMorph{get;set;}=true;
    public float slow2Forces = 1;
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
            //fix a issue with the sprite pivot. 
            anim.SetBool(animatorHash[9], _onSpin && jumpType == JumpType.Default);//spin jump
            anim.SetBool(animatorHash[12], jumpType == JumpType.Screw && _onSpin && _groundState == GroundState.Stand);//screw
            anim.SetBool(animatorHash[15], jumpType == JumpType.Space && _onSpin);//gravity jump
        }
    }
    public bool leftLook { get; set; }
    public bool IsJumping
    {
        get => isJumping;
        set
        {
            isJumping = value;
            if (isJumping)
            {
                checkFloor = isGrounded = false;
                CancelAndInvoke("CheckFloor", .3f);
            }
        }
    }
    public bool ShootOnWalk
    {
        get => _shootOnWalk;
        set
        {
            _shootOnWalk = value;
            if (_shootOnWalk) CancelAndInvoke("shootingClocking", 2f);
            else CancelInvoke("shootingClocking");
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
                aim = 0;
                aimUpDown = 0;
            }
            else
            {
                anim.SetFloat(animatorHash[16], 1);
                gun.OnStand?.Invoke();
            }
        }
    }
    private Rigidbody2D rb;
    public bool isGrounded { get; set; }
    #endregion
    #region Unity methods
    private void OnEnable(){
        onEnable.Invoke();
        if(inputManager.lockFireInput)inputManager.DisableFireInput();
        if(inputManager.HorizontalMovement==null){
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
    private void OnDisable() {
        inputManager.DisablePlayerControls();
    }
    private void OnDestroy() {
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
        current=null;
    }
    void Awake()
    {
        current=this;
        slow2Gravity =1;
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
        for (int i = 0; i < anim.parameterCount - 2; i++) animatorHash[i] = Animator.StringToHash(anim.parameters[i].name);
        currentSpeed = speed;
        curGroundDis =groundDistance;
    }
     
    void Update()
    {
        if (status != Status.Damaged)
        {
            spriteCenter = spriteRenderer.bounds.size.y / 2;
            if (checkFloor) CheckGround();
            camHandle.position = TransformCenter();
            hurtBox.transform.position = capsule.bounds.center;
            hurtBox.size=new Vector2(capsule.bounds.size.x+ .01f,capsule.bounds.size.y+.01f);

            if (isGrounded) OnGround();
            else OnAir();
            if (isJumping)
            {
                if (jumpTimeCounter > 0) jumpTimeCounter -= Time.deltaTime;
                else IsJumping = false;
            }
        }
    }
    void FixedUpdate()
    {
        if (status != Status.Damaged)
        {
            if(groundOverHead && _groundState!=GroundState.Balled) 
                xInput=0;

            xVelocity = xInput * (currentSpeed / slow2Forces) * Time.deltaTime;
            if (!isGrounded)
            {
                if (isJumping && jumpTimeCounter > 0f) rb.velocity = Vector2.up * (currentJumpForce / slow2Forces) * Time.deltaTime;

                if (onJumpingState) rb.velocity = new Vector2(xVelocity/2 , rb.velocity.y);
                else if (_onSpin || morphSpin) rb.velocity = new Vector2(xVelocity, rb.velocity.y);
            }
            else if (isGrounded && xInput != 0 && !wallInFront)
            {
                rb.velocity = (!onSlope) ? new Vector2(xVelocity, 0f) : new Vector2(-xVelocity* slopePerp.x, -xVelocity * slopePerp.y);
                if ((frontAngle == 0 && backAngle != 0 && slopeAngle==0) && ((frontHit.point.y > backHit.point.y) || (frontHit.point.y < backHit.point.y)))
                {
                    transform.position.Set(frontHit.point.x,frontHit.point.y,0);
                    rb.velocity.Set(rb.velocity.x, 0f);
                }
            }
        }
    }
    void LateUpdate()
    {
        if (Time.timeScale > 0 && shinespark.ShinesparkState!=ShinesparkState.Full)AnimStates();
    }
    #endregion
    public void SetAllInput(bool active)=>inputManager.SetAllInput(active);
    public Vector3 TransformCenter(){
        return _onSpin ? transform.position: new Vector3(transform.position.x, transform.position.y + spriteCenter);
    }
    private void CancelAndInvoke(string method, float time)
    {
        CancelInvoke(method);
        Invoke(method, time);
    }

    #region On ground methods
    void OnGround()
    {
        CheckSlopesAndEdges();
        CheckWallInFront();
        CheckGroundUp();
        if (xInput != 0f)
        {
            if (_groundState == GroundState.Balled)
            {
                anim.SetFloat(animatorHash[16], 1);
            }
            else
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
            else
            if (_groundState == GroundState.Balled) anim.SetFloat(animatorHash[16], 0);
            rb.velocity = Vector2.zero;
            currentSpeed = speed;
        }
    }
    void CheckWallInFront(){
        RaycastHit2D wallHit=Physics2D.BoxCast(new Vector2(capsule.bounds.center.x+(capsule.size.x/2) * direction.x,
        capsule.bounds.center.y),
        new Vector2(wallDistance,capsule.bounds.size.y-wallEdgeOffset),0f,direction,wallDistance,groundLayer);
        if(wallHit && Vector2.Angle(wallHit.normal,Vector2.up)==90){
            wallInFront= true;
            xInput=0;
        }
        else wallInFront = false;
    }
    void CheckGround()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(new Vector2(capsule.bounds.center.x,capsule.bounds.min.y), 
        new Vector2(capsule.bounds.size.x / edgesOffset, curGroundDis), 0f, 
        Vector2.down, curGroundDis*curSpinOffset, groundLayer);
        if (raycastHit2D)
        {
            if (!firstLand) FirstOnGround(raycastHit2D.point.y);
            isGrounded = true;
            if(onSlope && xInput==0 && !rbStatic){
                transform.position=new Vector3(transform.position.x,raycastHit2D.point.y,0);
                rb.gravityScale=0;
                rbStatic=true;
            }else if(!onSlope || xInput!=0) {
                rbStatic=false;
                rb.gravityScale=1/slow2Gravity;
            }
        }
        else isGrounded = false;
    }
    private void FirstOnGround(float y)
    {
        aimUpDown = 0;
        curGroundDis = groundDistance;
        if(_onSpin && !morphSpin){
            StartCoroutine(FixPivot(y));
            if(groundOverHead)GroundState=GroundState.Crouched;
        }
        morphSpin=IsJumping = airShoot = OnSpin = firstAir = onJumpingState = fall = false;
        playerFX.StopAudio(StopAction.All);
        firstLand = true;
    }
    /// <summary>
    /// Fix the transform.position pivot center when player arrives to the ground at spin jump.
    /// </summary>
    /// <param name="y">point.y of the raycast collision with the ground</param>
    /// <returns></returns>
    IEnumerator FixPivot(float y){
        yield return new WaitWhile(()=>
            anim.GetBool(animatorHash[9])==true ||
            anim.GetBool(animatorHash[12])==true ||
            anim.GetBool(animatorHash[15]) == true
        );
        transform.position=new Vector3(transform.position.x,y,0);
    }
    private void CheckSlopesAndEdges()
    {
        Vector2 v=new Vector2(capsule.bounds.min.x + capsule.size.x/2, capsule.bounds.min.y);
        posFrontRay = new Vector2(transform.position.x + capsule.size.x/2*(direction.x - slopeEdgesOffset), capsule.bounds.min.y+ .02f);
        RaycastHit2D hit2D = Physics2D.Raycast(posFrontRay, Vector2.down, groundHitSlope, groundLayer);
        RaycastHit2D hit = Physics2D.Raycast(v, Vector2.down, groundHitSlope, groundLayer);

        if (hit2D && hit)
        {
            frontAngle = Vector2.Angle(hit2D.normal, Vector2.up);
            slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            slopePerp = Vector2.Perpendicular(hit2D.normal).normalized;
            if ((slopePerp.y < 0 && xInput < 0) || (slopePerp.y > 0 && xInput > 0)) frontAngle *= -1;

            if ((frontAngle != 0) || (frontAngle == 0 && slopeAngle != 0) || slopeAngle != 0)
                onSlope = true;
            else if(frontAngle==0 && slopeAngle==0 && backAngle==0){
                onSlope = false;
            }
            
        }

        posBackRay = new Vector2(transform.position.x - capsule.size.x/2 * (direction.x - slopeEdgesOffset), capsule.bounds.min.y+.02f);

        frontHit = Physics2D.Raycast(posFrontRay, Vector2.down, slopeFrontRay, groundLayer);
        backHit = Physics2D.Raycast(posBackRay, Vector2.down, slopeBackRay, groundLayer);
        if (frontHit && backHit)
        {
            frontAngle = Vector2.Angle(frontHit.normal, Vector2.up);
            backAngle = Vector2.Angle(backHit.normal, Vector2.up);
        }

        if ((!frontHit || !backHit) && !hit && !onSlope)
        { // enable the edge collider when player is on ground edge
            edgeCollider.enabled = true;
            edgeCollider.transform.localPosition = new Vector3(0, 0.075f / 2);
        }
        else edgeCollider.enabled = false;
    }
    #endregion
    #region On air methods 
    void OnAir()
    {
        FirstAir();
        if (_groundState == GroundState.Balled) anim.SetFloat(animatorHash[16], 1);
        else
        if (aimUpDown == 0 && !isJumping && status == Status.Normal && shinespark.ShinesparkState != ShinesparkState.Full 
        && gun.fireType == FireType.Normal && aim == 0 && !airShoot && !onJumpingState && !_onSpin)
        {
            fall = true;
        }
        else fall = false;
        if (isJumping)
        {
            if (Physics2D.Raycast(transform.position + new Vector3(0, spriteCenter), Vector2.up, spriteCenter, groundLayer))
            {
                IsJumping = false;
                jumpTimeCounter = 0;
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
            curGroundDis = airGroundDistance;
            if(_onSpin)transform.position=new Vector3(transform.position.x,transform.position.y+spriteCenter,0);
            edgeCollider.enabled = false; rb.gravityScale = 1/slow2Gravity; firstAir = true;
            ShootOnWalk = firstLand = onSlope = false;
        }
    }
    #endregion
    private void AnimStates()
    {
        SetAnimation(0,aim<0);
        SetAnimation(1, aim > 0);
        SetAnimation(2, _groundState == GroundState.Balled);
        SetAnimation(3, isGrounded && xInput != 0 && !wallInFront  && !groundOverHead);
        SetAnimation(4, (_shootOnWalk || gun.fireType!=FireType.Normal) && isGrounded && xInput != 0);
        SetAnimation(5, _groundState == GroundState.Crouched);
        SetAnimation(6, leftLook);
        SetAnimation(7, isGrounded && xInput == 0);//idle
        SetAnimation(8, isGrounded);
        SetAnimation(9, _onSpin && jumpType == JumpType.Default);//spin jump
        SetAnimation(11, (gun.fireType != FireType.Normal || airShoot) && !isGrounded && !_onSpin);//airshoot
        SetAnimation(12, jumpType == JumpType.Screw && _onSpin && _groundState == GroundState.Stand);//screw
        SetAnimation(13,onJumpingState);//jump state
        SetAnimation(14, fall);
        SetAnimation(15, jumpType == JumpType.Space && _onSpin);//gravity jump
        anim.SetFloat(animatorHash[10], rb.velocity.y);

        anim.SetFloat(animatorHash[16], 1 / slow2Forces);
        anim.SetInteger(animatorHash[18], aimUpDown);
        anim.SetInteger(animatorHash[19], (int)xInput);
    }
    #region Delayed Methods
    void CheckFloor() => checkFloor = true;
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
        this.enabled = checkFloor = true;
    }
    public void ShootOnAir(){
        airShoot = true;
        OnSpin = onJumpingState = false;
    }
    #region Rigidbody
    public void SetGravitySlow(float amount){
        slow2Gravity=amount;
        rb.gravityScale/=slow2Gravity;
    }
    public void ResetGravity(){
        if(onSlope)rb.gravityScale=0;
        else rb.gravityScale=1;
    }
    public void SetStatus(Status newStatus)=>status = newStatus;
    public void SetConstraints(RigidbodyConstraints2D constraints)=>rb.constraints=constraints;
    public void SetVelocity(Vector2 velocity) => rb.velocity = velocity;
    #endregion
    #region Animator
    public void SetAnimation(int index,bool enable) => anim.SetBool(animatorHash[index],enable);
    #endregion
    public void SetSpeedToDefault() => maxSpeed = runningSpeed;
    public void SetSpeedToBooster() => maxSpeed = speedBooster;
    //Called in PlayerKnockBack UnityEvent
    public void SetDamageState(){
        ResetState();
        status=Status.Damaged;
        if (GroundState != GroundState.Balled) { anim.SetTrigger("Hitted"); }
    }
    public void ResetState()
    {
        CancelInvoke();
        fall = IsJumping = onJumpingState =checkFloor=
        isGrounded = OnSpin = ShootOnWalk = airShoot =false;
        xInput = yInput = 0;
        aim = 0;
        aimUpDown = 0;
        HyperJumpTimeAction();
        gun.fireType = FireType.Normal;
        if(GroundState!=GroundState.Balled)GroundState = GroundState.Stand;
        runningState = RunningState.None;
        shinespark.ShinesparkState = ShinesparkState.None;
        anim.Rebind();
        AnimStates();
        anim.SetFloat(animatorHash[16], 1);
    }
    public void SetTransformCenter(Vector3 vector) => transform.position=vector;
    public void RestoreValuesAfterHit()
    {
        checkFloor = true;
        status = Status.Normal;
        if(groundOverHead) GroundState=GroundState.Crouched;
        inputManager.EnablePlayerInput();
    }
    public void Freeze(bool freeze){
        if(freeze){
            xInput = yInput = 0;
            inputManager.DisablePlayerInput();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            anim.enabled=this.enabled=false;
        }else{
            anim.enabled=this.enabled=true;
            inputManager.EnablePlayerInput();
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    #region Aim methods
    private void AngleAimUpCanceled(InputAction.CallbackContext context){
        switch (aimState)
        {
            case AngleAim.Both:
                aimState = AngleAim.Down;
                aim = -1;
                break;
            case AngleAim.Up:
                aimState = AngleAim.None;
                aim = 0;
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
            aim = 0;
            aimUpDown = 1;
            CheckAimUp();
        }
        ShootOnWalk = false;
    }
    private void AngleAimDownCanceled(InputAction.CallbackContext context){
        switch (aimState)
        {
            case AngleAim.Both:
                aimState = AngleAim.Up;
                aim = 1;
                break;
            case AngleAim.Down:
                aimState = AngleAim.None;
                aim = 0;
                break;
        }
        aimUpDown = 0;
    }
    public void AngleAimDown(InputAction.CallbackContext context)
    {
        if (aimState != AngleAim.Up)AimDown();
        else
        {
            aimState = AngleAim.Both;
            aim = 0;
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
                    if (xInput == 0 && aim == 0) aimUpDown = 1;
                    break;
                case GroundState.Crouched:
                    if(!groundOverHead)GroundState = GroundState.Stand;
                    break;
                case GroundState.Balled:
                    if(!groundOverHead)GroundState = GroundState.Crouched;
                    break;
            }
        }
        else
        {
            switch (_groundState)
            {
                case GroundState.Stand:
                    if (aim == 0) aimUpDown = 1;
                    break;
                case GroundState.Balled:
                    if(!groundOverHead){
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
        aim = 1;
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
        aim = -1;
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
     
    private void MoveHorStartedAction(){
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
    private void MoveHorCanceledAction(){
        if (aim > 0) AimUp();
        else if (aim < 0) AimDown();
        if (isGrounded && speedBoosterComp.isInvoking) speedBoosterComp.CancelGhost();
        ShootOnWalk = false;
    }
    private void MoveVerStartedAction(){
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
                if (_groundState != GroundState.Balled && xInput == 0f && aimState == AngleAim.None && !isGrounded) { aimUpDown = -1; aim = 0; }
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
    private void MoveVerCanceledAction(){
        if (aimState == AngleAim.None && aimUpDown > 0) aimUpDown = 0;
    }
    private void MoveHorStarted(InputAction.CallbackContext context){
        xInput =  context.ReadValue<float>() > 0 ? 1: -1;
        if(Time.deltaTime>0) MoveHorStartedAction();
        else inputManager.AddToUnscaledActions(2,MoveHorStartedAction);
    }
    private void MoveHorCanceled(InputAction.CallbackContext context){
        xInput = context.ReadValue<float>();
        if(Time.deltaTime>0) MoveHorCanceledAction();
        else inputManager.AddToUnscaledActions(0,MoveHorCanceledAction);
    }
    private void MoveVerStarted(InputAction.CallbackContext context){
        yInput = context.ReadValue<float>();
        if(Time.deltaTime>0)  MoveVerStartedAction();
        else inputManager.AddToUnscaledActions(3,MoveVerStartedAction);
    }
    private void MoveVerCanceled(InputAction.CallbackContext context){
        yInput = context.ReadValue<float>();

        if(Time.deltaTime>0) MoveVerCanceledAction();
        else inputManager.AddToUnscaledActions(1,MoveVerCanceledAction);
    }
    private void CheckGroundUp()
    {
        groundOverHead=(Physics2D.Raycast(capsule.bounds.max, Vector2.up, overHeadCheck, groundLayer)
                  && _groundState!=GroundState.Stand
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
    private void Run(InputAction.CallbackContext context){
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
            JumpManager(() =>
            {
                IsJumping = onJumpingState = true;
            },
            () =>
            {
                if (_groundState == GroundState.Balled) morphSpin=IsJumping = true;
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