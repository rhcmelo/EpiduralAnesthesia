using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class HapticInjection : HapticClassScript {
	
	//Generic Haptic Functions
	private GenericFunctionsClass myGenericFunctionsClassScript;

	private float myDopLimit;
    
    // Exibição da interface
    public GameObject Hud;
    //public GameObject goHud2;
	public HUD2 cpHUD2;

	public GameObject goHUDCanvas;

	private Arquivo cpArquivo;  // Gravação por tempo - trajetoria
	private Arquivo cpArquivo2; // Gravação por profundidade

	// Objeto seringa
	public GameObject goSeringa;
	public float tempoTrocaSeringa = 0.5f;
	private float tempoUltimaTroca;

	// Tempo de gravação de registro
	private float tempo, tempoGravacao;
	public float intervaloGravacao = 1.0f;
	public float tempoMaximoGravacao = 35.0f;
	private bool fechouArquivo = false;

	// Profundidade de gravação do registro
	public float intervaloGravacao2;
	public float profundidadeGravacao2;

	private Tecido propriedadesTecido;
	private HapticProperties propriedadesOP;

	// para o cálculo da angulação da agulha
	public MeshFilter malhaNormal; 

	// Verificando a angulação da agulha 
	float angulo, anguloX, anguloY, anguloZ;
    /*****************************************************************************/
	
	void Awake()
	{
		//malhaNormal.mesh.normals[0]
		myGenericFunctionsClassScript = transform.GetComponent<GenericFunctionsClass>();
	}

	/*
	public void GetForce()
	{
		double[] forceArray = new double[3];
		PluginImport.hdGetDoublev(PluginImport.ParameterName.HD_CURRENT_FORCE, forceArray);

		Debug.Log ("X:" + forceArray [0].ToString () + ",Y:" + forceArray [1].ToString () + ",Z:" + forceArray [2].ToString ());

		//forceArray[0] = force.x;
		//forceArray[1] = force.y;
		//forceArray[2] = -force.z;
	}
	*/

	void OnApplicationQuit()
	{
		if (cpArquivo != null) cpArquivo.Fechar ();
		if (cpArquivo2 != null) cpArquivo2.Fechar ();
	}

	void Start()
	{
		tempoUltimaTroca = 0.0f; // Troca de seringa

        // rafael para não ultrapasar tamanho da seringa
        maxPenetration = 0.6f;

        // gravação por profundidade
        intervaloGravacao2 = 1.0f;
		profundidadeGravacao2 = -0.9f;

		// Inicializando arquivos de registro
		cpArquivo = new Arquivo ();
		cpArquivo.Abrir("Log.txt");
		tempo = 0.0f;
		tempoGravacao = 0.0f;

		cpArquivo2 = new Arquivo ();
		cpArquivo2.Abrir("LogProfundidade.txt");

		// inicializando angulação da agulha 
		angulo = 0.0f; anguloX = 0.0f; anguloY = 0.0f; anguloZ=0.0f;

		if(PluginImport.InitHapticDevice())
		{
			Debug.Log("OpenGL Context Launched");
			Debug.Log("Haptic Device Launched");
			
			myGenericFunctionsClassScript.SetHapticWorkSpace();
			myGenericFunctionsClassScript.GetHapticWorkSpace();
			
			//Update Workspace as function of camera
			PluginImport.UpdateWorkspace(myHapticCamera.transform.rotation.eulerAngles.y);
			
			//Set Mode of Interaction
			/*
			 * Mode = 0 Contact
			 * Mode = 1 Manipulation - So objects will have a mass when handling them
			 * Mode = 2 Custom Effect - So the haptic device simulate vibration and tangential forces as power tools
			 * Mode = 3 Puncture - So the haptic device is a needle that puncture inside a geometry
			 */
			PluginImport.SetMode(ModeIndex);
			//Show a text descrition of the mode
			myGenericFunctionsClassScript.IndicateMode();
				
            //Define a Penetration direction Vector - useful for brain biopsy for instance.
            //For realism, the orientation of the needle will need to be detached from that of the haptic device
            //The new orientation on local Z of the needle will be that indicated by the vector set below.
            /*double[] injectionDir = new double[3]{0.0f,0.0f,1.0f};
            PluginImport.SetPunctureDirection(ConverterClass.ConvertDouble3ToIntPtr(injectionDir));*/

			//Set the lenght of the syringue needle to penetrate inside the tissue
			PluginImport.SetMaximumPunctureLenght(maxPenetration);

			//Set the touchable face(s)
			PluginImport.SetTouchableFace(ConverterClass.ConvertStringToByteToIntPtr(TouchableFace));

			Debug.Log ("device:" + PluginImport.hdGetCurrentDevice().ToString());
			
		}
		else
			Debug.Log("Haptic Device cannot be launched");


		/***************************************************************/
		//Set Environmental Haptic Effect
		/***************************************************************/

			// Constant Force Example - We use this environmental force effect to simulate the weight of the cursor
			myGenericFunctionsClassScript.SetEnvironmentConstantForce();
		myGenericFunctionsClassScript.SetEnvironmentViscosity();
		//myGenericFunctionsClassScript.SetEnvironmentSpring();
			
		/***************************************************************/
		//Setup the Haptic Geometry in the OpenGL context
		/***************************************************************/
		myGenericFunctionsClassScript.SetHapticGeometry();

		//Get the Number of Haptic Object
		//Debug.Log ("Total Number of Haptic Objects: " + PluginImport.GetHapticObjectCount());

		/***************************************************************/
		//Launch the Haptic Event for all different haptic objects
		/***************************************************************/
		PluginImport.LaunchHapticEvent();
	}

	void Update()
	{
		// Atualizando a contagem do tempo
		tempo += Time.deltaTime;

		// Inserção / Retirada da Seringa
		AtualizaSeringaPressao ();

        AtualizaProfundidadePerfuracao();

        /***************************************************************/
        //Update Workspace as function of camera
        /***************************************************************/
        PluginImport.UpdateWorkspace(myHapticCamera.transform.rotation.eulerAngles.y);
		
		/***************************************************************/
		//Update cube workspace
		/***************************************************************/
		myGenericFunctionsClassScript.UpdateGraphicalWorkspace();
		
		/***************************************************************/
		//Haptic Rendering Loop
		/***************************************************************/
		PluginImport.RenderHaptic ();
		
		myGenericFunctionsClassScript.GetProxyValues();
		// teste posicao inclinacao agulha
		//hapticCursor.transform.rotation = Quaternion.LookRotation(new Vector3(0.0f,-0.97f,-0.25f),new Vector3 (0.70f,-0.20f,-0.63f));

		myGenericFunctionsClassScript.GetTouchedObject();

		//For the Puncture Mode effect
		if(PluginImport.GetMode() == 3)
		{
			//Debug.Log ("Contact state is set to " + PluginImport.GetContact());
			//Debug.Log ("Penetration State " + PluginImport.GetPenetrationRatio());

            // Obtendo o % de penetração
            float profundidade = PluginImport.GetPenetrationRatio();
			float profundidademm = profundidade * cpHUD2.profundidadeMaxima;

            // Atualizando a profundidade da penetracao
            //cpHUD2.depth = profundidade * maxPenetration;
			cpHUD2.depth = profundidademm;
			goHUDCanvas.GetComponent<HUDCanvas>().barraProfundidade.value = profundidade * cpHUD2.profundidadeMaxima;

			double[] myScp = new double[3];
			myScp = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetFirstScpPt());
			//Debug.Log (" SCP " + myScp[0] + " " + myScp[1] + " " + myScp[2]);

			Vector3 posInjectionHole;
			posInjectionHole = ConverterClass.ConvertDouble3ToVector3(myScp);
			GameObject.Find ("InjectionMarker").transform.position = posInjectionHole;

			/*double[] myProx = new double[3];
			myProx = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetProxyPosition());
			
			Vector3 posProx;
			posProx = ConverterClass.ConvertDouble3ToVector3(myProx);
			GameObject.Find ("ProxyTipMarker").transform.position = posProx;*/

			/*double[] myDev = new double[3];
			myDev = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetDevicePosition());

			Vector3 posDevice;
			posDevice = ConverterClass.ConvertDouble3ToVector3(myDev);
			GameObject.Find ("DeviceTipMarker").transform.position = posDevice;*/

			// Obtendo o torque e convertendo para vector3
			/*
			double[] meuTorque = new double[3];
			meuTorque = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetProxyTorque());
			Vector3 vTorque = new Vector3();
			vTorque = ConverterClass.ConvertDouble3ToVector3(meuTorque);

			// Exibindo o torque na HUD2
			cpHUD2.torque = vTorque;
			*/



			double[] myPinch = new double[3];
			myPinch = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetPunctureDirection());

			Vector3 start = new Vector3();
			start = ConverterClass.ConvertDouble3ToVector3(myScp);
			Vector3 end = new Vector3();
			end = ConverterClass.ConvertDouble3ToVector3(myPinch);
			end.Normalize();
			//GameObject.Find ("DirecaoPerfuracao").transform.position = end;

			Debug.DrawLine(start,start+end * maxPenetration, Color.green);

			//Ray Cast so we can determine the limitation of the puncture
			RaycastHit[] hits;
			hits = Physics.RaycastAll(start, end , maxPenetration);

			// Verificando a angulação da agulha 
			//calcularAngulacaoAgulha (out angulo, out anguloX, out anguloY, out anguloZ);
			calcularAngulacaoAgulhaNormal (malhaNormal.mesh.normals[1], malhaNormal.gameObject.transform.position, out angulo, out anguloX, out anguloY, out anguloZ);
			//Debug.Log (anguloX);

			// Exibindo valores na interface do game
			GameManager.instancia.AtualizarAngulacao(angulo, anguloX,anguloY, anguloZ);

			if(hits.Length != 0)
			{
				//Declare a float array to store the tissue layer
				float[] tissueLayers = new float[hits.Length];
				//Declare a string array to store the name of the tissue layer
				string[] punctObjects = new string[hits.Length];
				int nbLayer = 0;

                float profundidade_camada = 0.0f;


				// Armazenar colisor do tecido atual penetrado
				Collider colisorTecidoAtual = null;

                for (int i = 0; i < hits.Length; i++) 
				{
					RaycastHit hit = hits[i];

					//Only if the object is declared as touchable
					if(hit.collider.gameObject.tag == "Touchable")
					{
						tissueLayers[nbLayer] = hit.distance;
						punctObjects[nbLayer] = hit.collider.name;
						nbLayer++;
					}

					// Identificando camada corrente da penetração
					if (//colisorTecidoAtual == null ||  // deu problema com dimensionamento dinamico da grossura dos tecidos (24/6)
						(profundidade > hit.distance && hit.distance > profundidade_camada))  // já atravessou essa camada e ela eh mais profunda
                    {
						colisorTecidoAtual = hit.collider;  // armazenando o colisor da camada perpassada
						profundidade_camada = hit.distance; // atualizando a profundidade da camada perpassada
					}
				}

				// Se não identificou ainda a camada atual sendo perfurada, configurando a pele (Body) como camada atual (24/6)
				if (colisorTecidoAtual == null) {
					//colisorTecidoAtual = GameObject.Find ("Body").GetComponent<Collider>();
					colisorTecidoAtual = GameManager.instancia.camadas[0].GetComponent<Collider>();
					profundidade_camada = 0;
				}

				// Verificando tarefas de jogo envolvendo seringa de pressao
				if (GameManager.instancia.seringaPressao && GameManager.instancia.emboloSeringaPressao)
                {
                    //if (cpHUD2.usandoSeringa && cpHUD2.apertandoEmbolo) {
                    bool bObjPlunger = false;
                    bool bObjPlungerISL = false;
                    //for (int i = 0; i < GameManager.instancia.objetivos.Length; i++) {
                    for (int i = 7; i < GameManager.instancia.objetivos.Length; i++) // rafael
                    {

                        // verificando se o objetivo pode ser pontuado

                        // seringa em qualquer tecido
                        if (!GameManager.instancia.objetivos [i].realizado)
                        {
                            if (GameManager.instancia.objetivos[i].id == "Plunger")
                            {
                                GameManager.instancia.AtualizarObjetivo(i);
                                bObjPlunger = true;
                            }
                            // Seringa no ISL
                            else if (GameManager.instancia.objetivos[i].id == "PlungerISL" && colisorTecidoAtual.name == "InterspinousLigament")
                            {
                                GameManager.instancia.AtualizarObjetivo(i);
                                bObjPlungerISL = true;
                            }

                            if (bObjPlunger && bObjPlungerISL)
                                break;
                        }

                    }
				}

				// Verificando tarefas de jogo envolvendo seringa de anestesia
				if (GameManager.instancia.seringaAnestesia && PluginImport.GetButton1State()) {
                    for (int i = GameManager.instancia.objetivos.Length-1; i >= 0; i--)
                    //for (int i = 0; i < GameManager.instancia.objetivos.Length; i++)
                    {
                        // verificando se o objetivo pode ser pontuado

                        // seringa de anestesia em qualquer tecido
                        if (!GameManager.instancia.objetivos [i].realizado && GameManager.instancia.objetivos [i].id == "Anestesia")
                        {
							GameManager.instancia.AtualizarObjetivo(i);
                            break; // rafael
                        }
					}
				}

				// Troca de tecido - atualizando propriedades do tecido
				if (cpHUD2.objectName != colisorTecidoAtual.name) { // Trocou de camada
					// Atualizando os dados do tecido penetrado
					cpHUD2.objectName = colisorTecidoAtual.name; // atualizando a camada
					//Debug.Log(i + ":" + hits[i].collider.name + ":" + hits[i].distance + " > " + profundidade);

					// Percorrendo os objetivos do jogo
					if (GameManager.instancia.agulhaEpidural) {
                        for (int i = 0; i < GameManager.instancia.objetivos.Length; i++)
                        {
                            // verificando se o objetivo pode ser pontuado
                            if (!GameManager.instancia.objetivos [i].realizado && GameManager.instancia.objetivos [i].id == colisorTecidoAtual.name) {
								GameManager.instancia.AtualizarObjetivo(i);
                                break; // rafael
							}
						}
					}

					// Tornando tecido semi-transparente
					colisorTecidoAtual.gameObject.GetComponent<MeshRenderer> ().material.color = new Color (1.0f, 1.0f, 1.0f, 0.5f);

					// Obtendo as propriedades do Objeto Perfurado (OP)
					//HapticProperties propriedadesOP = colisorTecidoAtual.gameObject.GetComponent<HapticProperties> ();
					propriedadesOP = colisorTecidoAtual.gameObject.GetComponent<HapticProperties> ();
					string objectName = colisorTecidoAtual.gameObject.name;
					// Atualizando a HUD com as propriedades do objeto perfurado
					AtualizaHUD (propriedadesOP, objectName);

					// Atualizando o nome da camada/tecido atual na HUD do canvas
					goHUDCanvas.GetComponent<HUDCanvas>().textoCamada.text = objectName;

					//if (PluginImport.GetTouchedObjectId () == -1) { // já penetrou na camada (após pop-through)
						// Atualizando força constante do tecido no haptico
						//myGenericFunctionsClassScript.myContantForceScript.magnitude = propriedadesOP.magnitudeConstante;
						//myGenericFunctionsClassScript.SetEnvironmentConstantForce ();

						// Atualizando viscosidade do tecido no haptico
						myGenericFunctionsClassScript.myViscosityScript.magnitude = propriedadesOP.magnitudeViscosidade;
						myGenericFunctionsClassScript.myViscosityScript.gain = propriedadesOP.ganhoViscosidade;
						myGenericFunctionsClassScript.SetEnvironmentViscosity ();
					//}

					// Atualizando as forças hapticas exercidas de acordo com as propriedades do objeto perfurado
					//myGenericFunctionsClassScript.AtualizarForcasHapticas(objectName);
					//CopiarPropriedadesHapticas(objectName,GameManager.instancia.camadas[0].name);

					// Obtendo as propriedades do tecido (pressão kPa)
					//Tecido propriedadesTecido = colisorTecidoAtual.gameObject.GetComponent<Tecido> ();
					propriedadesTecido = colisorTecidoAtual.gameObject.GetComponent<Tecido> ();
					cpHUD2.pressaoSalina = propriedadesTecido.kPaSalina;
					cpHUD2.pressaoAr = propriedadesTecido.kPaAr;
				}

				/*Declaration of the Puncture Stack
				 * Additionally, on the basis of the puncture stack components, the plugin setup a penetration restriction
				 * - due to the fact that Proxy Method along such constraint line is not accurate - most probably due to the fact
				 * that device position and proxy position differ because the constraint applies forces onto the device.
				 * So, the plugin impedes the proxy to penetrate in underlying layer when their popthrough values is null
				 */
				SetPunctureStack(nbLayer, punctObjects, tissueLayers);

                // Gravação do registro por profundidade no arquivo2
				if (profundidademm >= (profundidadeGravacao2 + intervaloGravacao2)) {
					//Debug.Log (profundidademm.ToString () + " >= " + "(" + profundidadeGravacao2.ToString () + " + " + intervaloGravacao2.ToString () + ")");
					profundidadeGravacao2 = profundidademm; // Atualizando a profundidade
					string linha = MontarLinhaGravacao (profundidademm.ToString("F0"));
					cpArquivo2.GravarLinha (linha);
				}
			}

			Debug.Log (PluginImport.GetTouchedObjectId().ToString() + "/" + 
				       PluginImport.GetTouchedObjectName().ToString());

			// Obtendo os valores das forças Hapticas exercidas
			// Exibindo as forças na HUD2
			cpHUD2.forcasHapticas = myGenericFunctionsClassScript.ObterForcasHapticas ();
			cpHUD2.posicaoProxy = myGenericFunctionsClassScript.ObterPosicaoProxy();
			cpHUD2.direcaoProxy = myGenericFunctionsClassScript.ObterDirecaoProxy();
			cpHUD2.torqueProxy = myGenericFunctionsClassScript.ObterTorqueProxy();

			// Exibindo barra de forças
			goHUDCanvas.GetComponent<HUDCanvas>().barraForcaX.value = cpHUD2.forcasHapticas.x;
			goHUDCanvas.GetComponent<HUDCanvas>().barraForcaY.value = cpHUD2.forcasHapticas.y;
			goHUDCanvas.GetComponent<HUDCanvas>().barraForcaZ.value = cpHUD2.forcasHapticas.z;

			// Atualizando valores na UHD
			goHUDCanvas.GetComponent<HUDCanvas>().textoProfundidade.text = cpHUD2.depth.ToString("F1") + " mm";
			goHUDCanvas.GetComponent<HUDCanvas>().textoForcaX.text = cpHUD2.forcasHapticas.x.ToString("F1") + " N";
			goHUDCanvas.GetComponent<HUDCanvas>().textoForcaY.text = cpHUD2.forcasHapticas.y.ToString("F1") + " N";
			goHUDCanvas.GetComponent<HUDCanvas>().textoForcaZ.text = cpHUD2.forcasHapticas.z.ToString("F1") + " N";

		}

		// Gravação do registro no arquivo
		if (tempo < tempoMaximoGravacao) {
			tempoGravacao += Time.deltaTime;

			if (tempoGravacao >= intervaloGravacao) {
				string linha = MontarLinhaGravacao ("0");
				cpArquivo.GravarLinha (linha);
				tempoGravacao = 0.0f;
			}
		} else if (!fechouArquivo) {
			cpArquivo.Fechar ();
			fechouArquivo = true;
		}

        /*
          if (Input.GetKeyDown(KeyCode.G))
        {
            PluginImport.SetPuncturedDynamicFriction(0, 1.0f);
        }
         */
	}

	void OnDisable()
	{
		if (PluginImport.HapticCleanUp())
		{
			Debug.Log("Haptic Context CleanUp");
			Debug.Log("Desactivate Device");
			Debug.Log("OpenGL Context CleanUp");
		}
	}


	void SetPunctureStack(int nbLayer ,string[] name, float[] array)
	{
		IntPtr[] objname = new IntPtr[nbLayer];
		//Assign object encounter along puncture vector to the Object array
		for (int i = 0; i < nbLayer; i++)
			objname[i] = ConverterClass.ConvertStringToByteToIntPtr(name[i]);

		PluginImport.SetPunctureLayers(nbLayer, objname,ConverterClass.ConvertFloatArrayToIntPtr(array));

	}

	string MontarLinhaGravacao(string profundidade)
	{
		// Obtendo posicao da ponta da agulha
		Vector3 posicaoProxy = myGenericFunctionsClassScript.ObterPosicaoProxy();
		Vector3 direcaoProxy = myGenericFunctionsClassScript.ObterDirecaoProxy();
		Vector3 torqueProxy  = myGenericFunctionsClassScript.ObterTorqueProxy();

		// Gravação das propriedades gerais e hápticas
		string linha = "" + tempo.ToString ("F0") +  				// 0 - tempo
			";" + cpHUD2.objectName +					// 1 - camada
			";" + cpHUD2.depth +						// 2 - profundidade
			";" + cpHUD2.forcasHapticas.x +				// 3 - força x
			";" + cpHUD2.forcasHapticas.y +				// 4 - força y
			";" + cpHUD2.forcasHapticas.z +				// 5 - força z
			";" + posicaoProxy.x +						// 6 - posicao x
			";" + posicaoProxy.y +						// 7 - posicao y
			";" + posicaoProxy.z +						// 8 - posicao z
			";" + direcaoProxy.x +						// 9 - direcao x
			";" + direcaoProxy.y +						// 10 - direcao y
			";" + direcaoProxy.z +						// 11- direcao z
			";" + torqueProxy.x +						// 12- torque x
			";" + torqueProxy.y +						// 13 - torque y
			";" + torqueProxy.z;						// 14 - torque z

		// Gravação das propriedades do tecido
		if (propriedadesOP != null) { // Já colidiu com algum tecido
			linha +=
				";" + propriedadesOP.stiffness + 				// 15 - rigidez (stiffness)
				";" + propriedadesOP.damping + 					// 16 - amortecimento (damping)
				";" + propriedadesOP.staticFriction + 			// 17 - fricção estática (static friction)
				";" + propriedadesOP.dynamicFriction + 			// 18 - fricção dinâmica (dynamic friction)
				";" + propriedadesOP.popThrough + 				// 19 - resistência ao rompimento (pop through)
				";" + propriedadesOP.puncturedStaticFriction + 	// 20 - fricção estática interna (punctured static friction)
				";" + propriedadesOP.puncturedDynamicFriction;  // 21 - fricção dinâmica interna (punctured dynamic friction)
		} else {
			linha += ";0;0;0;0;0;0;0";
		}

		if (propriedadesTecido != null) { // Já colidiu com algum tecido
			linha +=
				";" + propriedadesTecido.espessura +			// 22 - espessura (thickness)
				";" + propriedadesTecido.profundidade +			// 23 - profundidade (depth)
				";" + propriedadesTecido.kPaSalina  + 			// 24 - pressão salina (saline kPa)
				";" + propriedadesTecido.kPaAr;		 			// 25 - pressão do ar (air kPa)
		} else {
			linha += ";0;0;0;0";
		}

		linha += ";" + profundidade;							// 26 - profundidade delimitada (intervalos de 0.5)

		// angulação da agulha
		linha +=
			";" + angulo.ToString("F1")   +			// 27 - angulacao da agulha (geral)
			";" + anguloX.ToString("F1")  +			// 28 - angulacao da agulha (horizontal - x)
			";" + anguloY.ToString("F1")  +			// 29 - angulacao da agulha (vertical - y)
			";" + anguloZ.ToString("F1");		    // 30 - angulacao da agulha (profundidade - z)

		return linha;
	}

    // Atualiza HUD com propriedades do objeto perfurado (OP)
    void AtualizaHUD(HapticProperties propriedadesOP, string objectName)
    {
        // Obtendo o script da Hud
        //HUD scriptHUD = GameObject.Find("HUD").GetComponent<HUD>();
        HUD scriptHUD = Hud.GetComponent<HUD>();

        // atualizando o id e propriedades do objeto perfurado na HUD
        scriptHUD.objectName = objectName;
        scriptHUD.objectId = propriedadesOP.objectId;
        scriptHUD.puncturedDynamicFriction = propriedadesOP.puncturedDynamicFriction;
        scriptHUD.puncturedStaticFriction = propriedadesOP.puncturedStaticFriction;
        scriptHUD.popThrough = propriedadesOP.popThrough;
        scriptHUD.stiffness = propriedadesOP.stiffness;
        scriptHUD.damping = propriedadesOP.damping;
        scriptHUD.staticFriction = propriedadesOP.staticFriction;
        scriptHUD.dynamicFriction = propriedadesOP.dynamicFriction;
		scriptHUD.forcaConstante = propriedadesOP.magnitudeConstante * 3;
		scriptHUD.ganhoViscosidade = propriedadesOP.ganhoViscosidade;
		scriptHUD.magnitudeViscosidade = propriedadesOP.magnitudeViscosidade;
        
        //Debug.Log(propriedadesOP.objectId);

        // atualizando as propriedades do objeto perfurado na HUD
    }

	// Colocação/Retirada da Seringa
	void AtualizaSeringaPressao()
	{
		// contando o tempo da ultima ação de troca da seringa
		tempoUltimaTroca += Time.deltaTime;

		// Verificando se o botão 2 foi pressionado (uso/não uso da seringa)
		if (PluginImport.GetButton2State () && tempoUltimaTroca > tempoTrocaSeringa) {
			// Trocando seringa
			tempoUltimaTroca = 0.0f;
            //cpHUD2.usandoSeringa = !cpHUD2.usandoSeringa;

            bool bEstaPerfurando = false;
            // se está perfurando não muda para a seringa
            if (PluginImport.GetPenetrationRatio() > 0)
                bEstaPerfurando = true;

            // código anterior
            //GameManager.instancia.UsarSeringaPressao();
            // rafael para alternar entre equipamentos com o segundo botão
            if ((!bEstaPerfurando && GameManager.instancia.seringaAnestesia) || (bEstaPerfurando && GameManager.instancia.seringaPressao))
                GameManager.instancia.UsarAgulhaEpidural();
            else if (GameManager.instancia.agulhaEpidural && !GameManager.instancia.seringaPressao)
                GameManager.instancia.UsarSeringaPressao ();
            else
                GameManager.instancia.UsarSeringaAnestesia();

            /*
			// Colocou a seringa na agulha tuohy
			if (cpHUD2.usandoSeringa && !goSeringa.activeSelf) {
				// Fazendo aparecer a seringa
				goSeringa.SetActive (true);
			}
			// Retirou a seringa da agulha tuohy
			if (!cpHUD2.usandoSeringa && goSeringa.activeSelf) {
				goSeringa.SetActive (false);
			}
			*/
        }

		// Verificando se o botão 1 está sendo pressionado (dedão no embolo da seringa)
		if (GameManager.instancia.seringaPressao && PluginImport.GetButton1State ()) {
		//if (cpHUD2.usandoSeringa && PluginImport.GetButton1State ()) {
			GameManager.instancia.emboloSeringaPressao = true;
			//cpHUD2.apertandoEmbolo = true;
		} 
		else {
			GameManager.instancia.emboloSeringaPressao = false;
			//cpHUD2.apertandoEmbolo = false;
		}
	}

    // rafael
    void AtualizaProfundidadePerfuracao()
    {
        bool bMudouProfundidade = false;

        // dependendo da ferramenta ativa deve-se mudar a profunidade máxima de perfuração
        if (GameManager.instancia.seringaAnestesia)
        {
            if (maxPenetration != 0.6f)
                bMudouProfundidade = true;
            maxPenetration = 0.6f;
        }
        else
        {
            if (maxPenetration != 1.3f)
                bMudouProfundidade = true;
            maxPenetration = 1.3f;
        }
        if(bMudouProfundidade)
            //Set the lenght of the syringue needle to penetrate inside the tissue            
            PluginImport.SetMaximumPunctureLenght(maxPenetration);
    }

    // atualizado para calcular angulos a partir da comparação da direção da agulha com uma normal já existente (de um plano ou outro objeto)
    void calcularAngulacaoAgulhaNormal(Vector3 normal, Vector3 ponto, out float angulo, out float anguloX, out float anguloY, out float anguloZ)
	{
		// linha da perfuração (preview) antes de perfurar
		double[] direcaoProxy = new double[3];
		direcaoProxy = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetProxyDirection());

		// desenhando a linha da perfuração (preview)
		Vector3 direcaoAgulha = new Vector3();
		direcaoAgulha = ConverterClass.ConvertDouble3ToVector3(direcaoProxy);
		direcaoAgulha.Normalize ();

		Debug.DrawLine(hapticCursor.transform.position,hapticCursor.transform.position + direcaoAgulha * maxPenetration, Color.yellow);

		Vector3 normalX = new Vector3 (normal.x, -direcaoAgulha.y, -direcaoAgulha.z);
		Vector3 normalY = new Vector3 (-direcaoAgulha.x, normal.y, -direcaoAgulha.z);
		Vector3 normalZ = new Vector3 (-direcaoAgulha.x, -direcaoAgulha.y, normal.z);

		// Calculando angulos (geral, horizontal, vertical, profundidade)
		angulo = Vector3.Angle (-direcaoAgulha, normal);
		anguloX = Vector3.Angle (-direcaoAgulha, normalX);
		anguloY = Vector3.Angle (-direcaoAgulha, normalY);
		anguloZ = Vector3.Angle (-direcaoAgulha, normalZ);

		// desenhando linhas da normal e das angulações a partir de um ponto fornecido (de colisão?)
		Debug.DrawRay (ponto, normal,  Color.red);
		Debug.DrawRay (ponto, normalX, Color.cyan);
		Debug.DrawRay (ponto, normalY, Color.magenta);
	}

	// acha a normal a partir de um raycast: verifica se o raio acertou algum objeto para obter sua normal e calcular os angulos comparando com a direção da agulha
	void calcularAngulacaoAgulha(out float angulo, out float anguloX, out float anguloY, out float anguloZ)
	{
		// linha da perfuração (preview) antes de perfurar
		double[] direcaoProxy = new double[3];
		direcaoProxy = ConverterClass.ConvertIntPtrToDouble3(PluginImport.GetProxyDirection());

		// desenhando a linha da perfuração (preview)
		Vector3 direcaoAgulha = new Vector3();
		direcaoAgulha = ConverterClass.ConvertDouble3ToVector3(direcaoProxy);
		direcaoAgulha.Normalize ();

		Debug.DrawLine(hapticCursor.transform.position,hapticCursor.transform.position + direcaoAgulha * maxPenetration, Color.yellow);

		//RayCast para verificar a angulação 
		RaycastHit acerto;
		bool acertou;
		acertou = Physics.Raycast(hapticCursor.transform.position, direcaoAgulha, out acerto, maxPenetration);

		// Verificando se acertou superfície
		if (acertou) {
			Debug.DrawRay(acerto.point,acerto.normal,Color.red);

			Vector3 normalX = new Vector3 (acerto.normal.x, -direcaoAgulha.y, -direcaoAgulha.z);
			Vector3 normalY = new Vector3 (-direcaoAgulha.x, acerto.normal.y, -direcaoAgulha.z);
			Vector3 normalZ = new Vector3 (-direcaoAgulha.x, -direcaoAgulha.y, acerto.normal.z);

			// Calculando angulos (geral, horizontal, vertical, profundidade)
			angulo = Vector3.Angle  (-direcaoAgulha, acerto.normal);
			anguloX = Vector3.Angle (-direcaoAgulha, normalX);
			anguloY = Vector3.Angle (-direcaoAgulha, normalY);
			anguloZ = Vector3.Angle (-direcaoAgulha, normalZ);

			//Debug.Log ("angulo:" + angulo.ToString ("F1") + ", anguloX:" + anguloX.ToString ("F1") + ", anguloY:" + anguloY.ToString ("F1"));
				
			//Plane plano;
			//plano.SetNormalAndPosition (Vector3.right, acerto.point);
			Debug.DrawRay (acerto.point, normalX, Color.cyan);
			Debug.DrawRay (acerto.point, normalY, Color.magenta);

			//return angulo;

			//GameObject.CreatePrimitive (PrimitiveType.Plane);
		}
		else { //return 0.0f; 
			angulo = 0.0f;
			anguloX = 0.0f;
			anguloY = 0.0f;
			anguloZ = 0.0f;
		}
	}

	void CopiarPropriedadesHapticas(string objetoOrigem, string objetoDestino)
	{
		HapticProperties propriedadesOrigem  = GameObject.Find (objetoOrigem).GetComponent<HapticProperties> ();
		HapticProperties propriedadesDestino = GameObject.Find (objetoDestino).GetComponent<HapticProperties> ();

		propriedadesDestino.puncturedDynamicFriction = propriedadesOrigem.puncturedDynamicFriction;
		propriedadesDestino.puncturedStaticFriction  = propriedadesOrigem.puncturedStaticFriction;
		propriedadesDestino.popThrough      = propriedadesOrigem.popThrough;
		propriedadesDestino.dynamicFriction = propriedadesOrigem.dynamicFriction;
		propriedadesDestino.staticFriction  = propriedadesOrigem.staticFriction;
		propriedadesDestino.damping         = propriedadesOrigem.damping;
		propriedadesDestino.stiffness       = propriedadesOrigem.stiffness;
	}
}
