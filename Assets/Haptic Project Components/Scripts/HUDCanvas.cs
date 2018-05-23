using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDCanvas : MonoBehaviour {
	public Slider barraProfundidade;
	public Slider barraForcaX;
	public Slider barraForcaY;
	public Slider barraForcaZ;
	public Text textoPontuacao;
	public Text   textoAngulacao;
	public Text   textoAngulacaoX;
	public Text   textoAngulacaoY;
	public Slider barraAngulacao;
	public Slider barraAngulacaoX;
	public Slider barraAngulacaoY;
	public Text   textoForcaX;
	public Text   textoForcaY;
	public Text   textoForcaZ;
	public Text   textoProfundidade;
	public Text   textoCamada;

	public GameObject goObjetivo;
	public Text txtObjetivo;
	public Text txtPontosObjetivo;

	// Use this for initialization
	void Start () {
        goObjetivo.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
	}

	public void ExibirObjetivo(int id)
	{
		goObjetivo.SetActive (true);
	}

	public void AtualizarObjetivo(string textoObjetivo, string textoPontuacao)
	{
		txtObjetivo.text = textoObjetivo;
		txtPontosObjetivo.text = textoPontuacao;
	}
}
