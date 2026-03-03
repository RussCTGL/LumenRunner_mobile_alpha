using UnityEngine;
using System.Collections;

public class BackwardsLaser : MonoBehaviour 
{
    [Header("Required Settings")]
    public float laserRange = 1000f;
    public LayerMask hitLayers;
    public string playerTag = "Player";
    
    [Header("Time Settings")]
    public float activeTime = 2.0f;
    public float inactiveTime = 7.0f;
    public float startDelay = 0f;
    
    [Header("Direction Settings")]
    public bool shootBackwards = true;

    private LineRenderer lineRenderer;
    private bool isLaserActive = false;
    
    private GameManager gameManager;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; 
        lineRenderer.enabled = false;
        
        gameManager = FindObjectOfType<GameManager>();
        
        StartCoroutine(LaserRoutine());
    }

    void Update()
    {
        if (isLaserActive)
        {
            FireLaser();
        }
    }

    IEnumerator LaserRoutine()
    {
        if (startDelay > 0) yield return new WaitForSeconds(startDelay);

        while (true)
        {
            isLaserActive = true;
            lineRenderer.enabled = true;
            yield return new WaitForSeconds(activeTime);

            isLaserActive = false;
            lineRenderer.enabled = false;
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    void FireLaser()
    {
        Vector3 direction = shootBackwards ? -transform.forward : transform.forward;
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, laserRange, hitLayers))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag(playerTag))
            {
                KillPlayer();
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (direction * laserRange));
        }
    }

    void KillPlayer()
    {
        Debug.Log("Player hit by laser. Triggering Game Over!");
        
        if (gameManager != null)
        {
            gameManager.EndGame();
        }
        else
        {
            Debug.LogWarning("GameManager not found! Please ensure there is a GameObject with the GameManager script in the scene.");
        }
    }
}