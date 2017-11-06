using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frisbeScript : MonoBehaviour {

	public Camera cam;

	public GameObject kitten;

	public bool isTrowed = false;

	private GameObject frisbeGO;

	private Vector3 posInicialFrisbe;

	// Use this for initialization
	void Start () {
		frisbeGO = gameObject;

		GetComponent<Rigidbody> ().useGravity = false;

		posInicialFrisbe = new Vector3 (0, 0, -2.0f);
		frisbeGO.transform.position = posInicialFrisbe;
	}
	
	// Update is called once per frame
	void Update () {
		if (isTrowed)
			direcionarFrisbe ();
	}

	public void Posicionar(float porcentagem){
		// Vector3(0,-1,[2,4])
		float profundidade = (4-2)*porcentagem+2;
		transform.position = new Vector3(0,-1,profundidade);
	}

	public void Arremessar(){
		if (isTrowed)
			return;
		isTrowed = true;
		float tempoDeColisao = 1.4f;

		float distance = Vector3.Distance (transform.position, kitten.transform.position);
		float velocity = distance / tempoDeColisao;

		this.GetComponent<Rigidbody> ().AddForce (Vector3.forward * velocity, ForceMode.VelocityChange);

		StartCoroutine (AnimaComDelay(tempoDeColisao));
		Destroy (frisbeGO.gameObject, tempoDeColisao+0.1f);
	}

	private IEnumerator AnimaComDelay(float temp){
		yield return new WaitForSeconds (temp);
		print ("O Script 'frisbeScript' está com a animação do objeto filho do Image Target DESATIVADA!");
		//kitten.GetComponent<KittenScript> ().setaAnimation ("Meow");
	}

	public void direcionarFrisbe(){
		transform.LookAt(kitten.transform.position);
	}
}
