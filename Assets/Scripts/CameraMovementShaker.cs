using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementShaker : MonoBehaviour
{
   #region ShakeVariables
    private float shakeTime = 0;

    private float shakeSpeed = 5;
    private float shakeXrange = 0.05f;
    private float shakeYrange = 0.12f;

    private float shakeStrength = 0;
    #endregion

    #region ShakeHelpVariables
    [SerializeField]
    private float walkShakeSpeed = 5;
    [SerializeField]
    private float walkShakeXrange = 0.05f;
    [SerializeField]
    private float walkShakeYrange = 0.05f;


    [SerializeField]
    private float stillShakeSpeed = 1;
    [SerializeField]
    private float stillShakeXrange = 0.1f;
    [SerializeField]
    private float stillShakeYrange = 0.07f;
    #endregion

    private float rotationShake = 0f;

    private Vector3 velocity;

    public void SetVelocity(Vector3 velocity){
        this.velocity = velocity;
    }

     private void Update() {
        ChangeMovementState();
        shakeTime += Time.deltaTime * shakeSpeed;
        shakeTime %= Mathf.PI * 2;
        float shakeSin = Mathf.Sin(shakeTime * 2);
        float shakeCos = Mathf.Cos(shakeTime);

        shakeStrength = Mathf.Lerp(shakeStrength, velocity.magnitude, Time.deltaTime * 7 * ((velocity.magnitude + 1.5f) / 2));
        shakeStrength = Mathf.Clamp(shakeStrength, 0.1f, 1f);

        Vector3 shakePosition = new Vector3(shakeCos * shakeXrange, shakeSin * shakeYrange, 0) * shakeStrength;
        transform.localPosition = shakePosition;
        transform.localRotation = Quaternion.Euler(shakeSin * shakeYrange * -7f, 0f, shakeCos * shakeXrange * -7f + rotationShake * -5f);
    }

    void ChangeMovementState() {
        float shakeInterpolateSpeed = 2f;
        if (velocity.magnitude <= 0.01) {
            shakeSpeed = Mathf.Lerp(shakeSpeed, stillShakeSpeed, Time.deltaTime * shakeInterpolateSpeed);
            shakeXrange = Mathf.Lerp(shakeXrange, stillShakeXrange, Time.deltaTime * shakeInterpolateSpeed);
            shakeYrange = Mathf.Lerp(shakeYrange, stillShakeYrange, Time.deltaTime * shakeInterpolateSpeed);
            return;
        }
        if (velocity.magnitude >= 0.01) {
            shakeSpeed = Mathf.Lerp(shakeSpeed, walkShakeSpeed, Time.deltaTime * shakeInterpolateSpeed);
            shakeXrange = Mathf.Lerp(shakeXrange, walkShakeXrange, Time.deltaTime * shakeInterpolateSpeed);
            shakeYrange = Mathf.Lerp(shakeYrange, walkShakeYrange, Time.deltaTime * shakeInterpolateSpeed);
            return;
        }
    }
}
