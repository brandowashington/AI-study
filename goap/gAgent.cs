using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SubGoal
{
    public Dictionary<string, int> sGoals;
    public bool remove;

    public SubGoal(string s, int i, bool r)
    {
        sGoals = new Dictionary<string, int>();
        sGoals.Add(s, i);
        remove = r;
    }
}
public class gAgent : MonoBehaviour {
    public List<gAction> actions = new List<gAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    gPlanner planner;
    Queue<gAction> actionQueue;
    public gAction currentAction;
    SubGoal currentGoal;

    public void Start() {
        gAction[] acts = this.GetComponents<gAction>();
        foreach(gAction a in acts)
        actions.Add(a);
    }

    bool invoked = false;
    void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;
    }
    void LateUpdate() {
        
        if(currentAction != null && currentAction.running)
        {
            if(currentAction.agent.hasPath && currentAction.agent.remainingDistance < 1f)
            {
                if(!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        if(planner == null || actionQueue == null)
        {
            planner = new gPlanner();
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach(KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                actionQueue = planner.Plan(actions, sg.Key.sGoals, null);
                if(actionQueue != null)
                {
                    currentGoal = sg.Key;
                    break;
                }    
            }
        }
        if(actionQueue != null && actionQueue.Count == 0)
        {
            if(currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }
            planner = null;
        }

        if(actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if(currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                if(currentAction.target != null)
                {
                    currentAction.running = true;
                    currentAction.agent.SetDestination(currentAction.target.transform.position);
                }
                else
                {
                    actionQueue = null;
                }
            }
        }
    }
}