using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RHC_PlayerControllerDemo : MonoBehaviour {

    [Header("Movement")]
    public float f_WalkSpeed;
    public float f_RunSpeed;
    private float f_UsedSpeed;
    public float f_JumpSpeed;
    public float f_Gravity;
    public string s_StandKey;
    private float f_StandHeight;
    public string s_CrouchKey;
    public float f_CrouchHeight;
    public float f_CrouchSpeedMultiplier;
    public string s_ProneKey;
    public float f_CrawlHeight;
    public float f_CrawlSpeedMultiplier;
    public float f_HeightChangeSpeed;
    private float f_SpeedMultiplier = 1.0f;
    private Coroutine co_HeightChange;
    private Vector3 v3_MoveDir;
    private bool b_RunToggle;
    private CharacterController cc_This;
    
    private void Awake()
    { 
        cc_This = GetComponent<CharacterController>();
        f_UsedSpeed = f_WalkSpeed;
        f_StandHeight = cc_This.height;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            RHC_EventManager.PSVO_CallRecoil();

        if(cc_This.isGrounded)
        {
            if (RHC_EventManager.en_CurrentStanceState == RHC_EventManager.StanceState.OnAir)
                RHC_EventManager.PSVO_ChangeStanceState(RHC_EventManager.StanceState.Standing);

            if (Input.GetKeyDown(KeyCode.LeftShift) 
                && (RHC_EventManager.en_CurrentMovementState == RHC_EventManager.MovementState.Walking || RHC_EventManager.en_CurrentMovementState == RHC_EventManager.MovementState.Running))
            {

                b_RunToggle = !b_RunToggle;
                f_UsedSpeed = b_RunToggle ? f_RunSpeed : f_WalkSpeed;

                if(cc_This.velocity.magnitude > 0.0f)
                {
                    if (b_RunToggle)
                        RHC_EventManager.PSVO_ChangeMovementState(RHC_EventManager.MovementState.Running);
                    else
                        RHC_EventManager.PSVO_ChangeMovementState(RHC_EventManager.MovementState.Walking);
                }

            }

            v3_MoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            v3_MoveDir = transform.TransformDirection(v3_MoveDir);
            v3_MoveDir *= f_UsedSpeed * f_SpeedMultiplier;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                v3_MoveDir.y = f_JumpSpeed;
                RHC_EventManager.PSVO_ChangeStanceState(RHC_EventManager.StanceState.OnAir);
            }

            if(cc_This.velocity.magnitude > 0.1f)
            {
                if (!b_RunToggle && RHC_EventManager.en_CurrentMovementState != RHC_EventManager.MovementState.Walking)
                    RHC_EventManager.PSVO_ChangeMovementState(RHC_EventManager.MovementState.Walking);
                if (b_RunToggle && RHC_EventManager.en_CurrentMovementState != RHC_EventManager.MovementState.Running)
                    RHC_EventManager.PSVO_ChangeMovementState(RHC_EventManager.MovementState.Running);
            }

            if (cc_This.velocity.magnitude < 0.1f && RHC_EventManager.en_CurrentMovementState != RHC_EventManager.MovementState.Idling)
                RHC_EventManager.PSVO_ChangeMovementState(RHC_EventManager.MovementState.Idling);

            if (Input.GetKeyDown(s_CrouchKey))
                VO_Crouch();
            if (Input.GetKeyDown(s_StandKey))
                VO_Stand();
            //if (Input.GetKeyDown(s_ProneKey))
            //    VO_Crawl();

        }

        v3_MoveDir.y -= Time.deltaTime * f_Gravity;
        cc_This.Move(v3_MoveDir * Time.deltaTime);
    }

    private void VO_Stand()
    {
        if (co_HeightChange != null)
            StopCoroutine(co_HeightChange);

        co_HeightChange = StartCoroutine(CO_LerpControllerHeight(f_StandHeight, 1.0f, RHC_EventManager.StanceState.Standing));
    }

    private void VO_Crouch()
    {
        if (co_HeightChange != null)
            StopCoroutine(co_HeightChange);

        co_HeightChange = StartCoroutine(CO_LerpControllerHeight(f_CrouchHeight, f_CrouchSpeedMultiplier, RHC_EventManager.StanceState.Crouching));
    }

    //private void VO_Crawl()
    //{
    //    if (co_HeightChange != null)
    //        StopCoroutine(co_HeightChange);

    //    co_HeightChange = StartCoroutine(CO_LerpControllerHeight(f_CrawlHeight, f_CrawlSpeedMultiplier, RHC_EventManager.StanceState.Crawling));
    //}

    IEnumerator CO_LerpControllerHeight(float target, float speedMult, RHC_EventManager.StanceState targetState)
    {
        float i = 0.0f;
        float current = cc_This.height;
        while(i < 1.0f)
        {
            i += Time.deltaTime * f_HeightChangeSpeed;
            cc_This.height = Mathf.Lerp(current, target, i);
            yield return null;
        }
        f_SpeedMultiplier = speedMult;
        RHC_EventManager.PSVO_ChangeStanceState(targetState);
    }
}
