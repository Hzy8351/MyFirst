using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAction
{
    public string id;
    public string content;
}

public enum AIEnum
{
    none = 0,

    rest = 101,         //在休息范围活动
    back = 102,         //回到休息范围
    escape = 103,       //逃跑
    chase = 104,        //追赶

    attack = 400,
    attack1 = 401,

    skill1 = 501,
}

public enum CommEnum
{
    none = 0,

    standby = 101,
    run = 102,

    attack = 400,
    skill1 = 501,
}

public class AICommand
{
    public CommEnum comState;
    public bool bComplete;

    public float tarTick;
    public Vector3 tarPos;
}

public class AIManager
{
    public AIEnum curState = AIEnum.none;
    public List<AICommand> comms = new List<AICommand>();
    public Queue<AIAction> actions = new Queue<AIAction>();
}
