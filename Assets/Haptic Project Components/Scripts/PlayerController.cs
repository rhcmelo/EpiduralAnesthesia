using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public GameObject player;
    public Text countText;
    public Text winText;
    public Text actionText;
    public Text resultText;

    public Sprite manPlayerImage;
    public Sprite womanPlayerImage;

    public float speed = 0.2f;
    public KeyCode keyToMoveUp;
    public KeyCode keyToMoveDown;
    public KeyCode keyToMoveLeft;
    public KeyCode keyToMoveRight;

    public AudioClip pickupSound;
    public AudioClip finishOkSound;
    public AudioClip finishWrongSound;
    private AudioSource source;

    //private Rigidbody2D  rb2d;

    private int count;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Init()
    {
        count = 0;
        SetTextInfo();
        winText.text = "";
        actionText.text = "";
        

        GameSetup.instance.InitGame();
        GameSetup.instance.SendMessage("Restart");

        player.transform.position = new Vector3(0f, 0f, 0f);
    }

    // Use this for initialization
    void Start ()
    {
        player.GetComponent<SpriteRenderer>().sprite = manPlayerImage;

        //rb2d = GetComponent<Rigidbody2D>();

        Init();
    }

    public void ChangeAvatar()
    {
        if (player.GetComponent<SpriteRenderer>().sprite == manPlayerImage)
            player.GetComponent<SpriteRenderer>().sprite = womanPlayerImage;
        else
            player.GetComponent<SpriteRenderer>().sprite = manPlayerImage;
    }

    // Update is called once per frame
    void Update ()
    {        
        if (count < 4)
        {
            if (Input.GetKey(keyToMoveUp))
            {
                player.transform.Translate(0, speed, 0);
            }
            if (Input.GetKey(keyToMoveDown))
            {
                player.transform.Translate(0, -speed, 0);
            }
            if (Input.GetKey(keyToMoveLeft))
            {
                player.transform.Translate(-speed, 0, 0);
            }
            if (Input.GetKey(keyToMoveRight))
            {
                player.transform.Translate(speed, 0, 0);
            }
        }
    }

    //OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
    void OnTriggerEnter2D(Collider2D other)
    {
        //Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);

            count++;

            SetTextInfo(other.gameObject.name);

            GameSetup.instance.PickedObject(other.gameObject);            

            if (count >= 4)
            {
                winText.text = "Fim do jogo";
                GameSetup.instance.SendMessage("Finnish");
                GameSetup.instance.Finish();

                //resultText.text = GameSetup.instance.GetResultText();
                if (GameSetup.instance.GetResultOk())
                    source.PlayOneShot(finishOkSound, 0.4f);
                else
                    source.PlayOneShot(finishWrongSound, 0.4f);
            }
            else
                source.PlayOneShot(pickupSound, 0.4f);            
        }
    }

    void SetTextInfo(string name = "")
    {
        if (countText != null)
            countText.text = "Ações efetuadas: " + count.ToString();

        if (name != "")
        {
            if (actionText.text == "")
                actionText.text = count.ToString() + ". " + name;
            else
                actionText.text += "\n" + count.ToString() + ". " + name;
        }
    }
}
