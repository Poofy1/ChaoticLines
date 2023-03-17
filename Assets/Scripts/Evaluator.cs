using UnityEngine;
using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;

public static class Evaluator
{
    public static Func<double[], double> CreateMethod(string expression)
    {
        // Define the regular expression pattern for variable names
        string pattern = @"\b[a-zA-Z]\b";

        // Extract variable names from the expression string
        MatchCollection matches = Regex.Matches(expression, pattern);
        string[] variableNames = matches.Cast<Match>().Select(m => m.Value).Distinct().ToArray();

        // Then, sort the remaining variable names alphabetically
        variableNames = variableNames.Where(v => !string.IsNullOrEmpty(v))
            .OrderBy(v => v)
            .ToArray();

        // Insert the fixed variables at the beginning
        variableNames = new string[] { "t", "x", "y", "z" }.Concat(variableNames).ToArray();

        /*
        for (int b = 0; b < variableNames.Length; b++)
        {
            Debug.Log(variableNames[b]);
        }
        */

        // Generate C# code for a method that evaluates the expression
        string methodCode = GenerateMethodCode(expression, variableNames);


        // Compile the method and get the resulting delegate
        return CompileMethod(methodCode, variableNames.Length);
    }




    private static string GenerateMethodCode(string expression, string[] variableNames)
    {

        // Create a regex pattern to match variable names
        string pattern = @"\b[a-zA-Z]\b";

        // Replace each variable with v[index]
        string result = Regex.Replace(expression, pattern, match =>
        {
            string name = match.Value;
            int i = Array.IndexOf(variableNames, name);
            return "v[" + i + "]";
        });

        // Filter out unwanted characters
        result = result.Replace("\"", string.Empty);
        result = result.Replace("\\", string.Empty);

        // Define the code template for the method
        string code =
            "using System;\n" +
            "using static System.Math;" + 
            "class Evaluator {\n" +
            "    public static double Evaluate(double[] v) {\n" +
            "        double result = " + result + ";\n" +
            "        return result;\n" +
            "    }\n" +
            "}";

        return code;
    }

    private static Func<double[], double> CompileMethod(string methodCode, int numVariables)
    {
        // Use Roslyn to compile the code
        var syntaxTree = CSharpSyntaxTree.ParseText(methodCode);
        var references = new MetadataReference[]
        {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(System.Diagnostics.Debug).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(System.Linq.Expressions.Expression).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create("Evaluator")
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTree);

        using (var memoryStream = new MemoryStream())
        {
            var result = compilation.Emit(memoryStream);
            if (!result.Success)
            {
                string errors = "";
                foreach (var error in result.Diagnostics)
                {
                    if (error.Severity == DiagnosticSeverity.Error)
                    {
                        errors += error.GetMessage() + "\n";
                    }
                }
                throw new Exception(errors);
            }
            else
            {
                var assembly = Assembly.Load(memoryStream.ToArray());
                var type = assembly.GetType("Evaluator");
                var method = type.GetMethod("Evaluate");
                return (Func<double[], double>)Delegate.CreateDelegate(typeof(Func<double[], double>), method);
            }
        }
    }
}