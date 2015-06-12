using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Main : MonoBehaviour {

	// Singleton Instance
	public static Main Instance { get; private set; } 
	
	private int game_width = 15;
	private int game_height = 8;

	private bool gameOver = true;

	private float difficulty;
	public float maxDifficulty;
	public float difficultyIncrement;
	
	private List<GameObject> obstructions;
	private Queue<GameObject> activeObstructions;
	private List<GameObject> inactiveObstructions;
	
	private List<GameObject> collectibles;
	private List<GameObject> activeCollectibles;
	private List<GameObject> inactiveCollectibles;

	// Timing related fields
	private float startTime;
	private float elapsedDurationMilliseconds;
	private int elapsedDurationSeconds;
	public float gameLength;

	private int lastObstructionDeactivation;
	private int lastCollectibleDeactivation;
	private int lastGravityFlip;

	private int score;

	private bool inverseGravity = false;

	public AudioClip explosion;
	public AudioClip beep;

	void Awake()
	{
		// singleton locking code
		if (Instance == null) Instance = this;
		else Destroy( gameObject );
	}

	// Use this for initialization
	void Start()
	{
		Screen.SetResolution(960, 540, false);
		startGame();
	}

	void startGame()
	{
		gameOver = false;
		
		InGamePanelController.Instance.panel.SetActive (true);
		EndGamePanelController.Instance.panel.SetActive (false);

		difficulty = 0.0f;
		maxDifficulty /= 1000;
		difficultyIncrement /= 1000;

		startTime = Time.time * 1000;
		gameLength *= 1000;
		elapsedDurationSeconds = 0;

		lastObstructionDeactivation = 0;
		lastCollectibleDeactivation = 0;

		score = 0;
		InGamePanelController.Instance.updateScore (score);
		
		Physics.gravity = new Vector3(0.0f, -9.8f, 0.0f) * 2;

		// Instantiate lists
		obstructions = new List<GameObject>();
		inactiveObstructions = new List<GameObject>();
		activeObstructions = new Queue<GameObject>();

		collectibles = new List<GameObject>();
		inactiveCollectibles = new List<GameObject>();
		activeCollectibles = new List<GameObject>();

		for (int i = 0; i < game_width; i++) {
			for (int j = 0; j < game_height; j++) {
				// Obstructions
				GameObject obstruction = GameObject.Find (i + "," + j);
				inactiveObstructions.Add (obstruction);
				obstructions.Add (obstruction);

				// Collectibles
				GameObject collectible = GameObject.Find ("collectible " + i + "," + j);
				inactiveCollectibles.Add (collectible);
				collectibles.Add (collectible);
			}
		}

		// Spin each collectible for the whole game
		foreach(GameObject collectible in collectibles) {
			collectible.transform.DORotate(new Vector3(0, 0, -360), 10, (RotateMode)RotateMode.LocalAxisAdd)
								 .SetEase(Ease.Linear)
								 .SetLoops(-1);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if(!gameOver) {
			elapsedDurationMilliseconds = Time.time * 1000 - startTime;
			int elapsedDurationSeconds = (int)System.Math.Round(elapsedDurationMilliseconds / 1000, 0);

			InGamePanelController.Instance.updateTimer (elapsedDurationSeconds);

			// Increase the difficulty a little bit
			if(difficulty < maxDifficulty) {
				difficulty += difficultyIncrement;
			}
			
			// Get a random obstruction and activate it, and store it in the activated obstructions queue
			bool obstructionShouldBeActivated = spawnAnObstruction (difficulty);
			if(obstructionShouldBeActivated) {
				activateObstruction (getRandomInactiveObstruction());
			}
			
			// Get a random collectible and activate it, and store it in the activated collectibles queue
			bool collectibleShouldBeActivated = spawnACollectible (difficulty);
			if(collectibleShouldBeActivated) {
				activateCollectible (getRandomInactiveCollectible());
			}
			
			// Pop the top obstruction from the queue and deactivate it (every 2 seconds)
			int secondsSinceLastObstructionDeactivation = elapsedDurationSeconds - lastObstructionDeactivation;
			if(activeObstructions.Count > 0 && secondsSinceLastObstructionDeactivation >= 2) {
				deactivateNextObstruction();
				lastObstructionDeactivation = elapsedDurationSeconds;
			}
			
			// Pop the top collectible from the queue and deactivate it (every 10 seonds)
			int secondsSinceLastCollectibleDeactivation = elapsedDurationSeconds - lastCollectibleDeactivation;
			if(activeCollectibles.Count > 0 && secondsSinceLastCollectibleDeactivation >= 10) {
				deactivateNextCollectible();
				lastCollectibleDeactivation = elapsedDurationSeconds;
			}
			
			// Invert gravity to make it a little harder
			int secondsSinceLastGravityFlip = elapsedDurationSeconds - lastGravityFlip;
			if(secondsSinceLastGravityFlip >= 10) {
				invertGravity();
				lastGravityFlip = elapsedDurationSeconds;
			}

			if(elapsedDurationMilliseconds >= gameLength) {
				//show scores ui etc
				endGame();
			}
		}
	}
	
	// Decide whether an obstruction should be spawned this frame or not
	private bool spawnAnObstruction(float probability)
	{
		// don't spawn one if there is no spare ones to activate
		if (inactiveObstructions.Count < 1) {
			return false;
		}
		
		// Determine whether an obstruction should be spawned
		// based on the probability / current difficulty level
		float randomNumber = Random.Range (0f, 5f);
		bool spawn = randomNumber <= probability;
		return spawn;
	}
	
	// Return a random inactive obstruction
	private GameObject getRandomInactiveObstruction()
	{
		int obstructionIndex = Random.Range(0, inactiveObstructions.Count);
		GameObject obstruction = inactiveObstructions[obstructionIndex];
		return obstruction;
	}
	
	// Make the obstruction come out of the background
	private void activateObstruction(GameObject obstruction)
	{
		activeObstructions.Enqueue (obstruction);
		inactiveObstructions.Remove (obstruction);
		obstruction.transform.DOMoveZ (0, 2);
	}
	
	// Deactivate the obstruction at the front of the queue
	private void deactivateNextObstruction()
	{
		GameObject obstruction = activeObstructions.Dequeue();
		inactiveObstructions.Add (obstruction);
		obstruction.transform.DOMoveZ (2, 2);
	}
	
	// Decide whether a collectible should be spawned this frame or not
	private bool spawnACollectible(float probability)
	{
		// don't spawn one if there is no spare ones to activate
		if (inactiveCollectibles.Count < 1) {
			return false;
		}
		
		// Collectibles should be rarer than obstructions
		probability /= 2f;
		
		// Determine whether a collectible should be spawned
		// based on the probability / current difficulty level
		float randomNumber = Random.Range (0f, 5f);
		bool spawn = randomNumber <= probability;
		return spawn;
	}
	
	// Return a random inactive collectible
	private GameObject getRandomInactiveCollectible()
	{
		int collectibleIndex = Random.Range(0, inactiveCollectibles.Count);
		GameObject collectible = inactiveCollectibles[collectibleIndex];
		return collectible;
	}
	
	// Make the obstruction come out of the background
	private void activateCollectible(GameObject collectible)
	{
		activeCollectibles.Add (collectible);
		inactiveCollectibles.Remove (collectible);
		collectible.SetActive(true);
		collectible.transform.DOMoveZ (0f, 0.25f);
	}
	
	// Deactivate the obstruction at the front of the queue
	private void deactivateNextCollectible()
	{
		if(activeCollectibles.Count > 0) {
			GameObject collectible = activeCollectibles[0];
			activeCollectibles.Remove(collectible);
			inactiveCollectibles.Add (collectible);
			collectible.transform.DOMoveZ (-16f, 0f);
			lastCollectibleDeactivation = elapsedDurationSeconds;
		}
	}
	
	// Deactivate the obstruction at the front of the queue
	public void deactivateSpecificCollectible(GameObject collectible)
	{
		audio.PlayOneShot (beep);
		activeCollectibles.Remove(collectible);
		inactiveCollectibles.Add (collectible);
		collectible.transform.DOMoveZ (-16f, 0f);

		incrementScore (10);
	}
	
	// Add the set amount to the score, or override
	public void incrementScore(int delta = 10)
	{
		score += delta;
		InGamePanelController.Instance.updateScore (score);
	}

	// Invert the gravity direction
	public void invertGravity()
	{
		inverseGravity = !inverseGravity;
		Physics.gravity = Physics.gravity * -1;
	}

	public bool getGravityDirection()
	{
		return inverseGravity;
	}

	// Do game end things
	private void endGame()
	{
		gameOver = true;
		
		InGamePanelController.Instance.panel.SetActive (false);
		EndGamePanelController.Instance.panel.SetActive (true);
		EndGamePanelController.Instance.updateScore (score);
		
		audio.PlayOneShot (explosion);

		// Explode all objects
		Physics.gravity = 0 * Physics.gravity;
		
		// Set physics interactable
		GameObject.Find ("Top").rigidbody.isKinematic = false;
		GameObject.Find ("Bottom").rigidbody.isKinematic = false;
		GameObject.Find ("Left").rigidbody.isKinematic = false;
		GameObject.Find ("Right").rigidbody.isKinematic = false;
		GameObject.Find ("Back").SetActive(false); // Disable and hide the back panel
		GameObject.Find ("Player").rigidbody.isKinematic = false;
		
		foreach (GameObject obstruction in obstructions) {
			// Set physics interactable
			obstruction.rigidbody.isKinematic = false;
			
			// boom!
			randomlyExplode (obstruction);
		}
		
		// remove all the collectibles from this process
		for (int i = 0; i < activeCollectibles.Count; i++) {
			deactivateNextCollectible();
		}

		randomlyExplode (GameObject.Find ("Top"));
		randomlyExplode (GameObject.Find ("Bottom"));
		randomlyExplode (GameObject.Find ("Left"));
		randomlyExplode (GameObject.Find ("Right"));
		randomlyExplode (GameObject.Find ("Player"));
	}

	// DRY implementation of randomly setting the explode epicentre
	private void randomlyExplode(GameObject thing)
	{
		float centre = Random.Range (-2f, 2f);
		Vector3 epicentre = new Vector3 (thing.transform.position.x - centre,
		                                 thing.transform.position.y - centre,
		                                 thing.transform.position.z - centre);
		thing.rigidbody.AddExplosionForce (1000f, epicentre, 1000f);
	}
}
