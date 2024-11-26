using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HellephantBullet : MonoBehaviour {
	
	public float speed = 600.0f;
	public float life = 3;
	public ParticleSystem normalTrailParticles;
	public ParticleSystem ImpactParticles;
	public int damage = 20;
	public Color bulletColor;
	public AudioClip hitSound;

	Vector3 velocity;
    Vector3 force;
	Vector3 newPos;
	Vector3 oldPos;
	Vector3 direction;
	bool hasHit = false;
	RaycastHit lastHit;
	AudioSource bulletAudio;  
	float timer;

	void Awake() {
		bulletAudio = GetComponent<AudioSource> ();
	}

	void Start() {
		newPos = transform.position;
		oldPos = newPos;

		var main = normalTrailParticles.main;
		main.startColor = bulletColor;
		main = ImpactParticles.main;
		main.startColor = bulletColor;
		normalTrailParticles.gameObject.SetActive(true);
	}

	void Update() {
		if (hasHit) {
			return;
		}


		timer += Time.deltaTime;


		if (timer >= life) {
			Dissipate();
		}

        velocity = transform.forward;

		velocity = velocity.normalized * speed;

	
		newPos += velocity * Time.deltaTime;
	

		direction = newPos - oldPos;
		float distance = direction.magnitude;

		if (distance > 0) {
            RaycastHit[] hits = Physics.RaycastAll(oldPos, direction, distance);

		  
		    for (int i = 0; i < hits.Length; i++) {
		        RaycastHit hit = hits[i];

				if (ShouldIgnoreHit(hit)) {
					continue;
				}


				OnHit(hit);

				lastHit = hit;

				if (hasHit) {
					newPos = hit.point;
					break;
				}
		    }
		}

		oldPos = transform.position;
		transform.position = newPos;
	}


	bool ShouldIgnoreHit (RaycastHit hit) {
		if (lastHit.point == hit.point || lastHit.collider == hit.collider || hit.collider.tag == "Enemy")
			return true;
		
		return false;
	}


	void OnHit(RaycastHit hit) {
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

        if (hit.transform.tag == "Environment") {
			newPos = hit.point;
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();
			hasHit = true;
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(0.6f, 0.8f);
			bulletAudio.Play();
			DelayedDestroy();
        }

        if (hit.transform.tag == "Player") {
			ImpactParticles.transform.position = hit.point;
			ImpactParticles.transform.rotation = rotation;
			ImpactParticles.Play();

			// Tenta encontrar um script EnemyHealth no gameobject atingido
			PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
			
			// Se o componente EnemyHealth existir...
			if (playerHealth != null) {
				// ... ele toma dano
				playerHealth.TakeDamage(damage);
			}
    		hasHit = true;
			DelayedDestroy();
			bulletAudio.clip = hitSound;
			bulletAudio.volume = 0.5f;
			bulletAudio.pitch = Random.Range(0.6f, 0.8f);
			bulletAudio.Play();
        }
	}


	void Dissipate() {
		var normalTrailParticlesEmissions = normalTrailParticles.emission.enabled;
		normalTrailParticlesEmissions = false;
		normalTrailParticles.transform.parent = null;
		var main = normalTrailParticles.main;
		Destroy(normalTrailParticles.gameObject, main.duration);
		Destroy(gameObject);
	}

	void DelayedDestroy() {
		normalTrailParticles.gameObject.SetActive(false);
		Destroy(gameObject, hitSound.length);
	}
}