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

    protected const float startPosX = 0f, startPosY = -4.3f;
    // Delegates and Events
    public delegate void onHitBall(int score, int timeToAdd = 0);
    public event onHitBall doOnHitBall;

    // public entries
    // public Projectiles attackEffect;
    public Transform frontSide, attackPoint, notePoint, bodyPoint;
    
    // Arrays and Lists
    
    // will contain the stats for the character to be set on the UI
    public float base_ATKbase, base_ATKmax, base_ATKdelay, base_ATKRange, base_DashDelay, base_DashPower, base_JumpPower, base_PushPower, attackRadius; 
    public int scoreOnHitBall, scoreOnStrikeBall, scoreOnHitTarget;

    // objects 
    private Rigidbody2D rbBody;
    protected Renderer render;
    private Vector2 moveInput, moveData, currentGravity;

    // primitives    
    private int castCounter = 0, xDirection = 1, jumpXDirection = 0;
    private float jumpXPower = 0, width, height, atkTimer = 0f, dashTimer = 0f;
    private bool canAttack = true, isJumping = false, isFalling = false, isGrounded = false, isDashing = false;
        
    // specifics
    [SerializeField]
    private AudioSource SFXCast;

    [SerializeField]
    protected GameObject body;
    protected Animator animBody;

    // Testing items here;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        rbBody = GetComponent<Rigidbody2D>();
        animBody = GetComponent<Animator>();
        render = body.GetComponent<Renderer>();
        currentGravity = Physics2D.gravity;

        Debug.Log("Gravity Check: " + currentGravity);
        // restartUnit("normal");
        
        // temporarily enable god mode;
        // isImmune = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (!isControlDisabled) {
            inputMovement();    
        }
        if (checkIfGamePaused()) return;

        if (isJumping && !isFalling) {
            // Debug.Log("Body velocity: " + rbBody.velocity.x + " - " + rbBody.velocity.y);
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

    void FixedUpdate()
    {
        // move the rigidbody here. 
        if (isGrounded && !isJumping && !isFalling && !isDashing) rbBody.MovePosition(rbBody.position + new Vector2(moveData.x, 0) * Time.fixedDeltaTime);
    }

    // ================ Stats and Status sequences start here  ================ //
    public override void restartUnit(string gameMode) {
        initializeStats();
    }


    // use this class at the start of every game
    private void initializeStats() {
        width = render.bounds.size.x;
        height = render.bounds.size.y;
        rbBody.velocity = new Vector2(0,0);
        atkTimer = 0f;
        dashTimer = 0f;
        transform.position = new Vector2(startPosX, startPosY);
    }

    // ================ Input action sequences start here  ================ //
    // Input for moving player
    private void inputMovement() {

        if (Input.GetKeyDown(controls.Pause)) {
            if (!isGamePaused) {
                playerPressedPause(true);
            }
            else {
                Debug.Log("Unpausing game...");
                playerPressedPause(false);
            }
        }
        if (isGamePaused) return; // reject every other control unless the game is unpaused

        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        moveData = moveInput.normalized * moveSpeed;

        // add default keycode for up, down left and right arrows
        // add jump force when pressing up
        if (!isJumping) {
            if (Input.GetKeyDown(controls.MoveUp) || Input.GetKeyDown(KeyCode.UpArrow)) setAnimationJumping();    
            if (Input.GetKeyDown(controls.MoveLeft) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                flipSprite(-1);
                xDirection = -1;
                jumpXDirection = 1;
                setAnimationWalking(3, true);
            }
            else if (Input.GetKeyDown(controls.MoveRight) || Input.GetKeyDown(KeyCode.RightArrow)) {
                flipSprite(1);
                xDirection = 1;
                jumpXDirection = 1;
                setAnimationWalking(4, true);
            }
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
        if (Input.GetKeyDown(controls.Attack)) animateAttack();    

        // keep walking animation while any of the directions are pressed
        if (Input.GetKey(controls.MoveLeft) || Input.GetKey(controls.MoveRight)) setAnimationWalking(0, true); 
        else {
            jumpXDirection = 0;
            setAnimationWalking(0, false);
        }

        // attack animation
        if (Input.GetKey(controls.Attack)) {
            if (canAttack) {
                canAttack = false;
                animateAttack();
                Invoke("updateAttackCD", ATKdelay);
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
        if (collision.collider.tag == "Floor") {
            isGrounded = true;
        }
        if (collision.collider.tag == "Ball") {
            // Debug.Log("Pushing ball");
            collision.collider.GetComponent<Rigidbody2D>().AddForce(new Vector2(rbBody.velocity.x, base_PushPower));
            doOnHitBall(scoreOnHitBall); // update the score
        }
    }


    //  ================ animation sequences start here  ================ //
    private void setAnimationWalking(int direction, bool isRunning) {
        animBody.SetBool("isRunning", isRunning);
    }

    private void setAnimationJumping() {
        Debug.Log("Applying Jump Power: " + jumpXPower * jumpXDirection + " (" + xDirection + ")");
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
        if (Time.time > atkTimer) {
            animBody.SetTrigger("attack");
            atkTimer = Time.time + base_ATKdelay; // add a cooldown to the atk button to prevent spamming
        }
    }

    private void animateDash() {
        if (Time.time > dashTimer) {
            animBody.SetTrigger("dash");
            isDashing = true;
            Debug.Log("Dash Power: " + (base_DashPower * xDirection));
            rbBody.AddForce(new Vector2(base_DashPower * xDirection, 0), ForceMode2D.Force);
            dashTimer = Time.time + base_DashDelay;
        }
    }

    private void addDashPower() {
        
    }

    protected void launchBall() {
        Rigidbody2D ballPower;
        float power;
        // detect if the ball is somewhere near the player if attack is fired
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position, width + attackRadius);
        if ((hitObject != null) && (hitObject.gameObject.name == "Player Ball")) {
            // apply force to the ball using 
            ballPower = hitObject.gameObject.GetComponent<Rigidbody2D>();
            power = Mathf.Max(Mathf.Abs(ballPower.velocity.y) * base_ATKbase, base_ATKbase);
            // Debug.Log("Power: " + power);
            ballPower.AddForce(new Vector2((rbBody.velocity.x), base_ATKbase));
        }
    }
    

    //  ================ testing sequences start here  ================ //

    private void initSkillTesting() {

    }

}
