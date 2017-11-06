using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SalvaDadosEntreScenes : MonoBehaviour
{
	private GameObject CantosDobrado;
	private GameObject CantosEsticado;
	private GameObject ImageTarget;
	private Camera cam;

	// Códigos para salvamento e leitura da calibragem
	public void InsereCantos(GameObject CantosDobrado, GameObject CantosEsticado){
		this.CantosDobrado = CantosDobrado;
		this.CantosEsticado = CantosEsticado;
	}

	public void InsereCamera(Camera c){
		cam = c;
	}

	public void InsereIMTarget(GameObject im){
		ImageTarget = im;
	}

	public bool salvarCalibragem(){
		if (!CantosDobrado || !CantosEsticado)
			return false;
		
		salvarXYZCantos("SEEsticado", CantosEsticado.transform.GetChild (0).transform.position);
		salvarXYZCantos("IDEsticado", CantosEsticado.transform.GetChild (1).transform.position);
		salvarXYZCantos("SEDobrado", CantosDobrado.transform.GetChild (0).transform.position);
		salvarXYZCantos("IDDobrado", CantosDobrado.transform.GetChild (1).transform.position);

		PlayerPrefs.Save ();
		print ("Configuracoes salvas!\n");
		return true;
	}

	public void salvarXYZCantos(string nome ,Vector3 position){
		PlayerPrefs.SetFloat ( nome+".x", position.x );
		PlayerPrefs.SetFloat ( nome+".y", position.y );
		PlayerPrefs.SetFloat ( nome+".z", position.z );
	}
	public Vector3 leXYZCantos(string nome){
		float x = PlayerPrefs.GetFloat ( nome+".x");
		float y = PlayerPrefs.GetFloat ( nome+".y");
		float z = PlayerPrefs.GetFloat ( nome+".z");
		return new Vector3 (x, y, z);
	}
	public bool ChecaKeyXYZ(string key){
		return (
			PlayerPrefs.HasKey ( key+".x") &&
			PlayerPrefs.HasKey ( key+".y") &&
			PlayerPrefs.HasKey ( key+".z")
		);
	}
	public bool EstaCalibrado(){
		return (
			ChecaKeyXYZ ("SEEsticado") && // PlayerPrefs.GetFloat("SEEsticado") > 0.0f &&
			ChecaKeyXYZ ("IDEsticado") && // PlayerPrefs.GetFloat("IDEsticado") > 0.0f &&
			ChecaKeyXYZ ("SEDobrado") &&  // PlayerPrefs.GetFloat("SEDobrado") > 0.0f &&
			ChecaKeyXYZ ("IDDobrado") //&&  // PlayerPrefs.GetFloat("IDDobrado") > 0.0f
		);
	}
	public float LerDimensaoMax(){
		Vector3 seEsticado = cam.WorldToScreenPoint(leXYZCantos("SEEsticado"));
		Vector3 idEsticado = cam.WorldToScreenPoint(leXYZCantos("IDEsticado"));
		seEsticado.z = 10;
		idEsticado.z = 10;

		return Vector3.Distance (seEsticado, idEsticado);
	}
	public float LerDimensaoMin(){
		Vector3 seDobrado = cam.WorldToScreenPoint(leXYZCantos("SEDobrado"));
		Vector3 idDobrado = cam.WorldToScreenPoint(leXYZCantos("IDDobrado"));
		seDobrado.z = 10;
		idDobrado.z = 10;

		return Vector3.Distance (seDobrado,idDobrado);
	}
	// FIM Códigos para salvamento e leitura da calibragem


	// ------------------------------------------------

	// Códigos para salvamento e leitura do circuito
	public void SalvarCircuito(List<int> circuito){
		// Limpando os valores de qualquer outro circuito passado
		for (int i = 0; PlayerPrefs.HasKey("CircuitoAtual_"+i); ++i)
			PlayerPrefs.DeleteKey ("CircuitoAtual_"+i);

		// Salvando os novos valores deste circuito
		for(int i=0; i < circuito.Count; ++i){			
			PlayerPrefs.SetInt ("CircuitoAtual_"+i, circuito[i]);
		}

		print ("Circuito Salvo!\n");
		PlayerPrefs.Save ();
	}
	public List<int> LerCircuito(){
		List<int> lista = new List<int> ();
		for (int i = 0; PlayerPrefs.HasKey ("CircuitoAtual_" + i); ++i)
			lista.Add (PlayerPrefs.GetInt ("CircuitoAtual_" + i));

		print ("Circuito Lido!\nCount: "+lista.Count);

		return lista;

	}
	// FIM Códigos para salvamento e leitura do circuito

	// ------------------------------------------------

	// Códigos para salvamento e leitura dos nomes de cada marcador
	public void SalvarNomesMarcadores(List<string> nomesMarcadores){
		// Limpando os valores de qualquer outro circuito passado
		for (int i = 0; PlayerPrefs.HasKey("NomeMarcadores_"+i); ++i)
			PlayerPrefs.DeleteKey ("NomeMarcadores_"+i);

		// Salvando os novos valores deste circuito
		for(int i = 0; i < nomesMarcadores.Count; ++i){
			PlayerPrefs.SetString ("NomeMarcadores_"+i, nomesMarcadores[i]);
		}

		PlayerPrefs.Save ();
		print ("Lista de Nomes dos Marcadores salva!\n");
	}
	public List<string> LerNomesMarcadores(){
		List<string> lista = new List<string> ();
		for (int i = 0; PlayerPrefs.HasKey ("NomeMarcadores_" + i); ++i)
			lista.Add (PlayerPrefs.GetString ("NomeMarcadores_" + i));
		print ("Lista de Nomes dos Marcadores obtida!\n");
		return lista;

	}
	// FIM Códigos para salvamento e leitura dos nomes de cada marcador


	// Códigos para o cálculo da pontuação para a fase
	public float LeTempoMaximoFase(string idFase){
		float tempo=0.0f;
		return tempo;
	}
	// FIM Códigos para o cálculo da pontuação para a fase


	// Códigos para abertura do programa
	public void tutorialJaVisto(bool op){
		string result;
		result = op ? "true" : "false";

		PlayerPrefs.SetString ("tutorialVisto", result);
		PlayerPrefs.Save ();
	}
	public bool tutorialJaVisto(){
		return PlayerPrefs.HasKey ("tutorialVisto") &&
		PlayerPrefs.GetString ("tutorialVisto") == "true";
	}
	// FIM Códigos para abertura do programa

	// Códigos para salvamento do nick do usuário
	public void salvaNick(string nii){
		PlayerPrefs.SetString ("nickName", nii);
		PlayerPrefs.Save ();
	}
	public string leNick(){
		if (PlayerPrefs.HasKey ("nickName")) {
			return PlayerPrefs.GetString ("nickName");
		}
		return null;
	}
	// FIM Códigos para salvamento do nick do usuário

	// Códigos para determinar se o menuPrincipal irá executar o modo tutorial
	public void setaMenuPrincipalTutorial(bool modo){
		string result;
		if (modo)
			result = "true";
		else
			result = "false";
		PlayerPrefs.SetString ("mainMenuTut", result);
		PlayerPrefs.Save ();
		print ("Salvei o tutorial do menu principal: "+result);
	}
	public bool leMenuPrincipalTutorial(){
		if (PlayerPrefs.HasKey ("mainMenuTut")) {
			if ("true" == PlayerPrefs.GetString ("mainMenuTut"))
				return true;
		}
		return false;
	}
	// FIM Códigos para determinar se o menuPrincipal irá executar o modo tutorial
}

