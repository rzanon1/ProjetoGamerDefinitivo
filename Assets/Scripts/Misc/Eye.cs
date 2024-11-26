using UnityEngine;
using System.Collections;

public class Eye : MonoBehaviour {

	// Referencia para o sistema de particulas anexado para que possa ser desativado
	public ParticleSystem deathParticles;  

	// O valor de cutoff para o shader de dissolucao. Isso e alterado para dissolver
	// os olhos quando o inimigo esta queimando
	float cutoffValue = 0f;
	// A bool we set to true when we start destroying the game object.
	bool triggered = false;

	void Update () {
		// Atualiza o valor de cutoff do material para que ele se dissolva gradualmente ao longo do tempo
		cutoffValue = Mathf.Lerp(cutoffValue, 1f, 0.8f * Time.deltaTime);
		GetComponent<Renderer>().materials[0].SetFloat("_Cutoff", cutoffValue);

		// Perto do final da dissolucao, comecamos a destruir o game object
		if (cutoffValue >= 0.8f && !triggered) {
			deathParticles.Stop();
			Destroy(gameObject, 1.5f);
			triggered = true;
		}
	}
}
