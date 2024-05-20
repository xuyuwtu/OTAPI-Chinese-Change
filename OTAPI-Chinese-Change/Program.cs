using Mono.Cecil;

namespace OTAPI_Chinese_Change;


internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            var fileNames = new string[] { "OTAPI.dll", "OTAPI.Runtime.dll", "TerrariaServer.dll", "TShockAPI.dll" };
            foreach(string fileName in fileNames)
            {
                if (File.Exists(fileName))
                {
                    SetToTarget(fileName);
                }
            }
        }
        else
        {
            foreach (var path in args)
            {
                if (File.Exists(path))
                {
                    using var assDef = AssemblyDefinition.ReadAssembly(path);
                    ChangeInfo.SetReferenceToSource(assDef);
                    assDef.Write(Path.GetFileNameWithoutExtension(path) + ".change.dll");
                }
            }
        }
    }
    static void SetToTarget(string path)
    {
        using var assDef = AssemblyDefinition.ReadAssembly(path);
        ChangeInfo.SetToTarget(assDef);
        assDef.Write(assDef.MainModule.Assembly.Name.Name + ".dll");
    }
}