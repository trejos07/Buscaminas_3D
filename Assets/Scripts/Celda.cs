using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Celda : MonoBehaviour {

    Collider collider;
    MeshRenderer meshRenderer;
    Color defaultColor;
    TextMesh text;

    public static int celId=0;
    public int id;

    State state = State.Oculta;
    bool bomb = false;
    int bombsArround=0;
    List <Celda> celdasVecinas;



    private void Start()
    {
        SwipeRotation.OnRevelarCelda += Revelar;
        SwipeRotation.OnMarcarCelda += MarcarCelda;
        collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        defaultColor = meshRenderer.material.color;
        text = GetComponentInChildren<TextMesh>();
        text.text = "";
    }


    #region accesores
    
    public bool Bomb
    {
        get
        {
            return bomb;
        }

        set
        {
            bomb = value;
        }
    }

    public State State1
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    public List<Celda> CeldasVecinas
    {
        get
        {
            return celdasVecinas;
        }

        set
        {
            celdasVecinas = value;
        }
    }




    #endregion

    public delegate void BombActive();
    public static event BombActive OnBombActive;

    public delegate void BombDesactive();
    public static event BombDesactive OnBombDesactive;

    private void Update()
    {
        text.transform.LookAt(FindObjectOfType<Camera>().transform.position);
    }

    int CalcularBombasCercanas()
    { 
        int BombsArround = 0;
        for (int i = 0; i < celdasVecinas.Count; i++)
        {
            if (celdasVecinas[i].bomb && celdasVecinas[i].State1 != State.Marcada)
                BombsArround++;
        }

        if(BombsArround>0)
            state = State.Descubierta;
        else
            state = State.Desactivada;
        return BombsArround;


    }

    public void Revelar(Celda celda)
    {
        if (celda.GetInstanceID() == GetInstanceID())
        {
            if(state == State.Oculta)
            {
                print("estudiando celda :" + name);
                if (bomb)
                {
                    print("Game Over");
                    if(OnBombActive!= null)
                    {
                        OnBombActive();
                    }
                }

                    
                else
                {
                    print("no tengo bomba estoy calculando las bombas cercanas");
                    bombsArround = CalcularBombasCercanas();
                    ActualizarCelda();
                    ExpandirBusqueda();
                    print("tengo :" + bombsArround + " bombas al rededor");

                }
            }
            if(state== State.Descubierta)
            {
                bombsArround = CalcularBombasCercanas();
                ActualizarCelda();
            }

        }
    }

    void ActualizarCelda() //grafico 
    {
        switch (state)
        {
            case State.Descubierta:
                switch (bombsArround)
                {
                    case 0:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = new Color(0, 0, 0, 0.0f);
                        text.text = bombsArround.ToString();
                        break;
                    case 1:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = new Color (1,0,0,1f);
                        text.text = bombsArround.ToString();
                        break;
                    case 2:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = new Color(0, 1, 0, 1f);
                        text.text = bombsArround.ToString();
                        break;
                    case 3:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = new Color(0, 0, 1, 1f);
                        text.text = bombsArround.ToString();
                        break;
                    case 4:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = new Color(1, 0, 1, 1f);
                        text.text = bombsArround.ToString();
                        break;
                    case 5:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = new Color(1, 1, 0, 1f);
                        text.text = bombsArround.ToString();
                        break;
                    case 6:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = new Color(0, 1, 1, 1f);
                        text.text = bombsArround.ToString();
                        break;

                    default:
                        collider.enabled = false;
                        meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                        text.color = Random.ColorHSV(0, 1, 0, 1, 0, 1, 1f, 1f);
                        text.text = bombsArround.ToString();
                        break;
                }

                break;

            case State.Desactivada:
                collider.enabled = false;
                meshRenderer.material.color = new Color(0, 0, 0, 0.0f);
                text.color = new Color(0, 0, 0, 0.0f);
                text.text = bombsArround.ToString();
                break;

            case State.Marcada:
                meshRenderer.material.color = Color.red;
                break;

            case State.Oculta:
                meshRenderer.material.color = defaultColor;
                break;
        }

        
    }

    void ExpandirBusqueda()
    {
        for (int i = 0; i < celdasVecinas.Count; i++)
        {
            Celda celda = celdasVecinas[i];
            if (!celda.bomb && (celda.state == State.Oculta || celda.state == State.Descubierta))
            {
                celda.state = State.Descubierta;
                celda.Revelar(celda);
            }
                
        }
    }

    void MarcarCelda(Celda celda)
    {
        if(celda.GetInstanceID() == GetInstanceID())
        {

            if (state == State.Marcada)
            {
                print("se desmarco una celda");
                state = State.Oculta;
                ActualizarCelda();
                return;
            }

            if (state== State.Oculta && Level.instance.CanMark())
            {
                if(bomb)
                {
                    print("se marco una celda con bomba");
                    state = State.Marcada;
                    bomb = false;
                    ActualizarCelda();
                    ExpandirBusqueda();

                    StartCoroutine(DesarmarBomba(celda));

                }
                else 
                {
                    print("se marco una celda sin bomba");
                    state = State.Marcada;
                    ActualizarCelda();
                }
            }
            
            
            else
            {
                print("se marco cualquier celda ");
                
            }
        }

    }

    IEnumerator DesarmarBomba(Celda celda)
    {
        yield return new WaitForSeconds(2f);
        state = State.Descubierta;
        Revelar(celda);
        if (OnBombDesactive != null)
            OnBombDesactive();
    }

    public enum State{Descubierta, Desactivada, Marcada, Oculta }



    

}
