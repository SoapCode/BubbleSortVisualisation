using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

public class CompilerExample : MonoBehaviour
{
    InputField _inputWindow;
    Button _cmplButton;
    string _wrapper;
    private Assembly _generatedAssembly;                    // Compiled code is called an "Assembly"
    private Type _myScriptType = null;                    // These two variables are used run the compiled code.
    private object _myScriptInstance = null;

    public void Awake()
    {
        _inputWindow = GetComponentInChildren<InputField>();
        _cmplButton = GetComponentInChildren<Button>();
        _wrapper =
            @"
                using System;
                using System.Collections.Generic;
                using UnityEngine;    

                public class CompilationUnit
                {                    

                    public void Compile()
                    {
            ";
    }

    public void OnCompileButton()
    {
        if (_inputWindow.text == "")
            return;

        _wrapper += @_inputWindow.text;
        _wrapper += @"}}";
        Debug.Log(_wrapper);
        
        try
        {
            // ********** Create an instance of the C# compiler   
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            // ********** add compiler parameters
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.CompilerOptions = "/target:library /optimize /warn:0";
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");
            compilerParams.ReferencedAssemblies.Add(@"C:/Program Files/Unity/Editor/Data/Managed/UnityEngine.dll");

            // ********** actually compile the code  ??????? THIS LINE WORKS IN UNITY EDITOR --- BUT NOT IN BUILD ??????????
            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, _wrapper);

            // ********** Do we have any compiler errors
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                     Debug.Log(error.ToString() + '\n' + error.Line);
            }
            else
            {
                // ********** get a hold of the actual assembly that was generated
                _generatedAssembly = results.CompiledAssembly;

                if (_generatedAssembly != null)
                {
                    // get type of class Calculator from just loaded assembly
                    _myScriptType = _generatedAssembly.GetType("CompilationUnit");

                    // create instance of class MyScript
                    _myScriptInstance = Activator.CreateInstance(_myScriptType);

                    _myScriptType.InvokeMember("Compile", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, _myScriptInstance, null);

                }
            }
        }
        catch (Exception o)
        {
            Debug.Log(o.Message + "\n" + o.Source + "\n" + o.StackTrace + "\n");
        }
    }

}

//-------------------------------------------------------------------------------------

/*
public class CompilerExample : MonoBehaviour
{

    //4117 1742 8314 5436

    void Update()
    {
        // Dont run our script until it has been COMPILED successfully.
        if (myScript_Type == null || myScript_Instance == null) return;

        // Run the script's Update() function
        myScript_Type.InvokeMember("Update", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, myScript_Instance, null);

        // Read the scripts "readme" variable.
        PropertyInfo readmePropertyInfo = myScript_Type.GetProperty("readme");        // Source: http://www.csharp-examples.net/reflection-examples/
        readme = (string)readmePropertyInfo.GetValue(myScript_Instance, null);
    }

    private string scriptText =
@"using System;
 using System.Collections.Generic;
 
 public class MyScript
 {
     public string readme {get;set;}  // Our unity program will read this variable.
     private int counter = 0;    
 
     public MyScript() // Constructor
     {
         readme = """";
     }
 
     public void Update()  // Our unity game will run this function.
     {
         counter++;        
         readme = ""Hi! Script is Running! "" + counter + ""\n"";
     }
 
 
 }
 ";

    private string readme = "";            // Displayed on the GUI
    private string compilerErrorMessages = "";        // Displayed on the GUI

    void OnGUI()
    {
        // ********** Display the script in Unity
        scriptText = GUI.TextArea(new Rect(10, 10, 700, 500), scriptText);

        // ********** Compile your script
        if (GUI.Button(new Rect(720, 10, 300, 30), "Compile and Run"))
        {
            Compile();      // Compile the script. Write errors to "compilerErrorMessages", if any.
        }

        // ********** Display any compiler errors
        GUI.TextArea(new Rect(10, 510, 700, 50), compilerErrorMessages);  // The console for displaying errors.

        // ********** Display the Script's Output.
        GUI.TextArea(new Rect(720, 50, 300, 30), readme);
    }

    private Assembly generatedAssembly;                    // Compiled code is called an "Assembly"
    private Type myScript_Type = null;                    // These two variables are used run the compiled code.
    private object myScript_Instance = null;            // These two variables are used run the compiled code.

    private void Compile()
    {
        try
        {
            compilerErrorMessages = "";  // clear any previous messages

            // ********** Create an instance of the C# compiler   
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            // ********** add compiler parameters
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.CompilerOptions = "/target:library /optimize /warn:0";
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = true;
            compilerParams.IncludeDebugInformation = false;
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Core.dll");

            // ********** actually compile the code  ??????? THIS LINE WORKS IN UNITY EDITOR --- BUT NOT IN BUILD ??????????
            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParams, scriptText);

            // ********** Do we have any compiler errors
            if (results.Errors.Count > 0)
            {
                foreach (CompilerError error in results.Errors)
                    compilerErrorMessages = compilerErrorMessages + error.ErrorText + '\n';
            }
            else
            {
                // ********** get a hold of the actual assembly that was generated
                generatedAssembly = results.CompiledAssembly;

                if (generatedAssembly != null)
                {
                    // get type of class Calculator from just loaded assembly
                    myScript_Type = generatedAssembly.GetType("MyScript");

                    // create instance of class MyScript
                    myScript_Instance = Activator.CreateInstance(myScript_Type);

                    // Say success!
                    compilerErrorMessages = "Success!";
                }
            }
        }
        catch (Exception o)
        {
            compilerErrorMessages = "" + o.Message + "\n" + o.Source + "\n" + o.StackTrace + "\n";
        }

    }

}
*/