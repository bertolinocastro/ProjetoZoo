using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menuPrincipalScript : MonoBehaviour {

	private SalvaDadosEntreScenes salvador;
	public GameObject bastao;
	public GameObject placaUser;
	public Button botaoJogar;
	public Button canvasBtn;

	public GameObject placa;
	public GameObject panda;

	public Text textCanvas;
	public TextAsset falasTutorial;
	private string[] msg;
	private int idFala = 0;

	private bool passavel = true;
	private bool pular = false;

	private int textLength = 54;

	private float delayEntreLetras = 0.03f;

	void Awake(){
		salvador = gameObject.AddComponent<SalvaDadosEntreScenes> ();
		checaNick ();
	}

	// Use this for initialization
	void Start () {
		checaTutorial ();
		checaCalibrado ();
	}
	
	// Update is called once per frame

	void Update () {
		
	}

	private void checaUsuario(){
		if (msg [idFala].Contains ("<usuario>")) {
			msg [idFala] = msg [idFala].Replace ("<usuario>",
				salvador.leNick()
			);
		}
	}

	private void checaPiscar(){
		if (msg [idFala].Contains ("<piscar>")) {
			msg [idFala] = msg [idFala].Replace ("<piscar>","");
			StartCoroutine ("piscaBotao");
		}
	}

	private void desativaTodosBotoes(){
		foreach (Transform filho in bastao.transform) {
			filho.gameObject.GetComponent<Button> ().interactable = false;
		}
		foreach (Transform filho in placaUser.transform) {
			filho.gameObject.GetComponent<Button> ().interactable = false;
		}
	}

	private void checaTutorial(){
		if(salvador.leMenuPrincipalTutorial()){
			msg = falasTutorial.text.Split (new string[] {"<fala>"}, System.StringSplitOptions.None);
			desativaTodosBotoes ();
			placa.SetActive (true);
			panda.SetActive (true);
			passaTexto ();
		} else {
			Destroy (canvasBtn.GetComponent<Button>());
		}
	}

	private IEnumerator piscaBotao(){
		bool state = false;
		while (true){
			botaoJogar.interactable = state;
			state = !state;
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}
		
	public void passaTexto(){
		if (idFala < msg.Length) {
			if (passavel) {
				checaUsuario ();
				checaPiscar ();
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

		string frase = msg [idFala];

		int end = 0, i = 0;
		do{
			textCanvas.text = "";
			if (textLength < frase.Length-end) {
				end = frase.LastIndexOf (' ', end+textLength-3);
			}else{
				end = frase.Length;
			}
			for (; i < end; ++i) {
				if(frase[i] == '\n'){
					continue;
				}
				textCanvas.text += frase [i];
				if(!pular)
					yield return new WaitForSeconds (delayEntreLetras);
			}
			pular = false;
			if (end < frase.Length) {
				textCanvas.text += "...";
				while(!pular)
					yield return new WaitForSeconds (delayEntreLetras);
				pular = false;
			}
		}while(i < frase.Length);

		idFala++;
		pular = false;
		passavel = true;
		yield break;
	}

	private void checaNick(){
		if(salvador.leNick() == null){
			SceneManager.LoadScene ("tutorial");
		}
	}

	private void checaCalibrado(){
		if (!salvador.EstaCalibrado ())
			botaoJogar.interactable = false;
	}

	public void Jogar()
	{
		SceneManager.LoadSceneAsync("Circuito");
	}

	public void Calibrar()
	{
		SceneManager.LoadSceneAsync("calibragem");
	}
	public void Configuracao()
	{
		SceneManager.LoadSceneAsync("Configuracao");
	}

	public void Loja()
	{
		SceneManager.LoadSceneAsync("loja");
	}

	public void Ranking()
	{
		SceneManager.LoadSceneAsync("Ranking");
	}
	public void Historico()
	{
		SceneManager.LoadSceneAsync("Historico");
	}

}
