using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityAsync;

public class MedicalKit : Consumable, IInterruptible
{
    Task CurrentTask;
    bool Interrupted = false;
    public async void Interrupt()
    {
        Interrupted = true;
        await CurrentTask;
        Interrupted = false;
    }

    public override void Init()
    {
        base.Init();
        Stats.AddStat(ItemTypes.StatsEnum.Quality, 0.5f);
    }

    //TODO Continue action after movement is finished
    public async override void Use(Actor UsingActor)
    {
        ISelectable Selected = await TargetSelect.StartSelect();
        

        if (Selected is Biotic Target)
        {
            UsingActor.InterruptCurrent();
            UsingActor.ActionInterrupt = this;
            Task MovementTask = UsingActor.EntityComponents.Movement.SetDestination(Target.transform.position, null);
            await Await.Until(() => MovementTask.IsCompleted).ConfigureAwait(this);
            CurrentTask = Heal(Target, UsingActor);
        }
    }

    private async Task Heal(Biotic Target, Actor UsingActor)
    {
        List<Medical.Injury> InjuriesBySeverity = Target.EntityComponents.Health.Injuries.OrderByDescending(injury => injury.SeverityCost).ToList();
        foreach (Medical.Injury injury in InjuriesBySeverity)
        {
            if (injury.Tended) continue;
            if (Stats.GetStat<int>(ItemTypes.StatsEnum.Quantity) <= 0)
            {
                DestroyEntity();
                return;
            }
            await HealInstance(Target, UsingActor, injury, Mathf.Pow(injury.SeverityCost, 0.5f));
            if (Interrupted)
            {
                return;
            }
        }
    }
    
    private async Task HealInstance(Biotic Target, Actor UsingActor, Medical.Injury injury, float time)
    {
        ProgressBar Bar = UIController.InstantiateProgressBar(Target.gameObject, 0, time, Color.grey, Color.green);
        float CurrentTime = 0;
        while (CurrentTime < time)
        {
            if (Interrupted)
            {
                Bar.Destroy();
                return;
            }
            CurrentTime += Time.deltaTime;
            Bar.UpdateBar(CurrentTime);
            await Await.NextUpdate().ConfigureAwait(this);
        }
        //TODO Come up with good formula for quality
        Stats.SetStat(ItemTypes.StatsEnum.Quantity, 1, ItemStats.OperationEnum.Subtract);
        float Quality = Mathf.Clamp01(UnityEngine.Random.Range(0.1f, 200) * (UsingActor.EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Medical].GetAdjustedLevel() / 100f) * Stats.GetStat<float>(ItemTypes.StatsEnum.Quality));
        UsingActor.EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Medical].AddXP(injury.SeverityCost);
        injury.Tend(Quality);
        Bar.Destroy();
    }
}
