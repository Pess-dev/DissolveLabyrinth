using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DynamicDeactivator : MonoBehaviour
{
    [SerializeField] float radius;
    List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    List<VisualEffect> vfxs = new List<VisualEffect>();
    void Start()
    {
        MazeGenerator.mazeGenerated.AddListener(InitializeRenderer);
    }

    void LateUpdate()
    {
        foreach (MeshRenderer child in meshRenderers){
            if (Vector3.Distance(child.transform.position, PlayerController.position) > radius) child.enabled = false;
            if (Vector3.Distance(child.transform.position, PlayerController.position) <= radius) child.enabled = true;
        }
        foreach (VisualEffect child in vfxs){
            if (Vector3.Distance(child.transform.position, PlayerController.position) > radius) child.enabled = false;
            if (Vector3.Distance(child.transform.position, PlayerController.position) <= radius) child.enabled = true;
        }
    }

    void InitializeRenderer(){
        foreach (MeshRenderer child in transform.GetComponentsInChildren<MeshRenderer>()){
            meshRenderers.Add(child);
        }foreach (VisualEffect child in transform.GetComponentsInChildren<VisualEffect>()){
            vfxs.Add(child);
        }
    }
}
