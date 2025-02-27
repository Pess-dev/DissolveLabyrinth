using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour{
    protected Movement movement;
    protected NavMeshAgent navMeshAgent;

    public float aggressiveSpeedModifier = 1.3f;
    public float aggressiveRadius = 15f;
    public float forgiveRadius = 20f;
    public float killRadius = 0.5f;
    public float patrolingTime = 10f;
    
    public float idleTime = 5f;

    public UnityEvent<AIState> aIStateChanged = new UnityEvent<AIState>(); 

    public enum AIState
    {
        Idle,
        Patroling,
        Aggressive
    }
    
    protected float stateTime = 0f;
    protected AIState aIState = AIState.Idle;

    protected Vector3 target;

    void Start(){
        movement = GetComponent<Movement>();    
        navMeshAgent = GetComponent<NavMeshAgent>();
        //target = PlayerController.instance.transform;
        
        aIStateChanged.AddListener(OnAIStateChanged);
    }

    void FixedUpdate(){
        stateTime+=Time.fixedDeltaTime;
        OnAny();
        switch(aIState){
            case AIState.Idle: OnIdle(); break;
            case AIState.Patroling: OnPatroling(); break;
            case AIState.Aggressive: OnAggressive(); break;
        }
        movement.SetMoveDirection((navMeshAgent.nextPosition - transform.position).normalized);
    }

    void OnAIStateChanged(AIState state){
        movement.ResetModifiers();
        switch(state){
            case AIState.Idle: OnIdleFirst(); break;
            case AIState.Patroling: OnPatrolingFirst(); break;
            case AIState.Aggressive: OnAggressiveFirst(); break;
        }
    }

    protected void OnIdle(){
        if (stateTime>idleTime){
            aIState = AIState.Patroling;
            aIStateChanged.Invoke(aIState);
            return;
        }
    }
    protected void OnIdleFirst(){
        navMeshAgent.SetDestination(transform.position);
    }

    protected void OnPatroling(){
        if (stateTime>patrolingTime){
            aIState = AIState.Idle;
            aIStateChanged.Invoke(aIState);
            return;
        }
    }
    protected void OnPatrolingFirst(){
        target = GetRandomDestination();
        navMeshAgent.SetDestination(target);
        
    }
    
    protected void OnAny(){
        if (DistanceToPlayer() < aggressiveRadius){
            aIState = AIState.Aggressive;
            aIStateChanged.Invoke(aIState);
            return;
        }
    }

    protected void OnAggressive(){
        if (DistanceToPlayer()>=forgiveRadius){
            aIState = AIState.Idle;
            aIStateChanged.Invoke(aIState);
            return;
        }

        target = GetPlayerPosition();
        navMeshAgent.SetDestination(target);
    }
    protected void OnAggressiveFirst(){
        movement.SetModifier("aggressive",aggressiveSpeedModifier);
    }

    protected float DistanceToPlayer(){
        return Vector3.Distance(transform.position, target);
    }
    protected Vector3 GetPlayerPosition(){
        return PlayerController.position;
    }
    protected Vector3 GetRandomDestination(){
        // Получаем данные триангуляции NavMesh
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        
        // Проверяем, есть ли данные
        if (navMeshData.indices.Length == 0)
        {
            Debug.LogError("NavMesh не найден!");
            return Vector3.zero;
        }

        // Выбираем случайный треугольник
        int triangleIndex = Random.Range(0, navMeshData.indices.Length / 3);
        int startIndex = triangleIndex * 3;

        // Получаем вершины треугольника
        Vector3 a = navMeshData.vertices[navMeshData.indices[startIndex]];
        Vector3 b = navMeshData.vertices[navMeshData.indices[startIndex + 1]];
        Vector3 c = navMeshData.vertices[navMeshData.indices[startIndex + 2]];

        // Генерируем случайную точку внутри треугольника
        return RandomPointInTriangle(a, b, c);
    }

    private Vector3 RandomPointInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        // Генерация барицентрических координат
        float u = Random.Range(0f, 1f);
        float v = Random.Range(0f, 1f);

        // Корректировка координат, если вышли за пределы треугольника
        if (u + v > 1)
        {
            u = 1 - u;
            v = 1 - v;
        }

        // Вычисление конечной позиции
        return a + u * (b - a) + v * (c - a);
    }
}