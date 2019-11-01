//Possible improvement: Reduce impact of Alignment when boids enter/near the acceptable destination radius

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    NavMeshAgent Agent;
    Camera Cam;
    Entity entity;

    Texture2D Tex;
    Vector3 Destination;
    NavMeshPath DestinationPath;

    public bool Arrived;

    FlockController Flock;

    int boids; //Count of Boids
    float DistanceScale = 10.0f;
    public float SeparationDistance;

    [Range(0,100)]
    public float PathResolution = 50;

    // Start is called before the first frame update
    void Start()
    {
        entity = GetComponent<Entity>();
        Agent = GetComponent<NavMeshAgent>();
        Cam = Camera.main;

        Tex = new Texture2D(1, 1);
        Tex.SetPixel(0, 0, Color.green);
        Tex.Apply();
    }
    bool HasRun = false;
    private void Update()
    {
        if (entity.EntityFlags.HasFlag(Entity.EntityFlagsEnum.HasStats) && !HasRun)
        {
            StartCoroutine(SetMoveSpeed());
            HasRun = true;
        }
    }

    IEnumerator SetMoveSpeed()
    {
        while (true)
        {
            if (entity.animator.GetBool("Moving"))
            {
                entity.EntityComponents.Stats.AddXP(StatsAndSkills.SkillsEnum.Athletics, 2500);
            }
            Agent.speed = entity.BaseStats.MoveSpeed * (1f + (entity.EntityComponents.Stats.GetSkillInfo(StatsAndSkills.SkillsEnum.Athletics).Level / 100f * 2.0f));
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void SetDestination(Vector3 Aim, FlockController flockController)
    {
        CancelInvoke(nameof(AdjustPath));
        float PathingDelay = Mathf.Clamp(1 - PathResolution / 100, 0.1f, 1f);
        if (Flock)
        {
            Flock.FlockMembers.Remove(this);
            if (Flock.FlockMembers.Count == 0) Destroy(Flock);
        }
        Flock = flockController;
        Agent.SetDestination(Aim);
        Destination = Agent.destination;
        DestinationPath = Agent.path;
        Arrived = false;

        InvokeRepeating(nameof(AdjustPath), 0.1f, PathingDelay);
        entity.animator.SetBool("Moving", true);
    }

    //Adjust waypoints according to flocking rules
    void AdjustPath()
    {
        if (Destination != null && Flock && Flock.FlockMembers.Count > 1)
        {
            NavMesh.CalculatePath(transform.position, Destination, NavMesh.AllAreas, DestinationPath);
            Vector3 Adjusted;

            if (DestinationPath.corners.Length > 1)
            {
                if (Vector3.Distance(transform.position, DestinationPath.corners[1]) > 2 && Flock)
                {
                    //Adjust first corner
                    Adjusted = CalculateAdjustment(DestinationPath.corners[1]);
                    Agent.SetDestination(Adjusted);
                }
                else if (Vector3.Distance(transform.position, Destination) < Mathf.Pow(boids, 1 / 3) * DistanceScale)
                {
                    Arrived = true;
                    entity.animator.SetBool("Moving", false);
                    if (TestFlock())
                    {
                        CancelInvoke(nameof(AdjustPath));
                        Agent.ResetPath();
                        return;
                    }
                }
                else if (DestinationPath.corners.Length > 2 && Flock)
                {
                    //Adjust second corner
                    Adjusted = CalculateAdjustment(DestinationPath.corners[2]);
                    Agent.SetDestination(Adjusted);
                }
            }
        } else if (Destination != null)
        {
            if (Vector3.Distance(transform.position, Destination) < 0.25)
            {
                Arrived = true;
                entity.animator.SetBool("Moving", false);
                CancelInvoke(nameof(AdjustPath));
                Agent.ResetPath();
                return;
            }
        }
    }

    Vector3 CalculateAdjustment(Vector3 point)
    {
        Vector3 pos = transform.position;
        Vector3 Alignment = (point - pos).normalized;
        Vector3 Cohesion = pos;
        Vector3 Separation = Vector3.zero;

        boids = 1;

        Collider[] _ = Physics.OverlapSphere(pos, 10);
        List<GameObject> NearbyObjs = new List<GameObject>();

        for (int i = 0; i < _.Length; i++)
        {
            NearbyObjs.Add(_[i].gameObject);
        }

        foreach (Movement boid in Flock.FlockMembers)
        {
            if (boid.gameObject == gameObject || !NearbyObjs.Contains(boid.gameObject)) continue;
            
            Transform t = boid.transform;

            Cohesion += t.position;
            Separation += GetSeparationVector(t);
            boids++;
        }

        if (boids > 0)
        {
            float Average = 1.0f / (boids);
            Cohesion *= Average;
        }
        Cohesion = (Cohesion - pos);

        Vector3 AdjustedHeading = (Alignment * 4 + Separation + Cohesion).normalized;
        Debug.DrawRay(transform.position, Alignment * (point - pos).magnitude, Color.magenta, 0.5f);
        Debug.DrawRay(transform.position, Separation, Color.white, 0.5f);
        Debug.DrawRay(transform.position, Cohesion, Color.blue, 0.5f);
        return pos + (AdjustedHeading * (point - pos).magnitude);
    }

    bool TestFlock()
    {

        foreach (Movement Boid in Flock.FlockMembers)
        {
            if (Boid.Arrived == false)
            {
                return false;
            }
        }
        return true;
    }

    Vector3 GetSeparationVector(Transform target)
    {
        Vector3 Opposite = transform.position - target.position;
        float OppositeMag = Opposite.magnitude;
        float ScaledDir = Mathf.Clamp01(1.0f - OppositeMag / SeparationDistance);
        return Opposite.normalized * ScaledDir;
    }

    private void OnGUI()
    {
        for (int i = 0; i < Agent.path.corners.Length; i++)
        {
            Vector3 waypoint = Agent.path.corners[i];
            Vector3 WTSP = Cam.WorldToScreenPoint(waypoint);
            Vector3 ScreenPos = new Vector3(WTSP.x, Screen.height - WTSP.y, WTSP.z);
            GUI.DrawTexture(new Rect(ScreenPos, new Vector2(5, 5)), Tex);
            if (Agent.path.corners.Length > i + 1)
            {
                Debug.DrawLine(Agent.path.corners[i], Agent.path.corners[i + 1], Color.green, 0.1f);
            }
        }
    }
}
