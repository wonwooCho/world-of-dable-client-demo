using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    // mouse rotation contants
    private readonly float TURN_SMOOTH_TIME = 0.1f;
    private float turnSmoothVelocity;

    // player properties
    private CharacterBase playerScript;
    private Transform playerTransform;
    private Transform cam;
    private PlayerMoveInfo moveInfo = new PlayerMoveInfo();

    // network send
    private Coroutine sendMovingInfoCoroutine = null;

    public bool Init(CharacterBase playerChar)
    {
        if (!playerChar)
        {
            Debug.LogError("[PlayerInputManager] Init error! playerChar is NULL");
            return false;
        }

        playerScript = playerChar;
        playerTransform = playerChar.transform;
        cam = Camera.main.transform;
        return true;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (0.1f <= direction.magnitude)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TURN_SMOOTH_TIME);
            playerScript.Rotate(Quaternion.Euler(0f, angle, 0f));

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 dir = moveDir.normalized;
            playerScript.Walk(dir);

            moveInfo.position = playerScript.transform.position;
            moveInfo.dir = dir;
            moveInfo.angle = angle;
            moveInfo.isMoving = true;
            StartSendMovingInfo();
        }
        else
        {
            moveInfo.position = playerScript.transform.position;
            moveInfo.isMoving = false;
            StopSendMovingInfo();
            playerScript.Idle(true);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerScript.TryJump(true);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            playerScript.TryAttack(true);
        }
    }

    private void StartSendMovingInfo()
    {
        if (sendMovingInfoCoroutine == null)
        {
            sendMovingInfoCoroutine = StartCoroutine(CoSendMovingInfo());
        }
    }

    private void StopSendMovingInfo()
    {
        if (sendMovingInfoCoroutine != null)
        {
            StopCoroutine(sendMovingInfoCoroutine);
            sendMovingInfoCoroutine = null;

            NetworkManager.Instance.SendPlayerEndMove(moveInfo);
        }
    }

    private IEnumerator CoSendMovingInfo()
    {
        while (true)
        {
            NetworkManager.Instance.SendPlayerMoving(moveInfo);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
