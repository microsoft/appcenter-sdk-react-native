#addin "nuget:?package=System.Xml.ReaderWriter"

using System.Xml;
using System.Collections.Generic;

public static class ConfigFile
{
    public const string Path = "scripts/configuration/ac-build-config.xml";
    public static XmlReader CreateReader()
    {
        return XmlReader.Create(Path);
    }
}

#load "AppCenterModule.cake"
#load "AssemblyGroup.cake"
#load "BuildGroup.cake"
#load "PlatformPaths.cake"
#load "VersionReader.cake"
