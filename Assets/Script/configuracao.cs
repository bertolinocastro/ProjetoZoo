using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class configuracao : MonoBehaviour {
	private SalvaDadosEntreScenes salvador;

	// Use this for initialization
	void Start () {
		salvador = gameObject.AddComponent<SalvaDadosEntreScenes> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AtivarTutorial(){
		salvador.setaMenuPrincipalTutorial (true);
		salvador.tutorialJaVisto (false);
		SceneManager.LoadSceneAsync ("start");
	}

	public void Voltar()
	{
		SceneManager.LoadSceneAsync("menuInicial");
	}

}
