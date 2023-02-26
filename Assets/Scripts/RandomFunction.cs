using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomFunction
{
    private static string vars = "";

    //Compiles
    public static string Create(int max, List<FunctionInput> f)
    {
        //Set vars
        vars = "t";
        for (int i = 0; i < f.Count; i++) vars += f[i].name;


        string output = "";

        for (int i = 0; i < Random.Range(1, max); i++)
        {
            if (i != 0)
            {
                if (Random.Range(0, 2) == 0) output += "-";
                else output += "+";
            }

            output += multiplier();
        }


        //Additional addition at end
        for (int i = 0; i < Random.Range(0, 5); i++)
        {
            if (Random.Range(0, 2) == 0) output += "-";
            else output += "+";

            output += vars[Random.Range(0, vars.Length)];
        }

        return output;
    }


    //Returns random (-x * y) type function
    private static string multiplier()
    {
        string output = "(";
        

        for (int i = 0; i < Random.Range(2, 4); i++)
        {
            if (i != 2 || Random.Range(0, 12) == 0)
            {
                if (i != 0) output += "*";
                output += randVar();
            }

        }

        return output + ")";
    }

    //Selects random var, -/+
    private static string randVar(bool trig = true)
    {
        string output = "";
        if (Random.Range(0, 3) == 0) output += "-";

        //pick var letter
        string v = char.ToString(vars[Random.Range(0, vars.Length)]);

        if (trig && Random.Range(0, 12) == 0)
        {
            if (Random.Range(0, 3) == 0) v = "-" + v;

            int trigFunc = Random.Range(0, 3);
            if (trigFunc == 0) return output + "Cos(" + v + ")";
            if (trigFunc == 1) return output + "Sin(" + v + ")";
            if (trigFunc == 2) return output + "Tan(" + v + ")";
        }

        return output + v;
    }

}