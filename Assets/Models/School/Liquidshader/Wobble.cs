using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Wobble : MonoBehaviour
{
    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;  
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;
    
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    private void Update()
    {
        time += Time.deltaTime;
        // Wobbel tijd word verminderd over tijd
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));
 
        // Sin wave dat de wobbel verminderd
        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);
 
        // Word toegestuurd naar de shader
        rend.material.SetFloat("_BewegenOpDeX", wobbleAmountX);
        rend.material.SetFloat("_BewegenOpDeZ", wobbleAmountZ);
 
        // Velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;
 
 
        // Clamp op de wobble
        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
 
        // Blijft op de laatste positie
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }
 
 
 
}