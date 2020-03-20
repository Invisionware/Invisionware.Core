#addin "nuget:?package=Cake.Json&version=4.0.0"
#addin "nuget:?package=Newtonsoft.Json&version=12.0.3"

using Newtonsoft.Json;

public class SettingsUtils
{
	public static Settings LoadSettings(ICakeContext context)
	{
		var settingsFile = context.Argument<string>("settingsFile", "./build.settings.json");
		
		context.Information("Loading Settings: {0}", settingsFile);

		if (!context.FileExists(settingsFile))
		{
			context.Error("Settings File Does Not Exist");
			return null;
		}

		var obj = context.DeserializeJsonFromFile<Settings>(settingsFile);

		obj.SettingsFile = settingsFile;

		obj.RootPath = context.MakeAbsolute(context.Directory(obj.RootPath)).ToString() + "/";
		//context.Information("Root Path {0}", obj.RootPath);

		obj.SettingsFile = ExpandSettingsPath(obj.SettingsFile, obj.RootPath);

		if (obj.Build == null) obj.Build = new BuildSettings();

		obj.Build.ArtifactsPath 	= ExpandSettingsPath(obj.Build.ArtifactsPath, obj.RootPath);
		obj.Build.SourcePath 		= ExpandSettingsPath(obj.Build.SourcePath, obj.RootPath);
		obj.Build.SolutionFileSpec 	= ExpandSettingsPath(obj.Build.SolutionFileSpec, obj.RootPath);
		
		// Allow for any overrides
		obj.Target 				= context.Argument<string>("target", obj.Target);
		obj.Configuration 		= context.Argument<string>("configuration", obj.Configuration);

		obj.VersionFile 		= context.Argument<string>("versionFile", obj.VersionFile);
		obj.VersionFile 		= ExpandSettingsPath(obj.VersionFile, obj.RootPath);
					
		obj.ExecuteBuild 		= GetBoolArgument(context, "build", obj.ExecuteBuild);
		obj.ExecuteBuild 		= !GetBoolArgument(context, "skipBuild", !obj.ExecuteBuild);

		obj.ExecutePackage 		= GetBoolArgument(context, "package", obj.ExecutePackage); 
		obj.ExecutePackage 		= !GetBoolArgument(context, "skipPackage", !obj.ExecutePackage);

		obj.ExecuteUnitTest 	= GetBoolArgument(context, "unitTest", obj.ExecuteUnitTest); 
		obj.ExecuteUnitTest 	= GetBoolArgument(context, "unitTests", obj.ExecuteUnitTest); 
		obj.ExecuteUnitTest 	= !GetBoolArgument(context, "skipUnitTest", !obj.ExecuteUnitTest); 
		obj.ExecuteUnitTest 	= !GetBoolArgument(context, "skipUnitTests", !obj.ExecuteUnitTest); 
		obj.ExecuteUnitTest 	= !GetBoolArgument(context, "skipTest", !obj.ExecuteUnitTest); 
		obj.ExecuteUnitTest 	= !GetBoolArgument(context, "skipTests", !obj.ExecuteUnitTest); 

		obj.ExecuteClean 		= GetBoolArgument(context, "clean", obj.ExecuteClean); 
		obj.ExecuteClean 		= !GetBoolArgument(context, "skipClean", !obj.ExecuteClean); 

		if (obj.Version == null) obj.Version = new VersionSettings();

		obj.Version.VersionFile 			= obj.VersionFile;
		obj.Version.AutoIncrementVersion	= GetBoolArgument(context, "autoincrementversion", obj.Version.AutoIncrementVersion);
		obj.Version.AutoIncrementVersion 	= GetBoolArgument(context, "autoversion", obj.Version.AutoIncrementVersion);
		obj.Version.LoadFrom				= context.Argument<VersionSourceTypes>("loadVersionFrom", obj.Version.LoadFrom);
		
		if (obj.Xamarin == null) obj.Xamarin = new XamarinSettings();

		obj.Xamarin.EnableXamarinIOS 		= GetBoolArgument(context, "enableXamarinIOS", obj.Xamarin.EnableXamarinIOS);
		obj.Xamarin.MacAgentIPAddress 		= context.Argument<string>("macAgentIP", obj.Xamarin.MacAgentIPAddress);
		obj.Xamarin.MacAgentUserName		= context.Argument<string>("macUserName", obj.Xamarin.MacAgentUserName);
		obj.Xamarin.MacAgentUserPassword 	= context.Argument<string>("macPassword", obj.Xamarin.MacAgentUserPassword);

		if (obj.NuGet == null) obj.NuGet = new NuGetSettings();
		
		obj.NuGet.BuildType		= context.Argument<string>("NugetBuildType", obj.NuGet.BuildType);
		obj.NuGet.PublishType	= context.Argument<string>("NugetPublishType", obj.NuGet.PublishType);
		obj.NuGet.FeedUrl		= context.Argument<string>("nugetFeed", obj.NuGet.FeedUrl);
		obj.NuGet.FeedUrl		= context.Argument<string>("nugetFeedUrl", obj.NuGet.FeedUrl);
		obj.NuGet.ArtifactsPath	= context.Argument<string>("NugetArtifacts", obj.NuGet.ArtifactsPath);
		obj.NuGet.ArtifactsPath	= context.Argument<string>("NugetArtifactsPath", obj.NuGet.ArtifactsPath);
		obj.NuGet.NuSpecPath	= context.Argument<string>("NugetNuspec", obj.NuGet.NuSpecPath);
		obj.NuGet.NuSpecPath	= context.Argument<string>("NugetNuspecPath", obj.NuGet.NuSpecPath);
		obj.NuGet.NuGetConfig	= context.Argument<string>("NugetConfig", obj.NuGet.NuGetConfig);

		obj.NuGet.FeedApiKey	= context.Argument<string>("nugetApiKey", obj.NuGet.FeedApiKey);
		
		obj.NuGet.LibraryMinVersionDependency 		= (context.Argument<string>("dependencyVersion", obj.NuGet.LibraryMinVersionDependency)).Replace(":",".");
		obj.NuGet.VersionDependencyTypeForLibrary 	= context.Argument<VersionDependencyTypes>("dependencyType", obj.NuGet.VersionDependencyTypeForLibrary);

		obj.NuGet.ArtifactsPath	= ExpandSettingsPath(obj.NuGet.ArtifactsPath, obj.RootPath);
		obj.NuGet.NuSpecPath	= ExpandSettingsPath(obj.NuGet.NuSpecPath, obj.RootPath);
		obj.NuGet.NuGetConfig	= ExpandSettingsPath(obj.NuGet.NuGetConfig, obj.RootPath);
		obj.NuGet.FeedUrl		= ExpandSettingsPath(obj.NuGet.FeedUrl, obj.RootPath);
		
		if (obj.Test == null) obj.Test = new TestSettings();

		obj.Test.SourcePath 	= context.Argument<string>("TestSourcePath", obj.Test.SourcePath);
		obj.Test.ResultsPath 	= context.Argument<string>("TestResultsPath", obj.Test.ResultsPath);
		obj.Test.FileSpec 		= context.Argument<string>("TestFileSpec", obj.Test.FileSpec);

		return obj;
	}
		
	private static bool GetBoolArgument(ICakeContext context, string argumentName, bool defaultValue)
	{
		var result = context.Argument<string>(argumentName, defaultValue.ToString()).ToLower() == "true" || 
						context.Argument<string>(argumentName, argumentName.ToString()) == "1";
		
		return result;
	}

	private static string ExpandSettingsPath(string settingsPath, string rootPath)
	{
		if (settingsPath.StartsWith("./")) settingsPath = settingsPath.Replace("./", rootPath);
		if (settingsPath.StartsWith(".\\")) settingsPath = settingsPath.Replace(".\\", rootPath);
		if (settingsPath.Contains("[ROOTPATH]")) settingsPath = settingsPath.Replace("[ROOTPATH]", rootPath);

		return settingsPath;
	}

	private static string ExpandEnvironmentVariables(ICakeContext context, string variableName, string defaultValue)
	{		
		return null;
	}
	
	public static void DisplayHelp(ICakeContext context)
	{
		var defaultValues = new Settings();
		
		context.Information("Command Line Help/Syntax:");
		context.Information("\t.\\build.ps1 <Target>\t\t\t\t(Default: {0})", defaultValues.Target);
		context.Information("\t\t-Configuration=<Configuration>\t\t(Default: {0})", defaultValues.Configuration);
		context.Information("\t\t-settingsFile=<Settings File>\t\t(Default: {0})", defaultValues.SettingsFile);
		context.Information("\t\t-versionFile=<Version File>\t\t(Default: {0})", defaultValues.VersionFile);
		context.Information("\t\t-autoincrement=<0|1>\t\t\t\t(Default: {0})", defaultValues.Version.AutoIncrementVersion);
		context.Information("\t\t-versionMajor=<Major Version Number>");
		context.Information("\t\t-versionMinor=<Minor Version Number>");
		context.Information("\t\t-versionBuild=<Build Version Number>");
		context.Information("\t\t-versionSuffix=<Version Suffix> (ex: pre, beta, alpha)");
		context.Information("\t\t-versionPreRelease=<PreRelease Number>");
		context.Information("\t\t-build=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecuteBuild);
		context.Information("\t\t-package=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecutePackage);
		context.Information("\t\t-unitTest=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecuteUnitTest);
		context.Information("\t\t-clean=<0|1>\t\t\t\t(Default: {0})", defaultValues.ExecuteClean);
		context.Information("\t\t-nugetFeed=<Nuget Feed URL>\t\t(Default: {0})", defaultValues.NuGet.FeedUrl);
		context.Information("\t\t-nugetApiKey=<Nuget Feed API Key>\t(Default: {0})", defaultValues.NuGet.FeedApiKey);
		context.Information("\t\t-dependencyVersion=<Version Number>\t(Default: {0})", defaultValues.NuGet.LibraryMinVersionDependency);
		context.Information("\t\t-dependencyType=<none|exact|greaterthan|greaterthanorequal|lessthan>\t\t(Default: {0})", defaultValues.NuGet.VersionDependencyTypeForLibrary);
		context.Information("\t\t-enableXamarinIOS=<0|1>\t\t\t(Default: {0})", defaultValues.Xamarin.EnableXamarinIOS);
		context.Information("\t\t-macAgentIP=<Mac IP Address>\t\t(Default: {0})", defaultValues.Xamarin.MacAgentIPAddress);
		context.Information("\t\t-macUserName=<Mac User Name>\t\t(Default: {0})", defaultValues.Xamarin.MacAgentUserName);
		context.Information("\t\t-macPassword=<Mac Password>\t\t(Default: {0})", defaultValues.Xamarin.MacAgentUserPassword);
		context.Information("");
		context.Information("Examples:");
		context.Information("\t.\\build Build -Configuration=Release");
		context.Information("\t.\\build UnitTest -build=0");
	}
}

public class Settings
{
	public Settings()
	{
		ExecuteBuild = true;
		ExecutePackage = true;
		ExecuteUnitTest = true;
		ExecuteClean = true;
		
		RootPath = ".\\";
		Target = "DisplayHelp";
		Configuration = "Release";
		SettingsFile = ".\\build.settings.json";
		VersionFile = ".\\build.version.json";
		
		Version = new VersionSettings();
		Build = new BuildSettings();
		Xamarin = new XamarinSettings();
		Test = new TestSettings();
		NuGet = new NuGetSettings();
	}

	public string Target {get;set;}
	public string Configuration {get;set;}
	public string SettingsFile {get;set;}
	public string VersionFile {get;set;}
	public string RootPath {get;set;}
	
	public bool ExecuteBuild {get;set;}
	public bool ExecutePackage {get;set;}
	public bool ExecuteUnitTest {get;set;}
	public bool ExecuteClean {get;set;}
	
	public VersionSettings Version {get;set;}
	public BuildSettings Build {get;set;}
	public TestSettings Test {get;set;}
	public NuGetSettings NuGet {get;set;}
	public XamarinSettings Xamarin {get;set;}
	
	public void Display(ICakeContext context)
	{
		context.Information("Settings:");

		context.Information("\tTarget: {0}", Target);
		context.Information("\tRoot Path: {0}", RootPath);
		context.Information("\tConfiguration: {0}", Configuration);
		context.Information("\tSettings File: {0}", SettingsFile);
		context.Information("\tVersion File: {0}", VersionFile);
		
		context.Information("\tExecute Build: {0}", ExecuteBuild);
		context.Information("\tExecute Package: {0}", ExecutePackage);
		context.Information("\tExecute UnitTests: {0}", ExecuteUnitTest);
		context.Information("\tExecute Clean: {0}", ExecuteClean);		
		
		Version.Display(context);
		Build.Display(context);
		Xamarin?.Display(context);
		Test?.Display(context);
		NuGet?.Display(context);
	}
}

public class VersionSettings
{
	public VersionSettings()
	{
		LoadFrom = VersionSourceTypes.versionfile;
	}
	
	public string VersionFile {get;set;}
	public string AssemblyInfoFile {get;set;}
	public VersionSourceTypes LoadFrom {get;set;}
	public bool AutoIncrementVersion {get;set;}
	
	public void Display(ICakeContext context)
	{
		context.Information("Version Settings:");
		context.Information("\tVersion File: {0}", VersionFile);
		context.Information("\tAssemblyInfo File: {0}", AssemblyInfoFile);
		context.Information("\tLoad From: {0}", LoadFrom);
		context.Information("\tAutoIncrement Version: {0}", AutoIncrementVersion);
	}
}

public class BuildSettings
{
	public BuildSettings()
	{
		SourcePath = "./source";
		ArtifactsPath = "./artifacts";
		BuildOutputPath = "./source/**/[CONFIGURATION]";
		SolutionFileSpec = "*.sln";
		TreatWarningsAsErrors = false;
		MaxCpuCount = 0;
		BuildType = "msbuild";
	}
	
	public string SourcePath {get;set;}
	public string SolutionFileSpec {get;set;}
	public bool TreatWarningsAsErrors {get;set;}
	public string ArtifactsPath {get;set;}
	public string BuildOutputPath {get;set;}
	public int MaxCpuCount {get;set;}
	public string BuildType {get;set;}
	
	public string SolutionFilePath {
		get {
			if (SolutionFileSpec.Contains("/")) return SolutionFileSpec;
			
			return string.Format("{0}{1}{2}", SourcePath, (SourcePath.EndsWith("/") || SolutionFileSpec.StartsWith("/")) ? "" : "/", SolutionFileSpec);
		}
	}
	
	public void Display(ICakeContext context)
	{
		context.Information("Build Settings:");
		context.Information("\tBuild Type: {0}", BuildType);
		context.Information("\tSource Path: {0}", SourcePath);
		context.Information("\tArtifacts Path: {0}", ArtifactsPath);
		context.Information("\tSolution File Spec: {0}", SolutionFileSpec);
		context.Information("\tSolution File Path: {0}", SolutionFilePath);
		context.Information("\tTreat Warnings As Errors: {0}", TreatWarningsAsErrors);
		context.Information("\tMax Cpu Count: {0}", MaxCpuCount);
	}
}

public class XamarinSettings
{
	public XamarinSettings()
	{
		EnableXamarinIOS = false;
	}

	public bool EnableXamarinIOS {get;set;}
	public string MacAgentIPAddress {get;set;}
	public string MacAgentUserName {get;set;}
	public string MacAgentUserPassword {get;set;}

	public void Display(ICakeContext context)
	{
		context.Information("Xamarin Settings:");	
		context.Information("\tEnable Xamarin IOS: {0}", EnableXamarinIOS);
		context.Information("\tMac Agent IP Address: {0}", MacAgentIPAddress);
		context.Information("\tMac Agent User Name: {0}", MacAgentUserName);
		//context.Information("\tMac Agent User Password: {0}", MacAgentUserPassword);
	}
}

public class TestSettings
{
	public TestSettings()
	{
		SourcePath = "./tests";
		ResultsPath = "./tests";
		FileSpec = "*.UnitTests.dll";
		Framework = TestFrameworkTypes.NUnit3;
	}
	
	public string SourcePath {get;set;}
	public string ResultsPath {get;set;}
	public string FileSpec {get;set;}
	public TestFrameworkTypes Framework {get;set;}
			
	public void Display(ICakeContext context)
	{
		context.Information("Test Settings:");
		context.Information("\tSource Path: {0}", SourcePath);
		context.Information("\tResults Path: {0}", ResultsPath);
		context.Information("\tTest File Spec: {0}", FileSpec);
	}
}

public class NuGetSettings
{
	public NuGetSettings()
	{
		NuSpecPath = "./nuspec";
		NuGetConfig = "./tools/NuGet.Config";
		ArtifactsPath = "./artifacts/packages";
		ArtifactsFileSpec = "*.nupkg";
		UpdateVersion = false;
		VersionDependencyTypeForLibrary = VersionDependencyTypes.none;
		UpdateLibraryDependencies = false;
		LibraryNamespaceBase = null;
		LibraryMinVersionDependency = null;
		IncludeSymbols = true;
	}

	public string BuildType {get;set;}
	public string PublishType {get;set;}
	public string NuGetConfig {get;set;}
	public string FeedUrl {get;set;}
	public string FeedApiKey {get;set;}
	public string NuSpecPath {get;set;}
	public string ArtifactsPath {get;set;}
	public string ArtifactsFileSpec {get;set;}
	public bool UpdateVersion {get;set;}
	public VersionDependencyTypes VersionDependencyTypeForLibrary {get;set;}
	public bool UpdateLibraryDependencies {get;set;}
	public string LibraryNamespaceBase {get;set;}
	public string LibraryMinVersionDependency {get;set;}
	public bool IncludeSymbols {get;set;}
	
	public string NuSpecFileSpec {
		get {
			return string.Format("{0}{1}**/*.nuspec", NuSpecPath, (NuSpecPath.EndsWith("/") || NuSpecPath.EndsWith("\\")) ? "" : "/");
		}
	}

	public string NuGetPackagesSpec {
		get {
			return string.Format("{0}{1}{2}", ArtifactsPath, (ArtifactsPath.EndsWith("/") || ArtifactsPath.EndsWith("\\")) ? "" : "/", ArtifactsFileSpec);
		}
	}
	
	public void Display(ICakeContext context)
	{
		context.Information("NuGet Settings:");
		context.Information("\tBuild Type: {0}", BuildType);
		context.Information("\tPublish Type: {0}", PublishType);
		context.Information("\tNuGet Config: {0}", NuGetConfig);
		context.Information("\tFeed Url: {0}", FeedUrl);
		//context.Information("\tFeed API Key: {0}", FeedApiKey);
		context.Information("\tNuSpec Path: {0}", NuSpecPath);
		context.Information("\tNuSpec File Spec: {0}", NuSpecFileSpec);
		context.Information("\tArtifacts Path: {0}", ArtifactsPath);
		context.Information("\tArtifacts Spec: {0}", ArtifactsFileSpec);
		context.Information("\tNuGet Packages Spec: {0}", NuGetPackagesSpec);
		context.Information("\tUpdate Version: {0}", UpdateVersion);
		context.Information("\tUpdate Library Dependencies: {0}", UpdateLibraryDependencies);
		context.Information("\tForce Version Match: {0}", VersionDependencyTypeForLibrary);
		context.Information("\tLibrary Namespace Base: {0}", LibraryNamespaceBase);
		context.Information("\tLibrary Min Version Dependency: {0}", LibraryMinVersionDependency);
		context.Information("\tInclude Symbols: {0}", IncludeSymbols);
	}
}

public enum VersionDependencyTypes {
	none,
	exact,
	greaterthan,
	greaterthanorequal,
	lessthan
}

public enum VersionSourceTypes {
	none,
	versionfile,
	assemblyinfo,
	git,
	commandline,
	azuredevops,
	tfs
}

public enum TestFrameworkTypes {
	none,
	NUnit2,
	NUnit3,
	XUnit,
	XUnit2,
	DotNetCore
}