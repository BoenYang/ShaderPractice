using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mosaic : MonoBehaviour
{
    public int RowCount = 300;

    public int ColumnCount = 500;

    public Shader shader;

    private Material mat
    {
        get
        {
            if (_mat == null && shader != null)
            {
                _mat = new Material(shader);
            }
            return _mat;
        }
    }

    private Material _mat;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mat != null)
        {
            mat.SetFloat("_RowCount", RowCount);
            mat.SetFloat("_ColumnCount", ColumnCount);
            Graphics.Blit(source, destination, mat);
        }
    }
}
