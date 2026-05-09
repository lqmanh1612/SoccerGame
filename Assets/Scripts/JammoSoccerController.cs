using UnityEngine;

[RequireComponent(typeof(Animator))]
public sealed class JammoSoccerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float turnSpeed = 14f;
    [SerializeField] private Vector2 minBounds = new Vector2(-14.301229f, -10.049449f);
    [SerializeField] private Vector2 maxBounds = new Vector2(14.206429f, 10.121249f);

    [Header("Animation")]
    [SerializeField] private string blendParameter = "Blend";
    [SerializeField] private string normalTrigger = "normal";
    [SerializeField] private float runningBlendValue = 0.6f;
    [SerializeField] private float animationDampTime = 0.08f;

    [Header("Top Down Camera")]
    [SerializeField] private Camera followCamera;
    [SerializeField] private float cameraHeight = 49.2f;
    [SerializeField] private Vector3 cameraOffset = Vector3.zero;
    [SerializeField] private float cameraSmoothTime = 0.12f;

    private Animator animator;
    private Vector3 cameraVelocity;
    private float fixedGroundY;
    private int blendHash;
    private int normalHash;
    private bool hasBlendParameter;
    private bool hasNormalTrigger;
    private Transform cameraTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        fixedGroundY = transform.position.y;
        blendHash = Animator.StringToHash(blendParameter);
        normalHash = Animator.StringToHash(normalTrigger);
        CacheAnimatorParameters();
        cameraTarget = transform;

        if (followCamera == null)
        {
            followCamera = Camera.main;
        }
    }

    public void SetCameraTarget(Transform target)
    {
        cameraTarget = target;
    }

    public void ResetCameraTarget()
    {
        cameraTarget = transform;
    }


    private void Start()
    {
        if (hasNormalTrigger)
        {
            animator.SetTrigger(normalHash);
        }

        ClampToField();
        SnapCameraToTarget();
    }

    private void Update()
    {
        Vector3 moveDirection = ReadMoveDirection();
        bool isMoving = moveDirection.sqrMagnitude > 0.0001f;

        if (isMoving)
        {
            Move(moveDirection);
            FaceMoveDirection(moveDirection);
        }

        UpdateAnimation(isMoving);
    }

    private void LateUpdate()
    {
        FollowWithTopDownCamera();
    }

    private Vector3 ReadMoveDirection()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontal -= 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontal += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            vertical -= 1f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            vertical += 1f;
        }

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        return direction.sqrMagnitude > 1f ? direction.normalized : direction;
    }

    private void Move(Vector3 direction)
    {
        Vector3 nextPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, minBounds.x, maxBounds.x);
        nextPosition.y = fixedGroundY;
        nextPosition.z = Mathf.Clamp(nextPosition.z, minBounds.y, maxBounds.y);
        transform.position = nextPosition;
    }

    private void FaceMoveDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void UpdateAnimation(bool isMoving)
    {
        if (!hasBlendParameter)
        {
            return;
        }

        float targetBlend = isMoving ? runningBlendValue : 0f;
        animator.SetFloat(blendHash, targetBlend, animationDampTime, Time.deltaTime);
    }

    private void FollowWithTopDownCamera()
    {
        if (followCamera == null || cameraTarget == null)
        {
            return;
        }

        Vector3 targetPosition = cameraTarget.position + cameraOffset;
        targetPosition.y = cameraHeight;
        followCamera.transform.position = Vector3.SmoothDamp(
            followCamera.transform.position,
            targetPosition,
            ref cameraVelocity,
            cameraSmoothTime);
        followCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void SnapCameraToTarget()
    {
        if (followCamera == null || cameraTarget == null)
        {
            return;
        }

        Vector3 targetPosition = cameraTarget.position + cameraOffset;
        targetPosition.y = cameraHeight;
        followCamera.transform.position = targetPosition;
        followCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private void ClampToField()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = fixedGroundY;
        position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
        transform.position = position;
    }

    private void CacheAnimatorParameters()
    {
        hasBlendParameter = false;
        hasNormalTrigger = false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == blendHash && parameter.type == AnimatorControllerParameterType.Float)
            {
                hasBlendParameter = true;
            }
            else if (parameter.nameHash == normalHash && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                hasNormalTrigger = true;
            }
        }
    }
}
