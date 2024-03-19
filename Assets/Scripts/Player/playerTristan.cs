using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Player class for the player unit the user directly controls
/// Inherits from Player Unit, but no need to assign its playerunit object
/// </summary>

public class PlayerTristan : PlayerUnit
{
    // public entries
    public Projectiles attackEffect;
    public Transform frontSide, attackPoint, notePoint, bodyPoint;
    
    // Arrays and Lists
    public MusicNote[] skillNotes;
    protected List<MusicNote> noteList;
    protected int[] skillCombo = new int[4];    // skill activation requires four notes
    private List<Buff> buffsToRemove;
    
    // will contain the stats for the character to be set on the UI
    public float base_ATKbase, base_ATKmax, base_ATKdelay, base_ATKRange; 

    // objects 
    private Rigidbody2D rbBody;
    private Vector2 moveInput, moveData;

    // primitives    
    private int castCounter = 0;
    private bool canAttack = true, startCast = false, castSuccess = false;
        
    // specifics
    [SerializeField]
    private AudioSource SFXCast;

    [SerializeField]
    protected GameObject body;
    protected Animator animBody;

    // Testing items here;
    public ActiveFireball skill1;
    public BuffSpeedUp1 skill2;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        rbBody = GetComponent<Rigidbody2D>();
        animBody = body.GetComponent<Animator>();
        noteList = new List<MusicNote>();
        initializeStats();
        initSkillTesting();
        resetTapCheckerStats();

        // temporarily enable god mode;
        // isImmune = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isControlDisabled) {
            checkIfAlive();
            inputMovement();
            checkForSkillTaps();
        }
        // check these stuff if the player is still alive
        if (isAlive) { 
            buffListManagement();
        }
    }

    void FixedUpdate()
    {
        // move the rigidbody here
        rbBody.MovePosition(rbBody.position + moveData * Time.fixedDeltaTime);
    }

    // ================ Stats and Status sequences start here  ================ //
    private void checkIfAlive () {
        isAlive = checkHPifAlive() ? true : false;
    }

    // use this class at the start of every game
    private void initializeStats() {
        ATKbase = base_ATKbase;
        ATKmax = base_ATKmax;
        ATKdelay = base_ATKdelay;
        ATKRange = base_ATKRange;

        // reset the buffStatsList;
        playerBuffs = new buffStatsList(0, 0, 0);

        // reset the skills used
        skillList = new SkillManager(this);
    }

    // reset tap checker stats here
    private void resetTapCheckerStats() {
        startCast = false; 
        castSuccess = false;
        castCounter = 0;
        for (int i = 0; i < skillCombo.Length; i++) skillCombo[i] = 0;    // reset the skillCombo flags
        noteList.Clear(); // reset the notes list
    }

    // Manage buffs here
    private void buffListManagement() {
        if (buffList.checkExpiredBuffs()) {
            Debug.Log("One or more buffs have expired");
            buffsToRemove = buffList.getExpiredBuffs();
            foreach (Buff buff in buffsToRemove) {
                buff.doOnRemoveBuff();
            }
        }
    }

    // ================ Input action sequences start here  ================ //
    // Input for moving player
    private void inputMovement() {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveData = moveInput.normalized * (moveSpeed + playerBuffs.moveSpeed);

        // add default keycode for up, down left and right arrows
        if (Input.GetKeyDown(controls.MoveUp) || Input.GetKeyDown(KeyCode.UpArrow)) setAnimationWalking(1, true);
        else if (Input.GetKeyDown(controls.MoveDown) || Input.GetKeyDown(KeyCode.DownArrow)) setAnimationWalking(2, true);
        else if (Input.GetKeyDown(controls.MoveLeft) || Input.GetKeyDown(KeyCode.LeftArrow)) setAnimationWalking(3, true);
        else if (Input.GetKeyDown(controls.MoveRight) || Input.GetKeyDown(KeyCode.RightArrow)) setAnimationWalking(4, true);
        
        // keep walking animation while any of the directions are pressed
        if (Input.GetKey(controls.MoveUp) || Input.GetKey(controls.MoveDown) || Input.GetKey(controls.MoveLeft) || Input.GetKey(controls.MoveRight)) setAnimationWalking(0, true); 
        else setAnimationWalking(0, false);

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

    // disable the damage shield as needed
    private void updateDamageShield() {
        isdamageShldActive = false;
    }

    //  ================ animation sequences start here  ================ //
    private void setAnimationWalking(int direction, bool isWalking) {
        animBody.SetBool("isWalking", isWalking);
        if (direction > 0) animBody.SetInteger("walkDirection", direction);
    }

    private void animateAttack() {
        Projectiles tempSlash; 
        int frontSideRotation = 0;
        frontSideRotation = animBody.GetInteger("walkDirection");
        animBody.SetTrigger("normalAttack");
        tempSlash = Instantiate(attackEffect, attackPoint.position, attackPoint.rotation);
        tempSlash.DMG = Random.Range(base_ATKbase, base_ATKmax) + playerBuffs.ATKmin;
    }
    
    // do on takes damage
    protected override void doOnTakeDamage(float DMG) {
        Debug.Log("Getting damaged: " + isdamageShldActive);
        if (!isdamageShldActive && HP > 0) {
            isdamageShldActive = true;
            HP -= DMG;
            if (HP <= 0) {
                HP = 0;
                // isAlive = false; // disable if you don't want the player to die
            }
            
            // inform parent class that HP bar can now be updated;
            updateHPBar();
            Invoke("updateDamageShield", base_DMGdelay);
        }
    }

    // where start casting for skill occurs
    private void checkForSkillTaps() {
        // if either buttons are pressed
        if (Input.GetKeyDown(controls.Skillsync1) || Input.GetKeyDown(controls.Skillsync2)) {
        
            // check for taps and build up the skill Combo list
            if (RhythmHandler.checkTap()) {
                if (!startCast) {
                    startCast = true;
                }
                if (Input.GetKeyDown(controls.Skillsync1)) {
                    if (castCounter < skillCombo.Length) {
                        skillCombo[castCounter] = 1;
                        addMusicNotes(1, true);
                    }
                    else Debug.Log("castCounter out of bounds: " + castCounter);
                }
                else if (Input.GetKeyDown(controls.Skillsync2)) {
                    if (castCounter < skillCombo.Length) {
                        skillCombo[castCounter] = 2;
                        addMusicNotes(2, true);
                    }
                    else Debug.Log("castCounter out of bounds: " + castCounter);
                }
                consoleUI.Log("Tap Successful");
            }
            else {
                if (Input.GetKeyDown(controls.Skillsync1)) {
                    addMusicNotes(1, false);
                }
                else if (Input.GetKeyDown(controls.Skillsync2)) {
                    addMusicNotes(2, false);
                }
                 
                consoleUI.Log("Tap Missed");
            }
        }
    }

    // create note under Tristan using noteType
    private void addMusicNotes(int noteType, bool success) {
        MusicNote tempNote;
        if (noteType <= skillNotes.Length && noteType > 0) {
            tempNote = Instantiate(skillNotes[noteType - 1], notePoint.position, notePoint.rotation);
            tempNote.noteStarted();
            tempNote.transform.SetParent(notePoint);
            noteList.Add(tempNote);
            if (success) castCounter++;
            adjustNotePosition(success);
        }
    }

    // move the notes accordingly
    private void adjustNotePosition(bool success) {
        float noteWidth = 0, posXFactor = 0, startXPos = notePoint.position.x, startYPos = notePoint.position.y;
        // get the length of noteList first to determine how many times to move a note
        int l = noteList.Count, startXFactor = 0;
        
        // get the length of a musical note 
        if (noteList.Count > 0) {
            noteWidth = noteList[0].GetComponent<Renderer>().bounds.size.x;
            posXFactor = noteWidth * 2; // multiply by 2 to include gaps in between
        }
        if (l > 1) {
            float xpos = 0;
            // determine middle number and start at leftside most of the spacing
            startXFactor = (l / 2) * -1; // get the half value and truncate the decimals;
            if (l % 2 == 0) startXPos += noteWidth;

            // start moving the notes as needed.
            for (int i = 0; i < l; i++) {
                xpos = startXPos + (posXFactor * startXFactor);
                Vector2 moveNote = new Vector2(xpos,startYPos);
                noteList[i].transform.position = moveNote;
                startXFactor++;
            }
        }

        // if the notes failed at least once break the notes and restart
        // otherwise if the cast counter is completed, cast the skill
        if (!success || castCounter >= skillCombo.Length) completeNoteList(success);
    }

    // animate the notes in the noteList list if they pass or fail, then remove them;
    private void completeNoteList(bool success) {
        MusicNote tempNote;
        consoleUI.Log("Cast Status: " + success);
        while (noteList.Count > 0) {
            tempNote = noteList[0]; 
            if (success) tempNote.comboPassed();
            else tempNote.comboFailed();
            noteList.RemoveAt(0);
        }

        // trigger the skills if success
        if (success) {
            castSuccess = true;
            triggerSkills();
        }
        // reset tap checker stats upon failing or completing
        else resetTapCheckerStats();
    }

    // trigger completed skillCombo here and find with matching skill;
    private void triggerSkills() {
        
        Skill skillToUse;
        if (castSuccess) {
            skillToUse = skillList.matchTriggerCombo(skillCombo);
            if (skillToUse != null) {
                skillToUse.triggerSkill();
                SFXCast.Play(); // play casting SFX
            }
            resetTapCheckerStats();
        }

    }

    //  ================ testing sequences start here  ================ //

    private void initSkillTesting() {
        // temporary skills here;
        skill1.DMG = base_ATKbase;
        skill1.SPEED = 30;
        skill1.RANGE = 5;
        skill1.Owner = this;
        skill1.Castpoint = frontSide;
        
        // buff skill here
        skill2.Owner = this;
        skill2.Target = this;
        skill2.Castpoint = frontSide;
        skill2.Attachpoint = bodyPoint;

        // add those skills to the skillList
        skillList.addSkill(skill1);
        skillList.addSkill(skill2);
    }

}
