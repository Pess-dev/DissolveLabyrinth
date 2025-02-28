using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(WorldSyncPosition))]
//[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour{
    protected Movement movement;
    protected WorldSyncPosition worldSyncPosition;
    //protected NavMeshAgent navMeshAgent;

    public float aggressiveSpeedModifier = 1.3f;
    public float aggressiveRadius = 15f;
    public float forgiveRadius = 20f;
    public float killRadius = 0.6f;
    public float abilityRadius = 3f;
    public float patrolingTime = 10f;
    public float minDistanceToTarget = 0.5f;
    public string avoidTag = "Enemy";
    
    public float idleTime = 5f;
    
    public float abilityCooldown = 5f;
    public float abilitySpeedModifier = 0.2f;
    public float abilityTime = 3f;
    public float abilityFactor = 2f;
    float timerAbility = 0;
    bool isActiveAbility = false;

    public UnityEvent<AIState> aIStateChanged = new UnityEvent<AIState>(); 
    public static EnemyAI nearestAggresor; 


    public enum AIState
    {
        Idle,
        Patroling,
        Aggressive
    }
    
    protected float stateTime = 0f;

    protected AIState aIState = AIState.Idle;

    protected Vector3 target;

    protected NavMeshPath path;
    protected int currentCorner = 0;
    List<Transform> avoidTransforms;

    public UnityEvent abilityActivated = new UnityEvent();
    public UnityEvent abilityDectivated = new UnityEvent(); 

    private Vector3 abilityDirection = Vector3.forward;

    void Start(){
        movement = GetComponent<Movement>();    
        path = new NavMeshPath();
        stateTime = Random.value*idleTime;
        worldSyncPosition = GetComponent<WorldSyncPosition>();
        worldSyncPosition.onSync.AddListener(RecalculatePath);
        //navMeshAgent = GetComponent<NavMeshAgent>();
        //target = PlayerController.instance.transform;
        
        aIStateChanged.AddListener(OnAIStateChanged);

        avoidTransforms = GameObject.FindGameObjectsWithTag(avoidTag).Select(t => t.transform).ToList();
    }

    void FixedUpdate(){
        stateTime+=Time.fixedDeltaTime;
        timerAbility += Time.fixedDeltaTime;

        if (timerAbility>abilityTime && isActiveAbility && IsOnMesh()) {
            movement.RemoveModifier("ability");    
            isActiveAbility = false;
            print("ability end");
        }

        OnAny();
        switch(aIState){
            case AIState.Idle: OnIdle(); break;
            case AIState.Patroling: OnPatroling(); break;
            case AIState.Aggressive: OnAggressive(); break;
        }
        Vector3 nextPosition = GetNextPosition();
        Vector3 direction = (nextPosition - transform.position).normalized;
        if (isActiveAbility){
            direction = abilityDirection;
            //nextPosition = PlayerController.position;
        }
        //print(Vector3.Distance(nextPosition, transform.position)+" "+path.status);
        if (Vector3.Distance(nextPosition, transform.position)>=minDistanceToTarget||isActiveAbility){
                movement.SetMoveDirection(direction);
                movement.SetLookDirection(direction);
            }
        else 
            movement.SetMoveDirection(Vector3.zero);
    }

    void OnAIStateChanged(AIState state){
        //if (state == AIState.Aggressive)print("bik");
        if (aIState == AIState.Aggressive) movement.RemoveModifier("aggressive");
        stateTime = 0f;
        switch(state){
            case AIState.Idle: OnIdleFirst(); break;
            case AIState.Patroling: OnPatrolingFirst(); break;
            case AIState.Aggressive: OnAggressiveFirst(); break;
        }
    }

    protected void OnIdleFirst(){
        SetDestination(transform.position);
    }

    protected void OnIdle(){
        if (stateTime>idleTime){
            aIState = AIState.Patroling;
            aIStateChanged.Invoke(aIState);
            return;
        }
    }
    
    protected void OnPatrolingFirst(){
        target = GetPatrolDestination();
        SetDestination(target);  
    }
    protected void OnPatroling(){
        if (stateTime>patrolingTime||Vector3.ProjectOnPlane(target-transform.position,Vector3.up).magnitude<=minDistanceToTarget){
            aIState = AIState.Idle;
            aIStateChanged.Invoke(aIState);
            return;
        }
    }

    protected void OnAggressiveFirst(){
        movement.SetModifier("aggressive",aggressiveSpeedModifier);
    }

    protected void OnAggressive(){
        if (DistanceToPlayer()>=forgiveRadius){
            aIState = AIState.Idle;
            aIStateChanged.Invoke(aIState);
            nearestAggresor = null;
            return;
        }
        float length = GetPathLength(path);
        if (timerAbility > abilityCooldown 
        && DistanceToPlayer() <= abilityRadius 
        && !isActiveAbility 
        && (length>DistanceToPlayer()*abilityFactor||path.status!=NavMeshPathStatus.PathComplete&&length<=abilityRadius)) {
            print("ability start");
            abilityDirection = (PlayerController.position-transform.position).normalized;
            abilityActivated.Invoke();
            movement.SetModifier("ability",abilitySpeedModifier);
            isActiveAbility = true;
            timerAbility=0;
        }

        target = GetPlayerPosition();
        SetDestination(target);
    }

    public static float GetPathLength( NavMeshPath path )
    {
        float lng = 0.0f;
       
        if (path.status != NavMeshPathStatus.PathInvalid)
        {
            for ( int i = 1; i < path.corners.Length; ++i )
            {
                lng += Vector3.Distance( path.corners[i-1], path.corners[i] );
            }
        }
       
        return lng;
    }

    protected void OnAny(){
        float distanceToPlayer = DistanceToPlayer(); 
        if (distanceToPlayer < aggressiveRadius && nearestAggresor != this && aIState != AIState.Aggressive){
            if (nearestAggresor == null || nearestAggresor.DistanceToPlayer()>distanceToPlayer){
                aIState = AIState.Aggressive;
                aIStateChanged.Invoke(aIState);
                if (nearestAggresor!=null)
                {
                    nearestAggresor.aIState = AIState.Idle;
                    nearestAggresor.aIStateChanged.Invoke(aIState);
                }
                nearestAggresor = this;
                return;
            }
        }
    }
    
    protected float FlatDistance(Vector3 point1, Vector3 point2){
        return Vector3.ProjectOnPlane(point1-point2,Vector3.up).magnitude;
    }

    protected float DistanceToPlayer(){
        return FlatDistance(transform.position, GetPlayerPosition());
    }
    protected Vector3 GetPlayerPosition(){
        return PlayerController.position;
    }
    

    public float patrolSeparationDistance = 5f;  // Минимальное расстояние между патрульными точками врагов
    public int patrolAttemptCount = 10;          // Количество попыток найти подходящую точку

    bool IsOnMesh(){
        NavMeshHit hit;
        
        float maxDistance = 0.5f;
        
        return NavMesh.SamplePosition(transform.position, out hit, maxDistance, NavMesh.AllAreas);;
    }

    // Новый метод для выбора патрульной точки
    protected Vector3 GetPatrolDestination(){
        // Получаем данные триангуляции NavMesh
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        
        if (navMeshData.indices.Length == 0){
            Debug.LogError("NavMesh не найден!");
            return transform.position;
        }
        List<Vector3> candidates = new List<Vector3>();
        for (int i = 0; i < patrolAttemptCount; i++){
            // Выбираем случайный треугольник
            int triangleIndex = Random.Range(0, navMeshData.indices.Length / 3);
            int startIndex = triangleIndex * 3;
            Vector3 a = navMeshData.vertices[navMeshData.indices[startIndex]];
            Vector3 b = navMeshData.vertices[navMeshData.indices[startIndex + 1]];
            Vector3 c = navMeshData.vertices[navMeshData.indices[startIndex + 2]];

            // Генерируем случайную точку внутри треугольника
            Vector3 candidate = RandomPointInTriangle(a, b, c);

            // Проверяем, чтобы точка была достаточно удалена от точек других врагов
            bool valid = true;
            foreach(var enemy in avoidTransforms){
                if(enemy == null)
                    continue;
                if(Vector3.Distance(candidate, enemy.position) < patrolSeparationDistance){
                    valid = false;
                    break;
                }
            }
            if(valid){
                candidates.Add(candidate);
            }
        }

        if (candidates.Count > 0){
            return candidates.OrderBy(x => Vector3.Distance(x, PlayerController.position)).First();
        }

        // Если подходящую точку не нашли, возвращаем любую случайную точку
        return GetRandomDestination();
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

    protected void SetDestination(Vector3 pos){
        currentCorner = 1;
        NavMesh.CalculatePath(transform.position, pos,NavMesh.AllAreas, path);

        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(pos, out hit, 3f, NavMesh.AllAreas);

        if (path.status == NavMeshPathStatus.PathInvalid&&isValid) 
            NavMesh.CalculatePath(transform.position, hit.position,NavMesh.AllAreas, path);
    }

    protected void RecalculatePath(){
        SetDestination(target);
    }

    protected Vector3 GetNextPosition(){
        if (path == null) return transform.position;
        if (currentCorner>=path.corners.Count()) return transform.position;
        if (FlatDistance(path.corners[currentCorner],transform.position)<=minDistanceToTarget) currentCorner++;
        if (currentCorner>=path.corners.Count()) return transform.position;
        return path.corners[currentCorner];
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggressiveRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, forgiveRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistanceToTarget);

        Gizmos.color = Color.blue;
        for (int i = 0; i < path.corners.Length-1; i++) {
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            
            Gizmos.DrawSphere(path.corners[i + 1], 0.1f);
        }
        Gizmos.DrawSphere(target, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GetNextPosition(), 0.11f);

        print(aIState+" "+currentCorner);
    }
}