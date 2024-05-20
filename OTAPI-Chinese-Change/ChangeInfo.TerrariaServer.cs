using TerrariaApi.Server;

namespace OTAPI_Chinese_Change;

internal partial class ChangeInfo
{
    static void Add_TerrariaServer()
    {
        var convertInfo = new AssemblyNameConversionInfo("TerrariaServer", "TerrariaServer-Chinese");
        Add_TerrariaServer_TerrariaApi_Server(convertInfo);
        AssemblyNameConverts.Add(convertInfo);
    }
    static void Add_TerrariaServer_TerrariaApi_Server(AssemblyNameConversionInfo convertInfo)
    {
        var types = new List<TypeConversionInfo>()
        {
            new(nameof(TerrariaPlugin), "泰拉插件",
            methods: new()
            {
                new(nameof(TerrariaPlugin.Initialize), "初始化")
            },
            properties: new()
            {
                new(nameof(TerrariaPlugin.Name), "名称"),
                new(nameof(TerrariaPlugin.Author), "作者"),
                new(nameof(TerrariaPlugin.Version), "版本"),
                new(nameof(TerrariaPlugin.Description), "描述"),
            })
        };
        convertInfo.Namespaces.Add(new NameSpaceConversionInfo("TerrariaApi.Server", types));
    }
}
