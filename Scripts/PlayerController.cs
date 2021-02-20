using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 lastMousePos;
    public float sensivitiy = 0.16f, clampDelta = 42f;

    public float bounds=5;
    
    [HideInInspector]
    public bool canMove = false , gameOver , finish;

    
    private int maxLevel = 2;

    public GameObject breakablePlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

   
    void Start()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            PlayerPrefs.SetInt("Level", 1);
        }
    }

    private void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -bounds, bounds),transform.position.y,transform.position.z);

        if(canMove)
        {
            transform.position += FindObjectOfType<CameraMovement>().camVel;
        }
        else if(Input.GetMouseButton(0) && !gameOver && !finish)
        {
            FindObjectOfType<GameManager>().RemoveUI();
            canMove = true;
        }

        if(gameOver)
        {
            if(Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        if(rb.velocity.magnitude <10)
        {
            rb.mass = 50;
        }
        else if(rb.velocity.magnitude > 10)
        {
            rb.mass = 75;
        }
        else if(rb.velocity.magnitude > 20)
        {
            rb.mass = 100;
        }
    }


    private void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }
        
        if(Input.GetMouseButton(0) && canMove )
        {
            Vector3 vector = lastMousePos - Input.mousePosition;
            lastMousePos = Input.mousePosition;
            vector = new Vector3(vector.x, 0, vector.y);

            Vector3 moveForce = Vector3.ClampMagnitude(vector, clampDelta);
            rb.AddForce(-moveForce * sensivitiy - rb.velocity / 5, ForceMode.VelocityChange);
            
        }

        rb.velocity.Normalize();

    }

    private void GameOver()
    {

        GetComponentInChildren<TrailRenderer>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        canMove = false;
        gameOver = true;

        GameObject shatterShpere = Instantiate(breakablePlayer, transform.position, Quaternion.identity);
        foreach (Transform o in shatterShpere.transform)
        {
            o.GetComponent<Rigidbody>().AddForce(Vector3.forward * rb.velocity.magnitude, ForceMode.Impulse);
        }

        Time.timeScale = .3f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Enemy" && !gameOver)
        {
            GameOver();
        }

    }


    IEnumerator NextLevel()
    {
        
        finish = true;
        canMove = false;
        if (PlayerPrefs.GetInt("Level",1) >= maxLevel)
        {
            Debug.Log("You reached the max level");
        }
        else
        {
            yield return new WaitForSeconds(1);
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
            SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("Level"));
        }
         
      
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name == "Finish")
        {
           
            StartCoroutine(NextLevel());
        }
    }

}
