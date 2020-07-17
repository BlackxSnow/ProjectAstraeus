using AI.States;
using Medical;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class MedicalKit : Consumable, IInterruptible
{
    CancellationTokenSource tokenSource = new CancellationTokenSource();

    public void Interrupt()
    {
        tokenSource.Cancel();
    }

    public override void Init()
    {
        base.Init();
    }

    public async override void Use(Actor UsingActor)
    {
        ISelectable Selected = await TargetSelect.StartSelect();

        if (Selected is Biotic Target)
        {
            UsingActor.StateMachine.SetState(new UseItem(UsingActor, null, Target, this));
        }
    }

    public override bool Act(Entity user, Entity target, object iteratedOn)
    {
        Injury injury = iteratedOn as Injury;
        //TODO Come up with good formula for quality
        Stats.Quantity--;
        float Quality = Mathf.Clamp01(UnityEngine.Random.Range(0.1f, 200) * (user.EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Medical].GetAdjustedLevel() / 100f) * Stats.Quality);
        user.EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Medical].AddXP(injury.SeverityCost * 100);
        injury.Tend(Quality);

        if (Stats.Quantity <= 0)
        {
            DestroyEntity();
            return false;
        }
        return true;
    }

    public override bool GetNextIteration(Entity target, out object iterateOn, out float time)
    {
        List<Medical.Injury> injuriesBySeverity = target.EntityComponents.Health.Injuries.Where(injury => injury.Tended == false).OrderByDescending(injury => injury.SeverityCost).ToList();
        if (injuriesBySeverity.Count == 0)
        {
            iterateOn = null;
            time = 0;
            return false;
        }
        iterateOn = injuriesBySeverity[0];
        time = Mathf.Pow(injuriesBySeverity[0].SeverityCost, 0.5f);
        return true;
    }

    public override void CalculateStats()
    {
        throw new System.NotImplementedException();
    }
}
