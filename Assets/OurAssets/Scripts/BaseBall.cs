using HoloToolkit.Unity.InputModule;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class BaseBall : MonoBehaviour, IInputClickHandler
{
    //Variables
    private GameController gameController;
    private AudioSource[] objectSound;
	public AudioSource impactSource;
	public AudioClip[] impacts; //array of different impact sfx for ball collisions]


    // Use this for initialization
    void Start()
    {
        //Get GameController
        GameObject gamecontroller = GameObject.FindGameObjectWithTag("GameController");
        gameController = gamecontroller.GetComponent<GameController>();

        //Get AudioSources
		objectSound = gameObject.GetComponents<AudioSource>();
		objectSound[0].pitch = Random.Range (1.1f, 0.9f);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Respawn")
        {
            gameController.Respawn(gameObject);
            return;
        }

        var center = gameObject.GetComponent<SphereCollider>().center + gameObject.transform.position;
        var impactPoint = collider.ClosestPointOnBounds(center);
        var dir = impactPoint - center;
        dir.Normalize();
        RaycastHit hit;
        if (dir.magnitude == 0)
            return;
        if (collider.Raycast(new Ray(center, dir), out hit, 100))
        {
            try
            {
                if (collider.Raycast(new Ray(center, dir), out hit, 10))
                {
                    Collide(hit);
                }
            }
            catch (MissingReferenceException e)
            {
                Debug.Log("Exception: " + e.Message);
            }
        }
		//play random impact sound with random pitch
		impactSource.pitch = Random.Range (1.2f, 0.8f);
		impactSource.clip = impacts [Random.Range (0, 3)];
		impactSource.Play();
    }

    protected virtual void Collide(RaycastHit hit)
    {
        var normal = GetCollisionNormal(hit);

        // Now reflect the object's velocity around that normal to have it bounce off
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(gameObject.GetComponent<Rigidbody>().velocity, normal);


        if (hit.collider.gameObject.tag == "Player")
        {
            HitPlayer();
        }
    }

    protected virtual void HitPlayer()
    {
        Debug.Log("Hit the player!");
        // I hit a player, better kill him
        gameController.KillPlayer();
        // Then destroy the ball so it doesn't instantly kill him more
        Destroy(gameObject);
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        Destroy(gameObject);
        //var vel = gameObject.GetComponent<Rigidbody>().velocity;
        //gameObject.GetComponent<Rigidbody>().velocity = new Vector3(vel.x, -vel.y, vel.z); 
    }

    private Vector3 GetCollisionNormal(RaycastHit hit)
    {
        if (hit.collider is SphereCollider)
        {
            return GetSphereNormal(hit);
        }
        
        MeshCollider collider = (MeshCollider)hit.collider;
        Mesh mesh = collider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

        Vector3 baryCenter = hit.barycentricCoordinate;
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        interpolatedNormal.Normalize();
        interpolatedNormal = hit.transform.TransformDirection(interpolatedNormal);
        return interpolatedNormal;
    }

    private Vector3 GetSphereNormal(RaycastHit hit)
    {
        var center = ((SphereCollider)hit.collider).center + hit.collider.gameObject.transform.position;
        var dir = hit.point - center;
        dir.Normalize();
        return dir;
    }
}
