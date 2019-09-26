using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCube : MonoBehaviour
{
    private class MeshPoint {
        public Vector2 worldPos;
        public float underWaterDistance;
        public int index;
    }

    private class TriangleData {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;

        public float Area { get { return area; } }

        public Vector3 CenterPos { get { return centerPos; } }

        private float area;

        private Vector3 centerPos;

        public TriangleData(Vector3 p1, Vector3 p2, Vector3 p3) {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;


            this.centerPos = (p1 + p2 + p3) / 3.0f;

            float a = Vector3.Distance(p1, p2);

            float c = Vector3.Distance(p3, p1);

            this.area = (a * c * Mathf.Sin(Vector3.Angle(p2 - p1, p3 - p1) * Mathf.Deg2Rad)) / 2f;
        }

    }

    private class PolygonData {

        public List<TriangleData> triangles;

        private float area = 0;

        private Vector3 centerPos;

        public float Area { get { return area; } }

        public Vector3 CenterPos { get
            {
                if (area > float.Epsilon)
                {
                    //Debug.Log("Center Pos = " + centerPos + " Area = " + area);
                    return centerPos / area;
                }
                else
                {
                    return centerPos;
                }
            }
        }

        public PolygonData() {
            triangles = new List<TriangleData>();
            centerPos = Vector3.zero;
            area = 0;
        }

        public void AddTriangleData(TriangleData triangle) {
            triangles.Add(triangle);
            area += triangle.Area;
            centerPos += triangle.Area * triangle.CenterPos;
        }

        public void Reset() {
            triangles.Clear();
            centerPos = Vector3.zero;
            area = 0;
        }
    }

    private MeshFilter m_meshFilter;

    private Mesh m_mesh;

    public MeshFilter m_underWaterMeshFilter;

    private Mesh m_underWaterMesh;

    private MeshPoint[] m_meshPoints;

    private PolygonData underWaterPolygon;

    private Rigidbody2D rb;

    private float density;

    private float area = 0;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        m_meshFilter = GetComponent<MeshFilter>();
        m_mesh = m_meshFilter.mesh;

        if (m_underWaterMeshFilter != null) {
            m_underWaterMeshFilter.mesh = new Mesh();
            m_underWaterMesh = m_underWaterMeshFilter.mesh;
        }

        m_meshPoints = new MeshPoint[m_mesh.vertexCount];

        Debug.Log("mesh points count " + m_mesh.vertices.Length);

        for (int i = 0; i < m_meshPoints.Length; i++) {
            m_meshPoints[i] = new MeshPoint();
        }

        underWaterPolygon = new PolygonData();

        density = rb.mass;

        area = transform.localScale.x * transform.localScale.y;
    }

    void FixedUpdate() {
        CaluateUnderWaterDistance();
        ApplyBouncyForce();
        DisplayUnderWaterMesh();
    }

    private void Update()
    {
        if (underWaterPolygon.triangles.Count > 0) {
            //ApplyDrag();
        }
    }

    void CaluateUnderWaterDistance() {
        int underWaterPointCount = 0;
        //计算所有的点到水面的距离
        for (int i = 0; i < m_meshPoints.Length; i++) {
            Vector2 worldPos = transform.TransformPoint(m_mesh.vertices[i]);
            m_meshPoints[i].worldPos = worldPos;
            m_meshPoints[i].index = i;
            m_meshPoints[i].underWaterDistance = Wave.Ins.CaculateUnderWaterDistance(worldPos);
            if (m_meshPoints[i].underWaterDistance < 0) {
                underWaterPointCount++;
            }
        }


        //Debug.Log(underWaterPointCount);


        int[] triangles = m_mesh.triangles;
        int j = 0;


        List<MeshPoint> trianglePoints = new List<MeshPoint>();
        trianglePoints.Add(new MeshPoint());
        trianglePoints.Add(new MeshPoint());
        trianglePoints.Add(new MeshPoint());


        underWaterPolygon.Reset();

        while (j < triangles.Length) {

            //遍历所有的三角面
            for (int x = 0; x < 3; x++) {
                MeshPoint vertex = m_meshPoints[triangles[j]];
                trianglePoints[x].underWaterDistance = vertex.underWaterDistance;
                trianglePoints[x].worldPos = vertex.worldPos;
                trianglePoints[x].index = x;
                j++;
            }

            //三个顶点都在水面以上
            if (trianglePoints[0].underWaterDistance > 0 && trianglePoints[1].underWaterDistance > 0 && trianglePoints[2].underWaterDistance > 0) {
                continue;
            }

            //三个顶点都在水面以下
            if (trianglePoints[0].underWaterDistance < 0 && trianglePoints[1].underWaterDistance < 0 && trianglePoints[2].underWaterDistance < 0)
            {
                //Debug.Log("三个顶点都在水面下");
                Vector3 p1 = trianglePoints[0].worldPos;
                Vector3 p2 = trianglePoints[1].worldPos;
                Vector3 p3 = trianglePoints[2].worldPos;

                underWaterPolygon.AddTriangleData(new TriangleData(p1, p2, p3));
            }
            else {
   
                //根据距离水面的距离从大到小排序
                trianglePoints.Sort((x, y) => x.underWaterDistance.CompareTo(y.underWaterDistance));
                trianglePoints.Reverse();

                //有一个顶点在水上
                if (trianglePoints[0].underWaterDistance > 0 && trianglePoints[1].underWaterDistance < 0 && trianglePoints[2].underWaterDistance < 0)
                {
                    //Debug.Log("两个顶点在水面下");
                    this.AddTrianglesOneAboveWater(trianglePoints);
                }
                //有两个顶点在水上
                else if(trianglePoints[0].underWaterDistance > 0 && trianglePoints[1].underWaterDistance > 0 && trianglePoints[2].underWaterDistance < 0)
                {
                    //Debug.Log("一个顶点在水面下");
                    this.AddTrianglesTwoAboveWater(trianglePoints);
                }
            }
        }

    }

    void AddTrianglesOneAboveWater(List<MeshPoint> trianglePoints) {
        Vector3 H = trianglePoints[0].worldPos;

        int M_index = trianglePoints[0].index - 1;
        if (M_index < 0) {
            M_index = 2;
        }

        float h_H = trianglePoints[0].underWaterDistance;
        float h_M = 0f;
        float h_L = 0;

        Vector3 M = Vector3.zero;
        Vector3 L = Vector3.zero;

        if (trianglePoints[1].index == M_index)
        {
            M = trianglePoints[1].worldPos;
            L = trianglePoints[2].worldPos;

            h_M = trianglePoints[1].underWaterDistance;
            h_L = trianglePoints[2].underWaterDistance;
        }
        else {
            M = trianglePoints[2].worldPos;
            L = trianglePoints[1].worldPos;

            h_M = trianglePoints[2].underWaterDistance;
            h_L = trianglePoints[1].underWaterDistance;
        }

        Vector3 MH = H - M;
        float t_M = -h_M / (h_H - h_M);
        Vector3 I_M = M + t_M * MH;

        Vector3 LH = H - L;
        float t_L = -h_L / (h_H - h_L);
        Vector3 I_L = L + t_L * LH;

        underWaterPolygon.AddTriangleData(new TriangleData(M, I_M, I_L));
        underWaterPolygon.AddTriangleData(new TriangleData(M, I_L, L));

    }

    void AddTrianglesTwoAboveWater(List<MeshPoint> trianglePoints)
    {
        Vector3 L = trianglePoints[2].worldPos;

        int H_index = trianglePoints[2].index + 1;
        if(H_index > 2) {
            H_index = 0;
        }

        float h_L = trianglePoints[2].underWaterDistance;
        float h_H = 0;
        float h_M = 0;

        Vector3 H = Vector3.zero;
        Vector3 M = Vector3.zero;

        if (trianglePoints[1].index == H_index)
        {
            h_H = trianglePoints[1].underWaterDistance;
            h_M = trianglePoints[0].underWaterDistance;

            H = trianglePoints[1].worldPos;
            M = trianglePoints[0].worldPos;
        }
        else {
            h_H = trianglePoints[0].underWaterDistance;
            h_M = trianglePoints[1].underWaterDistance;

            H = trianglePoints[0].worldPos;
            M = trianglePoints[1].worldPos;
        }


        Vector3 LM = M - L;
        float t_M = - h_L / (h_M - h_L);
        Vector3 J_M = L + t_M * LM;

        Vector3 LH = H - L;
        float t_H = - h_L / (h_H - h_L);
        Vector3 J_H = L + t_H * LH;

        underWaterPolygon.AddTriangleData(new TriangleData(L, J_H, J_M));
    }

    void ApplyBouncyForce()
    {

        if (underWaterPolygon.triangles.Count == 0)
        {
            return;
        }

   
        Vector3 bouyanceForce;
        Vector3 localCenterPos;

        //List<TriangleData> triangleList = underWaterPolygon.triangles;
        //for (int i = 0; i < triangleList.Count; i++)
        //{
        //    float underwaterDistance = Wave.Ins.CaculateUnderWaterDistance(triangleList[i].CenterPos);
        //    bouyanceForce = CaculateBouyanceForce(rb.mass / 5, triangleList[i].Area, underwaterDistance);
        //    localCenterPos = transform.InverseTransformPoint(triangleList[i].CenterPos);
        //    rb.AddForceAtPosition(bouyanceForce, localCenterPos, ForceMode2D.Force);
        //}


        Vector3 polygonCenterPos = underWaterPolygon.CenterPos;
        float polygonUnderwaterDistance = Wave.Ins.CaculateUnderWaterDistance(polygonCenterPos);
        bouyanceForce = CaculateBouyanceForce(density, underWaterPolygon.Area, polygonUnderwaterDistance);
        localCenterPos = transform.InverseTransformPoint(polygonCenterPos);
        rb.AddForceAtPosition(bouyanceForce, localCenterPos, ForceMode2D.Force);

        Debug.Log("浮力" + bouyanceForce + " local center pos " + localCenterPos + " world center pos " + polygonCenterPos + " underwater area = " + underWaterPolygon.Area + " under water distance = " + polygonUnderwaterDistance);

        ApplyDrag(bouyanceForce);

    }

    Vector2 CaculateBouyanceForce(float density, float area, float underWaterDistance)
    {
        Vector2 bouyanceForce = density * area * Physics2D.gravity * underWaterDistance;
        return bouyanceForce;
    }

    void ApplyDrag(Vector2 bouyanceForce) {
        Vector3 velocity = rb.velocity;
        float vel = velocity.magnitude;

        float dragMag = vel *  underWaterPolygon.Area;
        Vector2 dragForce = - dragMag * velocity.normalized;
        rb.drag = dragMag;

        float mg = -rb.mass * Physics2D.gravity.y;
        float angularDrag = 0;

        Debug.Log("重力 = " + mg + " 浮力 = " + bouyanceForce.y);
        //重力大于浮力
        if ( Mathf.Abs(mg - bouyanceForce.y) <= 2 )
        { 
            angularDrag = 10000;
        }
        else  //重力小于浮力
        {
            angularDrag = rb.angularVelocity * 100 * mg / bouyanceForce.y ;
        }

        rb.angularDrag = Mathf.Abs(angularDrag);
        //rb.AddTorque(angularDrag);

        Debug.Log("drag force = " + dragForce + " angular drag = " + angularDrag +  " angular velocity = " + rb.angularVelocity + " under water area = " + underWaterPolygon.Area);
    }



    void DisplayUnderWaterMesh() {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();


        //Debug.Log("水下三角形数量" + underWaterPolygon.triangles.Count);

        List<TriangleData> triangleList = underWaterPolygon.triangles;
        for (int i = 0; i < triangleList.Count; i++) {


            Vector3 p1 = transform.InverseTransformPoint(triangleList[i].p1);
            Vector3 p2 = transform.InverseTransformPoint(triangleList[i].p2);
            Vector3 p3 = transform.InverseTransformPoint(triangleList[i].p3);

            vertices.Add(p1);
            triangles.Add(vertices.Count - 1);

            vertices.Add(p2);
            triangles.Add(vertices.Count - 1);

            vertices.Add(p3);
            triangles.Add(vertices.Count - 1);
        }

        if(m_underWaterMesh != null) {
            m_underWaterMesh.Clear();

            m_underWaterMesh.vertices = vertices.ToArray();
            m_underWaterMesh.triangles = triangles.ToArray();

            m_underWaterMesh.RecalculateNormals();

            //Debug.Log("水下部分顶点数" + vertices.Count);
        }
    }

}
