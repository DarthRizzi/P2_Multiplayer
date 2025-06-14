using UnityEngine;

public class HockeyPuckController : MonoBehaviour
{
    [Header("Configurações do Disco")]
    public float initialForce = 5f;
    public float bounceForce = 5f;
    [Range(0f, 1f)] public float randomnessIntensity = 0.3f; // De 0 (nenhum) a 1 (máximo)
    public float resetDelay = 1f;
    public Vector3 resetPosition = new Vector3(0, 0.062f, 0);

    private Rigidbody rb;
    private bool isResetting = false;

    public ScoreManager scoreManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ApplyRandomForce();
    }

    void ApplyRandomForce()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(randomDirection * initialForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // 1. Calcula a direção refletida principal
            Vector3 reflectedDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);

            // 2. Adiciona um componente aleatório que NÃO contradiz a direção principal
            Vector3 randomVariation = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            ).normalized * randomnessIntensity;

            // 3. Combina as direções garantindo que o movimento principal prevaleça
            Vector3 finalDirection = (reflectedDir + randomVariation).normalized;

            rb.linearVelocity = Vector3.zero;
            rb.AddForce(finalDirection * bounceForce, ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("LeftGoal") || collision.gameObject.CompareTag("RightGoal") && !isResetting)
        {
            if(collision.gameObject.CompareTag("LeftGoal"))
            {
                scoreManager.AddRightScore(1);
            }
            else
            {
                scoreManager.AddLeftScore(1);
            }
            StartCoroutine(ResetPuck());
        }
    }

    System.Collections.IEnumerator ResetPuck()
    {
        isResetting = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = resetPosition;
        
        yield return new WaitForSeconds(resetDelay);
        
        ApplyRandomForce();
        isResetting = false;
    }
}