using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UsabilidadeGameManager : MonoBehaviour {

	public Sprite perfilPe, perfilDob, perfilEst, pUsar;
	public GameObject modalInstruir, modalFinalizar;
	public Image instrutor;
	private float delayEntreInstrutor = 0.8f;

	private const int textLength = 68;
	private const float delayEntreLetras = 0.03f;
	private bool passavel = true;
	private bool pular = false;

	private string[] msgAct;

	private int subCenaAct = 0;
	public Camera cam;

	private SalvaDadosEntreScenes salvador;
	public GameObject ImageTarget;
	public GameObject CantosDobrado;
	public GameObject CantosEsticado;

	public ReadTarget imDetector;

	public Text textoFalas;
	public TextAsset textoFalasAsset;

	private bool salvo = false;

	private bool definido1 = false;
	private bool definido2 = false;

	private float temp = -10.0f;
	private float recemCalibrado = 0.0f;

	private bool ePraSalvar;

	private string ultMsg = "";

	private bool pausaTexto = false;
	private bool temCalibrar = false;

	// Textos de output
	const string texto0 = "Aponte sua câmera para o marcador.";
	// Fim Textos

	// Use this for initialization
	void Start () {
		// Criando componente para salvar os dados entre as scenes
		salvador = gameObject.AddComponent<SalvaDadosEntreScenes>();
		salvador.InsereCantos (CantosDobrado, CantosEsticado);
		mensagensInit ();
	}

	// Update is called once per frame
	void Update () {
		//if (!(definido1 && definido2)) {
			if (imDetector.isFound) {
				pausaTexto = false;
				textoFalas.text = ultMsg;
			} else {
				pausaTexto = true;
				textoFalas.text = texto0;
			}
		//} else {
			//if (Time.time < recemCalibrado + 2.0f)
			//	null;//textoFalas.text = corTagIni+texto3+corTagFim;
			//else
			//	textoFalas.text = "";
		//}//

	}

	/*
		A inserir no local correto depois
		if (!salvo && EstaCalibrado ()) {
			salvaDados.salvarCalibragem ();
			salvo = true;
		}
	*/

	private void mensagensInit(){
		msgAct = textoFalasAsset.text.Split(new string[] {"<fala>"}, System.StringSplitOptions.None);
		passaTexto ();
	}

	void calibraDobrado(){
		CantosDobrado.transform.GetChild (0).transform.position = ImageTarget.transform.GetChild (0).transform.position;
		CantosDobrado.transform.GetChild (1).transform.position = ImageTarget.transform.GetChild (1).transform.position;

		CantosDobrado.GetComponent<SpawnaCantos> ().setaCantosPosicoes ();

		definido1 = true;
		temCalibrar = false;
		print ("entrei calibrar dobrado");
		passaTexto ();
	}

	void calibraEsticado(){
		CantosEsticado.GetComponent<SpawnaCantos> ().setaSEeID (
			ImageTarget.transform.GetChild (0).transform.position,
			ImageTarget.transform.GetChild (1).transform.position);
		CantosEsticado.GetComponent<SpawnaCantos> ().setaCantosPosicoes ();

		recemCalibrado = Time.time;
		definido2 = true;
		passaTexto ();
	}
	/*
	void calibraMargens(){
		if (!definido1) {
			textoFalas.text = corTagIni+texto1+corTagFim;

			if (calibrarBtn) {

			}

		}

		if (definido1 && !definido2) {
			//print (texto2);
			textoFalas.text = corTagIni+texto2+corTagFim;

			if (calibrarBtn && Time.time > temp + 2.0f) {
		
			}
		}


	}*/

	public void resetarCalibracao(){
		SceneManager.LoadScene("usabilidade");
	}

	public void toqueNaTela(){
		if (!(definido1 && definido2))
			checaCalibrar ();
		if (!definido1 && temCalibrar)
			calibraDobrado ();
		else if (!definido2 && temCalibrar)
			calibraEsticado ();
		else
			passaTexto ();
	}

	public void passaTexto(){
		if (subCenaAct < msgAct.Length) {
			if (passavel) {
				//checaInput ();
				checaUsuario();
				checaDobrarEst ();
				checaFinalizacao ();
				StartCoroutine ("printaLetras");
			} else {
				pular = true;
			}
		}
	}

	private IEnumerator printaLetras() {
		passavel = false;
		pular = false;
		//textoFalas.text = "";

		string frase = msgAct [subCenaAct];

		int end = 0, i = 0;
		do{
			textoFalas.text = "";
			if (textLength < frase.Length-end) {
				end = frase.LastIndexOf (' ', end+textLength-3);
			}else{
				end = frase.Length;
			}
			for (; i < end; ++i) {
				while(pausaTexto)// para "pausar" a coroutine
					yield return null;
				
				if(frase[i] == '\n'){
					continue;
				}
				textoFalas.text += frase [i];
				ultMsg = textoFalas.text;

				if(!pular)
					yield return new WaitForSeconds (delayEntreLetras);
			}
			pular = false;
			if (end < frase.Length) {
				textoFalas.text += "...";
				ultMsg = textoFalas.text;

				while(!pular)
					yield return new WaitForSeconds (delayEntreLetras);
				pular = false;
			}
		}while(i < frase.Length);

		subCenaAct++;
		pular = false;
		passavel = true;
		yield break;
	}

	private void checaUsuario(){
		if (msgAct [subCenaAct].Contains ("<usuario>")) {
			msgAct [subCenaAct] = msgAct [subCenaAct].Replace ("<usuario>",
				salvador.leNick()
			);
		}
	}

	private void checaCalibrar(){
		if (msgAct [subCenaAct].Contains ("<calibrar>")) {
			//msgAct [subCenaAct] = msgAct [subCenaAct].Replace ("<calibrar>", "");
			temCalibrar = true;
			subCenaAct++;
		} else
			temCalibrar = false;
	}

	private void checaFinalizacao(){
		if (msgAct [subCenaAct].Contains ("<finalizacao>")) {
			msgAct [subCenaAct] = msgAct [subCenaAct].Replace ("<finalizacao>", "");
			modalFinalizar.SetActive (true);
		}
	}

	private void checaDobrarEst(){
		if (msgAct [subCenaAct].Contains ("<dobrar>")) {
			msgAct [subCenaAct] = msgAct [subCenaAct].Replace ("<dobrar>", "");
			pUsar = perfilDob;
			modalInstruir.SetActive (true);
			StartCoroutine ("animaInstrutor");

		} else if (msgAct [subCenaAct].Contains ("<esticar>")) {
			msgAct [subCenaAct] = msgAct [subCenaAct].Replace ("<esticar>", "");
			pUsar = perfilEst;
			modalInstruir.SetActive (true);
			StartCoroutine ("animaInstrutor");

		} else {
			modalInstruir.SetActive (false);
			StopCoroutine ("animaInstrutor");
		}
	}
		
	private IEnumerator animaInstrutor(){
		bool a = false;
		for (;;) {
			instrutor.sprite = a ?
				perfilPe : pUsar;
			a = !a;
			yield return new WaitForSeconds (delayEntreInstrutor);
		}
		yield break;
	}


	public void finalizaTutorial(){
		// Carregar scene de início de usabilidade
		SceneManager.LoadSceneAsync ("menuInicial");
	}
}
