using UnityEngine;
using System.Collections;
using System.IO;

public class Arquivo {
	private string nome;
	private StreamWriter arquivo;
	private StreamReader arquivoleitura;

	public bool FimArquivo()
	{
		return arquivoleitura.EndOfStream;
	}

	public void Abrir(string nomeArquivo)
	{
		nome = nomeArquivo;
		arquivo = new StreamWriter (nomeArquivo);
	}

	public void AbrirLeitura(string nomeArquivo)
	{
		nome = nomeArquivo;
		arquivoleitura = new StreamReader (nomeArquivo);
	}

	public void InicioArquivo()
	{
		arquivoleitura.BaseStream.Position = 0;
	}

	public void Fechar()
	{
		arquivo.Close ();
	}

	public void FecharLeitura()
	{
		arquivoleitura.Close ();
	}

	public void GravarLinha (string linha)
	{
		arquivo.WriteLine (linha);		
	}

	public string LerLinha ()
	{
		return arquivoleitura.ReadLine ();
	}

	public void Copiar(string destino)
	{
		File.Copy (nome, destino, true);
	}

	/*

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnApplicationQuit()
	{
		Fechar ();
	}

	*/
}
