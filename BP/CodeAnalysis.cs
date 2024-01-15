using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;


class CodeAnalysis
{

    private List<String> methodsName = new List<String>();
    private List<String> methodsReturn = new List<String>();
    private List<String> arguments = new List<String>();
    private string codeStr = "";
    public CodeAnalysis(string path)
    {
        ExtractMethodInfo(path);
    }


    private void ExtractMethodInfo(string path)
    {

        string code = File.ReadAllText(path);
        codeStr = code;
        string patternFunction = @"(?<returnType>\w+)\s+(?<functionName>\w+)\s*\((?<parameters>.*?)\)";
        string patternVariable = @"(?<variableType>\w+)\s+(?<variableName>\w+)\s*=";

        MatchCollection functionMatches = Regex.Matches(code, patternFunction);
        MatchCollection variableMatches = Regex.Matches(code, patternVariable);

        foreach (Match functionMatch in functionMatches)
        {
            string returnType = functionMatch.Groups["returnType"].Value;
            string functionName = functionMatch.Groups["functionName"].Value;
            string parameters = functionMatch.Groups["parameters"].Value;

            methodsName.Add(functionName);
            methodsReturn.Add(returnType);
            foreach (string parameter in parameters.Split(", "))
            {
                arguments.Add(parameter);
            }
        }

        foreach (Match variableMatch in variableMatches)
        {
            string variableType = variableMatch.Groups["variableType"].Value;
            string variableName = variableMatch.Groups["variableName"].Value;

        }
    }

    public List<String> MethodName { get { return methodsName; } }
    public List<String> ReturnType { get { return methodsReturn; } }
    public List<String> Arguments { get { return arguments; } }
    public  string Code
    {
        get { return codeStr; }
    }


}

