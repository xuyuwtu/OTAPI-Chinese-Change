using System.Diagnostics.CodeAnalysis;

using Mono.Cecil;

namespace OTAPI_Chinese_Change;

static class StringConversionInfoExtension
{
    public static void SetSourceName(this IStringConversionInfo conversionInfo, MemberReference memberRef) => memberRef.Name = conversionInfo.SourceName;
    public static void SetSourceName(this IStringConversionInfo conversionInfo, AssemblyNameReference assemblyNameReference) => assemblyNameReference.Name = conversionInfo.SourceName;
    public static void SetTargetName(this IStringConversionInfo conversionInfo, MemberReference memberRef)
    {
        if (conversionInfo.TargetName is not null)
        {
            memberRef.Name = conversionInfo.TargetName;
        }
    }
    public static void SetTargetName(this IStringConversionInfo conversionInfo, AssemblyNameReference assemblyNameReference)
    {
        if (conversionInfo.TargetName is not null)
        {
            assemblyNameReference.Name = conversionInfo.TargetName;
        }
    }

    public static bool TryGetBySourceName<T>(this IList<T> stringConversionInfos, string name, [MaybeNullWhen(false)] out T value) where T : class, IStringConversionInfo
    {
        foreach (var info in stringConversionInfos)
        {
            if (name.Equals(info.SourceName, StringComparison.Ordinal))
            {
                value = info;
                return true;
            }
        }
        value = null;
        return false;
    }
    public static bool TryGetByTargetName<T>(this IList<T> stringConversionInfos, string name, [MaybeNullWhen(false)] out T value) where T : class, IStringConversionInfo
    {
        foreach (var info in stringConversionInfos)
        {
            if (name.Equals(info.TargetName, StringComparison.Ordinal))
            {
                value = info;
                return true;
            }
        }
        value = null;
        return false;
    }

    public static bool TryGetByTargetNameOrSourceName<T>(this IList<T> stringConversionInfos, string name, [MaybeNullWhen(false)] out T value) where T : class, IStringConversionInfo
    {
        if (stringConversionInfos.TryGetByTargetName(name, out value))
        {
            return true;
        }
        return stringConversionInfos.TryGetBySourceName(name, out value);
    }
}
