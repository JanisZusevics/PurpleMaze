using UnityEngine;

public class GrassWindEffect : MonoBehaviour
{
    public float windStrength = 1.0f;
    public float windFrequency = 1.0f;
    public Vector3 windDirection = new Vector3(1, 0, 0);

    private Material grassMaterial;

    void Start()
    {
        grassMaterial = GetComponent<MeshRenderer>().material;
        grassMaterial.SetFloat("_WindStrength", windStrength);
        grassMaterial.SetFloat("_WindFrequency", windFrequency);
        grassMaterial.SetVector("_WindDirection", windDirection.normalized);
    }
}