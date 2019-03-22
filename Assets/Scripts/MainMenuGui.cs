using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenuGui : MonoBehaviour {


    public static MainMenuGui instance;

    [SerializeField]
    Slider tamañoMapa;
    [SerializeField]
    Dropdown forma, dificultad;
    [SerializeField]
    Text text;

    public delegate void StartGame(int tamaño, string forma, string dif);
    public event StartGame OnStartGame;

    // Use this for initialization
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        dificultad.options.Clear();
        string[] dificultades = Enum.GetNames(typeof(Level.Dificultad));
        foreach (string dif in dificultades)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(dif);
            dificultad.options.Add(option);
        }

        forma.options.Clear();
        string[] formas = Enum.GetNames(typeof(Level.Forma));
        foreach (string form in formas)
        {
            Dropdown.OptionData option = new Dropdown.OptionData(form);
            forma.options.Add(option);
        }
    }

    void Start () {

        

       

        text.text = string.Format("{0} X {0} X {0}", tamañoMapa.value);



    }
	
	void Update () {
        text.text = string.Format("{0} X {0} X {0}", tamañoMapa.value);
    }

    public void StartGameButton()
    {
        if(OnStartGame!=null)
        {
            OnStartGame((int)tamañoMapa.value, forma.value.ToString(),dificultad.value.ToString());
        }
    }
}
