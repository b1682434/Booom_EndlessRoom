using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public delegate void OnInteraction(IInteractRequest IInter);

	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
	
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;		
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;
		
		// interaction
		public OnInteraction onInteraction;
		
		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

	public playerInputState pstate = playerInputState.Walking;
	public float interactLength;
	public Image aimUI;
	public Text dialogText;
	Vector2 defaultAimUIpos;
	bool mouseOverTextChanged = false;
	public GameEvent exitFocusModeEvent;
	
		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

		defaultAimUIpos = aimUI.rectTransform.position;//aimui的默认位置
		}

		private void Update()
		{

		switch (pstate)
        {
			case playerInputState.Walking:
				GroundedCheck();
				Move();
				CameraRotation();
				WalkmodeInteractInput();
				MouseOverInteractable();
				JumpAndGravity();
				break;
			case playerInputState.Interacting:
				MouseOverInteractable();
				FocusMode();
				JumpAndGravity();
				break;
			case playerInputState.Donothing:
				break;
        }
		

		}
		
		private void LateUpdate()
		{
			//CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed =  MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
				_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			}

            // move the player
           
				_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			
			
		}

		
	void WalkmodeInteractInput()
    {
		if (_input.jump)
		{
			_input.jump = false;
			Interaction();
		}
		if (_input.confirm)
		{
			_input.confirm = false;
			Interaction();
		}
	}
	 void Interaction()
    {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(aimUI.transform.position);
		if (Physics.Raycast(ray, out hit, interactLength))
        {
			IInteractRequest IInter = hit.transform.GetComponent<IInteractRequest>();
            if (IInter != null)
            {
	            if(IInter.ObjectType == 1)//如果是门
                {
					if (pstate != playerInputState.Interacting)//防止专注模式点到门，有奇奇怪怪bug
					{
						StartCoroutine(PassingDoor(hit.transform.GetComponent<BasicPortal>().DoorWayPoints(transform.position), hit.normal));
						pstate = playerInputState.Donothing;
						IInter.InteractRequest(0);
					}
				}
                else
                {
					IInter.InteractRequest(0);
				}
	            
                if (IInter.returnWord != null)
                {
					dialogText.text = IInter.returnWord;
                }
                
                // 告知其他组件
                onInteraction?.Invoke(IInter);
            }

		}

	}

	public void EnterFocusMode()
    {
		pstate = playerInputState.Interacting;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = false;
    }
	void FocusMode()
    {
		aimUI.rectTransform.position = Mouse.current.position.ReadValue();
		if (_input.jump)
		{
			_input.jump = false;
			//FindFirstObjectByType<GameManger>().ExitFocusVcam();
			exitFocusModeEvent.Raise();
			aimUI.rectTransform.position = defaultAimUIpos;
			Cursor.lockState = CursorLockMode.Locked;
			pstate = playerInputState.Walking;
		}//离开专注模式
        if (_input.confirm)
        {
			_input.confirm = false;
			Interaction();
        }
	}
	IEnumerator PassingDoor(Vector3[] wayPoints, Vector3 doorFacingVec)
    {
		int i = 0;
		
		for(; ; )
        {
			i += 1;
            if (i < 50)
            {
				transform.position = Vector3.Lerp(transform.position, wayPoints[0], i / 50f);
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward, doorFacingVec * -1), i / 50f);
				CinemachineCameraTarget.transform.rotation = Quaternion.Lerp(CinemachineCameraTarget.transform.rotation, transform.rotation, i / 50f);

			}
            else
            {
				transform.position = Vector3.Lerp(transform.position, wayPoints[1], (i-50) / 50f);
			}
            if (i > 100)
            {
				StopAllCoroutines();
				_cinemachineTargetPitch = transform.rotation.x;
				pstate = playerInputState.Walking;
            }
			yield return new WaitForSeconds(0.02f);
        }
    }
	
	void MouseOverInteractable()
    {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(aimUI.transform.position);
		/*if (Physics.Raycast(ray, out hit, interactLength))
        {
			IInteractRequest IInter = hit.transform.GetComponent<IInteractRequest>();
			if (IInter != null)
            {
				IInter.MouseOver();
                if (!mouseOverTextChanged)
                {
					dialogText.text = IInter.returnWord;
					mouseOverTextChanged = true;
				}
            }
            else
            {
				if (mouseOverTextChanged)
				{
					dialogText.text = null;
					mouseOverTextChanged = false;
				}//好麻烦 但也不知道怎么简化。。。。
			}
        }
        else
        {
			if (mouseOverTextChanged)
			{
				dialogText.text = null;
				mouseOverTextChanged = false;
			}
		}*/
		IMouseOver Imouse;
		   if (Physics.Raycast(ray, out hit, interactLength))
		{ Imouse = hit.transform.GetComponent<IMouseOver>(); Debug.DrawLine(Camera.main.ScreenToWorldPoint(aimUI.transform.position), hit.point); }
        else
        {
			Imouse = null; Debug.DrawLine(Camera.main.ScreenToWorldPoint(aimUI.transform.position),ray.direction*interactLength+ Camera.main.ScreenToWorldPoint(aimUI.transform.position));

		}
			if (Imouse != null)
			{
			Imouse.MouseOver();				
			dialogText.text = Imouse.returnWord;
			mouseOverTextChanged = true;
				
			}
			else
			{
				if (mouseOverTextChanged)
				{
					dialogText.text = null;
					mouseOverTextChanged = false;
				}//好麻烦 但也不知道怎么简化。。。。
			}
		
		

	}
		private void JumpAndGravity()
		{
			
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
			

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				//_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);

			
		}
	}
