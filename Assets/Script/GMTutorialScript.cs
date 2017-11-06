using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GMTutorialScript : MonoBehaviour {

	public GameObject skipa;

	public GameObject modalNick;
	public Text nick;
	private SalvaDadosEntreScenes salvador;

	public List<Sprite> imagens;
	public int idImagens = 0;
	private int qtdImagensPAnimacao = 0;
	private float delayPAnimacao = 0.5f;
	private int idAnim = 0;

	public List<string> imagensName;

	public Image background;

	public Text textoFalas;
	public TextAsset textoFalasAsset;

	private const int textLength = 68;
	private const float delayEntreLetras = 0.03f;
	private bool passavel = true;
	private bool pular = false;

	private string[] msg;
	private string[] msgAct;

	private int cenaAct = 0;
	private int subCenaAct = 0;

	private float start;

	// Use this for initialization
	void Start () {
		salvador = gameObject.AddComponent<SalvaDadosEntreScenes> ();
		filenamesInit ();
		backgroudInit ();
		mensagensInit ();
		ativaTodosOsTutoriais ();
	
		start = Time.time;
	}

	private void ativaTodosOsTutoriais(){
		salvador.setaMenuPrincipalTutorial (true);
	}

	private void backgroudInit(){
		// Carregamento das imagens do tutorial a serem usadas como background do Canvas
		imagens = new List<Sprite> (Resources.LoadAll<Sprite> ("imagensTutorial/"));
		proximaCena ();
	}

	private void filenamesInit(){
		int i = 0;
		imagensName = new List<string> ();
		foreach(Object fi in Resources.LoadAll<Object> ("imagensTutorial/")){
			imagensName.Add (fi.name);
		}
		imagensName = imagensName.Distinct ().ToList ();
	}

	private void mensagensInit(){
		msg = textoFalasAsset.text.Split(new string[] {"<cena>"}, System.StringSplitOptions.None);
		msgAct = msg [cenaAct++].Split(new string[] {"<fala>"}, System.StringSplitOptions.None);
		textoFalas.text = msgAct [subCenaAct++];
	}

	// Update is called once per frame
	void Update () {
	}

	public void passaTexto(){
		if (subCenaAct < msgAct.Length) {
			if (passavel) {
				//pular = false;
				checaInput ();
				StartCoroutine ("printaLetras");
				//pular = false;
			} else {
				pular = true;
			}
		} else {
			if (cenaAct < msg.Length) {
				subCenaAct = 0;
				checaUsuario ();
				msgAct = msg [cenaAct++].Split (new string[] {"<fala>"}, System.StringSplitOptions.None);
				//textoFalas.text = msgAct [subCenaAct++];
				if (passavel) {
					StartCoroutine ("printaLetras");
				}
				if (idImagens < imagens.Count) {
					proximaCena ();
				} else {
					print ("Faltam imagens para as cenas!");
				}
			} else {
				finalizaTutorial ();
				print ("fim do tutorial");
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
				if(frase[i] == '\n'){
					continue;
				}
				textoFalas.text += frase [i];
				if(!pular)
					yield return new WaitForSeconds (delayEntreLetras);
			}
			pular = false;
			if (end < frase.Length) {
				textoFalas.text += "...";
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

	public void skipaTutorial() {
		int ccena, image, novaCat; char categoria; bool achou = false;
		for(ccena = cenaAct; ccena < msg.Length && !(achou=msg[ccena].Contains("<input>")); ++ccena);
		if (!achou)
			return;

		for (image = idImagens, novaCat = 0,
			categoria = imagensName [image] [0];
			image < imagensName.Count && novaCat < (ccena-cenaAct); ++image) {
			if(categoria != imagensName[image][0]){
				categoria = imagensName [image] [0];
				novaCat++;
			}
		};

		cenaAct = ccena;
		idImagens = image;
		subCenaAct = 99;
		desativaSkip ();
		passaTexto ();
		print ("Skipei o Tutorial");
	}

	private void finalizaTutorial(){
		// Carregar scene de início de usabilidade
		SceneManager.LoadSceneAsync("usabilidade");
	}

	private void proximaCena() {
		StopCoroutine ("animaImagem");
		idAnim = idImagens;

		char cenaGrupo = imagensName [idImagens] [0];
		qtdImagensPAnimacao = 1;
		for (int i = idAnim + 1; i < imagensName.Count && cenaGrupo == imagensName [i] [0]; ++i) {
			++qtdImagensPAnimacao;
		}
		
		StartCoroutine ("animaImagem");
		
		idImagens+=qtdImagensPAnimacao;
	}

	private IEnumerator animaImagem(){
		for (;;) {
			for (int i = 0; i < qtdImagensPAnimacao; ++i) {
				background.sprite = imagens [idAnim+i];
				yield return new WaitForSeconds (delayPAnimacao);
			}
		}
		yield break;
	}

	public void salvaNickName(){
		if (string.IsNullOrEmpty (nick.text))
			return;
		salvador.salvaNick (nick.text);
		modalNick.SetActive (false);
		passaTexto ();
	}

	private void checaInput(){
		if (msgAct [subCenaAct].Contains ("<input>")) {
			msgAct [subCenaAct] = msgAct [subCenaAct].Replace("<input>", "");
			modalNick.SetActive (true);
			desativaSkip ();
		}
	}

	private void checaUsuario(){
		if (msg [cenaAct].Contains ("<usuario>")) {
			msg [cenaAct] = msg [cenaAct].Replace ("<usuario>",
				salvador.leNick()
			);
		}
	}

	private void desativaSkip(){
		skipa.SetActive(false);
	}
}
