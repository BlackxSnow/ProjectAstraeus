//Possible improvement: Reduce impact of Alignment when boids enter/near the acceptable destination radius
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityAsync;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    NavMeshAgent Agent;
    Camera Cam;
    Actor ParentActor;

    Texture2D Tex;
    Vector3 Destination;
    NavMeshPath DestinationPath;

    CancellationTokenSource PathTokenSource = new CancellationTokenSource();
    public bool Arrived;

    FlockController Flock;

    int boids; //Count of Boids
    float DistanceScale = 10.0f;
    public float SeparationDistance;

    [Range(0,100)]
    public float PathResolution = 50;

    public delegate void MovementHandler();
    public event MovementHandler MovementFinished;
    

    // Start is called before the first frame update
    void Start()
    {
        ParentActor = GetComponent<Actor>();
        Agent = GetComponent<NavMeshAgent>();
        Cam = Camera.main;

        Tex = new Texture2D(1, 1);
        Tex.SetPixel(0, 0, Color.green);
        Tex.Apply();
    }
    bool HasRun = false;
    private void Update()
    {
        if (ParentActor.EntityFlags.HasFlag(Entity.EntityFlagsEnum.HasStats) && !HasRun)
        {
            SetMoveSpeed();
            HasRun = true;
        }
    }

    private async void SetMoveSpeed()
    {
        while (true)
        {
            if (ParentActor.animator.GetBool("Moving"))
            {
                ParentActor.EntityComponents.Stats.AddXP(StatsAndSkills.SkillsEnum.Athletics, 25);
            }
            await DataManager.DataLoaded.WaitAsync();
            Agent.speed = ParentActor.BaseEntityStats.MovementSpeed * (1f + (ParentActor.EntityComponents.Stats.GetSkillInfo(StatsAndSkills.SkillsEnum.Athletics).Level / 100f * 2.0f));
            await Await.Seconds(0.25f).ConfigureAwait(this);
        }
    }

    public void StopMove()
    {
        Arrived = true;
        ParentActor.animator.SetBool("Moving", false);
        Agent.velocity = Vector3.zero;
        Agent.ResetPath();
    }

    /// <summary>
    /// Moves an actor within distance of aim
    /// </summary>
    /// <param name="target">The target location as Vector3</param>
    /// <param name="distance">The maximum distance from the aim</param>
    /// <param name="flockController">The flocking controller if this character is in a flock; may be null</param>
    /// <returns>A task that indicates whether the move was successful</returns>
    public async Task<bool> MoveWithin(Vector3 target, float distance, FlockController flockController, CancellationToken token)
    {
        bool Completed = false;
        while(!Completed)
        {
            Vector3 positionDiff = transform.position - target;
            positionDiff.y = Mathf.Pow(positionDiff.y, 2);
            float ModifiedDistance = positionDiff.magnitude;
            if (ModifiedDistance < distance)
            {
                StopMove();
                return true;
            }
            else if (token.IsCancellationRequested)
            {
                StopMove();
                return false;
            }

            Task<bool> destinationTask = SetDestination(target, flockController, token, false);
            await destinationTask;
            Completed = destinationTask.Result;
        }
        
        return true;
    }
    public async Task<bool> MoveWithin(GameObject Target, float distance, FlockController flockController, CancellationToken token)
    {
        bool Completed = false;

        while (!Completed)
        {
            Vector3 positionDiff = transform.position - Target.transform.position;
            positionDiff.y = Mathf.Abs(positionDiff.y * 2);
            float ModifiedDistance = positionDiff.magnitude;
            if (ModifiedDistance < distance)
            {
                StopMove();
                return true;
            }
            else if (token.IsCancellationRequested)
            {
                StopMove();
                return false;
            }
            await SetDestination(Target.transform.position, flockController, token, false);
        }

        return false;
    }

    //TODO Implement move within function
    /// <summary>
    /// Sets the destination of the character's movement AI
    /// </summary>
    /// <param name="Aim">The target location as a Vector3</param>
    /// <param name="flockController">The flocking controller if this character is in a flock; may be null</param>
    /// <returns>A task that completes upon reaching the destination</returns>
    public async Task<bool> SetDestination(Vector3 Aim, FlockController flockController, CancellationToken token, bool loop = true)
    {
        float PathingDelay = SetDestination_Setup(Aim, flockController);

        bool Completed = false;


        while (!Completed)
        {
            Completed = AdjustPath();
            try
            {
                await Task.Delay(Mathf.RoundToInt(PathingDelay * 1000f), token);
            }
            catch (TaskCanceledException) { }

            if (!loop) return Completed;
            else if (token.IsCancellationRequested)
            {
                StopMove();
                return false;
            }
        } 

        return Completed;
    }

    private float SetDestination_Setup(Vector3 Aim, FlockController flockController)
    {
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
        ParentActor.animator.SetBool("Moving", true);
        return PathingDelay;
    }

    //Adjust waypoints according to flocking rules
    private bool AdjustPath()
    {
        if (Destination != null && Flock && Flock.FlockMembers.Count > 1)
        {
            return AdjustPath_Flock();
        }
        else if (Destination != null)
        {
            if (Vector3.Distance(transform.position, Destination) < 0.25)
            {
                StopMove();
                return true;
            }
        }
        return false;
    }

    private bool AdjustPath_Flock()
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
                ParentActor.animator.SetBool("Moving", false);
                if (TestFlock())
                {
                    Agent.ResetPath();
                    if (MovementFinished != null) MovementFinished.Invoke();
                    return true;
                }
                else return false;
            }
            else if (DestinationPath.corners.Length > 2 && Flock)
            {
                //Adjust second corner
                Adjusted = CalculateAdjustment(DestinationPath.corners[2]);
                Agent.SetDestination(Adjusted);
                return false;
            }
        }
        throw new System.Exception($"AdjustPath_Flock did not return");
    }
    private Vector3 CalculateAdjustment(Vector3 point)
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

    private bool TestFlock()
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

    private Vector3 GetSeparationVector(Transform target)
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
