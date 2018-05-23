using UnityEngine;
using System.Collections;
//using UnityEngine.UI;

public class HUD : MonoBehaviour {
    public int objectId;  // objeto atual sendo perfurado
    public string objectName;
    public float puncturedDynamicFriction;
    public float puncturedStaticFriction;
    public float popThrough; 
    public float stiffness;
    public float damping;
    public float staticFriction;
    public float dynamicFriction;
	public bool visivel;
	public float forcaConstante;
	public float ganhoViscosidade;
	public float magnitudeViscosidade;

	public int perfilPropriedadesTecidos; // codigo do perfil de propriedades dos tecidos
	public string nomeArquivoLeitura; // Nome do arquivo original para leitura
	public string nomeArquivoGravacao; // Nome do arquivo temporario para gravacao

	private Arquivo cpArquivoLeitura;  // Gravação por tempo - trajetoria
	private Arquivo cpArquivoGravacao; // Gravação por profundidade

    //Slider barra;

	// Use this for initialization
	void Start () {
        stiffness = 0.0f;
		visivel = false;
        //barra.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Exibir () {
		visivel = !visivel;
	}

    void OnGUI()
    {
		if (visivel) {
			
			//if (oldstiff != stiffness)
			//{
			//  HapticProperties script = GameObject.Find("Impassavel3").GetComponent<HapticProperties>();
			//  script.stiffness = stiffness;
            
			// gameobject 0 = cubo rosa tentando configurar a rigidez in runtime
			/* PluginImport.SetStiffness(0, 1.0f );
            if (stiffness == 1.0f)
                script.popThrough = 0.0f;
            else
                script.popThrough = 0.05f; */
			//PluginImport.SetStiffness(0, 0.0f);
			//}
			GUI.color = Color.black;
			//GUI.Label (new Rect (5, 40, 100, 30), "Objeto:" + objectId.ToString ());
			GUI.Label (new Rect (5, 40, 300, 30), "Tecido:" + objectName.ToString ());

			GUI.Label (new Rect (5, 60, 150, 30), "Fricção Estática (P):");
			puncturedStaticFriction = GUI.HorizontalSlider (new Rect (145, 65, 100, 30), puncturedStaticFriction, 0.0f, 1.0f);
			GUI.Label (new Rect (255, 60, 150, 30), puncturedStaticFriction.ToString ());

			GUI.Label (new Rect (5, 85, 150, 30), "Fricção Dinâmica (P):");
			puncturedDynamicFriction = GUI.HorizontalSlider (new Rect (145, 90, 100, 30), puncturedDynamicFriction, 0.0f, 1.0f);
			GUI.Label (new Rect (255, 85, 100, 30), puncturedDynamicFriction.ToString ());

			GUI.Label (new Rect (5, 110, 150, 30), "Resistência Perfuração:");
			popThrough = GUI.HorizontalSlider (new Rect (145, 115, 110, 30), popThrough, 0.0f, 1.0f);
			GUI.Label (new Rect (255, 110, 100, 30), popThrough.ToString ());

			//float oldstiff = stiffness;

			GUI.Label (new Rect (5, 135, 150, 30), "Rigidez:");
			stiffness = GUI.HorizontalSlider (new Rect (145, 140, 100, 30), stiffness, 0.0f, 1.0f);
			GUI.Label (new Rect (255, 135, 100, 30), stiffness.ToString ());

			GUI.Label (new Rect (5, 160, 150, 30), "Amortecimento:");
			damping = GUI.HorizontalSlider (new Rect (145, 165, 100, 30), damping, 0.0f, 1.0f);
			GUI.Label (new Rect (255, 160, 100, 30), damping.ToString ());

			GUI.Label (new Rect (5, 190, 150, 30), "Fricção Estática:");
			staticFriction = GUI.HorizontalSlider (new Rect (145, 195, 100, 30), staticFriction, 0.0f, 1.0f);
			GUI.Label (new Rect (255, 190, 100, 30), staticFriction.ToString ());

			GUI.Label (new Rect (5, 215, 150, 30), "Fricção Dinâmica:");
			dynamicFriction = GUI.HorizontalSlider (new Rect (145, 220, 100, 30), dynamicFriction, 0.0f, 1.0f);
			GUI.Label (new Rect (255, 215, 100, 30), dynamicFriction.ToString ());

			GUI.Label (new Rect (5, 240, 150, 30), "Força Constante:");
			forcaConstante = GUI.HorizontalSlider (new Rect (145, 245, 100, 30), forcaConstante, 0.0f, 20.0f);
			GUI.Label (new Rect (255, 240, 100, 30), forcaConstante.ToString ());

			GUI.Label (new Rect (5, 265, 150, 30), "Viscosidade (ganho):");
			ganhoViscosidade = GUI.HorizontalSlider (new Rect (145, 270, 100, 30), ganhoViscosidade , 0.0f, 100.0f);
			GUI.Label (new Rect (255, 265, 100, 30), ganhoViscosidade.ToString ());

			GUI.Label (new Rect (5, 290, 150, 30), "Viscosidade (magnit.):");
			magnitudeViscosidade = GUI.HorizontalSlider (new Rect (145, 295, 100, 30), magnitudeViscosidade, 0.0f, 100.0f);
			GUI.Label (new Rect (255, 290, 100, 30), magnitudeViscosidade.ToString ());

			// Atualizando propriedades do objeto perfurado

			GUI.color = Color.white;
			if (GUI.Button (new Rect (100, 320, 70, 30), "Atualizar")) {
				HapticProperties propriedadesOP = GameObject.Find (objectName).GetComponent<HapticProperties> ();

				// Atualizando propriedades no objeto
				propriedadesOP.puncturedStaticFriction = puncturedStaticFriction;
				propriedadesOP.puncturedDynamicFriction = puncturedDynamicFriction;
				propriedadesOP.popThrough = popThrough;

				propriedadesOP.staticFriction = staticFriction;
				propriedadesOP.dynamicFriction = dynamicFriction;
				propriedadesOP.damping = damping;
				propriedadesOP.stiffness = stiffness;

				// atualizando força constante e viscosidade do tecido
				propriedadesOP.magnitudeConstante   = forcaConstante/3;
				propriedadesOP.ganhoViscosidade     = ganhoViscosidade;
				propriedadesOP.magnitudeViscosidade = magnitudeViscosidade;

				// Atualizando propriedades no dispositivo haptico          
				PluginImport.SetPuncturedDynamicFriction (objectId, puncturedDynamicFriction);
				PluginImport.SetPuncturedStaticFriction (objectId, puncturedStaticFriction);
				PluginImport.SetPopThrough (objectId, popThrough);
				PluginImport.SetDynamicFriction (objectId, dynamicFriction);
				PluginImport.SetStaticFriction (objectId, staticFriction);
				PluginImport.SetDamping (objectId, damping);
				PluginImport.SetStiffness (objectId, stiffness);    

				// Atualizando força constante e viscosidade no haptico

				// Gravando propriedades hapticas
				GravarPropriedadesHapticas();
			}

		}
    }

	void GravarPropriedadesHapticas()
	{
		cpArquivoLeitura = new Arquivo ();
		cpArquivoLeitura.AbrirLeitura(nomeArquivoLeitura);

		cpArquivoGravacao = new Arquivo ();
		cpArquivoGravacao.Abrir(nomeArquivoGravacao);

		string linha;
		string[] valores;

		// construção da nova linha
		string novalinha = perfilPropriedadesTecidos.ToString()  + // 1 - codigo perfil
			";" + objectName + // 2 - camada
			";" + stiffness.ToString("F3") + // 3 - rigidez
			";" + damping.ToString("F3") + // 4 - amortecimento
			";" + staticFriction.ToString("F3") + // 5 - friccao estatica
			";" + dynamicFriction.ToString("F3") + // 6 - friccao dinamica
			";" + popThrough.ToString("F3") + // 7 - resistencia a perfuraçao
			";" + puncturedStaticFriction.ToString("F3") + // 8 - friccao estatica na perfuracao
			";" + puncturedDynamicFriction.ToString("F3") + // 9 - friccao dinamica na perfuracao
			";" + forcaConstante.ToString("F3") + // 10 - força constante
			";" + magnitudeViscosidade.ToString("F3") + // 11 - magnitude da viscosidade
			";" + ganhoViscosidade.ToString("F3");		// 12 - ganho da viscosidade

		// percorrendo o arquivo original
		while (!cpArquivoLeitura.FimArquivo()) {
			linha = cpArquivoLeitura.LerLinha ();
			valores = linha.Split (';');
			if (valores [0] == perfilPropriedadesTecidos.ToString() && valores [1] == objectName) { // Perfil correto e Tecido correto
				cpArquivoGravacao.GravarLinha (novalinha);
			} else {
				cpArquivoGravacao.GravarLinha (linha);
			}
		}

		cpArquivoLeitura.FecharLeitura ();
		cpArquivoGravacao.Fechar ();

		cpArquivoGravacao.Copiar (nomeArquivoLeitura);
	}
}
