using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GhostMovement : MonoBehaviour
{
    public Transform target;
    public GameObject gameOverUI;
    public GameObject[] heartImages;
    public float damageCooldown = 5f;

    private NavMeshAgent agent;
    private int currentLives;
    private float lastDamageTime;
    private bool isFleeing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentLives = heartImages.Length;

        if (gameOverUI != null) gameOverUI.SetActive(false);
    }

    void Update()
    {
        if (target == null || Time.timeScale <= 0) return;

        if (!isFleeing)
        {
            agent.SetDestination(target.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time > lastDamageTime + damageCooldown)
        {
            currentLives--;
            lastDamageTime = Time.time;

            if (currentLives >= 0 && currentLives < heartImages.Length)
            {
                heartImages[currentLives].SetActive(false);
            }

            if (currentLives <= 0)
            {
                if (gameOverUI != null) gameOverUI.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                StartCoroutine(FleeRoutine());
            }
        }
    }

    IEnumerator FleeRoutine()
    {
        isFleeing = true;

        Vector3 randomDirection = Random.insideUnitSphere * 20f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 20f, 1);
        Vector3 finalPosition = hit.position;

        agent.SetDestination(finalPosition);
        agent.speed = 3f;

        yield return new WaitForSeconds(damageCooldown);

        agent.speed = 5f;
        isFleeing = false;
    }
}
