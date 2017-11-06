using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HistoricoScript : MonoBehaviour {

    //private string firstJogada = PlayerPrefs.HasKey("firstJogada") ? PlayerPrefs.GetString("firstJogada"):null;
    string firstJogada = "30/09/2017";
    public Text qtAtivdade;
    public Text meAtivdade;

    // Use this for initialization
    void Start () {
        //ResgataPrefs("score");
        //CriarPrefs("score", "1000");
        //HistoricoMedia("teste");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //VARIAVEL TIPO SERIA A PONTUACAO,DATA DA ATIVIDADE FEITA,HORA DA ATIVIDADE FEITA
    public void CriarPrefs(string tipo,string valor)
    {
        string concat,juncao;
        var dataAtual = DateTime.Now;
        //PRA PEGAR SO A DATA DO DIA  E NÃO O HORARIO COMO NORMALMENTE VEM
        //concat = tipo + dataAtual.ToString("dd/MM/yyyy");

        //SO PARA USAR NO TESTE
        concat = tipo;


        string getDados = PlayerPrefs.GetString(concat);

        if (!(PlayerPrefs.HasKey(getDados)))
        {
            //CRIA O SEPARADOR PRA FACILITAR NA PICOTAGEM
            valor += ";";
            Debug.Log(concat);
            PlayerPrefs.SetString(concat,valor);
        }
        else
        {
            valor += ";";
            juncao = getDados + valor;
            PlayerPrefs.SetString(concat, juncao);
        }
        
    }
    public ArrayList PegarPrefs(string concat)
    {
        int cont = 0;
        ArrayList dadosPicotado = new ArrayList();
        //string concat;
        //var dataAtual = DateTime.Now;
        //concat = tipo + dataAtual.Day + "/" + dataAtual.Month + "/" + dataAtual.Year;
        //concat = tipo + dataAtual;
        if (PlayerPrefs.HasKey(concat))
        {
            string getDados = PlayerPrefs.GetString(concat);
            //string getDados = "Testando;sera;que;funciona;";
            int tamString = getDados.Length;

            //CORTA UMA VARIAVEL SEMPRE QUE ENCONTRAR ';'
            //TIRAR QUALQUER STRING VAZIA DO CODIGO
            string[] cortes = getDados.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //PRA PODER RESGATAR AS VARIAVEIS CORTADA PRECISA USAR O FOREACH
            foreach (string s in cortes)
            {
                dadosPicotado.Add(s);
                //Debug.Log("PEgar PRefs" + dadosPicotado[cont]);
            }
            return dadosPicotado;
        }
        return null;
    }


    public List<ArrayList> ResgataPrefs(string tipo)
    {
        //CONVERTER STRING EM DATA... O MES ELE PRECISA SER MAIUSCULO PRA PODE FUNCIONAR
        DateTime primeiraVezJogo = DateTime.ParseExact(firstJogada,"dd/MM/yyyy",null);
        var hojeData = DateTime.Now;
        //SUBTRAIR A DATA PASSADA COM A DATA ATUAL
        TimeSpan diferencaData = primeiraVezJogo.Subtract(hojeData);
        List<ArrayList> valores = new List<ArrayList>();
        //CONVERTER A DIFERENCA DE DATA EM INTEIRO PRA USAR NO FOR
        int intervaloTempo = (int) -(diferencaData.TotalDays);
        string concat;

        for (int i = 0; i < intervaloTempo; i++)
        {
            DateTime acresDia = primeiraVezJogo.AddDays(+i);
            concat = tipo+ acresDia.ToString("dd/MM/yyyy");
            //Debug.Log("ResgatarPrefs Concat " + concat);
            if(PegarPrefs(concat) != null)
            {
                valores.Add(PegarPrefs(concat));
            }
            
        }
        if(valores.Count >= 1)
        {
            return valores;
        }else
        {
            return null;
        }
        
    }

    public double HistoricoMedia(string tipo)
    {
        int aux = 0, qtItens = 0;
        double media = 0;
        if (ResgataPrefs(tipo) != null)
        {
            List<ArrayList> listaDados = ResgataPrefs(tipo);
            int tam = listaDados.Count;
            
            if (tam > 0)
            {
                for (int i = 0; i < tam; i++)
                {
                    for (int y = 0; y < listaDados[i].Count; y++)
                    {
                        Debug.Log("Valor de Listar Historico "+listaDados[i][y]);
                        if(listaDados[i][y] != null)
                        {
                            aux += Convert.ToInt32( listaDados[i][y]);
                        }
                        
                        qtItens++;
                    }
                }
                media = aux / qtItens;
            }
            qtAtivdade.text = qtItens.ToString();
            Debug.Log(media);
            return media;
        }
        
        return 0;
    }

    public double HistoricoSemana(string tipo)
    {
        string concat;
        var dataAtual = DateTime.Now;
        ArrayList dadosDia = new ArrayList();
        double media = 0;
        dataAtual = dataAtual.AddDays(-7);
        for(int i =0;i <7;i++)
        {
            DateTime acresDia = dataAtual.AddDays(+i);
            concat = tipo + acresDia.ToString("dd/MM/yyyy");
            dadosDia = PegarPrefs(concat);
            for(int y = 0; y < dadosDia.Count; y++)
            {
                media += (double) dadosDia[y];
            }
            
            //media = Double.Parse(PlayerPrefs.GetString(concat));
            media /= 7;
        }
        return media;
    }

    
    public void gerarDados()
    {
        string concat;
        System.Random random = new System.Random();
        int dia = random.Next(1, 15);
        int valor = random.Next(1, 100);
        if(dia < 10)
        {
            concat = "score0" + dia + "/10/2017";
        }else
        {
            concat = "score" + dia + "/10/2017";
        }
        
        Debug.Log(concat);
        Debug.Log("Valor " + valor);
        CriarPrefs(concat, valor.ToString());
    }

    public void GerarHistorico()
    {
        double media = HistoricoMedia("score");
        meAtivdade.text = media.ToString();
    }
    
    public void DeletarPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void EscritaDados(string tipo, string dados)
    {
        Stream entrada = File.Open(Application.dataPath + "/Resources/" + tipo + ".ini", FileMode.Open);
        StreamReader leitura = new StreamReader(entrada);
        StreamReader escrita = new StreamReader(Application.dataPath + "/Resources/Historico/" + tipo + ".ini");
    }

	public void Voltar()
	{
		SceneManager.LoadSceneAsync("menuInicial");
	}

}
