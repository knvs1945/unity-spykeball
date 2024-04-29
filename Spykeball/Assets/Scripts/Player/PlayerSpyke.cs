using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Player class for the player unit the user directly controls
/// Inherits from Player Unit, but no need to assign its playerunit object
/// </summary>

public class PlayerSpyke : PlayerUnit
{

    protected const float dashEffectGap = 0.05f, ballImpactMin = 15f;
    
    // Delegates and Events
    public delegate void onHitBall(int score, int timeToAdd = 0);    
    public event onHitBall doOnHitBall;

    // public entries
    // public Projectiles attackEffect;
    public Transform frontSide, attackPoint, notePoint, bodyPoint;
    public Vector2 startPosition;
    
    // Arrays and Lists
    
    // will contain the stats for the character to be set on the UI
    public float base_ATKbase, base_ATKmax, base_ATKdelay, base_ATKRange, base_DashDelay, base_DashPower, base_JumpPower, base_PushPower, attackRadius; 
    public int scoreOnHitBall, scoreOnStrikeBall, scoreOnHitTarget;
    public bool impactReleased = false, sfxBallHit = false, kickSlashReleased = false;

    // objects 
    private Rigidbody2D rbBody;
    private Vector2 moveInput, moveData, currentGravity;
    protected Renderer render;
    protected SpriteRenderer spriteRnd;
    protected Animator animBody;

    // primitives    
    private int castCounter = 0, xDirection = 1, jumpXDirection = 0;
    private float jumpXPower = 0, width, height, atkTimer = 0f, dashTimer = 0f, dashEffectTimer = 0f;
    private bool canAttack = true, isJumping = false, isFalling = false, isGrounded = false, isDashing = false;
        
    // specifics
    [SerializeField]
    private AudioSource SFXCast;

    [SerializeField]
    protected GameObject body;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        rbBody = GetComponent<Rigidbody2D>();
        animBody = GetComponent<Animator>();
        render = body.GetComponent<Renderer>();
        spriteRnd = body.GetComponent<SpriteRenderer>();
        currentGravity = Physics2D.gravity;

        width = render.bounds.size.x;
        height = render.bounds.size.y;

    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == 0) return;
        
        inputMovement();    // prioritize inputMovement check so player can still unpause
        if (checkIfGamePaused()) return;

        movementChecks();
    }

    void FixedUpdate()
    {
        if (gameState == 0 || isControlDisabled) return;

        // move the rigidbody here. 
        if (isGrounded && !isJumping && !isFalling && !isDashing) rbBody.MovePosition(rbBody.position + new Vector2(moveData.x, 0) * Time.fixedDeltaTime);
        createDashEffects();
    }

    public override void restartUnit(string gameMode) {
        initializeStats();
    }

    public void animateIntroSequenceDone() {
        fireIntroSequenceEvent();
    }

    // ================ Stats and Status sequences start here  ================ //
    // use this function at the start of every game
    private void initializeStats() {
        animBody.SetTrigger("intro");   
        rbBody.velocity = new Vector2(0,0);
        atkTimer = 0f;
        dashTimer = 0f;
        transform.position = startPosition;
        isDashing = false;
    }

    // ================ Input action sequences start here  ================ //
    // Input for moving player
    private void inputMovement() {
        int moveDir = 0;
        if (isControlDisabled) {
            return;
        }

        if (Input.GetKeyDown(controls.Pause)) {
            if (!isGamePaused) playerPressedPause(true);
            else               playerPressedPause(false);
        }
        if (isGamePaused) return; // reject every other control unless the game is unpaused

        // add default keycode for up, down left and right arrows
        // add jump force when pressing up

        bool moveLeft = (Input.GetKey(controls.MoveLeft) || Input.GetKey(KeyCode.LeftArrow));
        bool moveRight = (Input.GetKey(controls.MoveRight) || Input.GetKey(KeyCode.RightArrow));
        if (!isJumping) {
            if (!isFalling && (Input.GetKeyDown(controls.MoveUp) || Input.GetKeyDown(KeyCode.UpArrow))) setAnimationJumping();    
            
            // move left OR right, or neither if both are pressed;
            if (moveLeft && moveRight){
                moveDir = 0;
            }
            else if (moveLeft) {
                flipSprite(-1);
                xDirection = -1;
                jumpXDirection = 1;
                moveDir = xDirection;
                setAnimationWalking(3, true);
            }
            else if (moveRight) {
                flipSprite(1);
                xDirection = 1;
                jumpXDirection = 1;
                moveDir = xDirection;
                setAnimationWalking(4, true);
            }
            else {
                moveDir = 0;
            }

            // keep walking animation while any of the directions are pressed
            if (Input.GetKey(controls.MoveLeft) || Input.GetKey(controls.MoveRight)) setAnimationWalking(0, true); 
            else {
                jumpXDirection = 0;
                setAnimationWalking(0, false);
            }

            moveInput = new Vector2(moveDir, 0);
            moveData = moveInput.normalized * moveSpeed;
            jumpXPower = moveSpeed * xDirection;
        }
        else {
            if (Input.GetKeyDown(controls.MoveLeft) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                flipSprite(-1);
                xDirection = -1;
                jumpXDirection = 1;
                applyXForce(moveSpeed);
            }
            else if (Input.GetKeyDown(controls.MoveRight) || Input.GetKeyDown(KeyCode.RightArrow)) {
                flipSprite(1);
                xDirection = 1;
                jumpXDirection = 1;
                applyXForce(moveSpeed);
            }
        }
        if (Input.GetKeyDown(controls.MoveDown) || Input.GetKeyDown(KeyCode.DownArrow)) animateDash();    

        // attack animation
        if (Input.GetKey(controls.Attack)) {
            if (canAttack) {
                canAttack = false;
                animateAttack();
                Invoke("updateAttackCD", ATKdelay);
            }
        }
    }

    // do movement checks here
    protected void movementChecks() {
        if (isJumping && !isFalling) {
                if (checkIfFalling()) {
                    animBody.SetTrigger("isFalling");
                    isFalling = true;
                }
            }
        else if (isFalling) {
            if (checkIfLanded()){
                    animBody.SetTrigger("hasLanded");
                    isFalling = false;
                    isJumping = false;
            }
        }
    }

    private void updateAttackCD() {
        canAttack = true;
    }

    // check if it is still falling
    private bool checkIfFalling() {
        bool isfalling = false;
        if (rbBody.velocity.y < 0) {
            isfalling = true;
        }
        return isfalling;
    }

    private bool checkIfLanded() {
        bool isLanded = false;
        if (isGrounded) {
            rbBody.velocity = new Vector2(0,0); // reset the velocity to prevent weird movement on the next jump
            isLanded = true;
        }
        return isLanded;
    }

    protected void resetDashState() {
        isDashing = false;
        rbBody.velocity = new Vector2(0, rbBody.velocity.y); // remove any force currently in it
        applyXForce(moveSpeed);
        
    }

    // pause player components to check if it is paused or not
    protected bool checkIfGamePaused() {
        
        if (isGamePaused) {
            if (rbBody.simulated) rbBody.simulated = false; // disable rigidbody physics when paused
            if (animBody.speed != 0) animBody.speed = 0;    // disable animator when paused
        }
        else {
            if (!rbBody.simulated) rbBody.simulated = true;
            if (animBody.speed == 0) animBody.speed = 1;
        }

        return isGamePaused;
    }

    //  ================ collision sequences start here  ================ //

    protected void OnCollisionEnter2D(Collision2D collision) {
        if (!isGrounded && collision.collider.tag == "Floor") {
            isGrounded = true;
        }
        if (collision.collider.tag == "Ball") {
            collision.collider.GetComponent<Rigidbody2D>().AddForce(new Vector2(rbBody.velocity.x, base_PushPower));
            doOnHitBall(scoreOnHitBall); // update the score
        }
    }

    
    protected void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.tag == "Floor") {
            // makes sure the player is grounded when touching the floor
            isGrounded = true;
        }
    }

    protected void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider.tag == "Floor") {
            isGrounded = false;
        }
    }

    //  ================ animation sequences start here  ================ //
    private void setAnimationWalking(int direction, bool isRunning) {
        if (isGrounded) animBody.SetBool("isRunning", isRunning);
    }

    private void setAnimationJumping() {
        rbBody.AddForce(new Vector2(jumpXPower * jumpXDirection, base_JumpPower), ForceMode2D.Impulse);
        isJumping = true;
        isGrounded = false;
        animBody.SetTrigger("isJumping");
    }

    private void applyXForce(float force) {
        rbBody.velocity = new Vector2(force * xDirection, rbBody.velocity.y);
    }

    private void flipSprite(int newDirection) {
        if (xDirection == 1) {
            if (newDirection == -1) transform.Rotate(new Vector3(0, 180, 0));
        } 
        else if (xDirection == -1) {
            if (newDirection == 1) transform.Rotate(new Vector3(0, 180, 0));
        } 
    }

    private void animateAttack() {
        if (Time.time <= atkTimer) return;
        animBody.SetTrigger("attack");
        SoundHandler.Instance.playSFX(SFXType.Spike);
        atkTimer = Time.time + base_ATKdelay; // add a cooldown to the atk button to prevent spamming
    }

    // does a dash and animates the player
    private void animateDash() {
        if (Time.time <= dashTimer) return;
        isDashing = true;

        animBody.SetTrigger("dash");
        SoundHandler.Instance.playSFX(SFXType.Dash);
        rbBody.AddForce(new Vector2(base_DashPower * xDirection, 0), ForceMode2D.Force);
        dashTimer = Time.time + base_DashDelay;
        dashEffectTimer = Time.time + dashEffectGap; // start creating dashes
    }

    private void createDashEffects() {
        if (!isDashing) return;
        if (Time.time < dashEffectTimer) return;
        
        // create new dashing mirage
        bool isFlipped = xDirection < 0? true: false;
        
        EffectHandler.Instance.CreateEffectSpeedMirage(transform.position, spriteRnd.sprite, isFlipped, Color.green);
        dashEffectTimer = Time.time + dashEffectGap; // reload the timer
    }

    protected void launchBall() {
        Rigidbody2D ballPower;
        float power;

        // detect if the ball is somewhere near the player if attack is fired
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position, width + attackRadius);
        if ((hitObject != null) && (hitObject.gameObject.name == "Player Ball")) {
            // apply upward force to the ball 
            ballPower = hitObject.gameObject.GetComponent<Rigidbody2D>();
            power = Mathf.Max(Mathf.Abs(ballPower.velocity.y) * base_ATKbase, base_ATKbase);
            ballPower.AddForce(new Vector2((rbBody.velocity.x), base_ATKbase));

            // create kickslash effect when the ball is hit
            if (!kickSlashReleased) {
                Debug.Log("Creating Kickslash");
                bool isFlipped = xDirection < 0? true: false;
                EffectHandler.Instance.CreateKickSlash(transform.position, isFlipped);
                kickSlashReleased = true;
            }

            // create impact ring effect when striking the ball. only do this if ball's velocity is higher than a threshold
            if (!impactReleased) {
                
                if (ballPower.velocity.magnitude > ballImpactMin) {
                    EffectHandler.Instance.CreateEffectImpactRing(hitObject.gameObject.transform.position, 0.05f);
                    impactReleased = true;
                    sfxBallHit = true;
                }
                else if (!sfxBallHit) { // only need to play the spike hit sfx once
                    SoundHandler.Instance.playSFX(SFXType.SpikeHit);
                    sfxBallHit = true;
                }
            }
        }
    }

    // reset the impactReleased bool
    public void resetImpactReleased() {
        impactReleased = false;
        sfxBallHit = false;
        kickSlashReleased = false;
    }

    // do Outro animation here
    protected override void doOnOutroanimation() {
        isControlDisabled = true;
        animBody.Play("player_idle");   // force the player into player_idle state
        animBody.SetTrigger("isTired");   
    }


    //  ================ testing sequences start here  ================ //

    private void initSkillTesting() {

    }

}
