using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharaterStates
{
    none = 0,
    standby = 101,
    run = 102,
    run_up = 103,

    hit = 201, 
    stun = 211,
    win = 221,
    die = 231,

    prepare = 301,
    restore = 311,

    attack = 401,

    skill1 = 501,
}

public class CharaterManager : MonoBehaviour
{
    private HeroBehaviour hb;
    public HeroBehaviour HB { get { return hb; } set { hb = value; } }

    private Dictionary<CharaterStates, string> dicCharState = new Dictionary<CharaterStates, string>();

    public void inits()
    {
        dicCharState[CharaterStates.standby] = "standby";
        dicCharState[CharaterStates.run] = "run";
        dicCharState[CharaterStates.run_up] = "run_up";
        dicCharState[CharaterStates.hit] = "hit";
        dicCharState[CharaterStates.stun] = "stun";
        dicCharState[CharaterStates.win] = "win";
        dicCharState[CharaterStates.die] = "die";
        dicCharState[CharaterStates.prepare] = "prepare";
        dicCharState[CharaterStates.restore] = "restore";
        dicCharState[CharaterStates.attack] = "attack";
        dicCharState[CharaterStates.skill1] = "skill1";
    }

    public string getAniState(CharaterStates s)
    {
        return (dicCharState.ContainsKey(s)) ? dicCharState[s] : "";
    }

}
