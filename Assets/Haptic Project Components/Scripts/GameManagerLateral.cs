using UnityEngine;
using System.Collections;
using System;

public class GameManagerLateral : GameManager
{	
	public new static GameManagerLateral instancia = null;

    private Vector3 pontoCentral;

    void Awake()
	{
		// Persistencia do objeto GameManager
		if (instancia == null)
			instancia = this;
		else if (instancia != this)
			Destroy (gameObject);

		//DontDestroyOnLoad (gameObject);

        shaderTransparent = Shader.Find("Legacy Shaders/Transparent/Diffuse");
        shaderTexture = Shader.Find("Unlit/Texture");

        // Ajusta posição para decúbito lateral
        pontoCentral = camadas[0].GetComponent<Renderer>().bounds.center;

        GameObject sideCamera = GameObject.Find("Side Camera (Lateral View)");
        GameObject mainCamera = GameObject.Find("Main Camera");
        GameObject dummy = GameObject.Find("dummy");

        Vector3 eixoZ = new Vector3(0, 0, 1);
        float rotacaoZ = 90.0f;

        Vector3 eixoY = new Vector3(0, 1, 0);
        float rotacaoY = -21.0f;

        // Alternar para posição em decúbito lateral esquerdo
        
        for (int i = 0; i < camadas.Length; i++)
        {
            camadas[i].transform.RotateAround(pontoCentral, eixoZ, rotacaoZ);
            camadas[i].transform.RotateAround(pontoCentral, eixoY, rotacaoY);

            camadas[i].transform.Translate(1.5f,0,0);
        }
        spine.transform.RotateAround(pontoCentral, eixoZ, rotacaoZ);
        spine.transform.RotateAround(pontoCentral, eixoY, rotacaoY);
        spine.transform.Translate(1.5f,0,0); // ajustar câmera

        // Rodar a câmera lateral
        sideCamera.transform.RotateAround(pontoCentral, eixoZ, rotacaoZ);
        sideCamera.transform.RotateAround(pontoCentral, eixoY, rotacaoY);

        Vector3 angulosPosicaoDeitadaEsquerda = mainCamera.transform.eulerAngles;
        angulosPosicaoDeitadaEsquerda.x = 0.163f;
        mainCamera.transform.eulerAngles = angulosPosicaoDeitadaEsquerda;
    }
}
