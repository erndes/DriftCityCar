using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarController : MonoBehaviour
{
    [Header("Wheels collider")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider backLeftWheelCollider;
    public WheelCollider backRightWheelCollider;

    [Header("Wheels Transform")]
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform backLeftWheelTransform;
    public Transform backRightWheelTransform;

    [Header("Car Engine")]
    public float accelerationForce = 300f;
    public float breakingForce = 3000f;
    private float presentBreakForce = 0f;
    private float presentAcceleration = 0f;

    [Header("Car Steering")]
    public float wheelsTorque = 35f;
    private float presentTurnAngle = 0f;

    [Header("Car Sounds")]
    public AudioSource audioSource;
    public AudioClip accelerationSound;
    public AudioClip slowAccelerationSound;
    public AudioClip stopSound;

    private void Update(){
        MoveCar();
        CarSteering();
    }

/*
    private void MoveCar()
    {
        // 1. PRIMA calcoli quanto stai premendo (W o freccia su)
        presentAcceleration = accelerationForce * SimpleInput.GetAxis("Vertical");

        // 2. DOPO lo assegni alle ruote
        frontLeftWheelCollider.motorTorque = presentAcceleration;
        frontRightWheelCollider.motorTorque = presentAcceleration;

        
        //SE VOLESSI UN'ACCELERAZIONE CON TRAZIONE INTEGRALE AGGIUNGO:
        backLeftWheelCollider.motorTorque = presentAcceleration;
        backRightWheelCollider.motorTorque = presentAcceleration;

        if(presentAcceleration > 0){
            audioSource.PlayOneShot(accelerationSound, 0.2f);
        }
        else if(presentAcceleration < 0){
            audioSource.PlayOneShot(slowAccelerationSound, 0.2f);
        }
        else if(presentAcceleration == 0){
            audioSource.PlayOneShot(stopSound, 0.1f);
        }
        
    }
*/




private void MoveCar()
{
    if (Time.timeScale == 0)
    {
        if (audioSource.isPlaying) audioSource.Pause();
        return; 
    }
    else
    {
        // Se non sta suonando ma c'è una clip assegnata, riprendi l'audio
        if (!audioSource.isPlaying && audioSource.clip != null) 
        {
            audioSource.UnPause();
        }
    }

    
    presentAcceleration = accelerationForce * SimpleInput.GetAxis("Vertical");
    
    frontLeftWheelCollider.motorTorque = presentAcceleration;
    frontRightWheelCollider.motorTorque = presentAcceleration;
    backLeftWheelCollider.motorTorque = presentAcceleration;
    backRightWheelCollider.motorTorque = presentAcceleration;

    HandleEngineSound();
}

private void HandleEngineSound()
{
    AudioClip clipDaRiprodurre = null;

    if (presentAcceleration > 0) {
        clipDaRiprodurre = accelerationSound;
    } else if (presentAcceleration < 0) {
        clipDaRiprodurre = slowAccelerationSound;
    } else {
        clipDaRiprodurre = stopSound;
    }

    // Cambia la clip solo se è diversa da quella che sta già suonando
    if (audioSource.clip != clipDaRiprodurre)
    {
        audioSource.clip = clipDaRiprodurre;
        audioSource.loop = true; // Importante per suoni motore
        audioSource.Play();
    }
}


    private void CarSteering(){
        presentTurnAngle = wheelsTorque * SimpleInput.GetAxis("Horizontal");
        frontLeftWheelCollider.steerAngle = presentTurnAngle;
        frontRightWheelCollider.steerAngle = presentTurnAngle;

        SteeringWheels(frontLeftWheelCollider, frontLeftWheelTransform);
        SteeringWheels(frontRightWheelCollider, frontRightWheelTransform);
        SteeringWheels(backLeftWheelCollider, backLeftWheelTransform);
        SteeringWheels(backRightWheelCollider, backRightWheelTransform);

    }

    void SteeringWheels(WheelCollider WC, Transform WT){
        Vector3 position;
        Quaternion rotation;

        WC.GetWorldPose(out position, out rotation);

        WT.position = position;
        WT.rotation = rotation;
    }

    public void ApplyBreaks()
    {
        StartCoroutine(carBreaks());
    }

    IEnumerator carBreaks()
    {
        presentBreakForce = breakingForce;

        frontLeftWheelCollider.brakeTorque = presentBreakForce;
        frontRightWheelCollider.brakeTorque = presentBreakForce;
        backLeftWheelCollider.brakeTorque = presentBreakForce;
        backRightWheelCollider.brakeTorque = presentBreakForce;

        yield return new WaitForSeconds(2f);

        presentBreakForce = 0f;

        frontLeftWheelCollider.brakeTorque = presentBreakForce;
        frontRightWheelCollider.brakeTorque = presentBreakForce;
        backLeftWheelCollider.brakeTorque = presentBreakForce;
        backRightWheelCollider.brakeTorque = presentBreakForce;
    }
}



