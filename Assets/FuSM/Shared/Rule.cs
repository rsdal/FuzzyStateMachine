using UnityEngine;
using System.Collections;

public class Rule  {

    public float Weight { get; set; }
    public string Name { get; set; }
	
    public Rule(string name, float weight)
    {
        this.Weight = weight;
        this.Name = name;
    }
}
