using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceGroup {

    public double nRolls = 0;
    public List<int> diceIDs = new List<int>();

    public DiceGroup()
    {

    }

    public DiceGroup(double nRolls, List<int> diceIDs)
    {
        this.nRolls = nRolls;
        this.diceIDs = diceIDs;
    }

}
