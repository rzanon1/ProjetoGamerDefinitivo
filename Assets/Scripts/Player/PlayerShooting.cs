using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {

    public Color[] bulletColors;
    public float bounceDuration = 10;
    public float pierceDuration = 10;
    public int damagePerShot = 20;
    public int numberOfBullets = 1;
    public float timeBetweenBullets = 0.15f;
    public float angleBetweenBullets = 10f;
    public float range = 100f;
    public LayerMask shootableMask;
    public Image bounceImage;
    public Image pierceImage;
    public GameObject bullet;
    public Transform bulletSpawnAnchor;
	public GameObject pierceTimerObj;
	public GameObject bounceTimerObj;

    // Timer para determinar quando atirar
    float timer;
	// Um ray partindo da extremidade da arma para frente
    Ray shootRay;
	// Um RaycastHit para obter informacoes sobre o que foi atingido
    RaycastHit shootHit;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
	// A proporcao do timeBetweenBullets pelo qual os efeitos serao exibidos
    float effectsDisplayTime = 0.2f;
    float bounceTimer;
    float pierceTimer;
    bool bounce;
    bool piercing;
    Color bulletColor;

    public float BounceTimer {
        get { return bounceTimer; }
        set { bounceTimer = value; }
    }

    public float PierceTimer {
        get { return pierceTimer; }
        set { pierceTimer = value; }
    }

    void Awake() {
        gunParticles = GetComponent<ParticleSystem>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponentInChildren<Light>();

        bounceTimer = bounceDuration;
        pierceTimer = pierceDuration;
    }

    void Update() {
		// Desativando os labels de tempo para bounce e pierce
		bounceTimerObj.SetActive(false);
		pierceTimerObj.SetActive(false);

        if (bounceTimer < bounceDuration) {
            bounce = true;
        }
        else {
            bounce = false;
        }

        if (pierceTimer < pierceDuration) {
            piercing = true;
        }
        else {
            piercing = false;
        }

        bulletColor = bulletColors[0];
        if (bounce) {
			// Configurando e ativando o label
			bounceTimerObj.SetActive(true);
			Text bounceTime = bounceTimerObj.GetComponent<Text> ();
			float floatVal = bounceDuration - bounceTimer;
			int val = Mathf.CeilToInt(floatVal);
			bounceTime.text = val.ToString ();

            bulletColor = bulletColors[1];
            bounceImage.color = bulletColors[1];
        }
        bounceImage.gameObject.SetActive(bounce);

        if (piercing) {
			// Configurando e ativando o label
			pierceTimerObj.SetActive(true);
			Text pierceTime = pierceTimerObj.GetComponent<Text> ();
			float floatVal = pierceDuration - pierceTimer;
			int val = Mathf.CeilToInt(floatVal);
			pierceTime.text = val.ToString ();

            bulletColor = bulletColors[2];
            pierceImage.color = bulletColors[2];
        }
        pierceImage.gameObject.SetActive(piercing);

        if (piercing & bounce) {
            bulletColor = bulletColors[3];
            bounceImage.color = bulletColors[3];
            pierceImage.color = bulletColors[3];
        }

		var main = gunParticles.main;
		main.startColor = bulletColor;
		// Por algum motivo, a cor selecionada originalmente ficou extremamente 
		// avermelhada apos mudar para renderizacao deferred e modo linear, 
		// entao fixa manualmente uma cor mais clara e amarelada para a luz 
		// caso ambos os powerups pierce e bounce estejam ativos

        gunLight.color = (piercing & bounce) ? new Color(1, 140f / 255f, 30f / 255f, 1) : bulletColor;

		// Adiciona o tempo desde a ultima chamada de Update ao timer
        bounceTimer += Time.deltaTime;
        pierceTimer += Time.deltaTime;
        timer += Time.deltaTime;

        // Se o botao de Fire1 esta sendo pressionado ele atira
        if (Input.GetButton("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0) {
            // Atira
            Shoot();
        }

		// Se o timer excedeu a proporcao de timeBetweenBullets em que os efeitos devem ser exibidos...
        if (timer >= timeBetweenBullets * effectsDisplayTime) {
            // ... desabilita os efeitos
            DisableEffects();
        }
    }

    public void DisableEffects() {
		// Desativa o line renderer e a luz.
        gunLight.enabled = false;
    }

    void Shoot() {
        timer = 0f;

        gunAudio.pitch = Random.Range(1.2f, 1.3f);
        if (bounce) {
            gunAudio.pitch = Random.Range(1.1f, 1.2f);
        }
        if (piercing) {
            gunAudio.pitch = Random.Range(1.0f, 1.1f);
        }
        if (piercing & bounce) {
            gunAudio.pitch = Random.Range(0.9f, 1.0f);
        }
        gunAudio.Play();

        gunLight.intensity = 2 + (0.25f * (numberOfBullets - 1));
        gunLight.enabled = true;

        gunParticles.Stop();
		var main = gunParticles.main;
        main.startSize = 1 + (0.1f * (numberOfBullets - 1));
        gunParticles.Play();

		// Configura o shootRay para que ele comece na extremidade da arma e aponte para frente a partir do cano
        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        for (int i = 0; i < numberOfBullets; i++) {
            float angle = i * angleBetweenBullets - ((angleBetweenBullets / 2) * (numberOfBullets - 1));
            Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.up);
            GameObject instantiatedBullet = Instantiate(bullet, bulletSpawnAnchor.transform.position, rot) as GameObject;
            instantiatedBullet.GetComponent<Bullet>().piercing = piercing;
            instantiatedBullet.GetComponent<Bullet>().bounce = bounce;
            instantiatedBullet.GetComponent<Bullet>().bulletColor = bulletColor;
        }
    }
}
