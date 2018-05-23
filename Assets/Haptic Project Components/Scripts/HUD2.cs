using UnityEngine;
using System.Collections;

public class HUD2 : MonoBehaviour {
    public float depth;
	public float profundidadeMaxima = 65.0f; // profundidade máxima da perfuração, em milimetros
    public string objectName;
	//public Vector3 torque;
	public Vector3 forcasHapticas;
	public Vector3 posicaoProxy;
	public Vector3 direcaoProxy;  // orientacao Z
	public Vector3 torqueProxy;   // orientacao Y
	public float pressaoSalina;  // Pressão do líquido (salina) no embolo da seringa
	public float pressaoAr;  // Pressão do ar no embolo da seringa
	//public bool usandoSeringa;  // Verdadeiro - utilizando seringa (botão 2 do haptico)
	//public bool apertandoEmbolo;   // Verdadeiro quando utilizando o êmbolo da seringa (botão 1 do haptico)
    //public int objectId;  // objeto atual sendo perfurado
    //public string objectName;
	bool visivel;

	// Use this for initialization
	void Start () {
        depth = 0.0f;
        objectName = "";
		pressaoSalina = 0.0f;
		pressaoAr = 0.0f;
		//usandoSeringa = false;
		//apertandoEmbolo = false;
		visivel = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Exibir()
	{
		visivel = !visivel;
	}

    void OnGUI()
    {
		if (visivel) {
			GUI.color = Color.black;
        
			GUI.Label (new Rect (520, 85, 250, 30), "Camada:" + objectName);

			GUI.Label (new Rect (520, 100, 150, 30), "Profundidade:" + depth.ToString ());

			// Forças Hapticas
			GUI.Label (new Rect (520, 120, 150, 30), "Força X:" + forcasHapticas.x.ToString ("F2"));
			GUI.Label (new Rect (520, 140, 150, 30), "Força Y:" + forcasHapticas.y.ToString ("F2"));
			GUI.Label (new Rect (520, 160, 150, 30), "Força Z:" + forcasHapticas.z.ToString ("F2"));

			// Posição
			GUI.Label (new Rect (520, 180, 150, 30), "Posição X:" + posicaoProxy.x.ToString ("F2"));
			GUI.Label (new Rect (520, 200, 150, 30), "Posição Y:" + posicaoProxy.y.ToString ("F2"));
			GUI.Label (new Rect (520, 220, 150, 30), "Posição Z:" + posicaoProxy.z.ToString ("F2"));

			// Direção
			GUI.Label (new Rect (520, 240, 150, 30), "Direção X:" + direcaoProxy.x.ToString ("F2"));
			GUI.Label (new Rect (520, 260, 150, 30), "Direção Y:" + direcaoProxy.y.ToString ("F2"));
			GUI.Label (new Rect (520, 280, 150, 30), "Direção Z:" + direcaoProxy.z.ToString ("F2"));

			// Torque
			GUI.Label (new Rect (520, 300, 150, 30), "Torque X:" + torqueProxy.x.ToString ());
			GUI.Label (new Rect (520, 320, 150, 30), "Torque Y:" + torqueProxy.y.ToString ());
			GUI.Label (new Rect (520, 340, 150, 30), "Torque Z:" + torqueProxy.z.ToString ());

			// Pressão
			if (GameManager.instancia.seringaPressao && GameManager.instancia.emboloSeringaPressao) { // Está pressionando o embolo da seringa (botao 1)
				GUI.Label (new Rect (520, 360, 150, 30), "Pressão Salina:" + pressaoSalina.ToString ());
				//GUI.Label(new Rect(550, 300, 150, 30), "Pressão Ar:" + pressaoAr.ToString());
			} else {
				GUI.Label (new Rect (520, 360, 150, 30), "Pressão Salina: 0");
				//GUI.Label(new Rect(550, 300, 150, 30), "Pressão Ar: 0");
			}
		}
	}
}
