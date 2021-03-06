using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public gAction action;

    public Node(Node parent, float cost, Dictionary<string, int> allstates, gAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, int>(allstates);
        this.action = action;
    }
}

public class gPlanner 
{
    public Queue<gAction> Plan(List<gAction> actions, Dictionary<string, int> goal, worldStates states)
    {
        List<gAction> usableActions = new List<gAction>();
        foreach(gAction a in actions)
        {
            if (a.IsAchievable())
                usableActions.Add(a);
        }
        List<Node> leaves = new List<Node>();
        Node root = new Node(null, 0, gWorld.Instance.GetWorld().GetStates(), null);
        bool success = BuildGraph(root, leaves, usableActions, goal);
        if(!success)
        {
            Debug.Log("NO PLAN");
            return null;
        }

        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if(cheapest == null)
            {
                cheapest = leaf;
            } else
            {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }
        }
        List<gAction> result = new List<gAction>();
        Node n = cheapest;
        while(n != null)
        {
            if(n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }
        Queue<gAction> queue = new Queue<gAction>();
        foreach(gAction a in result)
        {
            queue.Enqueue(a);
        }
        Debug.Log("The Plan is: ");
        foreach(gAction a in queue)
        {
            Debug.Log("Q: " + a.actionName);
        }
        return queue;

    }
    private bool BuildGraph(Node parent, List<Node> leaves, List<gAction> usableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;
        foreach(gAction action in usableActions)
        {
            if(action.IsAchievableGiven(parent.state))
            {
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                foreach(KeyValuePair<string, int> eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                        currentState.Add(eff.Key, eff.Value);
                }
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                if(GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<gAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundPath = true;

                }
            }
        }
        return foundPath;
    }
    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach(KeyValuePair<string, int> g in goal)
        {
            if (!state.ContainsKey(g.Key))
                return false;
        }
        return true;
    }

    private List<gAction> ActionSubset(List<gAction> actions, gAction removeMe)
    {
        List<gAction> subset = new List<gAction>();
        foreach(gAction a in actions)
        {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }

}
