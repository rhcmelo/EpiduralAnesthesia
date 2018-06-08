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
    public Text   txtPressaoSalina;
    public float  pressaoSalina;
    public Text   txtAlturaPaciente;
    public Text   txtPesoPaciente;
    public Text   txtIdadePaciente;

    public void SetPressaoSalina(float _pressaoSalina)
    {
        pressaoSalina = _pressaoSalina;
    }

    public GameObject goObjetivo;
	public Text txtObjetivo;
	public Text txtPontosObjetivo;

	// Use this for initialization
	void Start () {
        goObjetivo.SetActive(false);
        txtPressaoSalina.text = "";
        pressaoSalina = 0;
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instancia.seringaPressao && GameManager.instancia.emboloSeringaPressao)
        { // Está pressionando o embolo da seringa (botao 1)
            string valor = pressaoSalina.ToString("f2");
            string strPressao = "Pressão Salina: " + valor;
            
            txtPressaoSalina.text = strPressao;
        }
        else
        {
            txtPressaoSalina.text = "";
        }
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

    public void AtualizarDadosPaciente(float _atura, float _peso, int _idade)
    {
        txtAlturaPaciente.text = _atura.ToString("f2") + " m";
        txtPesoPaciente.text = _peso.ToString("f1") + " kg";
        txtIdadePaciente.text = _idade.ToString() + " anos";
    }
}
