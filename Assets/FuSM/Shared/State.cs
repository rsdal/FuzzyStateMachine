using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class State {

    public string Name { get; set;}
    public List<Transition> Transitions { get; set; }
    public float Weight { get; set; }
    public float Value { get; set; }

    public State()
    {
        Transitions = new List<Transition>();
    }

    public Transition GetTransition(string Name)
    {
        return Transitions.FirstOrDefault(x=>x.Rule.Name.Equals(Name));
    }

    public Transition GetTransition(State to)
    {
        return Transitions.FirstOrDefault(x => x.To == to);
    }
}
