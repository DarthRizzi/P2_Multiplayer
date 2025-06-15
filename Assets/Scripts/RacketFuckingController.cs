using Unity.Netcode;
using UnityEngine;

public class RacketFuckingController : NetworkBehaviour
{
    [Header("Referências")]
    public Vector3 pivot;

    [Header("Teclas de Movimento")]
    public KeyCode rotateLeftKey = KeyCode.A;
    public KeyCode rotateRightKey = KeyCode.D;

    [Header("Movimentação")]
    public float rotationSpeed = 90f; // graus por segundo
    public float radius = 4.5f;       // distância constante do centro
    public bool Mirror = false;       // se verdadeiro, espelha a rotação em Y

    [Header("Limites de Movimento")]
    public bool useAngleLimits = false;
    public float minAngle = 0f;
    public float maxAngle = 360f;

    private float currentAngle = 0f;  // ângulo atual em graus

    void Start()
    {
        if(!IsOwner)return;
        // Calcula ângulo inicial com base na posição inicial da raquete
        
        Vector3 dir = transform.position - pivot;
        currentAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        
        // Garante que o ângulo inicial esteja dentro dos limites
        if (useAngleLimits)
        {
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
        }
        
        // Aplica o espelhamento inicial se necessário
        UpdateRacketOrientation();
    }

    void Update()
    {
        if(!IsOwner)return;
        
        float input = 0f;

        if (Input.GetKey(rotateLeftKey)) input -= 1f;
        if (Input.GetKey(rotateRightKey)) input += 1f;

        // Calcula o novo ângulo
        float newAngle = currentAngle + input * rotationSpeed * Time.deltaTime;

        // Aplica os limites se estiverem ativados
        if (useAngleLimits)
        {
            newAngle = Mathf.Clamp(newAngle, minAngle, maxAngle);
        }
        else
        {
            // Garante que fique entre 0 e 360
            newAngle = Mathf.Repeat(newAngle, 360f);
        }

        currentAngle = newAngle;

        // Converte o ângulo em uma posição circular
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
        transform.position = pivot + offset;

        // Atualiza a orientação da raquete
        UpdateRacketOrientation();
    }
    
    void UpdateRacketOrientation()
    {
        // Faz a raquete "olhar" para o centro
        Vector3 lookDirection = (pivot - transform.position).normalized;
        
        if (Mirror)
        {
            // Inverte a direção do olhar no eixo Y para espelhar
            lookDirection = -lookDirection;
        }
        
        transform.forward = lookDirection;
    }

    // Método para desenhar os limites no Editor (apenas para visualização)
    void OnDrawGizmosSelected()
    {
        if (pivot != null && useAngleLimits)
        {
            Gizmos.color = Color.green;
            DrawAngleLimitGizmo(minAngle);
            DrawAngleLimitGizmo(maxAngle);
        }
    }

    void DrawAngleLimitGizmo(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
        Vector3 limitPosition = pivot + offset;
        Gizmos.DrawLine(pivot, limitPosition);
        Gizmos.DrawSphere(limitPosition, 0.1f);
    }
}