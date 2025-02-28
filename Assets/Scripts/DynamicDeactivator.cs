using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class DynamicDeactivator : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float abilityRadius;
    [SerializeField] float chunkSize = 10f;
    Dictionary<Vector3,List<GameObject>> visuals = new Dictionary<Vector3,List<GameObject>>();
    Dictionary<Vector3,bool> visualsIsActive = new Dictionary<Vector3,bool>();

    [SerializeField] string deactivateTag = "visual"; 

    void Awake()
    {
        MazeGenerator.mazeGenerated.AddListener(Initialize);
    }

    void LateUpdate(){   
        float currentRadius = Ability.isActiveAbility ? abilityRadius : radius; 
        foreach (Vector3 g in visuals.Keys){
            if (Vector3.Distance( g+WorldManager.offset, PlayerController.position) > currentRadius && visualsIsActive[g]){  
                visuals[g].ForEach(x => x.SetActive(false));
                visualsIsActive[g] = false;
            };
            if (Vector3.Distance( g+WorldManager.offset, PlayerController.position) <= currentRadius&& !visualsIsActive[g]){
                visuals[g].ForEach(x => x.SetActive(true));
                visualsIsActive[g] = true;
            };
        }

    }

    void Initialize(){
        List<GameObject> preVisuals = new List<GameObject>();

        float maxDistance = 0;

        foreach (GameObject g in GameObject.FindGameObjectsWithTag(deactivateTag)){
            preVisuals.Add(g);
            maxDistance = Mathf.Max(g.transform.position.magnitude, maxDistance);
        }

        int chunkCount = (int)Mathf.Ceil(maxDistance/chunkSize);
        for (int x = -chunkCount; x< chunkCount; x++){
            for (int y = -chunkCount; y< chunkCount; y++){
                Vector3 vector3 = new Vector3(x*chunkSize,0,y*chunkSize);
                visuals.Add(vector3,new List<GameObject>());
                foreach(GameObject g in preVisuals){
                    if ((vector3 - g.transform.position).magnitude <= chunkSize){
                        visuals[vector3].Add(g);
                    }
                }
                if (visuals[vector3].Count == 0) visuals.Remove(vector3);
                else {
                    visualsIsActive.Add(vector3,true);
                }
            }
        }

        //preVisuals = preVisuals.OrderBy((x)=>Vector3.Distance(x.transform.position,PlayerController.position)).ToList();


    }
}
