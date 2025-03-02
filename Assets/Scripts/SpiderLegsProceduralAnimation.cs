using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SpiderLegsProceduralAnimation : MonoBehaviour
{
    [SerializeField]
    private List<Transform> legTargetTransforms;
    private List<Vector3> legTargetPositions;
    //private List<Coroutine> legCoroutines;

    [SerializeField]
    private float legsTime = 0.1f;
    [SerializeField]
    private float maxDistance = 0.1f;

    [SerializeField]
    private int cluster = 3;
    int clusterCount;

    float timer = 0f;

    public UnityEvent onLegMoved = new UnityEvent();

    private void Awake() {
        legTargetPositions = new List<Vector3>(legTargetTransforms.Count);
        //legCoroutines = new List<Coroutine>(legTargetTransforms.Count);
        for (int i = 0; i < legTargetTransforms.Count; i++) {
            legTargetPositions.Add(legTargetTransforms[i].position - transform.position);
            legTargetTransforms[i].SetParent(null, true);

            //legCoroutines.Add(null);
        }
    }

    private void Start() {
        //moveLegsCoroutine = StartCoroutine(MoveLegs());
        //StartCoroutine(MoveOneLeg(0));
        clusterCount = legTargetTransforms.Count / cluster;
    }

    int currentCluster = 0;
    private void Update() {
       
        if (timer> legsTime){
            timer = 0;
            currentCluster++;
            if (currentCluster>=clusterCount) currentCluster = 0;
            onLegMoved.Invoke();
            for (int i = currentCluster; i < legTargetTransforms.Count; i+=cluster) {
                    Vector3 legPos = transform.TransformPoint(legTargetPositions[i]);
                    float legDistance = Vector3.SqrMagnitude(legTargetTransforms[i].position - legPos);
                    if (maxDistance < legDistance) {
                        Vector3 legOffset = (legPos - legTargetTransforms[i].position).normalized;
                        legOffset *= Mathf.Clamp(Vector3.Magnitude(legTargetTransforms[i].position - legPos) / 2, 0f, 0.5f);
                        legPos += legOffset;
                        
                        //legTargetTransforms[i].position = legPos;
                        legTargetTransforms[i].DOJump(legPos, 0.1f, 1, legsTime/2);

                    }
            }
        }

        timer+=Time.deltaTime;
    }
    


    //private Coroutine moveLegsCoroutine = null;
    // private IEnumerator MoveLegs() {
        
    //     while (true) {
    //         int cluster = 3;
    //         int clusterCount = legTargetTransforms.Count / cluster; // 4
    //         for (int k = 0; k < cluster; k++) {
    //             yield return new WaitForSeconds(legsTime / cluster);
    //             for (int j = 0; j < clusterCount; j++) {
    //                 int i = k + j * cluster;
    //                 if (legCoroutines[i] != null) {
    //                     continue;
    //                 }
    //                 Vector3 legPos = transform.TransformPoint(legTargetPositions[i]);
    //                 float legDistance = Vector3.SqrMagnitude(legTargetTransforms[i].position - legPos);
    //                 if (maxDistance < legDistance) {
    //                     Vector3 legOffset = (legPos - legTargetTransforms[i].position).normalized;
    //                     legOffset *= Mathf.Clamp(Vector3.Magnitude(legTargetTransforms[i].position - legPos) / 2, 0f, 0.5f);
    //                     legPos += legOffset;
    //                     //legCoroutines[i] = StartCoroutine(MoveOneLeg(i));
    //                     legTargetTransforms[i].position = legPos;
    //                 }

    //             }
    //         }
    //     }

    //     moveLegsCoroutine = null;
    //     yield break;
    // }

    // private IEnumerator MoveOneLeg(int i) {
    //     yield return new WaitForEndOfFrame();
    //     Vector3 legPos = transform.TransformPoint(legTargetPositions[i]);
    //     float legDistance = Vector3.SqrMagnitude(legTargetTransforms[i].position - legPos);
    //     Vector3 legOffset = (legPos - legTargetTransforms[i].position).normalized;
    //     legOffset *= Mathf.Clamp(Vector3.Magnitude(legTargetTransforms[i].position - legPos) / 2, 0f, 0.5f);
    //     legPos += legOffset;
    //     while (legDistance > 0.01f) {
    //         yield return new WaitForEndOfFrame();

    //         legTargetTransforms[i].position = Vector3.Lerp(legTargetTransforms[i].position, legPos, Time.deltaTime * 30);
    //         legPos = transform.TransformPoint(legTargetPositions[i]);
    //         legPos += legOffset;
    //         legDistance = Vector3.SqrMagnitude(legTargetTransforms[i].position - legPos);
    //     }

    //     legCoroutines[i] = null;
    //     yield break;
    // }
}
