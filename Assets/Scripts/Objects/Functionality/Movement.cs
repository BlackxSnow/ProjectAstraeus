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
        await ParentActor.EntityComponents.Stats.Initialised.WaitAsync();
        while (true)
        {
            if (ParentActor.animator.GetBool("Moving"))
            {
                ParentActor.EntityComponents.Stats.AddXP(StatsAndSkills.SkillsEnum.Athletics, 25);
            }
            
            Agent.speed = ParentActor.BaseEntityStats.MovementSpeed * (1f + (ParentActor.EntityComponents.Stats.GetSkillInfo(StatsAndSkills.SkillsEnum.Athletics).Level / 100f * 2.0f));
            await Await.Seconds(0.25f).ConfigureAwait(this);
        }
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
