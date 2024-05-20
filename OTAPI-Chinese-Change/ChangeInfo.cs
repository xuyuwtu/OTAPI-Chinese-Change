using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

namespace OTAPI_Chinese_Change;

internal static partial class ChangeInfo
{
    private static List<AssemblyNameConversionInfo> AssemblyNameConverts = new();

    static ChangeInfo()
    {
        Add_OTAPI();
        Add_TerrariaServer();
        Add_TShockAPI();
    }

    private static void SetReferenceToTarget(AssemblyDefinition assembly)
    {
        foreach (var assemblyNameRef in assembly.MainModule.AssemblyReferences)
        {
            if (AssemblyNameConverts.TryGetBySourceName(assemblyNameRef.Name, out var assemblyNameInfo))
            {
                foreach (var memberRef in assembly.MainModule.GetMemberReferences())
                {
                    switch (memberRef)
                    {
                        case FieldReference fieldRef:
                            {
                                if (assemblyNameInfo.TryGetTypeBySource(fieldRef.DeclaringType, out var typeInfo) && typeInfo.TryGetFieldBySource(fieldRef.Name, out var fieldInfo))
                                {
                                    fieldInfo.SetTargetName(fieldRef);
                                }
                            }
                            break;
                        case MethodReference methodRef:
                            {
                                if (assemblyNameInfo.TryGetTypeBySource(memberRef.DeclaringType, out var typeInfo) && typeInfo.TryGetMethodBySource(memberRef.Name, out var methodInfo))
                                {
                                    methodInfo.SetTargetName(methodRef);
                                }
                            }
                            break;
                        default:
                            throw new InvalidDataException(memberRef.GetType().FullName);
                    }
                }

                foreach (var typeRef in assembly.MainModule.GetTypeReferences())
                {
                    if (assemblyNameInfo.TryGetTypeBySource(typeRef, out var typeInfo))
                    {
                        typeInfo.SetTargetName(typeRef);
                    }
                }

                assemblyNameInfo.SetTargetName(assemblyNameRef);
                break;
            }
        }
    }
    public static void SetToTarget(AssemblyDefinition assembly)
    {
        SetReferenceToTarget(assembly);
        if(AssemblyNameConverts.TryGetBySourceName(assembly.MainModule.Assembly.Name.Name, out var convertInfo))
        {
            convertInfo.SetToTarget(assembly);
        }
    }
    public static void SetReferenceToSource(AssemblyDefinition assembly)
    {
        foreach(var typeDef in assembly.MainModule.Types)
        {
            if(typeDef.BaseType is null)
            {
                continue;
            }
            if (AssemblyNameConverts.TryGetByTargetName(typeDef.BaseType.Scope.Name, out var assemblyNameInfo))
            {
                if (assemblyNameInfo.TryGetNamespace(typeDef.BaseType.Namespace, out var namespaceInfo))
                {
                    if(namespaceInfo.TryGetTypeByTargetOrSource(typeDef.BaseType.Name, out var typeInfo))
                    {
                        foreach (var propertyDef in typeDef.Properties)
                        {
                            if(propertyDef.GetMethod is not null)
                            {
                                if(propertyDef.GetMethod.IsVirtual && typeInfo.TryGetPropertyByTarget(propertyDef.Name, out var propertyInfo))
                                {
                                    propertyInfo.SetSourceName(propertyDef);
                                }
                            }
                            else if(propertyDef.SetMethod is not null)
                            {
                                if (propertyDef.SetMethod.IsVirtual && typeInfo.TryGetPropertyByTarget(propertyDef.Name, out var propertyInfo))
                                {
                                    propertyInfo.SetSourceName(propertyDef);
                                }
                            }
                        }
                        foreach (var methodDef in typeDef.Methods)
                        {
                            if (methodDef.IsVirtual && typeInfo.TryGetMethodByTarget(methodDef.Name, out var methodInfo))
                            {
                                methodInfo.SetSourceName(methodDef);
                            }
                        }
                    }
                }
            }
        }

        foreach(var memberRef in assembly.MainModule.GetMemberReferences())
        {
            if (memberRef.DeclaringType is null)
            {
                continue;
            }
            if (AssemblyNameConverts.TryGetByTargetName(memberRef.DeclaringType.Scope.Name, out var assemblyNameInfo))
            {
                if (assemblyNameInfo.TryGetNamespace(memberRef.DeclaringType.Namespace, out var namespaceInfo))
                {
                    if (namespaceInfo.TryGetTypeByTargetOrSource(memberRef.DeclaringType.Name, out var typeInfo))
                    {
                        switch (memberRef)
                        {
                            case FieldReference fieldRef:
                                {
                                    if (typeInfo.TryGetFieldByTarget(fieldRef.Name, out var convertInfo))
                                    {
                                        convertInfo.SetSourceName(fieldRef);
                                    }
                                }
                                break;
                            case MethodReference methodRef:
                                {
                                    if (memberRef.Name.Equals(".ctor", StringComparison.Ordinal))
                                    {
                                        break;
                                    }
                                    if (typeInfo.TryGetMethodByTarget(methodRef.Name, out var convertInfo))
                                    {
                                        convertInfo.SetSourceName(methodRef);
                                    }
                                }
                                break;
                            default:
                                throw new InvalidDataException(memberRef.GetType().FullName);
                        }
                    }
                }
            }
        }

        foreach (var typeRef in assembly.MainModule.GetTypeReferences())
        {
            if (AssemblyNameConverts.TryGetByTargetName(typeRef.Scope.Name, out var assemblyNameInfo))
            {
                if (assemblyNameInfo.TryGetNamespace(typeRef.Namespace, out var namespaceInfo))
                {
                    if (namespaceInfo.TryGetTypeByTarget(typeRef.Name, out var typeInfo))
                    {
                        typeInfo.SetSourceName(typeRef);
                    }
                }
            }
        }

        foreach (var assemblyNameRef in assembly.MainModule.AssemblyReferences)
        {
            if (AssemblyNameConverts.TryGetByTargetName(assemblyNameRef.Name, out var assemblyNameInfo))
            {
                assemblyNameInfo.SetSourceName(assemblyNameRef);
            }
        }
    }
}
sealed class AssemblyNameConversionInfo : IStringConversionInfo
{
    public string SourceName { get; set; }
    public string? TargetName { get; set; }
    public List<NameSpaceConversionInfo> Namespaces;

    public AssemblyNameConversionInfo(string sourceName, string targetName)
    {
        SourceName = sourceName;
        TargetName = targetName;
        Namespaces = new();
    }

    public bool TryGetNamespace(string @namespace, [MaybeNullWhen(false)] out NameSpaceConversionInfo namespaceConvertInfo)
    {
        foreach (var ns in Namespaces)
        {
            if (@namespace.Equals(ns.Namespace, StringComparison.Ordinal))
            {
                namespaceConvertInfo = ns;
                return true;
            }
        }
        namespaceConvertInfo = null;
        return false;
    }
    public bool TryGetTypeBySource(TypeReference typeRef, [MaybeNullWhen(false)] out TypeConversionInfo typeConvertInfo)
    {
        typeConvertInfo = null;
        return TryGetNamespace(typeRef.Namespace, out var nameSpaceInfo) && nameSpaceInfo.TryGetTypeBySource(typeRef.Name, out typeConvertInfo);
    }

    public void SetToTarget(AssemblyDefinition assemblyDef)
    {
        this.SetTargetName(assemblyDef.MainModule.Assembly.Name);
        foreach (var typeDef in assemblyDef.MainModule.Types)
        {
            if (TryGetNamespace(typeDef.Namespace, out var namespaceInfo))
            {
                namespaceInfo.SetToTarget(typeDef);
            }
        }
    }
}
sealed class NameSpaceConversionInfo
{
    public string Namespace;
    public List<TypeConversionInfo> Types;

    public NameSpaceConversionInfo(string @namespace, List<TypeConversionInfo> types)
    {
        Namespace = @namespace;
        Types = types;
    }
    public NameSpaceConversionInfo(string @namespace) : this(@namespace, new()) { }

    public bool TryGetTypeBySource(string type, [MaybeNullWhen(false)] out TypeConversionInfo typeConvertInfo) => Types.TryGetBySourceName(type, out typeConvertInfo);
    public bool TryGetTypeByTarget(string type, [MaybeNullWhen(false)] out TypeConversionInfo typeConvertInfo) => Types.TryGetByTargetName(type, out typeConvertInfo);
    public bool TryGetTypeByTargetOrSource(string type, [MaybeNullWhen(false)] out TypeConversionInfo typeConvertInfo) => Types.TryGetByTargetNameOrSourceName(type, out typeConvertInfo);

    public void SetToTarget(TypeDefinition typeDef)
    {
        if (TryGetTypeBySource(typeDef.Name, out var typeInfo))
        {
            typeInfo.SetToTarget(typeDef);
        }
    }

}
sealed class TypeConversionInfo : IStringConversionInfo
{
    public string SourceName { get; set; }
    public string? TargetName { get; set; }
    public List<NameConversionInfo> Fields;
    public List<NameConversionInfo> Properties;
    public List<NameConversionInfo> Methods;

    public TypeConversionInfo(string sourceName, string? targetName, List<NameConversionInfo>? fields = null, List<NameConversionInfo>? properties = null, List<NameConversionInfo>? methods = null)
    {
        SourceName = sourceName;
        TargetName = targetName;
        Fields = fields ?? new();
        Properties = properties ?? new();
        Methods = methods ?? new();
    }
    public TypeConversionInfo(string sourceName, string? targetName) : this(sourceName, targetName, new(), new(), new()) { }
    internal TypeConversionInfo(TypeConversionInfo typeConversionInfo, List<NameConversionInfo>? fields = null, List<NameConversionInfo>? properties = null, List<NameConversionInfo>? methods = null) 
        : this(typeConversionInfo.SourceName, typeConversionInfo.TargetName, fields, properties, methods) { }

    public bool TryGetFieldBySource(string fieldName, [MaybeNullWhen(false)] out NameConversionInfo convertInfo) => Fields.TryGetBySourceName(fieldName, out convertInfo);
    public bool TryGetFieldByTarget(string fieldName, [MaybeNullWhen(false)] out NameConversionInfo convertInfo) => Fields.TryGetByTargetName(fieldName, out convertInfo);
    public bool TryGetPropertyBySource(string propertyName, [MaybeNullWhen(false)] out NameConversionInfo convertInfo) => Properties.TryGetBySourceName(propertyName, out convertInfo);
    public bool TryGetPropertyByTarget(string propertyName, [MaybeNullWhen(false)] out NameConversionInfo convertInfo) => Properties.TryGetByTargetName(propertyName, out convertInfo);
    public bool TryGetMethodBySource(string methodName, [MaybeNullWhen(false)] out NameConversionInfo convertInfo) => Methods.TryGetBySourceName(methodName, out convertInfo);
    public bool TryGetMethodByTarget(string methodName, [MaybeNullWhen(false)] out NameConversionInfo convertInfo) => Methods.TryGetByTargetName(methodName, out convertInfo);

    public void SetToTarget(TypeDefinition typeDef)
    {
        this.SetTargetName(typeDef);
        SetToTargetName(Fields, typeDef.Fields);
        SetToTargetName(Methods, typeDef.Methods);
        SetToTargetName(Properties, typeDef.Properties);
    }
    private static void SetToTargetName<T>(IList<NameConversionInfo> conversionInfos, IList<T> values) where T : MemberReference
    {
        foreach (var memberRef in values)
        {
            if (conversionInfos.TryGetBySourceName(memberRef.Name, out var info))
            {
                info.SetTargetName(memberRef);
            }
        }
    }
}
sealed class NameConversionInfo : IStringConversionInfo
{
    public string SourceName { get; set; }
    public string? TargetName { get; set; }
    public NameConversionInfo(string source, string target)
    {
        SourceName = source;
        TargetName = target;
    }
}
interface IStringConversionInfo
{
    public string SourceName { get; set; }
    public string? TargetName { get; set; }
}