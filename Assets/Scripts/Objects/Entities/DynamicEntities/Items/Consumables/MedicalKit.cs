using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityAsync;
using System.Threading;
using AI.States;

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
        Stats.AddStat(ItemTypes.StatsEnum.Quality, 0.5f);
    }

    //TODO Continue action after movement is finished
    //TODO refactor to state system
    public async override void Use(Actor UsingActor)
    {
        ISelectable Selected = await TargetSelect.StartSelect();

        if (Selected is Biotic Target)
        {
            UsingActor.StateMachine.ClearStates();
            CancellationToken token = UsingActor.TokenSource.Token;
            UsingActor.ActionInterrupt = this;
            Collider col = Target.GetComponent<Collider>();
            float boundsSize = Mathf.Max(col.bounds.size.x, col.bounds.size.z);

            MoveWithin moveState = new MoveWithin(this, null, Target.gameObject, boundsSize * 2f, null);
            moveState.Token = token;
            UsingActor.StateMachine.SetState(moveState);
            await moveState.StateCompleted.WaitAsync();
            if (token.IsCancellationRequested || !moveState.Succeeded) return;
            
            _ = Heal(Target, UsingActor, boundsSize * 2f, token);
        }
    }

    private async Task Heal(Biotic Target, Actor UsingActor, float maxDistance, CancellationToken token)
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
            await HealInstance(Target, UsingActor, injury, Mathf.Pow(injury.SeverityCost, 0.5f), maxDistance, token);
            if (token.IsCancellationRequested || Vector3.Distance(UsingActor.transform.position, Target.transform.position) > maxDistance * 1.2f)
            {
                return;
            }
        }
    }

    private async Task HealInstance(Biotic Target, Actor UsingActor, Medical.Injury injury, float time, float maxDistance, CancellationToken token)
    {
        ProgressBar Bar = UIController.InstantiateProgressBar(Target.gameObject, 0, time, Color.grey, Color.green);
        float CurrentTime = 0;
        while (CurrentTime < time)
        {
            if (token.IsCancellationRequested || Vector3.Distance(UsingActor.transform.position, Target.transform.position) > maxDistance * 1.2f)
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
        UsingActor.EntityComponents.Stats.Skills[StatsAndSkills.SkillsEnum.Medical].AddXP(injury.SeverityCost * 100);
        injury.Tend(Quality);
        Bar.Destroy();
    }
}
