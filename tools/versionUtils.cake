#addin "nuget:?package=Cake.Json&version=4.0.0"
#addin "nuget:?package=Cake.Incubator&version=5.1.0"
#addin "nuget:?package=Cake.GitVersioning&version=3.0.25"
#addin "nuget:?package=Newtonsoft.Json&version=12.0.3"
#addin "nuget:?package=Cake.FileHelpers&version=3.2.1"
#tool "nuget:?package=GitVersion.CommandLine&version=5.1.3"

using Newtonsoft.Json;

public class VersionUtils
{
	public static VersionInfo LoadVersion(ICakeContext context, Settings settings)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}

		VersionInfo verInfo = null;
		
		switch (settings.Version.LoadFrom)
		{
			case VersionSourceTypes.none:
				break;
			case VersionSourceTypes.versionfile:
				verInfo = LoadVersionFromJson(context, settings.Version.VersionFile);
				break;
			case VersionSourceTypes.assemblyinfo:
				verInfo = LoadVersionFromAssemblyInfo(context, settings.Version.AssemblyInfoFile);
				break;
			case VersionSourceTypes.git:
				verInfo = LoadVersionFromGit(context);
				break;
			case VersionSourceTypes.azuredevops:
				//verInfo = LoadVersionFromAzureDevOps(context);
				break;
			case VersionSourceTypes.tfs:
				//verInfo = LoadVersionFromTfs(context);
				break;
			case VersionSourceTypes.commandline:
				verInfo = LoadVersionFromCommandLine(context);
				break;
		}
		
		if (verInfo != null)
		{
			verInfo.CakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();
		}
		
		return verInfo;
	}
	
	private static VersionInfo LoadVersionFromJson(ICakeContext context, string versionFile)
	{
		context.Information("Loading Version Info From File: {0}", versionFile);
		if (string.IsNullOrEmpty(versionFile) || !context.FileExists(versionFile))
		{
			context.Error("Version File Does Not Exist");
			return null;
		}
		
		var obj = context.DeserializeJsonFromFile<VersionInfo>(versionFile);
		
		return obj;
	}
	
	private static VersionInfo LoadVersionFromAssemblyInfo(ICakeContext context, string assemblyInfoFile)
	{
		context.Information("Fetching Version Info from AssemblyInfo File: {0}", assemblyInfoFile);
		
		if (!string.IsNullOrEmpty(assemblyInfoFile) || !context.FileExists(assemblyInfoFile))
		{
			context.Error("AssemblyInfo file does not exist");
			return null;
		}

		try {
			var assemblyInfo = context.ParseAssemblyInfo(assemblyInfoFile);
			var v = Version.Parse(assemblyInfo.AssemblyVersion);
			
			var verInfo = new VersionInfo {
				Major = v.Major,
				Minor = v.Minor,
				Build = v.Build,
				Semantic = assemblyInfo.AssemblyInformationalVersion,
				Milestone = string.Concat("v", v.ToString())
			};
			
			return verInfo;
		}
		catch {}
		
		return null;
	}
	
	private static VersionInfo LoadVersionFromGit(ICakeContext context)
	{
		context.Information("Fetching Verson Info from Git");

		try {
			// GitVersion gitVersionInfo = context.GitVersion(new GitVersionSettings
			// {
			// 	OutputType = GitVersionOutput.Json,
			// });

			var gitVersionInfo = context.GitVersioningGetVersion();

			//context.Information("Git Version Details:\n{0}", gitVersionInfo.Dump());

			var verInfo = new VersionInfo {
				Major = gitVersionInfo.VersionMajor,
				Minor = gitVersionInfo.VersionMinor,
				Build = gitVersionInfo.BuildNumber,
				//Suffix = gitVersionInfo.PreReleaseTag,
				//PreRelease = gitVersionInfo.PrereleaseVersion,
				Semantic = gitVersionInfo.SemVer2,
				Milestone = string.Concat("v", gitVersionInfo.VersionRevision)
			};

			//context.Information("Version Info:\n{0}", verInfo.Dump());

			//context.Information(context.GitVersioningGetVersion().Dump());

			return verInfo;
		} catch {
		}

		return null;
	}
	
	private static VersionInfo LoadVersionFromAzureDevOps(ICakeContext context)
	{
		context.Information("Fetching Verson Info from Azure DevOps");

		try {

			// var verInfo = new VersionInfo {
			// 	Major = assertedVersions.Major,
			// 	Minor = assertedVersions.Minor,
			// 	Build = assertedVersions.Patch,
			// 	Semantic = assertedVersions.LegacySemVerPadded,
			// 	Milestone = string.Concat("v", assertedVersions.MajorMinorPatch)
			// };

			// context.Information("Calculated Semantic Version: {0}", verInfo.Semantic);

			// return verInfo;
		} catch {}

		return null;
	}
	
	private static VersionInfo LoadVersionFromCommandLine(ICakeContext context)
	{
		context.Information("Fetching Verson Info from Command Line");

		try {

			var verInfo = new VersionInfo {
				Major = context.Argument<int?>("versionMajor", 0),
				Minor = context.Argument<int?>("versionMinor", 0),
				Build = context.Argument<int?>("versionBuild", 0),
				PreRelease = context.Argument<int?>("versionPreRelease", 0),
				Suffix = context.Argument<string>("versionSuffix", ""),
				Semantic = context.Argument<string>("versionSem", "")
			};

			context.Information("Calculated Semantic Version: {0}", verInfo.Semantic);

			return verInfo;
		} catch {}

		return null;
	}

	
	public static void UpdateVersion(ICakeContext context, Settings settings, VersionInfo verInfo)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}

		if (!string.IsNullOrEmpty(settings.Version.VersionFile) && context.FileExists(settings.Version.VersionFile))
		{	
			context.Information("Updating Version File {0}", settings.Version.VersionFile);
			
			context.SerializeJsonToPrettyFile(settings.Version.VersionFile, verInfo);
		}
		
		if (!string.IsNullOrEmpty(settings.Version.AssemblyInfoFile) && context.FileExists(settings.Version.AssemblyInfoFile))
		{
			context.Information("Updating Assembly Info File {0}", settings.Version.AssemblyInfoFile);
			
			context.ReplaceRegexInFiles(settings.Version.AssemblyInfoFile, "AssemblyVersion\\(.*\\)", string.Format("AssemblyVersion(\"{0}\")", verInfo.ToString(false)));
			context.ReplaceRegexInFiles(settings.Version.AssemblyInfoFile, "AssemblyFileVersion\\(.*\\)", string.Format("AssemblyFileVersion(\"{0}\")", verInfo.ToString(false)));
		}
	}

	public static void UpdateNuSpecVersion(ICakeContext context, Settings settings, VersionInfo verInfo, FilePath nuspecFile)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}

		var xpq = string.Format("/n:package/n:metadata/n:version");
		
		context.Information("\tUpdating Version in Nuspec File {0} to {1}", nuspecFile, verInfo.ToString());
		
		try {
			context.XmlPoke(nuspecFile, xpq, verInfo.ToString(), new XmlPokeSettings {
				PreserveWhitespace = true
				, Namespaces = new Dictionary<string, string> {
					 { /* Prefix */ "n", /* URI */ "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"}
				 }
			});
		} catch {} // Its ok to throw these away as it most likely means the file didn't exist or the XPath didn't find any nodes
	}
	
	public static void UpdateNuSpecVersionDependency(ICakeContext context, Settings settings, VersionInfo verInfo, FilePath nuspecFile)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}

		if (string.IsNullOrEmpty(settings.NuGet.LibraryNamespaceBase)) return;
		
		var xpq = string.Format("/n:package/n:metadata/n:dependencies//n:dependency[starts-with(@id, '{0}')]/@version", settings.NuGet.LibraryNamespaceBase);
		
		var replacementStr = !string.IsNullOrEmpty(settings.NuGet.LibraryMinVersionDependency) ? settings.NuGet.LibraryMinVersionDependency : verInfo.ToString();

		switch (settings.NuGet.VersionDependencyTypeForLibrary)
		{
			case VersionDependencyTypes.none: break;
			case VersionDependencyTypes.exact: replacementStr = string.Format("[{0}]", replacementStr); break;
			case VersionDependencyTypes.greaterthan: replacementStr = string.Format("(,{0})", replacementStr); break;
			case VersionDependencyTypes.greaterthanorequal: replacementStr = string.Format("(,{0}]", replacementStr); break;
			case VersionDependencyTypes.lessthan: replacementStr = string.Format("({0},)", replacementStr); break;
		}
		
		context.Information("\tUpdating Version for {0} Namespace Assemblies in Nuspec File {1} to {2}", settings.NuGet.LibraryNamespaceBase, nuspecFile, replacementStr);
		
		try {
			context.XmlPoke(nuspecFile, xpq, replacementStr, new XmlPokeSettings {
				PreserveWhitespace = true
				, Namespaces = new Dictionary<string, string> {
					 { /* Prefix */ "n", /* URI */ "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd"}
				 }
			});
		} catch {} // Its ok to throw these away as it most likely means the file didn't exist or the XPath didn't find any nodes
	}
}

public class VersionInfo
{
	[Newtonsoft.Json.JsonProperty("major")]
	public int? Major {get;set;}
	[Newtonsoft.Json.JsonProperty("minor")]
	public int? Minor {get;set;}
	[Newtonsoft.Json.JsonProperty("build")]
	public int? Build {get;set;}
	[Newtonsoft.Json.JsonProperty("suffix")]
	public string Suffix {get;set;}
	[Newtonsoft.Json.JsonProperty("preRelease")]
	public int? PreRelease {get;set;}
	[Newtonsoft.Json.JsonProperty("releaseNotes")]
	public string[] ReleaseNotes {get;set;}

	[Newtonsoft.Json.JsonIgnore]
	public string Semantic {get;set;}
	[Newtonsoft.Json.JsonIgnore]
	public string Milestone {get;set;}
	[Newtonsoft.Json.JsonIgnore]
	public string CakeVersion {get;set;}
	
	[Newtonsoft.Json.JsonIgnore]
	public bool IsPreRelease { get { return (PreRelease != null && PreRelease != 0); } }

	[Newtonsoft.Json.JsonIgnore]
	public bool IsSemantic { get { return !string.IsNullOrEmpty(Semantic); } }

	public string ToString(bool includePreRelease = true) 
	{ 
		if (IsSemantic) return Semantic;

		var str = string.Format("{0}{1}", ToVersionPrefix(), includePreRelease ? ToVersionSuffix() : "");

		return str; 
	}
	
	public string ToVersionPrefix()
	{
		if (Major == null || Major == 0) return string.Empty;
		
		var str = string.Format("{0:#0}.{1:#0}.{2:#0}", Major, Minor, Build);
		
		return str; 
	}
	
	public string ToVersionSuffix()
	{
		if (!IsPreRelease || IsSemantic) return string.Empty;
		
		if (string.IsNullOrEmpty(Suffix)) Suffix = "pre";

		var str = string.Format("-{0}{1:00}", Suffix, PreRelease);

		return str;
	}
	
	public void Display(ICakeContext context)
	{
		context.Information("Version:");
		context.Information("\tMajor: {0}", Major);
		context.Information("\tMinor: {0}", Minor);
		context.Information("\tBuild: {0}", Build);
		context.Information("\tSuffix: {0}", ToVersionSuffix());
		context.Information("\tIs PreRelease: {0}", IsPreRelease);
		context.Information("\tIs Semantic: {0}", IsSemantic);
		context.Information("\tSemantic: {0}", Semantic);
		context.Information("\tMilestone: {0}", Milestone);
		context.Information("\tCake Version: {0}", CakeVersion);
		
		if (ReleaseNotes != null && ReleaseNotes.Length > 0) context.Information("\tRelease Notes: {0}", ReleaseNotes);
	}
}