using UnityEngine;
using System.Collections;
using System;

// AimBehaviour inherits from GenericBehaviour. This class corresponds to aim and strafe behaviour.
public class BuildBehaviourBasic : GenericBehaviour
{
	public string aimButton = "Aim", shoulderButton = "Aim Shoulder";     // Default aim and switch shoulders buttons.
	public Texture2D crosshair;                                           // Crosshair texture.
	public float aimTurnSmoothing = 0.15f;                                // Speed of turn response when aiming to match camera facing.
	public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0f);         // Offset to repoint the camera when aiming.
	public Vector3 aimCamOffset = new Vector3(0f, 0.4f, -0.7f);         // Offset to relocate the camera when aiming.

	[SerializeField]
	private bool isPlacingBlock;
	[SerializeField]
	private GameObject staticBlockPrefab;
	[SerializeField]
	private GameObject[] availableBlocks;
    [SerializeField]
	private GameObject blockHolder;
	[SerializeField]
	private float blockDistance = 5;
	[SerializeField]
    private GameObject ghostBlock;                                        // Variable to hold the block that's being built.
	[SerializeField]
    private float transparencyMultiplier = 0.8f;



	private Color originalBlockColor;
	private int aimBool;                                                  // Animator variable related to aiming.
	private bool aim;                                                     // Boolean to determine whether or not the player is aiming.
	private int currentlySelectedBlock = 0;                           // Float with which we make an ugly calculation to determine the currently selected block.

	// Start is always called after any Awake functions.
	void Start()
	{
		// Set up the references.
		aimBool = Animator.StringToHash("Aim");
	}

	// Update is used to set features regardless the active behaviour.
	void Update()
	{
		// Activate/deactivate aim by input.
		if (Input.GetAxisRaw(aimButton) != 0 && !aim)
		{
			StartCoroutine(ToggleAimOn());
		}
		else if (aim && Input.GetAxisRaw(aimButton) == 0)
		{
			StartCoroutine(ToggleAimOff());
		}

		// No sprinting while aiming.
		canSprint = !aim;

		// Toggle camera aim position left or right, switching shoulders.
		if (aim && Input.GetButtonDown(shoulderButton))
		{
			aimCamOffset.x = aimCamOffset.x * (-1);
			aimPivotOffset.x = aimPivotOffset.x * (-1);
		}

		// Set aim boolean on the Animator Controller.
		behaviourManager.GetAnim.SetBool(aimBool, aim);
	}

	// Co-rountine to start aiming mode with delay.
	private IEnumerator ToggleAimOn()
	{
		yield return new WaitForSeconds(0.05f);
		// Aiming is not possible.
		if (behaviourManager.GetTempLockStatus(this.behaviourCode) || behaviourManager.IsOverriding(this))
			yield return false;

		// Start aiming.
		else
		{
			aim = true;
			int signal = 1;
			aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
			aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
			yield return new WaitForSeconds(0.1f);
			behaviourManager.GetAnim.SetFloat(speedFloat, 0);
			// This state overrides the active one.
			behaviourManager.OverrideWithBehaviour(this);

			if (isPlacingBlock == false) //make sure we're not placing more than one block.
				InstantiateBlock(availableBlocks[currentlySelectedBlock]);   // Instantiate the currently selected box from the list.
            
        }
    }

    void InstantiateBlock(GameObject chosenBlockPrefab)
    {
        //Make sure we destroy any ghostblocks that remain attached to us for some reason.
		Destroy(ghostBlock);
        Vector3 frontOfPlayer = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
		//Instantiate a block in front of the player.
		isPlacingBlock = true;
		var spawnedBlock = Instantiate(chosenBlockPrefab, blockHolder.transform);
		spawnedBlock.transform.position = transform.position + frontOfPlayer * blockDistance;
        //Set the ghost block to the spawned block so we can manipulate it.
		ghostBlock = spawnedBlock;
        originalBlockColor = ghostBlock.GetComponent<MeshRenderer>().material.color;  //Cache its original color so we can restore it later.
		//Make the ghost block transparent and ghostly :)
		ghostBlock.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f * transparencyMultiplier);
		//Disable collision on the ghost box while it's being "built"
		var ghostCollider = ghostBlock.GetComponent<Collider>();
		ghostCollider.enabled = false;
	}

	// Co-rountine to end aiming mode with delay.
	private IEnumerator ToggleAimOff()
	{
		aim = false;
		yield return new WaitForSeconds(0.3f);
		behaviourManager.GetCamScript.ResetTargetOffsets();
		behaviourManager.GetCamScript.ResetMaxVerticalAngle();
		yield return new WaitForSeconds(0.05f);
		behaviourManager.RevokeOverridingBehaviour(this);
		// Make the ghost block "real" again.
		DropBlock();
	}

    void DropBlock()
    {
        if (ghostBlock != null)
		{
			isPlacingBlock = false;
			var ghostCollider = ghostBlock.GetComponent<Collider>();
			ghostCollider.enabled = true;
			ghostBlock.GetComponent<MeshRenderer>().material.color = originalBlockColor;
			ghostBlock = null;
        }
		

	}

	// LocalFixedUpdate overrides the virtual function of the base class.
	public override void LocalFixedUpdate()
	{
		// Set camera position and orientation to the aim mode parameters.
		if (aim)
			behaviourManager.GetCamScript.SetTargetOffsets(aimPivotOffset, aimCamOffset);
	}

	// LocalLateUpdate: manager is called here to set player rotation after camera rotates, avoiding flickering.
	public override void LocalLateUpdate()
	{
		AimManagement();
		BlockManagement();
	}

	// Handle aim parameters when aiming is active.
	void AimManagement()
	{
		// Deal with the player orientation when aiming.
		Rotating();
	}

	// Rotate the player to match correct orientation, according to camera.
	void Rotating()
	{
		Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
		// Player is moving on ground, Y component of camera facing is not relevant.
		forward.y = 0.0f;
		forward = forward.normalized;

		// Always rotates the player according to the camera horizontal rotation in aim mode.
		Quaternion targetRotation = Quaternion.Euler(0, behaviourManager.GetCamScript.GetH, 0);

		float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

		// Rotate entire player to face camera.
		behaviourManager.SetLastDirection(forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);

	}

	// Draw the crosshair when aiming.
	void OnGUI()
	{
		if (crosshair)
		{
			float mag = behaviourManager.GetCamScript.GetCurrentPivotMagnitude(aimPivotOffset);
			if (mag < 0.05f)
				GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
										 Screen.height / 2 - (crosshair.height * 0.5f),
										 crosshair.width, crosshair.height), crosshair);
		}
	}

	// Position the ghost block while aiming.
	void BlockManagement()
	{
		MaintainBlockPosition();
		AllowBlockSwitching();
	}

    // Let the player switch between the blocks. At some point we'll want to insert the menu code here.
    void AllowBlockSwitching()
    {
        // This is an awful piece of placeholder code that flips between blocks when you click.
        if (Input.GetMouseButtonDown(0))
        {
			currentlySelectedBlock++;
            if (currentlySelectedBlock >= availableBlocks.Length)
            {
				currentlySelectedBlock = 0;
            }
			InstantiateBlock(availableBlocks[currentlySelectedBlock]);

        }
    }

    void MaintainBlockPosition()
    {
		Vector3 frontOfPlayer = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
		ghostBlock.transform.position = transform.position + frontOfPlayer * blockDistance;
		// Always keep the ghost block oriented downwards.
		ghostBlock.transform.rotation = Quaternion.Euler(0, 0, 0);
		var ghostCollider = ghostBlock.GetComponent<Collider>();
		ghostCollider.enabled = false;

	}

}

