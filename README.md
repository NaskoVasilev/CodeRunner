# Code Runenr
Console Applications that compile and execute c#, java and c++ code. For c# use Roslyn library and for c++ and java use compilers.

C# code compiling
  1. Receive c# code as plain text
  
  2. Compile it using Roslyn (install Microsoft.CodeAnalysis.CSharp nuget package)
      - Create Syntax tree
      - Include needed libraries
  
  3. Create dynamically dll file that represents console application
  
  4. Create new Process that run the generated dll file in previsous step 
      - The process open cmd.exe and run the folowing command: dotnet full path to the generated dll file in previous step
  
  5. Give the needed arguments to the running pocess and then read the result of the process(application's output)
  
  Java and C++ code compiling
  1. Run precess to compile the code using some compilers and then execute compiled codde.
