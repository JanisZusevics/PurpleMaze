using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShell : MonoBehaviour {
    public Mesh shellMesh;
    public Shader shellShader;

    public bool updateStatics = true;

    // These variables and what they do are explained on the shader code side of things
    // You can see below (line 70) which shader uniforms match up with these variables
    [Range(1, 256)]
    public int shellCount = 16;

    [Range(0.0f, 1.0f)]
    public float shellLength = 0.15f;

    [Range(0.01f, 3.0f)]
    public float distanceAttenuation = 1.0f;

    [Range(1.0f, 1000.0f)]
    public float density = 100.0f;

    [Range(0.0f, 1.0f)]
    public float noiseMin = 0.0f;

    [Range(0.0f, 1.0f)]
    public float noiseMax = 1.0f;

    [Range(0.0f, 10.0f)]
    public float thickness = 1.0f;

    [Range(0.0f, 10.0f)]
    public float curvature = 1.0f;

    [Range(0.0f, 1.0f)]
    public float displacementStrength = 0.1f;

    public Color shellColor;

    [Range(0.0f, 5.0f)]
    public float occlusionAttenuation = 1.0f;
    
    [Range(0.0f, 1.0f)]
    public float occlusionBias = 0.0f;

    private Material shellMaterial;
    private GameObject[] shells;
    // Wind parameters
    public float windStrength = 1.0f;
    public float windFrequency = 1.0f;
    public Vector3 windDirection = new Vector3(1, 0, 0);

    void OnEnable() {
        // Initialize the shell material and game objects
        shellMaterial = new Material(shellShader);
        shells = new GameObject[shellCount];

        // Create the shells
        for (int i = 0; i < shellCount; ++i) {
            // Create a new game object for the shell
            shells[i] = new GameObject("Shell " + i.ToString());
            shells[i].AddComponent<MeshFilter>();
            shells[i].AddComponent<MeshRenderer>();

            // Set the mesh and material of the shell
            shells[i].GetComponent<MeshFilter>().mesh = shellMesh;
            shells[i].GetComponent<MeshRenderer>().material = shellMaterial;

            // Set the parent of the shell to this game object
            shells[i].transform.SetParent(this.transform, false);

            // Set the properties of the shell material
            Material shellMat = shells[i].GetComponent<MeshRenderer>().material;
            shellMat.SetInt("_ShellCount", shellCount);
            shellMat.SetInt("_ShellIndex", i);
            shellMat.SetFloat("_ShellLength", shellLength);
            shellMat.SetFloat("_Density", density);
            shellMat.SetFloat("_Thickness", thickness);
            shellMat.SetFloat("_Attenuation", occlusionAttenuation);
            shellMat.SetFloat("_ShellDistanceAttenuation", distanceAttenuation);
            shellMat.SetFloat("_Curvature", curvature);
            shellMat.SetFloat("_DisplacementStrength", displacementStrength);
            shellMat.SetFloat("_OcclusionBias", occlusionBias);
            shellMat.SetFloat("_NoiseMin", noiseMin);
            shellMat.SetFloat("_NoiseMax", noiseMax);
            shellMat.SetVector("_ShellColor", shellColor);
        }
    }

    void Update() {

        Shader.SetGlobalFloat("_WindStrength", windStrength);
        Shader.SetGlobalFloat("_WindFrequency", windFrequency);
        Shader.SetGlobalVector("_WindDirection", windDirection.normalized);

    }

    void OnDisable() {
        for (int i = 0; i < shells.Length; ++i) {
            Destroy(shells[i]);
        }

        shells = null;
    }


}