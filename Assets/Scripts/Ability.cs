using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Collider))]
public class Ability : MonoBehaviour
{
    [SerializeField] float maxDuration = 3f;
    [SerializeField] float waitDuration = 1f;
    [SerializeField] float cooldown = 0.5f;
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float costPerSecond = 10f;
    [SerializeField] float recoveryPerSecond = 10f;
    [SerializeField] float staminaToStart = 30f;
    [SerializeField] float speedModifier = 0.7f;
    [SerializeField] float inWallSpeedModifier = 0.5f;
    [SerializeField] float abilityMinSpeed = 1f;

    [SerializeField] float pushOutAcceleration = 5f;
    [SerializeField] LayerMask excludeDeactivated = 0;
    [SerializeField] LayerMask excludeActivated = 0;

    [SerializeField] float audioVolumeModifier = 0.5f;
    
    CharacterController cc;
    Movement movement;
    Collider playerCollider;

    float timer = 0f;
    [SerializeField] float stamina = 0f;

    public static bool isActiveAbility = false;
    bool pushOut = false;

    List<Collider> entered = new List<Collider>();

    public UnityEvent activated = new UnityEvent();
    public UnityEvent deactivated = new UnityEvent();
    
    public UnityEvent<float> staminaChanged = new UnityEvent<float>();

    public static Ability instance;

    float lastAudioVolume = 1;

    void Awake(){
        instance = this;
        stamina = maxStamina;
        InputManager.ability.AddListener(ChangeDissolve);
        cc = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        movement = GetComponent<Movement>();
    }

    void Start()
    {   
        
        activated.AddListener(DissolveController.instance.Activate);
        deactivated.AddListener(DissolveController.instance.Deactivate);
        

    }

    void ChangeDissolve(bool value){
        if (!value) return;
        if (isActiveAbility) return;
        if (GameManager.instance.gameState!=GameManager.GameState.Play) return;

        if (stamina >= staminaToStart && timer > cooldown){
            ActivateDissolve();
        }
    }

    public void ActivateDissolve(){
        float targetVolume = AudioListener.volume*audioVolumeModifier;
        lastAudioVolume = AudioListener.volume;
        DOTween.To(()=>AudioListener.volume,(x) =>AudioListener.volume=x, targetVolume, DissolveController.instance.duration );
        isActiveAbility = true;
        pushOut = false;
        timer = 0f;
        cc.excludeLayers = excludeActivated;

        movement.SetModifier("ability",speedModifier);

        activated.Invoke();
        //
    }
    void OnDestroy(){
        float targetVolume = lastAudioVolume;
        DOTween.To(()=>AudioListener.volume,(x) =>AudioListener.volume=x, GameSettingsManager.instance.audioVolume, DissolveController.instance.duration );
    }

    public void DeactivateDissolve(){
        DOTween.To(()=>AudioListener.volume,(x) =>AudioListener.volume=x, GameSettingsManager.instance.audioVolume, DissolveController.instance.duration );
        isActiveAbility = false;
        pushOut = false;
        timer = 0f;
        
        cc.excludeLayers = excludeDeactivated;

        movement.RemoveModifier("ability");

        deactivated.Invoke();
        //
    }

    void Update(){
        timer += Time.deltaTime;
        
        UpdateStamina();

        if (isActiveAbility && IsInsideWall()){
            movement.SetModifier("inWalls",inWallSpeedModifier);
            movement.SetMinimumSpeed(abilityMinSpeed);
        }
        else {
            movement.RemoveModifier("inWalls");
            movement.ResetMinimumSpeed();
        }

        if (isActiveAbility && (timer > maxDuration || stamina<=0 || entered.Count==0 && timer > waitDuration)){
            pushOut = true;
        }

        if (pushOut){
            if (entered.Count>0){
                Collider nearest = null;
                foreach (Collider c in entered){
                    if (!nearest)
                        nearest = c;
                    else {
                        if (Vector3.Distance(transform.position,c.transform.position)<Vector3.Distance(transform.position,nearest.transform.position)){
                            nearest = c;
                        }
                    }
                }
                Vector3 target = Vector3.zero;
                Vector3 direction = Vector3.zero;
                float distance = 0;
                bool isIn = nearest.bounds.Contains(transform.position);
                bool isPenetrating = Physics.ComputePenetration(playerCollider,transform.position,transform.rotation,
                nearest, nearest.transform.position, nearest.transform.rotation, out direction, out distance); 
                //nearest.ClosestPoint();
                //print(direction);
                Vector2 randomInCircle = Random.insideUnitCircle;
                //Vector3 random = new Vector3(randomInCircle.x,0,randomInCircle.y)*0.01f;
                //direction = Vector3.ProjectOnPlane(direction + random,Vector3.up).normalized;
               
                //target = nearest.ClosestPoint(transform.position);
                //direction = Vector3.ProjectOnPlane((transform.position - target)*(isIn?1:-1), Vector3.up).normalized;
                //cc.Move(direction*pushOutSpeed*Time.deltaTime);
                movement.AddFlatVelocity(direction*pushOutAcceleration*Time.deltaTime);
            }
            else DeactivateDissolve();
        }
    }

    void UpdateStamina(){
        float staminaOld = stamina;
        if (isActiveAbility) stamina = Mathf.Clamp(stamina-costPerSecond*Time.deltaTime,0,maxStamina);
        else stamina = Mathf.Clamp(stamina+recoveryPerSecond*Time.deltaTime,0,maxStamina);
        if (staminaOld!=stamina) {
            float value = stamina/maxStamina;
            staminaChanged.Invoke(1-value);
            
            //cameraShake.StartShake(value,cameraShakeRoughness,0);
        }
    }

    bool IsInsideWall(){
        return entered.Count>0;
    }

    void OnTriggerEnter(Collider collision){
        if (( excludeActivated & (1 << collision.gameObject.layer)) != 0)
            entered.Add(collision);
    }
    void OnTriggerExit(Collider collision){
        if (entered.Contains(collision))
            entered.Remove(collision);
    }

    void OnDrawGizmos(){
        if (entered.Count>0){
                Collider nearest = null;
                foreach (Collider c in entered){
                    if (!nearest)
                        nearest = c;
                    else {
                        if (Vector3.Distance(transform.position,c.transform.position)<Vector3.Distance(transform.position,nearest.transform.position)){
                            nearest = c;
                        }
                    }
                }

                bool isIn = nearest.bounds.Contains(transform.position);

                Vector3 target = nearest.ClosestPoint(transform.position);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(target,0.1f);
            }
    }
}
