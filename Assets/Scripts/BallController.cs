using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Script which controls the ball
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    public static BallController instance;                 

    [SerializeField] private LineRenderer lineRenderer;     //reference to lineRenderer child object
    [SerializeField] private float MaxForce;                //maximum force that an be applied to ball
    [SerializeField] private float forceModifier = 0.5f;    //multipliers of force
    [SerializeField] private GameObject areaAffector;       //reference to sprite object which show area around ball to click
    [SerializeField] private LayerMask rayLayer;            //layer allowed to be detected by ray


    [SerializeField] private GameObject ball1,ball2,hole1,hole2;



    private float force;                                    //actuale force which is applied to the ball
    private Rigidbody rgBody;                               //reference to rigidbody attached to this gameobject
    /// <summary>
    /// The below variables are used to decide the force to be applied to the ball
    /// </summary>
    private Vector3 startPos, endPos;
    private bool canShoot = false, ballIsStatic = true;    //bool to make shooting stopping ball easy
    private Vector3 direction;                              //direction in which the ball will be shot

    private void Awake()
    {
        rgBody = GetComponent<Rigidbody>();                 //get reference to the rigidbody
    }

    // Update is called once per frame
    void Update()
    {
        if (rgBody.velocity == Vector3.zero && !ballIsStatic)   //if velocity is zero and ballIsStatic is false
        {
            ballIsStatic = true;                                //set ballIsStatic to true
            rgBody.angularVelocity = Vector3.zero;              //set angular velocity to zero
            UIManager.instance.ShotTaken();                  //inform LevelManager of shot taken
            areaAffector.SetActive(true);                       //activate areaAffector
        }

        if (Input.GetMouseButtonDown(0) && !canShoot)
        {
            startPos = ClickedPoint();
            lineRenderer.gameObject.SetActive(true);
            lineRenderer.SetPosition(0, lineRenderer.transform.localPosition);
        }

        if (Input.GetMouseButton(0))
        {
            endPos = ClickedPoint();
            force = Mathf.Clamp(Vector3.Distance(endPos,startPos) * forceModifier, 0, MaxForce);
            UIManager.instance.PowerBar.fillAmount = force / MaxForce;
            lineRenderer.SetPosition(1, transform.InverseTransformPoint(endPos));
        }

        if (Input.GetMouseButtonUp(0))
        {
            canShoot = true;
            lineRenderer.gameObject.SetActive(false);
        }
    }

    

    private void FixedUpdate()
    {
        if (canShoot)                                               //if canSHoot is true
        {
            canShoot = false;                                       //set canShoot to false
            ballIsStatic = false;                                   //set ballIsStatic to false
            direction = startPos - endPos;                          //get the direction between 2 vectors from start to end pos
            rgBody.AddForce(direction * force, ForceMode.Impulse);  //add force to the ball in given direction
            areaAffector.SetActive(false);                          //deactivate areaAffector
            UIManager.instance.PowerBar.fillAmount = 0;             //reset the powerBar to zero
            force = 0;                                              //reset the force to zero
            startPos = endPos = Vector3.zero;                       //reset the vectors to zero
        }
    }

    // Unity native Method to detect colliding objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Destroyer")                              //if the object name is Destroyer
        {
            SceneManager.LoadScene("GameOverwithRetry");
            // LevelManager.instance.LevelFailed();                    //Level Failed
        }
        else if (other.name =="Hole1")                              //if the object name is Hole
        {
            SceneManager.LoadScene("GameOver");
            // LevelManager.instance.LevelComplete();                  //Level Complete
        }
    }

    // private void OnCollisionEnter(Collision col){

    //     if (col.gameObject.name == "Hole1")
    //     {
    //         SceneManager.LoadScene("level2");
    //     }

    // }



    // public void MouseDownMethod()                                           //method called on mouse down by InputManager
    // {
    //     if(!ballIsStatic) return;                                           //no mouse detection if ball is moving
    //     startPos = ClickedPoint();                                          //get the vector in word space
    //     lineRenderer.gameObject.SetActive(true);                            //activate lineRenderer
    //     lineRenderer.SetPosition(0, lineRenderer.transform.localPosition);  //set its 1st position
    // }

    // public void MouseNormalMethod()                                         //method called by InputManager
    // {
    //     if(!ballIsStatic) return;                                           //no mouse detection if ball is moving
    //     endPos = ClickedPoint();                                                //get the vector in word space
    //     force = Mathf.Clamp(Vector3.Distance(endPos, startPos) * forceModifier, 0, MaxForce);   //calculate the force
    //     UIManager.instance.PowerBar.fillAmount = force / MaxForce;              //set the powerBar image fill amount
    //     //we convert the endPos to local pos for ball as lineRenderer is child of ball
    //     lineRenderer.SetPosition(1, transform.InverseTransformPoint(endPos));   //set its 1st position
    // }

    // public void MouseUpMethod()                                             //method called by InputManager
    // {
    //     if(!ballIsStatic) return;                                           //no mouse detection if ball is moving
    //     canShoot = true;                                                    //set canShoot true
    //     lineRenderer.gameObject.SetActive(false);                           //deactive lineRenderer
    // }

    // /// <summary>
    // /// Method used to convert the mouse position to the world position in respect to Level
    // /// </summary>
    Vector3 ClickedPoint()
    {
        Vector3 position = Vector3.zero;                                //get a new Vector3 varialbe
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //create a ray from camera in mouseposition direction
        RaycastHit hit = new RaycastHit();                              //create a RaycastHit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, rayLayer))    //check for the hit 
        {
            position = hit.point;                                       //save the hit point in position
        }
        return position;                                                //return position
    }

}