using Terraria;

using Microsoft.Xna.Framework;

namespace OTAPI_Chinese_Change;

partial class ChangeInfo
{
	static void Add_OTAPI()
	{
		var convertInfo = new AssemblyNameConversionInfo("OTAPI", "OTAPI-Chinese");
		Add_OTAPI_Terraria(convertInfo);
		Add_OTAPI_Microsoft_XNA_Framework(convertInfo);
		AssemblyNameConverts.Add(convertInfo);
	}
	static void Add_OTAPI_Terraria(AssemblyNameConversionInfo convertInfo)
	{
		var types = new List<TypeConversionInfo>()
		{
			new(nameof(Item), "物品",
			fields: new()
			{
				new(nameof(Item.type), "类型"),
				new(nameof(Item.stack), "数量"),
				new(nameof(Item.prefix), "前缀")
			},
			methods: new()
			{
				new(nameof(Item.NewItem), "新物品")
			}),

			new(nameof(Projectile), "射弹",
			fields: new()
            {
                new(nameof(Projectile.type), "类型"),
			},
			methods: new()
			{
				new(nameof(Projectile.NewProjectile), "新射弹")
			}),

			new(nameof(NetMessage), "网络消息",
			methods: new()
			{
				new(nameof(NetMessage.SendData), "发送数据")
			}),

			new(nameof(WorldGen), "世界信息")
		};
		convertInfo.Namespaces.Add(new NameSpaceConversionInfo("Terraria", types));
	}
	static void Add_OTAPI_Microsoft_XNA_Framework(AssemblyNameConversionInfo convertInfo)
    {
		var types = new List<TypeConversionInfo>()
		{
			new(nameof(Color), "颜色",
			properties: new()
			{
				new(nameof(Color.Red), "红"),
				new(nameof(Color.Yellow), "黄"),
				new(nameof(Color.Blue), "蓝"),
				new(nameof(Color.Green), "绿"),
			}),
		};
        convertInfo.Namespaces.Add(new NameSpaceConversionInfo("Microsoft.Xna.Framework", types));
    }
}
