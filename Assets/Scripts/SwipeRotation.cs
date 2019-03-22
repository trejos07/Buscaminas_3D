using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeRotation : MonoBehaviour {

    Vector2 starTouch, swipeDelta,secondTouchPos;
    bool isDragging, tap, rotated, marcar = false;
    Camera camera;


    float rotX = 0f;
    float rotY = 0f;
    [SerializeField]
    Vector3 origRot;
    [SerializeField]
    Vector3 CameraPos;
    [SerializeField]
    float zoomModifierSpeed =0.1f;

    [SerializeField]
    float rotSpeed = 5f;

    float touchesPrevPosDiference, touchesCurPosDiference, zoomModifier;

    public delegate void RevelarCelda(Celda celda);
    public static event RevelarCelda OnRevelarCelda;

    public delegate void MarcarCelda(Celda celda);
    public static event MarcarCelda OnMarcarCelda;


    // Use this for initialization
    void Start () {

        BotonMarcar.OnMarcarButtonDown += invertMarker;
        OnMarcarCelda += invertMarker;
        camera = GetComponentInChildren<Camera>();
        camera.transform.position = CameraPos;
        origRot = transform.eulerAngles;
        rotX = origRot.x;
        rotY = origRot.y;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            print("onWindows");
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            print("on Android");
        }
    }

    private void Update()
    {

        #region PcInputs
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
            {
                camera.transform.localPosition = new Vector3(0, 0, Mathf.Clamp(camera.transform.localPosition.z + Input.GetAxis("Mouse ScrollWheel") * zoomModifierSpeed, -30, -1));
                //camera.orthographicSize = Mathf.Clamp(camera.orthographicSize + Input.GetAxis("Mouse ScrollWheel"), 1, 20);
            }


            if (Input.GetMouseButtonDown(0))
            {
                tap = true;
                isDragging = true;
                starTouch = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (!rotated)
                {
                    Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100))
                    {
                        if (hit.collider.GetComponentInParent<Celda>())
                        {
                            print("se Revela una celda");
                            if (OnRevelarCelda != null)
                                OnRevelarCelda(hit.collider.GetComponent<Celda>());
                        }
                    }
                }
                isDragging = false;
                Reset();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.GetComponent<Celda>())
                    {
                        print("se Marca una celda");
                        if (OnMarcarCelda != null)
                            OnMarcarCelda(hit.collider.GetComponent<Celda>());
                    }
                }
                isDragging = false;
                Reset();
            }
        }

        #endregion

        #region MobileInputs
        if (Application.platform == RuntimePlatform.Android)
        {

            if (Input.touchCount == 2)
            {
                Touch firstTouch = Input.touches[0];
                Touch secondTouch = Input.touches[1];

                starTouch = firstTouch.position - firstTouch.deltaPosition;
                secondTouchPos = secondTouch.position - secondTouch.deltaPosition;

                touchesPrevPosDiference = (starTouch - secondTouchPos).magnitude;
                touchesCurPosDiference = (firstTouch.position - secondTouch.position).magnitude;

                zoomModifier = (firstTouch.deltaPosition - secondTouch.deltaPosition).magnitude * zoomModifierSpeed;

                if (touchesPrevPosDiference > touchesCurPosDiference)
                    camera.transform.localPosition = new Vector3(0, 0, Mathf.Clamp(camera.transform.localPosition.z + zoomModifier, -30, -1));

                if (touchesPrevPosDiference < touchesCurPosDiference)
                    camera.transform.localPosition = new Vector3(0, 0, Mathf.Clamp(camera.transform.localPosition.z - zoomModifier, -30, -1));


            }//zoom


            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    tap = true;
                    starTouch = touch.position;
                    isDragging = true;
                }//Inicio del toque


                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)//fin de toque
                {
                    if (marcar)
                    {
                        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 100))
                        {
                            if (hit.collider.GetComponent<Celda>())
                            {
                                print("se Marca una celda");
                                if (OnMarcarCelda != null)
                                    OnMarcarCelda(hit.collider.GetComponent<Celda>());
                            }
                        }
                        isDragging = false;
                        Reset();
                    }//marcar celda
                    else
                    {
                        if (!rotated)
                        {
                            Ray ray = camera.ScreenPointToRay(Input.touches[0].position);
                            RaycastHit hit;

                            if (Physics.Raycast(ray, out hit, 100))
                            {
                                if (hit.collider.GetComponent<Celda>())
                                {
                                    print("se Revela una celda");
                                    if (OnRevelarCelda != null)
                                        OnRevelarCelda(hit.collider.GetComponent<Celda>());
                                }
                            }
                        }
                        isDragging = false;
                        Reset();
                    }//revelar celda

                }
            }
        }
        #endregion

        // Calcular La Distancia

        swipeDelta = Vector2.zero;
        if (isDragging)
        {
            if (Input.touches.Length > 0)
                swipeDelta = Input.touches[0].position - starTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - starTouch;

        }

        if(swipeDelta.magnitude>10)
        {
            rotX += swipeDelta.x * rotSpeed *  Time.deltaTime;
            rotY -= swipeDelta.y * rotSpeed *  Time.deltaTime;
            transform.eulerAngles = new Vector3(rotY, rotX, 0);
            rotated = true;

        }
    }

    private void Reset()
    {
        starTouch = swipeDelta = Vector2.zero;
        isDragging = false;
        rotated = false;
    }

    void invertMarker(Celda celda)
    {
        marcar = !marcar;
    }

}
