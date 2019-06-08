# CSharpCodeCompiler
Console Application that receive c# code as plain text then compile it and generate dll file using Roslyn library

  1. Receive c# code as plain text
  
  2. Compile it using Roslyn (install Microsoft.CodeAnalysis.CSharp nuget package)
      - Create Syntax tree
      - Include needed libraries
  
  3. Create dynamically dll file that represents console application
  
  4. Create new Process that run the generated dll file in previsous step 
      - The process open cmd.exe and run the folowing command: dotnet full path to the generated dll file in previous step
  
  5. Give the needed arguments to the running pocess and then read the result of the process(application's output)
