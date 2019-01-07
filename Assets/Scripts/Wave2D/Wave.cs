using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave : MonoBehaviour
{
    public int Height;

    public int Width;

    public MeshFilter meshFilter;

    public float Spread = 0.25f;

    public float Tension = 0.025f;

    public float Dampening = 0.025f;

    private Mesh m_waveMesh;

    private Vector3[] m_vertices;

    private int[] m_indies;

    private Vector2[] m_uvs;

    private WaterSurfacePoint[] surfacePoints;

    [System.Serializable]
    public class WaterSurfacePoint
    {
        public float Height;
        public float Velocity;
        private float m_targetHeight;

        public WaterSurfacePoint(float h)
        {
            Height = h;
            m_targetHeight = Height;
            Velocity = 0;
        }

        public void Update(float tension,float dampening)
        {
            float x = m_targetHeight - Height;
            Velocity += tension * x - Velocity * dampening;
            Height += Velocity;
        }
    }

    void Start()
    {
        m_vertices = new Vector3[Width * 2];
        m_indies = new int[(Width - 1) * 6];
        m_uvs = new Vector2[Width * 2];
        surfacePoints = new WaterSurfacePoint[Width];
        m_waveMesh = new Mesh();
        meshFilter.sharedMesh = m_waveMesh;
        GenerateWaveMesh();
    }

    public void GenerateWaveMesh()
    {
        Debug.Log("Generate Wave Mesh" + Width);
        float topLeftX = - (Width - 1) / 2f;
        float topLeftY = (Height - 1) / 2f;


        int vertexIndex = 0;
        for (int i = 0; i < Width; i++)
        {
            m_vertices[i] = new Vector3(topLeftX + i, topLeftY, 0);
            m_vertices[Width + i] = new Vector3(topLeftX + i,topLeftY - Height);
            m_uvs[i] = new Vector2(i/(float)Width,0);
            m_uvs[Width + i] = new Vector2(i/(float)Width, 1);

            surfacePoints[i] = new WaterSurfacePoint(Height);

            if (i < Width - 1)
            {
                m_indies[vertexIndex++] = i;
                m_indies[vertexIndex++] = Width + i + 1;
                m_indies[vertexIndex++] = Width + i;

                m_indies[vertexIndex++] = Width + i + 1;
                m_indies[vertexIndex++] = i;
                m_indies[vertexIndex++] = i + 1;
            }
        }
        
    }

    void SimulatePhysics()
    {
        for (int i = 0; i < surfacePoints.Length; i++)
        {
            surfacePoints[i].Update(Tension,Dampening);
        }

        float[] lDeltas = new float[surfacePoints.Length];
        float[] rDeltas = new float[surfacePoints.Length];

        //模拟八次迭代
        for(int j = 0; j < 8; j++) {
            for (int i = 0; i < surfacePoints.Length ; i++)
            {
                //计算以波动点向右传播的速度
                if (i > 0)
                {
                    //计算第i点于其左边的高度差
                    lDeltas[i] = Spread * (surfacePoints[i].Height - surfacePoints[i - 1].Height);
                    //高度差作为速度增量
                    surfacePoints[i - 1].Velocity += lDeltas[i];
                }

                if (i < surfacePoints.Length - 1)
                { 
                    //计算第i点于其左边的高度差
                    rDeltas[i] = Spread * (surfacePoints[i].Height - surfacePoints[i + 1].Height);
                    //高度差作为速度增量
                    surfacePoints[i + 1].Velocity += rDeltas[i];
                }
            }

            for (int i = 0; i < surfacePoints.Length; i++)
            {
                if (i > 0)
                    surfacePoints[i - 1].Height += lDeltas[i];
                if (i < surfacePoints.Length - 1)
                    surfacePoints[i + 1].Height += rDeltas[i];
            }
        }
    }

    void UpdateVertices()
    {
        for (int i = 0; i < surfacePoints.Length; i++)
        {
            m_vertices[i].y = -(Height - 1) / 2.0f + surfacePoints[i].Height;
        }
    }


    void UpdateWaveMesh()
    {
        m_waveMesh.vertices = m_vertices;
        m_waveMesh.triangles = m_indies;
        m_waveMesh.uv = m_uvs;
        m_waveMesh.RecalculateNormals();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int index = Random.Range(0, this.surfacePoints.Length);
            surfacePoints[5].Velocity = -5;
        }

        SimulatePhysics();
        UpdateVertices();
        UpdateWaveMesh();
    }
}
