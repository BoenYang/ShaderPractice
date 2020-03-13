using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ShadowVolumeLight : MonoBehaviour
{
    [HideInInspector]
    public Shader ExtrusionShader;

    [HideInInspector]
    public Shader SetAlphaShader;

    public Light LightObject;

    public float ExtrusionDist;

    public MeshFilter[] Meshes;

    private Material extrusionMat
    {
        get
        {
            if (_extrusionMat == null)
            {
                _extrusionMat = new Material(ExtrusionShader);
            }
            return _extrusionMat;
        }
    }

    private Material _extrusionMat;

    private Material setAlphaMat
    {
        get
        {
            if (_setAlphaMat == null)
            {
                _setAlphaMat = new Material(SetAlphaShader);
            }
            return _setAlphaMat;
        }
    }

    private Material _setAlphaMat;



    public void OnPostRender()
    {

        GL.PushMatrix();
        GL.LoadOrtho();
        setAlphaMat.SetPass(0);
        DrawQuad();
        GL.PopMatrix();

        extrusionMat.SetFloat("_Extrusion",ExtrusionDist);

        Vector4 lightPos;

        if (LightObject.type == LightType.Directional)
        {
            Vector3 dir = -LightObject.transform.forward;
            dir = transform.InverseTransformDirection(dir);
            lightPos = new Vector4(dir.x,dir.y,-dir.z,0.0f);
        }
        else
        {
            Vector3 pos = LightObject.transform.position;
            pos = transform.InverseTransformPoint(pos);
            lightPos = new Vector4(pos.x,pos.y,-pos.z,1.0f);
        }

        extrusionMat.SetVector("_LightPosition",lightPos);

        foreach (var mesh in Meshes)
        {
            Mesh m= mesh.sharedMesh;
            Transform tr = mesh.transform;

            extrusionMat.SetPass(0);
            Graphics.DrawMeshNow(m,tr.localToWorldMatrix);

            extrusionMat.SetPass(1);
            Graphics.DrawMeshNow(m, tr.localToWorldMatrix);
        }

        GL.PushMatrix();
        GL.LoadOrtho();
        setAlphaMat.SetPass(1);
        DrawQuad();
        setAlphaMat.SetPass(2);
        DrawQuad();
        setAlphaMat.SetPass(3);
        DrawQuad();
        setAlphaMat.SetPass(4);
        DrawQuad();
        GL.PopMatrix();

    }

    public static void DrawQuad()
    {
        GL.Begin(GL.QUADS);
        GL.Vertex3(0, 0, 0.1f);
        GL.Vertex3(1, 0, 0.1f);
        GL.Vertex3(1, 1, 0.1f);
        GL.Vertex3(0, 1, 0.1f);
        GL.End();
    }
}
