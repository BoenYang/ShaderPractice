using UnityEngine;

public class VolumetricLight : MonoBehaviour
{
    public Shader LightShader;

    private Light light;

    private Mesh spotLightMesh;

    private Material material;

    private void Start()
    {
        material = new Material(LightShader);
        light = GetComponent<Light>();
        CreateSpotLightMesh();
    }

    private void CreateSpotLightMesh()
    {

        const int segment = 16;

        GameObject go = new GameObject("SpotLightMesh");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.name = "SpotMesh";
        meshRenderer.material = material;

        material.SetColor("_LightColor",light.color);
        material.SetVector("_LightPos",transform.position);
        material.SetFloat("_LightIndensity", light.intensity);



        Vector3[] vertices = new Vector3[2 + segment];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, 0, light.range);

        float angle = 0;

        float spotRaduis = light.range * Mathf.Tan(light.spotAngle * Mathf.Deg2Rad / 2f);

        float stepAngle = Mathf.PI * 2.0f / segment;

        for (int i = 0; i < segment; i++)
        {
            vertices[i + 2] = new Vector3(-spotRaduis * Mathf.Cos(angle), spotRaduis * Mathf.Sin(angle), light.range);
            angle += stepAngle;
        }

        int[] indices = new int[segment * 3 * 2];
        int index = 0;
        for (int i = 0; i < segment - 1; i++)
        {
            indices[index++] = 0;
            indices[index++] = i + 2;
            indices[index++] = i + 2 + 1;
        }

        indices[index++] = 0;
        indices[index++] = segment + 1;
        indices[index++] = 2;

        for (int i = 0; i < segment - 1; i++)
        {
            indices[index++] = 1;
            indices[index++] = i + 2 + 1;
            indices[index++] = i + 2;
        }


        indices[index++] = 1;
        indices[index++] = 2;
        indices[index++] = segment + 1;

        mesh.vertices = vertices;
        mesh.triangles = indices;

        meshFilter.mesh = mesh;
        mesh.RecalculateNormals();
    }

}
