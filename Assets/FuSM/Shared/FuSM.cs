using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Fuzzy State Machine
/// </summary>
public class FuSM  {

    private List<FuSMState> m_states;

    //Min atribbution
    private float b=0.1f;

    public FuSM()
    {
        m_states = new List<FuSMState>();
    }

    public void AddState(FuSMState state)
    {
        m_states.Add(state);
    }

    public void AddTransition(string from, string to, float weight)
    {
        FuSMState f = GetState(from);

        if(f != null)
        {
            FuSMState t = GetState(to);
            if(t != null)
            {
                f.Transitions.Add(new Transition(f, t, new Rule(f.Name + t.Name, weight)));
            }
        }
    }
    
    /// <summary>
    /// Update transition weight
    /// </summary>
    /// <param name="name">Transition name</param>
    /// <param name="weight">Weight to change</param>
    public void UpdateTransition(string name, float weight)
    {
        foreach (FuSMState item in m_states)
        {
            Transition t = item.GetTransition(name);
            if (t != null)
                t.Rule.Weight = weight;
        }
    }

    /// <summary>
    /// Update every transition weight
    /// </summary>
    /// <param name="transitions"></param>
    public void UpdateAllTransitions(params Rule[] transitions)
    {
        foreach (Rule item in transitions)
        {
            UpdateTransition(item.Name, item.Weight);
        }

        NormalizeTransitions();

        UpdateStates();

        NormalizeState();
    }

    private void UpdateStates()
    {
        foreach (FuSMState state in m_states)
        {  
            float weight = 0;
            foreach (FuSMState item in m_states)
            {
                if(item != state)
                {
                    Transition t = item.GetTransition(state);
                    if (t != null)
                        weight += t.Rule.Weight *  t.From.Weight;                    
                }
            }

            if (weight > 1)
                state.Weight = 1;
            else if (weight <= 1 && weight > 0)
                state.Weight = weight;
            else
                state.Weight = 0.1f;
        }
    }

    private void NormalizeState()
    {
        float max = 0;
        foreach (FuSMState item in m_states)
        {
            if (max == 0 || max < item.Weight)
                max = item.Weight;
        }

        max = 1/max;

        m_states.ForEach(x => x.Weight *= max);

        float med = m_states.Sum(x => x.Weight) / m_states.Count;

        foreach (FuSMState item in m_states)
        {
            if (item.Weight < med)
                item.Value = 0;
            else
                item.Value = item.Weight;
        }
    }

    /// <summary>
    /// Normalize transitions
    /// </summary>
    private void NormalizeTransitions()
    {
        float max = 0;
        foreach (FuSMState item in m_states)
        {
            foreach (Transition t in item.Transitions)
            {
                if (max == 0 || max < t.Rule.Weight)
                    max = item.Weight;
            }
        }

        m_states.ForEach(x => x.Transitions.ForEach(y => y.Rule.Weight *= max));
    }

    /// <summary>
    /// Get a state
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public FuSMState GetState(string name)
    {
        return m_states.FirstOrDefault(x => x.Name == name);
    }

    public void RunStates()
    {
        foreach (FuSMState state in m_states)
        {
            state.UpdateState();
        }
    }

}
