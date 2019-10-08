using System.Collections.Generic;
using UnityEngine;


public class SPHParticle {

    public Vector2 Position;

    public Vector2 Velocity;

    public float Density;

    public float Pressure;

    public Vector2 Force;

    public GameObject go;
    
    public SPHParticle(Vector2 pos) {
        Position = pos;
        Velocity = Vector2.zero;
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localPosition = Position;
        go.transform.localScale = Vector3.one * 0.25f;
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

    // Start is called before the first frame update
    void Start()
    {
        InitParticles();
    }

    void InitParticles() {

        int side = (int)Mathf.Sqrt(ParticlesNum);
        float dx = SmoothRaidus * 0.75f;
        Vector2 startPos = new Vector2(transform.position.x - dx * side / 2, transform.position.y + dx * side / 2);
        Debug.Log(startPos);
        for (int x = 0; x < side; x++) {
            for (int y = 0; y < side; y++) {
                SPHParticle p = new SPHParticle(new Vector2(startPos.x + dx * x, startPos.y - dx * y));
                m_particles.Add(p);
            }
        }
    }


    private List<SPHParticle> m_TempParticelList = new List<SPHParticle>();

    // Update is called once per frame
    void FixedUpdate()
    {

        //计算密度和压强
        for (int i = 0; i < m_particles.Count; i++)
        {
            SPHParticle p = m_particles[i];
            p.Density = 0;
            m_TempParticelList.Clear();
            GetNearParticle(p, m_TempParticelList);
            //Debug.Log(m_TempParticelList.Count);
            for (int j = 0; j < m_TempParticelList.Count; j++)
            {
                float sqrDistance = (p.Position - m_TempParticelList[j].Position).sqrMagnitude;
                p.Density += mass * Kernels.Poly6(sqrDistance, SmoothRaidus);
            }

            p.Density = Mathf.Max(p.Density, restDensity);

            p.Pressure = kStiffness * (p.Density - restDensity);
        }


        ////计算压力，粘力
        for (int i = 0; i < m_particles.Count; i++)
        {
            SPHParticle p = m_particles[i];
            p.Force = Vector2.zero;
            //压力梯度
            Vector2 presureGradient = Vector2.zero;
            //粘力梯度
            Vector2 visosityGradient = Vector2.zero;
            m_TempParticelList.Clear();
            GetNearParticle(p, m_TempParticelList);
            for (int j = 0; j < m_TempParticelList.Count; j++)
            {
                presureGradient += PressureForce(p, m_TempParticelList[j]);
                visosityGradient += VisosityForce(p, m_TempParticelList[j]);
            }

            p.Force = presureGradient + visosityGradient;
            p.Force += Physics2D.gravity;
        }

        //计算速度，加速度，位置
        for (int i = 0; i < m_particles.Count; i++) {
            SPHParticle p = m_particles[i];
            ForceBounds(p);
            Vector2 acceleration = p.Force;
            float dt = Time.deltaTime;
            p.Velocity += acceleration * dt;
            //Vector2 deltaPos = p.Velocity * dt + 0.5f * acceleration * dt * dt;
            p.Position += p.Velocity * dt;
            p.go.transform.localPosition = p.Position;

        }
    }

    Vector2 PressureForce(SPHParticle currentParticle, SPHParticle neightborParticle) {
        float diviend = currentParticle.Pressure + neightborParticle.Pressure;
        float divisor = 2 * currentParticle.Density * neightborParticle.Density;

        Vector2 r = currentParticle.Position - neightborParticle.Position;
        return -mass * (diviend / divisor) * Kernels.GradientSpiky(r, SmoothRaidus);
    }

    Vector2 VisosityForce(SPHParticle currentParticle, SPHParticle neightborParticle) {
        Vector2 r = currentParticle.Position - neightborParticle.Position;
        Vector2 v = neightborParticle.Velocity - currentParticle.Velocity;
        return Viscosity * v * (mass / currentParticle.Density) * Kernels.ViscosityLaplacian(r.magnitude, SmoothRaidus);
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
        float leftBoundX = - BoundSize.x / 2;
        float rightBoundX = BoundSize.x / 2;
        float bottomBoundY = - BoundSize.y / 2;
        float topBoundY = BoundSize.y / 2;

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
