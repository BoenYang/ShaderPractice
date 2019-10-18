using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class SPHParticle {

    public Vector2 Position;

    public Vector2 Velocity;

    public float Density;

    public float Pressure;

    public Vector2 Force;

    public GameObject go;

    public List<SPHParticle> neighbors;

    public Transform goTr;
    
    public SPHParticle(Vector2 pos) {
        Position = pos;
        Velocity = Vector2.zero;
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        goTr = go.transform;
        goTr.localPosition = Position;
        goTr.localScale = Vector3.one * 0.25f;
        go.GetComponent<SphereCollider>().enabled = false;
        neighbors = new List<SPHParticle>();
    }

}

public class SPH : MonoBehaviour
{
    public int ParticlesNum = 100;

    public float SmoothRaidus = 0.7f;

    public float mass = 20f;

    //液体密度
    public float restDensity = 82.0f;

    //流体刚度
    [Range(20f, 100f)]
    public float kStiffness = 50f;

    //流体粘度系数
    public float Viscosity = 0.6f;

    public Vector2 BoundSize = new Vector2(20,20);

    private List<SPHParticle> m_particles = new List<SPHParticle>();

    private Transform cacheTr;

    private float polyCoef;

    private float gradientSpikyCoef;

    private float viscosityLaplacianCoef;

    private Grid2D grid;
    
    // Start is called before the first frame update
    void Start()
    {
        InitParticles();
        cacheTr = transform;

        polyCoef = 315f / (64f * Mathf.PI * Mathf.Pow(SmoothRaidus, 9));

        gradientSpikyCoef = 45f / (Mathf.PI * Mathf.Pow(SmoothRaidus, 6));

        viscosityLaplacianCoef = 45f / (Mathf.PI * Mathf.Pow(SmoothRaidus, 6));

    }

    void InitParticles() {
        m_particles.Clear();
        int side = (int)Mathf.Sqrt(ParticlesNum);
        float dx = SmoothRaidus * 0.75f;
        Vector2 startPos = new Vector2(transform.position.x - dx * side / 2, transform.position.y + dx * side / 2);
        for (int x = 0; x < side; x++) {
            for (int y = 0; y < side; y++) {
                SPHParticle p = new SPHParticle(new Vector2(startPos.x + dx * x, startPos.y - dx * y));
                m_particles.Add(p);
            }
        }

        grid = new Grid2D(startPos, BoundSize, SmoothRaidus);
    }


    private List<SPHParticle> m_TempParticelList ;

    void Update()
    {
        grid.Clear();

        for (int i = 0; i < m_particles.Count; i++) {
            grid.Add(m_particles[i]);
        }

        Profiler.BeginSample("CalculateDensityAndPressure");
        //计算密度和压强
        for (int i = 0; i < m_particles.Count; i++)
        {
            SPHParticle p = m_particles[i];
            p.Density = 0;
            Profiler.BeginSample("Clear");
            p.neighbors.Clear();
            Profiler.EndSample();
            Profiler.BeginSample("GetNeighbor");
            m_TempParticelList = grid.GetNearby(p);
            //GetNearParticle(p, m_TempParticelList);
            Profiler.EndSample();
            Profiler.BeginSample("AddRange");
            p.neighbors.AddRange(m_TempParticelList);
            Profiler.EndSample();
            Profiler.BeginSample("GetDensity");
            for (int j = 0; j < p.neighbors.Count; j++)
            {
                if (p.neighbors[j] != p) {
                    float sqrDistance = (p.Position - p.neighbors[j].Position).sqrMagnitude;
                    p.Density += mass * Kernels.Poly6(sqrDistance, polyCoef, SmoothRaidus);
                }
            }
            Profiler.EndSample();

            p.Density = Mathf.Max(p.Density, restDensity);
            p.Pressure = kStiffness * (p.Density - restDensity);
        }
        Profiler.EndSample();


        Profiler.BeginSample("CalculateForce");
        ////计算压力，粘力
        for (int i = 0; i < m_particles.Count; i++)
        {
            SPHParticle p = m_particles[i];
            p.Force = Vector2.zero;
            //压力梯度
            Vector2 presureGradient = Vector2.zero;
            //粘力梯度
            Vector2 visosityGradient = Vector2.zero;
            //m_TempParticelList.Clear();
            //GetNearParticle(p, m_TempParticelList);
            for (int j = 0; j < p.neighbors.Count; j++)
            {
                if (p.neighbors[j] != p) {
                    Profiler.BeginSample("PressureForce");
                    presureGradient += PressureForce(p, p.neighbors[j]);
                    Profiler.EndSample();
                    Profiler.BeginSample("VisosityForce");
                    visosityGradient += VisosityForce(p, p.neighbors[j]);
                    Profiler.EndSample();
                }
            }

            p.Force = presureGradient + visosityGradient;
            p.Force += Physics2D.gravity;
        }
        Profiler.EndSample();

        Profiler.BeginSample("CalculatePosition");
        float dt = Time.fixedDeltaTime;
        //计算速度，加速度，位置
        for (int i = 0; i < m_particles.Count; i++) {
            SPHParticle p = m_particles[i];
            Profiler.BeginSample("ForceBounds");
            ForceBounds(p);
            Profiler.EndSample();
            Vector2 acceleration = p.Force;
            p.Velocity += acceleration * dt;
            //Vector2 deltaPos = p.Velocity * dt + 0.5f * acceleration * dt * dt;
            p.Position += p.Velocity * dt;
            Profiler.BeginSample("SetPosition");
            p.goTr.localPosition = p.Position;
            Profiler.EndSample();
        }
        Profiler.EndSample();
    }

    Vector2 PressureForce(SPHParticle currentParticle, SPHParticle neightborParticle) {

        float diviend = currentParticle.Pressure + neightborParticle.Pressure;
        float divisor = 2 * currentParticle.Density * neightborParticle.Density;

        Vector2 r = currentParticle.Position - neightborParticle.Position;
        return -mass * (diviend / divisor) * Kernels.GradientSpiky(r, gradientSpikyCoef, SmoothRaidus);
    }

    Vector2 VisosityForce(SPHParticle currentParticle, SPHParticle neightborParticle) {
        Vector2 r = currentParticle.Position - neightborParticle.Position;
        Vector2 v = neightborParticle.Velocity - currentParticle.Velocity;
        return Viscosity * v * (mass / currentParticle.Density) * Kernels.ViscosityLaplacian(r.magnitude,viscosityLaplacianCoef,SmoothRaidus);
    }

    private void GetNearParticle(SPHParticle particle, List<SPHParticle> list) {
        for (int i = 0; i < m_particles.Count; i++) {
            SPHParticle p = m_particles[i];
            if (particle == p) {
                continue;
            }

            Vector2 dir = particle.Position - p.Position;
            if (dir.sqrMagnitude <= SmoothRaidus * SmoothRaidus) {
                list.Add(p);
            }
        }
    }

    void ForceBounds(SPHParticle p) {
        float velocityDamping = 0.5f;

        Vector2 pos = p.Position;
        Vector2 parentPos = cacheTr.position;
        float leftBoundX = parentPos.x - BoundSize.x / 2;
        float rightBoundX = parentPos.x + BoundSize.x / 2;
        float bottomBoundY = parentPos.y - BoundSize.y / 2;
        float topBoundY = parentPos.y + BoundSize.y / 2;

        //Debug.Log(bottomBoundY);

        if (pos.x < leftBoundX)
        {
            p.Position.x = leftBoundX;
            p.Velocity.x = -p.Velocity.x * velocityDamping;
            p.Force.x = 0;
            //Debug.Log("out left bound x");
        }
        else if (pos.x > rightBoundX)
        {
            p.Position.x = rightBoundX;
            p.Velocity.x = -p.Velocity.x * velocityDamping;
            p.Force.x = 0;
            //Debug.Log("out right bound x");
        }
        else if (pos.y < bottomBoundY) {
            p.Position.y = bottomBoundY;
            p.Velocity.y = -p.Velocity.y * velocityDamping;
            p.Force.y = 0;
            //Debug.Log("out bottom bound y");
        } else if (pos.y > topBoundY) {
            p.Position.y = topBoundY;
            p.Velocity.y = -p.Velocity.y * velocityDamping;
            p.Force.y = 0;
            //Debug.Log("out top bound y");
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(BoundSize.x, BoundSize.x, 0.1f));
        Gizmos.color = Color.blue;
        if (m_particles.Count > 0 && Application.isPlaying) {
            for (int i = 0; i < m_particles.Count; i++) {
                //Gizmos.DrawWireSphere(m_particles[i].Position, 0.15f);
            }
        }
    }
}
