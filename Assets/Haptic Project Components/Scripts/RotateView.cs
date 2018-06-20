using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RotateView : MonoBehaviour {

    public static RotateView instancia = null;

    Vector3 angulosPosicaoSentada;
    Vector3 angulosPosicaoDeitadaEsquerda;

    void Awake()
    {
        // Persistencia do objeto GameManager
        if (instancia == null)
            instancia = this;
        else if (instancia != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        angulosPosicaoSentada = transform.eulerAngles;
        angulosPosicaoDeitadaEsquerda = angulosPosicaoSentada;
        angulosPosicaoDeitadaEsquerda.x = 0.163f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float move = 2.0f;        

        float x = transform.eulerAngles.x;
        float y = transform.eulerAngles.y;
        
        if (GameManager.instancia.posicaoPaciente == GameManager.Posicao.Sentada)
        {
            if (Input.GetKey(KeyCode.RightArrow) && ((y < 50) || (y > 320)))
            {
                Debug.Log("Angulos x,y: " + transform.eulerAngles.x.ToString() + " , " + transform.eulerAngles.y.ToString());

                transform.Rotate(0.0f, -move, 0.0f);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && ((y < 40) || (y > 310)))
            {
                Debug.Log("Angulos x,y: " + transform.eulerAngles.x.ToString() + " , " + transform.eulerAngles.y.ToString());

                transform.Rotate(0.0f, move, 0.0f);
            }
        }
        else //if (GameManager.instancia.posicaoPaciente == GameManager.Posicao.DeitadaEsquerda)
        {
            if (Input.GetKey(KeyCode.RightArrow) && ((x < 50) || (x > 320)))
            {
                Debug.Log("Angulos x,y: " + transform.eulerAngles.x.ToString() + " , " + transform.eulerAngles.y.ToString());

                transform.Rotate(move, 0.0f, 0.0f);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && ((x < 40) || (x > 310)))
            {
                Debug.Log("Angulos x,y: " + transform.eulerAngles.x.ToString() + " , " + transform.eulerAngles.y.ToString());

                transform.Rotate(-move, 0.0f, 0.0f);
            }
        }
    }

    public void PosicionarPaciente()
    {
        Debug.Log("Angulos x,y: " + transform.eulerAngles.x.ToString() + " , " + transform.eulerAngles.y.ToString());
        if (GameManager.instancia.posicaoPaciente == GameManager.Posicao.Sentada)
            transform.eulerAngles = angulosPosicaoSentada;
        else
            transform.eulerAngles = angulosPosicaoDeitadaEsquerda;
    }
}
