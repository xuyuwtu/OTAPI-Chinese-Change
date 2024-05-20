using TShockAPI;

namespace OTAPI_Chinese_Change;

internal partial class ChangeInfo
{
    static void Add_TShockAPI()
    {
        var convertInfo = new AssemblyNameConversionInfo("TShockAPI", "TShockAPI-Chinese");
        Add_TShockAPI_TShockAPI(convertInfo);
        AssemblyNameConverts.Add(convertInfo);
    }
    static void Add_TShockAPI_TShockAPI(AssemblyNameConversionInfo convertInfo)
    {
        var types = new List<TypeConversionInfo>()
        {
            new(nameof(Command), "命令"),

            new(nameof(Commands), "命令列表",
            fields: new()
            {
                new(nameof(Commands.ChatCommands), "聊天命令")
            }),

            new(nameof(CommandArgs), "命令参数",
            properties: new()
            {
                new(nameof(CommandArgs.Player), "玩家"),
                new(nameof(CommandArgs.TPlayer), "泰拉玩家")
            }),

            new(nameof(TSPlayer), null,
            methods: new()
            {
                new(nameof(TSPlayer.SendMessage), "发送消息"),
                new(nameof(TSPlayer.SendInfoMessage), "发送提示消息"),
                new(nameof(TSPlayer.SendErrorMessage), "发送错误消息"),
                new(nameof(TSPlayer.SendSuccessMessage), "发送成功消息")
            })
        };
        convertInfo.Namespaces.Add(new NameSpaceConversionInfo("TShockAPI", types));
    }
}
