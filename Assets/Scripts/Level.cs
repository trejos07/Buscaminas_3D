using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Level : MonoBehaviour {

    public static Level instance;

    public GameObject celdaPrefab;

    //parametros del nivel 
    [SerializeField]
    [Range(3, 30)] int tamaño;
    [SerializeField]
    [Range(1, 2)] float tamañoCeldas=1.5f;
    [SerializeField]
    Forma formaMapa = Forma.Cubo;
    [SerializeField]
    Dificultad dificultad = Dificultad.Facil;

    [SerializeField] // GUI Info del juego
    Text txtBombas, txtTime, txtMarcadres;

    float tiempo = 0;
    int bombas;
    int marcadores;

    Celda [,,] celdas;

    public delegate void Win();
    public event Win OnWin;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    public void Reiniciar()
    {
        Reset();

        if (formaMapa == Forma.Cubo)
            GetCeldas();
        else if (formaMapa == Forma.Esfera)
            GetCeldas();
    }

    public void init( int _tamaño, string _forma, string _dificultad)
    {

        
        tamaño = _tamaño;
        formaMapa = (Forma)Enum.Parse(typeof(Forma), _forma); 
        dificultad = (Dificultad)Enum.Parse(typeof(Dificultad), _dificultad);
        celdas = new Celda[tamaño, tamaño, tamaño];
        Reset();


        if (formaMapa == Forma.Cubo)
            GetCeldas();
        else if (formaMapa == Forma.Esfera)
            GetCeldas();

        
    }

    private void Reset()
    {
        tiempo = 0;
        bombas = 0;
        marcadores = 0;

        Transform[] transfors = GetComponentsInChildren<Transform>();
        for (int i = 1; i < transfors.Length; i++)
        {
            Destroy(transfors[i].gameObject);
        }

    }

    // Use this for initialization
    void Start () {
        SwipeRotation.OnMarcarCelda += GastarMarcador;
        SwipeRotation.OnRevelarCelda += OnrevelarCelda;
        MainMenuGui.instance.OnStartGame += init;
        Celda.OnBombDesactive += ContarBombas;

    }

    private void Update()
    {
        
        UpdateUi();
    }

    void UpdateUi()
    {
        tiempo += Time.deltaTime;
        txtBombas.text = bombas.ToString();
        txtTime.text = Mathf.Round(tiempo).ToString();
        txtMarcadres.text = marcadores.ToString();

    }

    Celda createCelda(Vector3 pos)
    {
        GameObject celda  = Instantiate(celdaPrefab, pos, Quaternion.identity, transform);
        Celda.celId += 1;
        Celda cel = celda.GetComponent<Celda>();
        cel.id = Celda.celId;
        celda.name = "Celda_"+ cel.id;
        return cel;
    }

    void GetCeldas()
    {
        
        float r = (tamaño * tamañoCeldas / 2);
        int x=0 , y=0, z=0;

        for (float i = -r + tamañoCeldas / 2; i < r; i += tamañoCeldas)//x  espacio entre celdas en eje x 
        {
            for (float j = -r + tamañoCeldas / 2; j < r; j += tamañoCeldas)//y  espacio entre celdas eje Y 
            {
                for (float k = -r + tamañoCeldas / 2; k < r; k += tamañoCeldas)//z  espacio entre celdas eje z 
                {
                    if (formaMapa == Forma.Esfera)
                    {
                        float distancia2 = Mathf.Pow(i, 2) + Mathf.Pow(j, 2) + Mathf.Pow(k, 2);
                        if (distancia2 <= Mathf.Pow(r, 2))
                        {
                            celdas[x, y, z] = createCelda(new Vector3(i, j, k));
                        }
                        else
                            celdas[x, y, z] = null;
                    }
                    else if (formaMapa == Forma.Cubo)
                    {
                        celdas[x, y, z] = createCelda(new Vector3(i, j, k));
                        
                    }
                    z++;
                }
                y++;
                z = 0;
            }
            x++;
            y = 0;
            z = 0;
        }


        for (int _x = 0; _x < celdas.GetLength(0); _x++)
        {
            for (int _y = 0; _y < celdas.GetLength(1); _y++)
            {
                for (int _z = 0; _z < celdas.GetLength(2); _z++)
                {
                    List<Celda> celdasVecinas = new List<Celda>();
                    //print("soy la " + celdas[_x, _y, _z].name + "y estoy en las codenadas " + _x + " " + _y + " " + _z);
                    for (int i = _x - 1; i <= _x + 1; i++)
                    {
                        for (int j = _y - 1; j <= _y + 1; j++)
                        {
                            for (int k = _z - 1; k <= _z + 1; k++)
                            {
                                //print("busco celda en las codenadas " + i + " " + j + " " + k);
                                if (i>=0 && i<celdas.GetLength(0)&& j >= 0 && j < celdas.GetLength(1)&& k >= 0 && k < celdas.GetLength(2))
                                {
                                   // print("la cordenada existe");
                                    if (celdas[i, j, k] != null && i+j+k !=0)
                                        celdasVecinas.Add(celdas[i, j, k]);
                                }
                                
                            }
                        }
                    }

                    if(celdas[_x, _y, _z] != null)
                        celdas[_x, _y, _z].CeldasVecinas = celdasVecinas;

                }
            }

        }
       
        foreach (Celda cel in celdas)
        {
            if(cel != null)
            {
                print("soy la " + cel.name + " y mis vecinos son :");
                foreach (Celda vecina in cel.CeldasVecinas) print("      - " + vecina.name);
            }
        }
        


        CalcularBombas();
        
    }

    void CalcularBombas()
    {
        bombas = 0;
        switch (dificultad)
        {
            case Dificultad.Facil:
                 bombas = Mathf.FloorToInt(celdas.Length * 1 / 6);
                 marcadores = Mathf.FloorToInt(bombas*1.3f);
                 CreateBombs(bombas);
                break;

            case Dificultad.Medio:
                bombas = Mathf.FloorToInt(celdas.Length * 1 / 4);
                marcadores = Mathf.FloorToInt(bombas * 1.25f);
                CreateBombs(bombas);
                break;

            case Dificultad.Dificil:
                bombas = Mathf.FloorToInt(celdas.Length * 1 / 3);
                marcadores = Mathf.FloorToInt(bombas * 1.15f);
                CreateBombs(bombas);
                break;

        }

        
        
        print(string.Format("hay {0} bombas", bombas));
        
    }

    void CreateBombs(int cantidad)
    {
        
        for (int i = 0; i < cantidad; i++)
        {
            int x = UnityEngine.Random.Range(0, tamaño);
            int y = UnityEngine.Random.Range(0, tamaño);
            int z = UnityEngine.Random.Range(0, tamaño);

            if (celdas[x, y, z] != null&& !celdas[x, y, z].Bomb)
                celdas[x, y, z].Bomb = true;
            else
                i--;
        }
    }

    void ContarBombas ()
    {
        int bombasRestantes = 0;
        foreach (Celda cel in celdas)
        {
            if (cel != null)
            {
                if (cel.Bomb)
                {
                    bombasRestantes++;
                }
            }
        }
        bombas = bombasRestantes;
    }

    IEnumerator CheckVictori()
    {
        yield return new WaitForEndOfFrame();
        int bombasRestantes = 0;
        int celdasSinRevelar = 0;
        foreach (Celda cel in celdas )
        {
            if(cel != null)
            {
                if (cel.Bomb)
                {
                    if (cel.State1 != Celda.State.Marcada)
                    {
                        bombasRestantes++;
                    }

                }
                else if (cel.State1 == Celda.State.Oculta)
                    celdasSinRevelar++;
            }
        }

        print("celdas sin revelar  "+ celdasSinRevelar);
        print("bombas restantes     "+bombasRestantes);

        if (bombasRestantes == 0 && celdasSinRevelar == 0)
        {
            if (OnWin != null)
                OnWin();

            print("Ganaste");
        }
    }

    void OnrevelarCelda(Celda celda)
    {
           StartCoroutine(CheckVictori());
    }

    void GastarMarcador(Celda celda)
    {
        StartCoroutine(CheckVictori());
    }

    public bool CanMark()
    {
        if (marcadores > 0)
        {
            marcadores--;
            return true;
        }
            
        else
            return false;
    }

    public enum Forma { Cubo, Esfera }
    public enum Dificultad { Facil, Medio, Dificil}

}
