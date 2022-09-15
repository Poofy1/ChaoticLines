using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomFunction
{
    private static string vars = "xyzt";

    //Compiles
    public static string Create(int max)
    {
        string output = "";

        for (int i = 0; i < Random.Range(2, max); i++)
        {
            if (i != 0)
            {
                if (Random.Range(0, 2) == 0) output += " - ";
                else output += " + ";
            }

            output += multiplier();
        }

        for (int i = 0; i < Random.Range(0, 4); i++)
        {
            if (Random.Range(0, 3) == 0) output += " - ";
            else output += " + ";

            output += vars[Random.Range(0, 4)];
        }

        return output;
    }


    //Returns random (-x * y) type function
    private static string multiplier()
    {
        string output = "(";
        

        for (int i = 0; i < Random.Range(2, 4); i++)
        {
            if (i != 2 || Random.Range(0, 8) == 0)
            {
                if (i != 0) output += " * ";
                output += randVar();
            }

        }

        return output + ")";
    }

    //Selects random var, -/+
    private static string randVar()
    {
        string output = "";
        if (Random.Range(0, 3) == 0) output += "-";
        return output + vars[Random.Range(0,4)];
    }

}
