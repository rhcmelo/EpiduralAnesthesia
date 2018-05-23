using UnityEngine;
using System.Collections;

public class Tecido : MonoBehaviour {

	// Propriedades de pressão salina
	public float kPaSalinaMedia;
	public float kPaSalinaDesvioPadrao;

	// Propriedades de pressão do ar
	public float kPaArMedia;
	public float kPaArDesvioPadrao;

	public float espessuraMedia;
	public float profundidadeMedia;

	// Propriedades importantes ao instanciar tecido
	public float espessura;
	public float profundidade;
	public float kPaSalina;
	public float kPaAr;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
