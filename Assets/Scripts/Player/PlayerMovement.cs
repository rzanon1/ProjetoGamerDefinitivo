using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public float speed = 6f;            

	Vector3 movement;
	Animator anim;    
	Rigidbody playerRigidbody;    
	int floorMask;    
	float camRayLength = 100f;          

	void Awake() {
	    floorMask = LayerMask.GetMask("Floor");

	    anim = GetComponent<Animator>();
	    playerRigidbody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate() {
	    // Armazena os eixos de input
	    float h = Input.GetAxisRaw("Horizontal");
	    float v = Input.GetAxisRaw("Vertical");

	    // Move o player pela cena
	    Move (h, v);

	    // Vira o player para onde o cursor esta
	    Turning ();

	    // Anima o modelo do player
	    Animating (h, v);
	}
	
	void Move(float h, float v) {
	    // Seta o vetor de movimento baseado no input do eixo
	    movement.Set (h, 0f, v);
	    
	    // Normaliza o vetor de movimento e faz ele ser proporcional a velocidade por segundo
		movement = movement.normalized * speed * Time.deltaTime;

	    // Movimenta o jogador para a posicao atual + o movimento
	    playerRigidbody.MovePosition(transform.position + movement);
	}
	
	void Turning() {
	    // Cria um ray a partir do cursor do mouse na tela em direcao a camera
	    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

	    // Cria uma variavel de RaycastHit para armazenar informacao do que foi atingido pelo ray
	    RaycastHit floorHit;

	    // Executa o raycast e se ele acerta algo na layer do chao...
	    if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {

			// Cria um vetor que vai do jogador ate o ponto no chao onde o raycast disparado a partir do mouse colidiu
	        Vector3 playerToMouse = floorHit.point - transform.position;

			// Garante que o vetor esteja completamente no plano do chao
	        playerToMouse.y = 0f;

	        // Cria um quaternion (rotacao) baseado em olhar o vetor do player ate o mouse
	        Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

	        // seta a rotacao do player para a nova rotacao
	        playerRigidbody.MoveRotation(newRotation);
	    }
	}
	
	void Animating(float h, float v) {
		// Cria um boolean que e verdadeiro se qualquer um dos eixos de entrada for diferente de zero.
	    bool walking = h != 0f || v != 0f;

	    // Fala para o animatior se o player esta se mexendo ou nao
	    anim.SetBool("IsWalking", walking);
	}
}