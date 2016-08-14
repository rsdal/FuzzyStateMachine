using UnityEngine;
using System.Collections;
using AForge.Fuzzy;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class FuSMController : MonoBehaviour {

    InferenceSystem IS;
    FuSM fusm;
    List<Rule> rules;
    StaterPattern m_pattern;

    public Text[] labels;

	void Start () {

        m_pattern = FindObjectOfType<StaterPattern>();

        //Distancia
        FuzzySet fsencostado = new FuzzySet("ENCOSTADO", new TrapezoidalFunction(
             0f, 10f, TrapezoidalFunction.EdgeType.Right));

        FuzzySet fsmedio = new FuzzySet("MEDIO", new TrapezoidalFunction(
               5f, 7.5f, 12.5f, 15f));

        FuzzySet fslonge = new FuzzySet("LONGE", new TrapezoidalFunction(
               10f, 20f, TrapezoidalFunction.EdgeType.Left));

        LinguisticVariable Distance = new LinguisticVariable("Distance", 0f, 20f);
        Distance.AddLabel(fsencostado);
        Distance.AddLabel(fsmedio);
        Distance.AddLabel(fslonge);

        //States
        FuzzySet fsvalue = new FuzzySet("VALUE", new TrapezoidalFunction(0, 1, TrapezoidalFunction.EdgeType.Left));

        LinguisticVariable patrolchase = new LinguisticVariable("PatrolChase", 0f, 1f);
        LinguisticVariable patrolalert = new LinguisticVariable("PatrolAlert", 0f, 1f);
        LinguisticVariable alertchase = new LinguisticVariable("AlertChase", 0f, 1f);
        LinguisticVariable alertpatrol = new LinguisticVariable("AlertPatrol", 0f, 1f);
        LinguisticVariable chasepatrol = new LinguisticVariable("ChasePatrol", 0f, 1f);
        LinguisticVariable chasealert = new LinguisticVariable("ChaseAlert", 0f, 1f);
        patrolchase.AddLabel(fsvalue);
        patrolalert.AddLabel(fsvalue);
        alertchase.AddLabel(fsvalue);
        alertpatrol.AddLabel(fsvalue);
        chasepatrol.AddLabel(fsvalue);
        chasealert.AddLabel(fsvalue);

        //DB
        Database fuzzyDB = new Database();
        fuzzyDB.AddVariable(Distance);
        fuzzyDB.AddVariable(patrolchase);
        fuzzyDB.AddVariable(patrolalert);
        fuzzyDB.AddVariable(alertchase);
        fuzzyDB.AddVariable(alertpatrol);
        fuzzyDB.AddVariable(chasepatrol);
        fuzzyDB.AddVariable(chasealert);

        //Rules
        IS = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));

        IS.NewRule("0", "IF Distance IS ENCOSTADO THEN PatrolChase IS VALUE");
        IS.NewRule("1", "IF Distance IS ENCOSTADO THEN AlertChase IS VALUE");

        IS.NewRule("2", "IF Distance IS MEDIO THEN PatrolAlert IS VALUE");
        IS.NewRule("3", "IF Distance IS MEDIO THEN ChaseAlert IS VALUE");

        IS.NewRule("4", "IF Distance IS LONGE THEN AlertPatrol IS VALUE");
        IS.NewRule("5", "IF Distance IS LONGE THEN ChasePatrol IS VALUE");
        

        rules = new List<Rule>();
        rules.Add(new Rule("PatrolChase",0.16f));
        rules.Add(new Rule("PatrolAlert",0.16f));
        rules.Add(new Rule("ChasePatrol", 0.16f));
        rules.Add(new Rule("ChaseAlert", 0.16f));
        rules.Add(new Rule("AlertPatrol", 0.16f));
        rules.Add(new Rule("AlertChase", 0.16f));
        

        AddStates();
	}

    
    private void AddStates()
    {
        fusm = new FuSM();

        fusm.AddState(new PatrolState() { Name = "Patrol", Weight = 0.33f });
        fusm.AddState(new FuSMState() { Name = "Chase", Weight = 0.33f });
        fusm.AddState(new FuSMState() { Name = "Alert", Weight = 0.33f });

        fusm.AddTransition("Patrol", "Chase", 0.16f);
        fusm.AddTransition("Patrol", "Alert", 0.16f);
        fusm.AddTransition("Chase", "Patrol", 0.16f);
        fusm.AddTransition("Chase", "Alert", 0.16f);
        fusm.AddTransition("Alert", "Patrol", 0.16f);
        fusm.AddTransition("Alert", "Chase",  0.16f);
    }
	
    void FixedUpdate()
    {
        if (fusm != null)
        {
            IS.SetInput("Distance", m_pattern.Distance);

            foreach (Rule item in rules)
                SetCorrectInference(item.Name);

            fusm.UpdateAllTransitions(rules.ToArray());
            fusm.RunStates();

            float alert = fusm.GetState("Alert").Value;
            float patrol = fusm.GetState("Patrol").Value;
            float chase = fusm.GetState("Chase").Value;


            if(alert > 0)
            m_pattern.UpdateAlert(alert);

            if(patrol > 0)
            m_pattern.UpdatePatrol(patrol);

            if(chase > 0)
            m_pattern.UpdateChase(chase);

            labels[0].text = "Patrol: " + patrol.ToString("0.0");
            labels[1].text = "Alert: " + alert;
            labels[2].text = "Chase: " + chase;
        }
    }

    private void SetCorrectInference(string name)
    {
        try
        {
            FuzzyOutput fuzzy = IS.ExecuteInference(name);

            float value = 0;

            if (fuzzy.OutputList.Count > 0)
                value = IS.Evaluate(name);

            ShowLabelInference(name, value);

            rules.First(x => x.Name.Equals(name)).Weight = value;
        }
        catch (System.Exception e)
        {
            print("Error:" + e.Message);
        }
    }

    private void ShowLabelInference(string name, float value)
    {
        switch (name)
        {
            case "PatrolChase":
                labels[3].text = name + ": " + value;
                break;
            case "PatrolAlert":
                labels[4].text = name + ": " + value;
                break;
            case "AlertChase":
                labels[5].text = name + ": " + value;
                break;
            case "AlertPatrol":
                labels[6].text = name + ": " + value;
                break;
            case "ChasePatrol":
                labels[7].text = name + ": " + value;
                break;
            case "ChaseAlert":
                labels[8].text = name + ": " + value;
                break;
        }
    }

}
