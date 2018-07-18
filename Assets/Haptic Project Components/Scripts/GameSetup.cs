using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour {

    public static GameSetup instance = null;

    public Text resultText;
    private bool gameResultOk = false;

    public GameObject[] elementos;
    private List<int> indicesElementos = new List<int>();
    private List<Vector3> transformacao = new List<Vector3>(); // posicionar objetos de acordo transformações em relação ao ponto central

    private List<GameObject> pickedObjects = new List<GameObject>();

    public Button nextButton;

    bool isRunning = true;

    public bool IsRunning()
    {
        Debug.Log("GameSetup isRunning " + isRunning);

        return isRunning;
        
    }

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);
    }

    public void InitGame()
    {
        indicesElementos.Clear();
        for (int i = 0; i < elementos.Length; i++)
        {
            indicesElementos.Add(i);
            elementos[i].SetActive(true);
        }

        Vector3 t1;
        t1 = new Vector3(-15f, 0f, 0f);
        transformacao.Add(t1);
        t1.Set(15f, 0f, 0f);
        transformacao.Add(t1);
        t1.Set(0F, 15f, 0f);
        transformacao.Add(t1);
        t1.Set(0F, -15f, 0f);
        transformacao.Add(t1);

        for (int i = 0; i < elementos.Length; i++)
        {
            int randomIndex = Random.Range(0, indicesElementos.Count);
            // aplicar transformacao
            elementos[indicesElementos[randomIndex]].transform.position = transformacao[i];

            indicesElementos.RemoveAt(randomIndex);
        }

        pickedObjects.Clear();

        if(resultText != null)
            resultText.text = "";

        gameResultOk = false;

        isRunning = true;
    }

	// Use this for initialization
	void Start () {
        InitGame();

    }
    
	// Update is called once per frame
	void Update ()
    {
        /*if(Input.GetKeyDown(KeyCode.P))
        {
            ChangeLevel();
        }*/

        nextButton.interactable = gameResultOk;
    }

    public bool GetResultOk()
    {
        return gameResultOk;
    }

    public void PickedObject(GameObject elementPicked)
    {
        pickedObjects.Add(elementPicked);
    }

    public void Finish()
    {
        isRunning = false;

        bool wrongOrder = false;
        for (int i = 0; i < pickedObjects.Count; i++)
        {
            if(pickedObjects[i].GetComponent<ChoiceOrder>().choiceOrder > (i + 1) )
                wrongOrder = true;
        }

        if (!wrongOrder)
        {
            resultText.text = "Parabéns!!!\nVocê executou as atividades na ordem correta.";
            resultText.color = Color.green;

            gameResultOk = true;
        }
        else
        {
            int orderPrevElement = pickedObjects[0].GetComponent<ChoiceOrder>().choiceOrder;
            string message = "";
            for (int i = 1; i < pickedObjects.Count; i++)
            {
                if (orderPrevElement > pickedObjects[i].GetComponent<ChoiceOrder>().choiceOrder)
                {
                    message = "Atenção, você deve '" + pickedObjects[i].name + "' antes de '" + pickedObjects[i - 1].name + "'.\nTente novamente!";
                    break;
                }
                orderPrevElement = pickedObjects[i].GetComponent<ChoiceOrder>().choiceOrder;
            }
            if (message == "")
                resultText.text = "Você errou na ordem de exeução das atividades. Tente novamente!";
            else
                resultText.text = message;

            resultText.color = Color.white;

            gameResultOk = false;
        }
    }
}
