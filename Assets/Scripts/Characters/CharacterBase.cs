using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    // animation clip keys
    protected abstract string ANIM_KEY_IDLE { get; }
    protected abstract string ANIM_KEY_WALK { get; }
    protected abstract string ANIM_KEY_ATTACK { get; }
    protected abstract string ANIM_KEY_DIE { get; }

    // control constants
    protected virtual float moveSpeed { get; } = 5f;
    protected virtual float jumpSpeed { get; } = 12f;
    protected virtual float jumpHeight { get; } = 3f;
    protected virtual float groundDistance { get; } = 0.2f;
    private LayerMask groundMask;

    // character's components
    protected Animator animator;
    protected CharacterController controller;
    protected Transform groundCheck;

    // properties
    public PlayerInfo Info { get; private set; }
    public bool IsGrounded { get; private set;}
    private Vector3 velocity = Vector3.zero;
    private string curAnimKey = string.Empty;
    private Coroutine attackAnimCo = null;
    private Coroutine lerpPosCo = null;

    private void Start()
    {
        groundMask = LayerMask.GetMask("Ground");

        animator = transform.Find("GFX")?.GetComponent<Animator>() ?? null;
        if (!animator) Debug.LogErrorFormat("[{0}] Init error! Can't find component Animator.", this.gameObject.name);

        groundCheck = transform.Find("GroundCheck")?.transform ?? null;
        if (!groundCheck) Debug.LogErrorFormat("[{0}] Init error! Can't find groundCheck object.", this.gameObject.name);

        controller = GetComponent<CharacterController>();
        if (!animator) Debug.LogErrorFormat("[{0}] Init error! Can't find component CharacterController.", this.gameObject.name);

        Idle();
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void ApplyGravity()
    {
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -5f;
        }

        velocity.y += PhysicsConstant.GRAVITY * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Update()
    {
        CheckGrounded();
        ApplyGravity();

        if(!Info.id.Equals(PlayerManager.Instance.MyId))
        {
            if(Info.moveInfo.isMoving)
            {
                Rotate(Quaternion.Euler(0f, Info.moveInfo.angle, 0f));
                Walk(Info.moveInfo.dir);
            }
            else
            {
                Idle();
            }
        }
    }

    public void AdjustCharacterPos()
    {
        lerpPosCo = StartCoroutine(CoAdjustCharacterPos());
    }

    public void StopAdjustCharacterPos()
    {
        if (lerpPosCo != null)
        {
            StopCoroutine(lerpPosCo);
            lerpPosCo = null;
        }
    }

    private IEnumerator CoAdjustCharacterPos()
    {
        Vector3 targetPos = Info.moveInfo.position;
        while (!Mathf.Approximately(this.transform.position.x, targetPos.x))
        {
            Vector3 curPos = this.transform.position;
            this.transform.position = Vector3.Lerp(curPos, Info.moveInfo.position, Time.deltaTime * 10);
            yield return null;
        }

        StopAdjustCharacterPos();
    }

    private void PlayAnim(string key, bool sendReq, ECHaracterAction action)
    {
        if (!curAnimKey.Equals(key))
        {
            curAnimKey = key;
            animator.Play(key);

            if (sendReq)
                NetworkManager.Instance.SendPlayerAction(action);
        }
    }

    private IEnumerator CoPlayAttack(bool sendReq = false)
    {
        PlayAnim(ANIM_KEY_ATTACK, sendReq, ECHaracterAction.Attack);
        yield return new WaitForSeconds(1.2f);

        curAnimKey = string.Empty;
        attackAnimCo = null;
    }

    // interfaces
    public virtual void SetPlayerInfo(PlayerInfo info)
    {
        Info = info;
    }
    
    public virtual void Idle(bool sendReq = false)
    {
        if (curAnimKey.Equals(ANIM_KEY_ATTACK)) return;

        PlayAnim(ANIM_KEY_IDLE, sendReq, ECHaracterAction.Idle);
    }

    public virtual void Walk(Vector3 direction)
    {
        controller.Move(direction * moveSpeed * Time.deltaTime);
        
        if (curAnimKey.Equals(ANIM_KEY_ATTACK)) return;
        PlayAnim(ANIM_KEY_WALK, false, ECHaracterAction.Idle);
    }

    public virtual void Rotate(Quaternion angle)
    {
        transform.rotation = angle;
    }

    public virtual void TryJump(bool sendReq = false)
    {
        if (IsGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -PhysicsConstant.GRAVITY);

            if (sendReq)
                NetworkManager.Instance.SendPlayerAction(ECHaracterAction.Jump);
        }
    }

    public virtual void TryAttack(bool sendReq = false)
    {
        if (attackAnimCo == null)
        {
            attackAnimCo = StartCoroutine(CoPlayAttack(sendReq));
        }
    }
}
