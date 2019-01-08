using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave : MonoBehaviour
{
    public int Height;

    public int Width;

    public int Segment = 10;

    public MeshFilter meshFilter;

    public float Spread = 0.25f;

    public float Tension = 0.025f;

    public float Dampening = 0.025f;

    private Mesh m_waveMesh;

    private Vector3[] m_vertices;

    private int[] m_indies;

    private Vector2[] m_uvs;

    private WaterSurfacePoint[] surfacePoints;

    private int surfacePointCount = 0;

    private float m_surfacePointGap = 0;

    private float m_topLeftX;

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

    void OnValidate()
    {
        if (Segment <= 0)
        {
            Segment = 1;
        }

        if (Width <= 0)
        {
            Width = 1;
        }

        if (Height <= 0)
        {
            Height = 1;
        }
    }

    void Start()
    {
        GenerateWaveMesh();
    }

    public void GenerateWaveMesh()
    {
        m_waveMesh = new Mesh();
        meshFilter.sharedMesh = m_waveMesh;

        surfacePointCount = Segment;
        m_vertices = new Vector3[surfacePointCount * 2];
        m_indies = new int[(surfacePointCount - 1) * 6];
        m_uvs = new Vector2[surfacePointCount * 2];
        m_uvs = new Vector2[surfacePointCount * 2];
        surfacePoints = new WaterSurfacePoint[surfacePointCount];

        m_topLeftX = - (Width - 1 )/ 2f;
        float topLeftY = (Height - 1) / 2f;
        int vertexIndex = 0;
        m_surfacePointGap = Width / (float) Segment;
        for (int i = 0; i < surfacePointCount; i++)
        {
            m_vertices[i] = new Vector3(m_topLeftX + i * m_surfacePointGap, topLeftY, 0);
            m_vertices[surfacePointCount + i] = new Vector3(m_topLeftX + i * m_surfacePointGap, topLeftY - Height);
            m_uvs[i] = new Vector2(i/(float)surfacePointCount, 0);
            m_uvs[surfacePointCount + i] = new Vector2(i/(float)surfacePointCount, 1);

            surfacePoints[i] = new WaterSurfacePoint(Height);

            if (i < surfacePointCount - 1)
            {
                m_indies[vertexIndex++] = i;
                m_indies[vertexIndex++] = surfacePointCount + i + 1;
                m_indies[vertexIndex++] = surfacePointCount + i;

                m_indies[vertexIndex++] = surfacePointCount + i + 1;
                m_indies[vertexIndex++] = i;
                m_indies[vertexIndex++] = i + 1;
            }
        }
        UpdateWaveMesh();
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
            m_vertices[i].y = -(Height - 1)/ 2.0f + surfacePoints[i].Height - 1;
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
            surfacePoints[surfacePoints.Length/2].Velocity = -10;
        }

        SimulatePhysics();
        UpdateVertices();
        UpdateWaveMesh();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        int hitCount = collision.Raycast(Vector2.down, hits, 1);
        Debug.Log(hitCount);
        if (hitCount > 0)
        {
            float x = hits[0].point.x;
            float distanceFromLeft = x - m_topLeftX;
            int colliderSurfacePointIndex = (int) (distanceFromLeft / m_surfacePointGap);
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            surfacePoints[colliderSurfacePointIndex].Velocity = rb.velocity.y;
            Debug.Log(rb.velocity.y);
        }
    }
}
