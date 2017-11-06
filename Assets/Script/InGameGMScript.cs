using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameGMScript : MonoBehaviour
{
	public Camera cam;
	public GameObject CanvasInGame;
	public GameObject CanvasFimGame;
	public Text balaoMascote;
	public Text balaoFimPontuacao;
	public Text balaoFimElogio;
	public GameObject[] estrelasGold;
	public GameObject[] estrelasGray;
	//public GameObject mascoteGuiaGO;

	public UnityEngine.UI.Image mascoteIMG;
	public Sprite mascoteFrontal;
	public Sprite mascotePerfilBaixo;
	public Sprite mascotePerfilEsticado;

	private MessengerScript messenger;
	private ListaImTargetsScript listaIMTargetScript;
	private GerenciadorCircuitoScript gerenciadorCircuito;
	private SalvaDadosEntreScenes salvador;
	private MascoteGuiaScript mascoteGuia;
	private ReadTarget imDetector;
	private IdentificadorJeb identificaJeb;

	// Bertolino: Série de objetos a serem retirados e convertê-los em List
	public GameObject kitten;
	public GameObject frisbe;
	private GameObject frisbeGO;
	private frisbeScript frisbeScrpt;

	private bool exitBtn = false;
	private bool circuitoImpossivel = false;

	private bool bracoDobrado = false;
	private bool bracoEsticado = false;

	private int ultimoMarcador = -1;

	private float tempoParaMarcadores;
	private float tempoDeJogoIni;

	private bool estaFinalizado = false;

	void Awake(){
		listaIMTargetScript = gameObject.GetComponent<ListaImTargetsScript> (); listaIMTargetScript.Inicializar ();
		messenger = gameObject.AddComponent<MessengerScript> ();
		gerenciadorCircuito = gameObject.AddComponent<GerenciadorCircuitoScript> ();
		salvador = gameObject.AddComponent<SalvaDadosEntreScenes> ();
		mascoteGuia = gameObject.AddComponent<MascoteGuiaScript> ();
		identificaJeb = gameObject.AddComponent<IdentificadorJeb> ();
	}

	// Use this for initialization
	void Start () {
		salvador.setaMenuPrincipalTutorial (false);
		salvador.tutorialJaVisto (true);

		// turtle -> lion -> cow -> turtle

		// TODO: Deletar quando implementar o substituto -------
		List<int> x = new List<int>(); x.Add(0); x.Add(2); x.Add(4); x.Add(0);
		salvador.SalvarCircuito(x);

		List<string> y = new List<string> (); y.Add ("Tartaruga"); y.Add ("Rinoceronte"); y.Add ("Leão"); y.Add ("Hipopotamo"); y.Add ("Vaca"); y.Add ("Mula");
		salvador.SalvarNomesMarcadores (y);
		// FIM: Deletar quando implementar o substituto ------

		// Checando se existe o circuito
		if (!listaIMTargetScript.ChecaSeExisteOCircuito (salvador.LerCircuito ()))
			CircuitoInexistente ();
		// FIM: Checando se existe o circuito

		messenger.InsereRect (new Rect(0, 0, Screen.width, Screen.height/4.0f));
		gerenciadorCircuito.InsereCircuito (salvador.LerCircuito());
		salvador.InsereCamera (cam);

		mascoteGuia.InsereBalaoTexto (balaoMascote);
		mascoteGuia.InsereBalaoFim (balaoFimPontuacao,balaoFimElogio);
		mascoteGuia.InsereEstrelas (estrelasGold, estrelasGray);
		mascoteGuia.InsereNomesMarcadores (salvador.LerNomesMarcadores());

		imDetector = listaIMTargetScript.LerReadTarget (0);

		identificaJeb.InsereImTarget (listaIMTargetScript.Get(0).gameObject);
		identificaJeb.InsereCamera (cam);
		identificaJeb.SetaCantos (
			salvador.leXYZCantos("SEDobrado"),
			salvador.leXYZCantos("IDDobrado"),
			salvador.leXYZCantos("SEEsticado"),
			salvador.leXYZCantos("IDEsticado")
		);

		mascoteGuia.Ativador (true);

		//print (Time.time);

		CanvasInGame.SetActive (true); CanvasFimGame.SetActive (false);
		//DEBUGAPONTOSDECALIBRAGEM ();
		tempoParaMarcadores = Time.time + 2.0f;
		tempoDeJogoIni = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		//messenger.messengerTxt = "<color=white>"+Time.time.ToString()+ "\nTempoParaMarcadores: "+tempoParaMarcadores.ToString()+" if- "+teste.ToString()+"</color>";

		if (circuitoImpossivel)
			return;

		if (frisbeGO)
			frisbeScrpt.gameObject.SetActive(imDetector.isFound);
		
		if (tempoParaMarcadores < Time.time) {
			if (gerenciadorCircuito.TemProximo ()) {
				CanvasInGame.SetActive (true); CanvasFimGame.SetActive (false);
				if (ultimoMarcador != gerenciadorCircuito.MarcadorAtual ()) {
					int marcadorAct = gerenciadorCircuito.MarcadorAtual ();
					Transform imTarget = listaIMTargetScript.Get (marcadorAct);
					//print ("Estamos no marcador no: " + marcadorAct);
					listaIMTargetScript.AtivaTarget (marcadorAct);
					mascoteGuia.ApontarMarcador (marcadorAct);
					imDetector = listaIMTargetScript.LerReadTarget (marcadorAct);
					identificaJeb.InsereImTarget (imTarget.gameObject);
					criaFrisbe ();
					atualizaAlvo (imTarget.GetChild(imTarget.childCount-1).gameObject);
					ultimoMarcador = marcadorAct;
				}
				if (imDetector.isFound) {
					if (!frisbeScrpt.isTrowed)
						frisbeScrpt.Posicionar (identificaJeb.PorcentagemEsticado());

					if (identificaJeb.DobrouBraco ()) {
						if (identificaJeb.EsticouBraco ()) {
							if (identificaJeb.DobrouNovamenteBraco ()) {
								frisbeScrpt.Arremessar ();
								gerenciadorCircuito.AvancarPasso ();
								mascoteFrente ();
								if (!gerenciadorCircuito.TemProximo ())
									mascoteGuia.FinalizaPassos ();
								tempoParaMarcadores = Time.time + 2.0f;
							} else {
								mascoteDobrado ();
								mascoteGuia.DobrarBracos ();
							}
						} else {
							mascoteEsticado ();
							mascoteGuia.EsticarBracos ();
						}
					} else {
						mascoteDobrado ();
						mascoteGuia.DobrarBracos ();
					}
				} else {
					mascoteGuia.ApontarMarcador (gerenciadorCircuito.MarcadorAtual ());
				}
			} else {
				if(!estaFinalizado){
					CanvasInGame.SetActive (false); CanvasFimGame.SetActive (true);
					//mascoteGuia.FinalizarFase (tempoDeJogoIni, salvador.LeTempoMaximoFase("####FASEATUAL####"));
					mascoteGuia.FinalizarFase (tempoDeJogoIni, 85.0f);
					estaFinalizado = true;
				}
			}
		} else {
			if (gerenciadorCircuito.TemProximo ()) {
				mascoteGuia.AvisaEstagio (gerenciadorCircuito.PassoAtual ()+1, gerenciadorCircuito.PassoMaximo ());
			}
		}
		
	}

	void criaFrisbe(){
		frisbeGO = Instantiate (frisbe,Vector3.zero, Quaternion.identity) as GameObject;
		frisbeScrpt = frisbeGO.GetComponent<frisbeScript> ();
		frisbeScrpt.cam = cam;
	}

	void atualizaAlvo(GameObject alvo){
		frisbeScrpt.kitten = alvo;
	}

	public void voltarMenuPrincipal(){
		SceneManager.LoadSceneAsync ("Circuito");
	}

	public void reiniciarFase(){
		SceneManager.LoadSceneAsync ("ingame");
	}

	public void mascoteFrente(){
		mascoteIMG.sprite = mascoteFrontal;
	}

	public void mascoteDobrado(){
		mascoteIMG.sprite = mascotePerfilBaixo;
	}

	public void mascoteEsticado(){
		mascoteIMG.sprite = mascotePerfilEsticado;
	}

	private void CircuitoInexistente(){
		messenger.messengerTxt = "<color=white>O circuito inserido é impossível!\nSaia e reinsira o circuito!!!</color>";
		circuitoImpossivel = true;
		//print ("Circuito impossivel!");
	}

	private void DEBUGAPONTOSDECALIBRAGEM(){
		GameObject go1 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		GameObject go2 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		GameObject go3 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		GameObject go4 = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		Vector3 sss = new Vector3 (0.3f,0.3f,0.3f);

		go1.transform.position = setaNoScreenSpace(salvador.leXYZCantos("SEEsticado"));
		go1.transform.localScale = sss;

		go2.transform.position = setaNoScreenSpace(salvador.leXYZCantos("IDEsticado"));
		go2.transform.localScale = sss;

		go3.transform.position = setaNoScreenSpace(salvador.leXYZCantos("SEDobrado"));
		go3.transform.localScale = sss;

		go4.transform.position = setaNoScreenSpace(salvador.leXYZCantos("IDDobrado"));
		go4.transform.localScale = sss;



		identificaJeb.INSERExyzCANTOS (
			go3.transform.position,
			go4.transform.position,
			go1.transform.position,
			go2.transform.position
		);


	}

	private Vector3 setaNoScreenSpace(Vector3 i){
		float myX = i.x;
		float myY = i.y;
		float myZ = i.z;
		Vector3 vet = cam.WorldToScreenPoint (new Vector3 (myX, myY, myZ));
		vet.z = 10;
		vet = cam.ScreenToWorldPoint (vet);
		return vet;
	}
}

