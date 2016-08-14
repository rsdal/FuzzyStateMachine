using UnityEngine;
using System.Collections;

public class Transition {

    public FuSMState From { get; set; }
    public FuSMState To { get; set; }
    public Rule Rule { get; set; }

    public Transition(FuSMState from, FuSMState to, Rule rule)
    {
        this.From = from;
        this.To = to;
        this.Rule = rule;
    }

}
