using UnityEngine;
using System.Collections;

public class Traceback : MonoBehaviour {
	public string nomeArquivo = "Log2.txt";
	public GameObject objeto;
	private Arquivo cpArquivo;
	private bool chegou, primeiroDestino, criarpontos;
	private Vector3 proximoDestino, direcaoDestino;
	public GameObject waypoint;
	private Vector3 direcaoRotacao, torqueRotacao;
	public bool ativo;

	// Use this for initialization
	void Start () {
		cpArquivo = new Arquivo ();
		cpArquivo.AbrirLeitura (nomeArquivo);
		chegou = true;
		primeiroDestino = true;
		criarpontos = true;
		proximoDestino = new Vector3 ();
		direcaoRotacao = new Vector3 ();
		torqueRotacao = new Vector3 ();

        //objeto = GameObject.Find("PontaAgulha");
        //waypoint = GameObject.Find("WayPoint");
    }
	
	// Update is called once per frame
	void Update () {
		if (ativo) {
			
			string linha;
			string[] valores;
		 
			// Lendo a próxima linha do arquivo de log
			if (!cpArquivo.FimArquivo () && chegou) {
				chegou = false;
				linha = cpArquivo.LerLinha ();
				valores = linha.Split (';');
				// Definindo posição do próximo destino, direcao e torque
				float.TryParse (valores [6], out proximoDestino.x);
				float.TryParse (valores [7], out proximoDestino.y);
				float.TryParse (valores [8], out proximoDestino.z);

				float.TryParse (valores [9], out direcaoRotacao.x);
				float.TryParse (valores [10], out direcaoRotacao.y);
				float.TryParse (valores [11], out direcaoRotacao.z);

				float.TryParse (valores [12], out torqueRotacao.x);
				float.TryParse (valores [13], out torqueRotacao.y);
				float.TryParse (valores [14], out torqueRotacao.z);

				// identificando o destino
				if (criarpontos) {
					Instantiate (waypoint, proximoDestino, Quaternion.identity);
				}

				direcaoDestino = objeto.transform.position - proximoDestino;

				// posicionando o objeto no primeiro ponto definido
				if (primeiroDestino) {
					objeto.transform.position = proximoDestino;
					primeiroDestino = false;
					chegou = true;
				}

				// rotacionando o objeto nas coordenadas definidas
				objeto.transform.rotation = Quaternion.LookRotation (direcaoRotacao, torqueRotacao);
			}

			// Movimentando até o próximo destino
			if (!chegou && !cpArquivo.FimArquivo ()) {
                objeto.transform.position -= direcaoDestino.normalized * Time.deltaTime;

				if (Vector3.Distance (objeto.transform.position, proximoDestino) < 0.02f) {
					chegou = true;
				}
			}

			if (cpArquivo.FimArquivo ()) {
				cpArquivo.InicioArquivo ();
				primeiroDestino = true;
				criarpontos = false;
			}
		}
	}

	void OnApplicationQuit()
	{
		if (cpArquivo != null) cpArquivo.FecharLeitura ();
	}
}
