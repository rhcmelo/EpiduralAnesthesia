using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public GameObject[] camadas;
	public Transform localizacaoPele;
	public float escala;
	public GameObject layers;
    public GameObject spine;
    private Paciente objPaciente;
	public int pontuacao;
    public int quantidadeDeObjetivos;
    public int objetivosCompletos;
    public GameObject HUDJogo;
	public static GameManager instancia = null;
	public Objetivo[] objetivos;

	// Instrumentos
	public GameObject goSeringaAnestesia;
	public GameObject goAgulhaEpidural;
	public GameObject goSeringaPressao;

	// Utilização dos Instrumentos
	public bool seringaAnestesia;
	public bool agulhaEpidural;
	public bool seringaPressao;
	public bool emboloSeringaPressao;
	//public bool emboloSeringaAnestesia;

	// Perfil de propriedades dos tecidos
	public int codigoPerfilPropriedadesTecidos; // código do perfil
	public string nomeArquivoPropriedadesTecidos; // nome do arquivo texto de propriedades

    public Shader shaderTransparent;
    public Shader shaderTexture;

    private bool visibilidadeInterna = false;

    void Awake()
	{
		// Persistencia do objeto GameManager
		if (instancia == null)
			instancia = this;
		else if (instancia != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

        shaderTransparent = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        shaderTexture = Shader.Find("Unlit/Texture");
    }

    // Use this for initialization
    void Start () {
		

        InicializaJogo();

    }

    private void DadosPaciente()
    {
        if (objPaciente == null)
            objPaciente = new Paciente();

        // paciente com maior IMC
        /*
        objPaciente.peso = 119.5f;
        objPaciente.altura = 1.56f;
        objPaciente.idade = 34;
        */

        // paciente com menor IMC 
        /*
        objPaciente.peso = 58.2f;
        objPaciente.altura = 1.72f;
        objPaciente.idade = 19;
        */

        // paciente com menor IMC médio
        /**/
        objPaciente.peso = 71.1f;
        objPaciente.altura = 1.60f; // rafael
        //objPaciente.pesoMedio = 71.1f;
        objPaciente.idade = 29;
        //objPaciente.raioMedioCintura = 13.52809f;
        //objPaciente.areaMediaCintura = 574.94f;
        //**/
    }

    private void InicializaJogo()
    {
        DadosPaciente();

        HUDJogo.GetComponent<HUDCanvas>().AtualizarDadosPaciente(objPaciente.altura, objPaciente.peso, objPaciente.idade);

        PosicionarCamadas();

        ZerarPontuacao();

        // Configurando objetivos
        ConfigurarObjetivos();

        // Carregando as propriedades dos tecidos
        CarregarPropriedadesTecidos(codigoPerfilPropriedadesTecidos);

        HUDJogo.GetComponent<HUDCanvas>().goObjetivo.SetActive(false);

        setVisibilidadeInterna(false);
    }

    private bool getVisibilidadeInterna()
    {
        return visibilidadeInterna;
    }

    private void setVisibilidadeInterna(bool ver)
    {
        visibilidadeInterna = ver;

        spine.GetComponent<Renderer>().enabled = visibilidadeInterna;

        for(int i=1; i< camadas.Length; i++)
            camadas[i].GetComponent<Renderer>().enabled = visibilidadeInterna;

        if (visibilidadeInterna)
            camadas[0].GetComponent<Renderer>().material.shader = shaderTransparent;
        else
            camadas[0].GetComponent<Renderer>().material.shader = shaderTexture;

        // Faz desaparecer mas não faz aparecer
        //GameObject.Find("FeedBackInfo").SetActive(visibilidadeInterna);
        //não esconde
        //GameObject.Find("FeedBackInfo").GetComponent<Renderer>().enabled = visibilidadeInterna;
        /*CanvasGroup cg = GameObject.Find("FeedBackInfo").GetComponent<CanvasGroup>();
        cg.interactable = false;
        if (visibilidadeInterna)
            cg.alpha = 1;
        else
            cg.alpha = 0;
        */
    }

    public void ExibirObjetivo(int id)
	{
		HUDJogo.GetComponent<HUDCanvas> ().ExibirObjetivo (id);
	}

    public void AtualizarObjetivo(int i)
    {
        objetivos[i].realizado = true;

        // Atualizando a pontuação do jogo
        AdicionarPontuacao(objetivos[i].pontos);

        HUDJogo.GetComponent<HUDCanvas>().AtualizarObjetivo(objetivos[i].descricao, objetivos[i].pontos.ToString() + " pontos!");

        ExibirObjetivo(i);
    }

    private void ZerarPontuacao()
    {
        pontuacao = 0;
        objetivosCompletos = 0;
        HUDJogo.GetComponent<HUDCanvas>().textoPontuacao.text = "Pontos: " + pontuacao;
    }

    public void AdicionarPontuacao(int valor)
	{
        if (valor > 0)
            objetivosCompletos++;
        pontuacao += valor;
		HUDJogo.GetComponent<HUDCanvas> ().textoPontuacao.text = "Pontos: " + pontuacao + " (" + objetivosCompletos + " de " + quantidadeDeObjetivos + " objetivos completos)";

        if (objetivosCompletos == quantidadeDeObjetivos)
            GameManager.instancia.SendMessage("Finnish");

    }

	public void AtualizarAngulacao(float angulo, float anguloX, float anguloY, float anguloZ)
	{
		HUDJogo.GetComponent<HUDCanvas> ().textoAngulacao.text  = angulo.ToString("F1") + " graus";
		HUDJogo.GetComponent<HUDCanvas> ().textoAngulacaoX.text  = anguloX.ToString("F1") + " graus";
		HUDJogo.GetComponent<HUDCanvas> ().textoAngulacaoY.text  = anguloY.ToString("F1") + " graus";
		HUDJogo.GetComponent<HUDCanvas> ().barraAngulacao.value = angulo;
		HUDJogo.GetComponent<HUDCanvas> ().barraAngulacaoX.value = anguloX;
		HUDJogo.GetComponent<HUDCanvas> ().barraAngulacaoY.value = anguloY;


		/*
		Color corVermelha = Color.red;
		Color corVerde = Color.green;

		// Feedback na cor da barra da angulação
		if (anguloX > 15f) {
			HUDJogo.GetComponent<HUDCanvas> ().barraAngulacaoX.colors.normalColor = corVermelha;
		} 
		else if (anguloX <= 15f) {
			HUDJogo.GetComponent<HUDCanvas> ().barraAngulacaoX.colors.normalColor = corVerde;
		}

		if (anguloY > 15f) {
			HUDJogo.GetComponent<HUDCanvas> ().barraAngulacaoX.colors.normalColor = corVermelha;
		} 
		else if (anguloY <= 15f) {
			HUDJogo.GetComponent<HUDCanvas> ().barraAngulacaoX.colors.normalColor = corVerde;
		}
		*/

	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            UsarSeringaAnestesia();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            UsarAgulhaEpidural();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            UsarSeringaPressao();

        if (Input.GetKeyDown(KeyCode.R))
        {
            // se não está perfurando pode reiniciar o jogo
            if (PluginImport.GetPenetrationRatio() == 0)
                InicializaJogo();

            GameManager.instancia.SendMessage("ReStart");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            setVisibilidadeInterna(!getVisibilidadeInterna());
        }
    }

    // 17-6: escala ok, falta ajustar posicionamento em relação a camada da pele
    // 24-6: posicionamento ok
    void PosicionarCamadas()
	{
		Tecido propriedadesTecido;

		//Vector3 posicaoCorpo = camadas [0].transform.position;
		Tecido propriedadesTecidoInterno = camadas [camadas.Length-1].GetComponent<Tecido> ();

		// Pegando como local de referencia um ponto da superficie de contato da pele 
		//Vector3 posicaoInterna = camadas [0].transform.position;
		Vector3 posicaoInterna = localizacaoPele.position;

		// atualizando a espessura da camada com base na escala
		//float tamanhoCamada = DimensionarCamada(propriedadesTecidoInterno.espessuraMedia);
		propriedadesTecidoInterno.espessura = DimensionarCamada(propriedadesTecidoInterno.espessuraMedia);
		float tamanhoCamada = propriedadesTecidoInterno.espessura;

		// Mantendo escala em x e y, e ajustando thickness em z
		camadas [camadas.Length-1].transform.localScale = new Vector3(camadas [camadas.Length-1].transform.localScale.x,
			 														  camadas [camadas.Length-1].transform.localScale.y, tamanhoCamada * escala);

		// Percorrendo as camadas (inversamente)
		for (int i = camadas.Length-2; i>=1; i--) {
			//Debug.Log (camadas [i].name);
			// definindo o tamanho da camada igual anteriores + espessura
			propriedadesTecido = camadas [i].GetComponent<Tecido> ();
			//tamanhoCamada = tamanhoCamada + DimensionarCamada(propriedadesTecido.espessuraMedia);
			propriedadesTecido.espessura = DimensionarCamada(propriedadesTecido.espessuraMedia);
			tamanhoCamada = tamanhoCamada + propriedadesTecido.espessura;

			// Mantendo escala em x e y, e ajustando thickness em z
			camadas [i].transform.localScale = new Vector3(camadas [i].transform.localScale.x,camadas [i].transform.localScale.y, tamanhoCamada * escala);
		}

		// atualizando posicao interna (z) e tamanho para todas as camadas a partir do tamanho total e da escala estabelecida, mexendo no objeto-pai das camadas (layers)
		//tamanhoCamada += DimensionarCamada(camadas [0].GetComponent<Tecido> ().espessuraMedia);
		propriedadesTecido = camadas [0].GetComponent<Tecido> ();
		propriedadesTecido.espessura = DimensionarCamada(propriedadesTecido.espessuraMedia);
		tamanhoCamada += propriedadesTecido.espessura;
        //posicaoInterna.z = posicaoInterna.z + 0.1f + tamanhoCamada * escala / 2; // (0.1f = ajuste na mão para distancia da pele adicional)
		posicaoInterna.z = posicaoInterna.z + tamanhoCamada * escala / 2; 
        layers.transform.position = posicaoInterna;

		// Calculando a profundidade das camadas e sorteando a pressão (kPa)
		float profundidadeCamada = tamanhoCamada;
		for (int i = camadas.Length - 1; i >= 0; i--) {
			// Calculando a profundidade da camada
			propriedadesTecido = camadas [i].GetComponent<Tecido> ();
			profundidadeCamada -= propriedadesTecido.espessura;
			propriedadesTecido.profundidade = profundidadeCamada;

			// Sorteio da pressão salina da camada (kPaSalina) com valores dentro do desvio padrão
			propriedadesTecido.kPaSalina = UnityEngine.Random.Range (propriedadesTecido.kPaSalinaMedia - propriedadesTecido.kPaSalinaDesvioPadrao,
				propriedadesTecido.kPaSalinaMedia + propriedadesTecido.kPaSalinaDesvioPadrao);

			// Sorteio da pressão do ar da camada (kPaAr) com valores dentro do desvio padrão
			propriedadesTecido.kPaAr = UnityEngine.Random.Range (propriedadesTecido.kPaArMedia - propriedadesTecido.kPaArDesvioPadrao,
				propriedadesTecido.kPaArMedia + propriedadesTecido.kPaArDesvioPadrao);

			// ajustando profundidade para zero na pele
			if (i==0) propriedadesTecido.profundidade = 0.0f;
		}
	}

	// Parametricas
	float DimensionarCamada(float espessuraMedia)
	{
        // esta bugado 4.71 vezes
        //return espessuraMedia;

        //float espessuraCamada = Mathf.Pow (Mathf.Sqrt ((objPaciente.areaMediaCintura * (objPaciente.peso / objPaciente.pesoMedio)) / Mathf.PI)
        //                       / objPaciente.raioMedioCintura, 3) * espessuraMedia;

        const float So = 0.625f;
        const float Bi = 1.1f;
        const float Ai = 0.1f;

        float espessuraCamada = So + (Bi * objPaciente.peso / Mathf.Pow(objPaciente.altura, 2) - Ai * objPaciente.idade) / espessuraMedia;

        //Debug.Log (espessuraCamada);

        return espessuraCamada;
	}

	void CarregarPropriedadesTecidos (int codigoPerfil)
	{
		Arquivo arquivoPT;
		string linha;
		string[] valores;

		arquivoPT = new Arquivo ();
		arquivoPT.AbrirLeitura (nomeArquivoPropriedadesTecidos);

		while (!arquivoPT.FimArquivo ()) {

			// Lendo linha a linha e atualizando as propriedades
			linha = arquivoPT.LerLinha ();
			valores = linha.Split (';');

			if (valores [0] == codigoPerfilPropriedadesTecidos.ToString()) {
				// Percorrendo as camadas
				for (int c = 0; c < camadas.Length ; c++) {
					// Definindo posição do próximo destino, direcao e torque
					if (camadas [c].name == valores [1]) {
						// Obtendo as propriedades da camada
						HapticProperties propriedadesCamada = camadas [c].GetComponent<HapticProperties> ();

						float.TryParse (valores [2], out propriedadesCamada.stiffness);
						float.TryParse (valores [3], out propriedadesCamada.damping);
						float.TryParse (valores [4], out propriedadesCamada.staticFriction);
						float.TryParse (valores [5], out propriedadesCamada.dynamicFriction);
						float.TryParse (valores [6], out propriedadesCamada.popThrough );
						float.TryParse (valores [7], out propriedadesCamada.puncturedStaticFriction);
						float.TryParse (valores [8], out propriedadesCamada.puncturedDynamicFriction);
						float.TryParse (valores [9], out propriedadesCamada.magnitudeConstante);
						float.TryParse (valores [10], out propriedadesCamada.magnitudeViscosidade);
						float.TryParse (valores [11], out propriedadesCamada.ganhoViscosidade);
					}
				}
			}
		}

		arquivoPT.FecharLeitura ();
	}

	void ConfigurarObjetivos()
	{
        // Configurando os objetivos do jogo
        //objetivos = new Objetivo[10];
        objetivos = new Objetivo[11];

        objetivos [0] = new Objetivo ();
		objetivos [0].realizado = false;
		objetivos [0].descricao = "Penetrar a pele";
		objetivos [0].id        = "Body";
		objetivos [0].pontos    = 50;

		objetivos [1] = new Objetivo ();
		objetivos [1].realizado = false;
		objetivos [1].descricao = "Alcançar a gordura subcutânea";
		objetivos [1].id        = "SubcutaneousFat";
		objetivos [1].pontos    = 100;

		objetivos [2] = new Objetivo ();
		objetivos [2].realizado = false;
		objetivos [2].descricao = "Atingir o tecido muscular";
		objetivos [2].id        = "Muscle";
		objetivos [2].pontos    = 200;

		objetivos [3] = new Objetivo ();
		objetivos [3].realizado = false;
		objetivos [3].descricao = "Perfurar o ligamento inter-espinhoso";
		objetivos [3].id        = "InterspinousLigament";
		objetivos [3].pontos    = 200;

		objetivos [4] = new Objetivo ();
		objetivos [4].realizado = false;
		objetivos [4].descricao = "Atingiu o Ligamentum Flavum";
		objetivos [4].id        = "LigamentumFlavum";
		objetivos [4].pontos    = 200;

		objetivos [5] = new Objetivo ();
		objetivos [5].realizado = false;
		objetivos [5].descricao = "Alcançar o espaço epidural";
		objetivos [5].id        = "EpiduralSpace";
		objetivos [5].pontos    = 500;

		objetivos [6] = new Objetivo ();
		objetivos [6].realizado = false;
		objetivos [6].descricao = "Perfurar a dura-mater";
		objetivos [6].id        = "DuraMater";
		objetivos [6].pontos    = -1000;

		objetivos [7] = new Objetivo ();
		objetivos [7].realizado = false;
		objetivos [7].descricao = "Usar a seringa para verificar a pressão";
		objetivos [7].id        = "Plunger";
		objetivos [7].pontos    = 100;

		objetivos [8] = new Objetivo ();
		objetivos [8].realizado = false;
		objetivos [8].descricao = "Usar a seringa no ligamento inter-espinhoso";
		objetivos [8].id        = "PlungerISL";
		objetivos [8].pontos    = 250;

		objetivos [9] = new Objetivo ();
		objetivos [9].realizado = false;
		objetivos [9].descricao = "Anestesiar local";
		objetivos [9].id        = "Anestesia";
		objetivos [9].pontos    = 150;

        objetivos[10] = new Objetivo();
        objetivos[10].realizado = false;
        objetivos[10].descricao = "Esqueceu de anestesiar local";
        objetivos[10].id = "SemAnestesia";
        objetivos[10].pontos = -100;

        quantidadeDeObjetivos = 0;
        for (int i = 0; i < objetivos.Length; i++)
        {
            if (objetivos[i].pontos > 0)
                quantidadeDeObjetivos++;
        }
    }

    public bool AnestesiaLocalRealizada()
    {
        return objetivos[9].realizado;
    }


    public void UsarSeringaAnestesia()
	{
        // rafael
        // Obtendo o % de penetração
        float profundidade = PluginImport.GetPenetrationRatio();

        // se está perfurando não muda para a seringa
        if (profundidade > 0)
            return;
        ///

        seringaAnestesia = true;
        //seringaAnestesia = !seringaAnestesia;
        goSeringaAnestesia.SetActive (seringaAnestesia);

        if (agulhaEpidural) { // Desativa agulha epidural se estava usando antes
			agulhaEpidural = !agulhaEpidural;
			goAgulhaEpidural.SetActive (agulhaEpidural);
		}

		if (seringaPressao) { // Desativa seringa pressao se estava usando antes
			seringaPressao = !seringaPressao;
			goSeringaPressao.SetActive (seringaPressao);
		}
	}

	public void UsarAgulhaEpidural()
	{
        // rafael
        // Obtendo o % de penetração
        float profundidade = PluginImport.GetPenetrationRatio();

        // se está perfurando não muda da seringa
        if (profundidade > 0 && seringaAnestesia)
            return;
        ///

        agulhaEpidural = true;
        //agulhaEpidural = !agulhaEpidural;
        goAgulhaEpidural.SetActive (agulhaEpidural);

        if (seringaAnestesia) { // Desativa seringa anestesia se estava usando antes
			seringaAnestesia = !seringaAnestesia;
			goSeringaAnestesia.SetActive (seringaAnestesia);
		}

        if (seringaPressao)
        { // Desativa seringa pressao se estava usando antes
            seringaPressao = !seringaPressao;
            goSeringaPressao.SetActive(seringaPressao);
        }

    }

    public void UsarSeringaPressao()
	{
        // rafael
        // Obtendo o % de penetração
        float profundidade = PluginImport.GetPenetrationRatio();

        // se está perfurando não muda da seringa
        if (profundidade > 0 && seringaAnestesia)
            return;
        ///

        seringaPressao = !seringaPressao;
        goSeringaPressao.SetActive (seringaPressao);

        if (seringaPressao && !agulhaEpidural)
        { // Ativa agulha epidural se não estava usando antes
            agulhaEpidural = !agulhaEpidural;
            goAgulhaEpidural.SetActive(agulhaEpidural);
        }

        if (seringaAnestesia) { // Desativa seringa anestesia se estava usando antes
			seringaAnestesia = !seringaAnestesia;
			goSeringaAnestesia.SetActive (seringaAnestesia);
		}
	}
}
