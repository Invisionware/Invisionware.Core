///////////////////////////////////////////////////////////////////////////////
// Directives
///////////////////////////////////////////////////////////////////////////////

#l "tools/versionUtils.cake"
#l "tools/settingsUtils.cake"
#addin "nuget:?package=Cake.Incubator&version=5.1.0"
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.10.0"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var settings = SettingsUtils.LoadSettings(Context);
var versionInfo = VersionUtils.LoadVersion(Context, settings);

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles(settings.Build.SolutionFilePath);
//Information("Solutions Found:"); Information("\t{0}", string.Join("\n\t", solutions.Select(x => x.GetFilename().ToString()).ToList()));

var solutionPaths = solutions.Select(solution => solution.GetDirectory());
//Information("Solution Paths Found:"); Information("\t{0}", string.Join("\n\t", solutionPaths.Select(x => x.ToString()).ToList()));

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup((c) =>
{
	c.Information("Command Line:");
	c.Information("\tConfiguration: {0}", settings.Configuration);
	c.Information("\tRoot Path: {0}", settings.RootPath);
	c.Information("\tSettings Files: {0}", settings.SettingsFile);
	c.Information("\tExecute Build: {0}", settings.ExecuteBuild);
	c.Information("\tExecute Clean: {0}", settings.ExecuteClean);
	c.Information("\tExecute Unit Tests: {0}", settings.ExecuteUnitTest);
	c.Information("\tExecute Package: {0}", settings.ExecutePackage);
	c.Information("\tSolutions Found: {0}", solutions.Count);

	// Executed BEFORE the first task.
	try { settings.Display(c); } catch (Exception ex) { Error("Failed to Display Settings: {0}", ex.ToString()); }
	try { versionInfo.Display(c); } catch (Exception ex) { Error("Failed to Version Info: {0}", ex.ToString()); }
});

Teardown((c) =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("CleanAll")
	.Description("Cleans all directories that are used during the build process.")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in solutionPaths)
	{
		Information("Cleaning {0}", path);

		try {
			CleanDirectories(path + "/**/bin");
			CleanDirectories(path + "/**/obj");
		}
		catch {
			Warning("\tFailed to clean path");
		}
	}
	
	var pathTest = MakeAbsolute(Directory(settings.Test.SourcePath)).FullPath;
	Information("Cleaning {0}", pathTest);
	try { CleanDirectories(pathTest + "/**/bin"); } catch {}
	try { CleanDirectories(pathTest + "/**/obj"); } catch {}

	var pathArtificats = MakeAbsolute(Directory(settings.Build.ArtifactsPath)).FullPath;
	Information("Cleaning {0}", pathArtificats);
	try { CleanDirectories(pathArtificats); } catch {}

});

Task("Clean")
	.Description("Cleans all directories that are used during the build process.")
	.WithCriteria(settings.ExecuteBuild)
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in solutionPaths)
	{
		Information("Cleaning {0}", path);
		try { CleanDirectories(path + "/**/bin/" + settings.Configuration); } catch {}
		try { CleanDirectories(path + "/**/obj/" + settings.Configuration); } catch {}
	}

	var pathTest = MakeAbsolute(Directory(settings.Test.SourcePath)).FullPath;
	Information("Cleaning {0}", pathTest);
	try { CleanDirectories(pathTest + "/**/bin/" + settings.Configuration); } catch {}
	try { CleanDirectories(pathTest + "/**/obj/" + settings.Configuration); } catch {}
	
});

Task("CleanPackages")
	.Description("Cleans all packages that are used during the build process.")
	.Does(() =>
{
	var pathArtificats = MakeAbsolute(Directory(settings.Build.ArtifactsPath)).FullPath;
	Information("Cleaning {0}", pathArtificats);
	try { CleanDirectories(pathArtificats); } catch {}
});

Task("Restore")
	.Description("Restores all the NuGet packages that are used by the specified solution.")
	.WithCriteria(settings.ExecuteBuild)
	.Does(() =>
{
	
	// Restore all NuGet packages.
	foreach(var solution in solutions)
	{
		Information("Restoring {0}...", solution);

		if (FileExists(settings.NuGet.NuGetConfig))
		{
			NuGetRestore(solution, new NuGetRestoreSettings { ConfigFile = settings.NuGet.NuGetConfig });
		} else {
			NuGetRestore(solution);
		}
	}
});

Task("Build")
	.Description("Builds all the different parts of the project.")
	.WithCriteria(settings.ExecuteBuild)
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("UpdateVersion")
	.Does(() =>
{
	if (settings.Version.AutoIncrementVersion)
	{
		RunTarget("IncrementVersion");
	}

	// Build all solutions.
	foreach(var solution in solutions)
	{
		Information("Building {0}", solution);
		try {
			switch (settings.Build.BuildType)
			{
				case "dotnetcore":
					var dotNetCoreBuildSettings = new DotNetCoreMSBuildSettings();
					if (!string.IsNullOrEmpty(versionInfo.ToVersionPrefix()))
						dotNetCoreBuildSettings.SetVersionPrefix(versionInfo.ToVersionPrefix());
					if (!string.IsNullOrEmpty(versionInfo.ToVersionSuffix()))
						dotNetCoreBuildSettings.SetVersionSuffix(versionInfo.ToVersionSuffix());			
					if (!string.IsNullOrEmpty(versionInfo.ToString()))
						dotNetCoreBuildSettings.SetFileVersion(versionInfo.ToString(false));			

					DotNetCoreBuild(solution.ToString(), new DotNetCoreBuildSettings
													{
														Configuration = settings.Configuration,
														MSBuildSettings = dotNetCoreBuildSettings,
														Verbosity = DotNetCoreVerbosity.Minimal
													}
									);
					break;
				default:
					MSBuild(solution, configurator =>
											configurator.SetConfiguration(settings.Configuration)
											// .SetVerbosity(Verbosity.Minimal)
											// .UseToolVersion(MSBuildToolVersion.VS2015)
											// .SetMSBuildPlatform(MSBuildPlatform.x86)
											// .SetPlatformTarget(PlatformTarget.MSIL)
							);
					break;
			}			
		} 
		catch (Exception ex)
		{
			Error("Files to build project: " + solution + ". Error: " + ex.Message);
		}
	}
});

Task("UnitTest")
	.Description("Run unit tests for the solution.")
	.WithCriteria(settings.ExecuteUnitTest)
	.IsDependentOn("Build")
	.Does(() => 
{
	switch (settings.Test.Framework)
	{
		case TestFrameworkTypes.DotNetCore:
			RunTarget("UnitTest-DotNetCore");

			break;
		default: 
			RunTarget("UnitTest-CLI");
			
			break;
	}
});

Task("UnitTest-DotNetCore")
	.Description("Run dotnetcore based unit tests for the solution.")
	.WithCriteria(settings.ExecuteUnitTest)
	.Does(() => 
{
	// Run all unit tests we can find.
			
	var filePath = string.Format("{0}/**/{1}", settings.Test.SourcePath, settings.Test.FileSpec);
	
	Information("Unit Test Files: {0}", filePath);
	
	var testProjects = GetFiles(filePath);
	var testSettings = new DotNetCoreTestSettings()
				{
					Configuration = settings.Configuration,
					NoBuild = true
				};

	foreach(var p in testProjects)
	{
		Information("Executing Tests for {0}", p);
		
		DotNetCoreTest(p.ToString(), testSettings);
	}
});

Task("UnitTest-CLI")
	.Description("Run unit tests for the solution.")
	.WithCriteria(settings.ExecuteUnitTest)
	.Does(() => 
{
	// Run all unit tests we can find.
			
	var assemplyFilePath = string.Format("{0}/**/bin/{1}/**/{2}", settings.Test.SourcePath, settings.Configuration, settings.Test.FileSpec);
	
	Information("Unit Test Files: {0}", assemplyFilePath);
	
	var unitTestAssemblies = GetFiles(assemplyFilePath);
	
	foreach(var uta in unitTestAssemblies)
	{
		Information("Executing Tests for {0}", uta);
		
		switch (settings.Test.Framework)
		{
			case TestFrameworkTypes.NUnit2:
				NUnit(uta.ToString(), new NUnitSettings { });
				break;
			case TestFrameworkTypes.NUnit3:
				NUnit3(uta.ToString(), new NUnit3Settings { Configuration=settings.Configuration });
				break;
			case TestFrameworkTypes.XUnit:
				XUnit(uta.ToString(), new XUnitSettings { OutputDirectory = settings.Test.ResultsPath });
				break;
			case TestFrameworkTypes.XUnit2:
				XUnit2(uta.ToString(), new XUnit2Settings { OutputDirectory = settings.Test.ResultsPath, XmlReportV1 = true });
				break;
		}
	}
});

Task("Publish")
	.Description("Publish application to the artifacts folder")
	.IsDependentOn("UnitTest")
	.Does(() =>
{
	var artifactsPath = Directory(settings.Build.ArtifactsPath.Replace("[CONFIGURATION]", settings.Configuration));
	var buildOutputPath = settings.Build.BuildOutputPath.Replace("[CONFIGURATION]", settings.Configuration);

	Information("Copying Files from {0} to {1}", buildOutputPath, artifactsPath, settings.Configuration);

	if (!DirectoryExists(artifactsPath))
	{
		CreateDirectory(artifactsPath);
	} else {
		Information("\tCleaning Publish Path");
		CleanDirectories(artifactsPath);
	}

	CopyFiles(buildOutputPath + "/*", artifactsPath, true);
});

Task("Nuget-Package")
	.Description("Packages all nuspec files into nupkg packages.")
	.WithCriteria(settings.ExecutePackage)
	.IsDependentOn("UnitTest")
	.Does(() =>
{
	var artifactsPath = Directory(settings.NuGet.ArtifactsPath);
		
	CreateDirectory(artifactsPath);

	switch (settings.NuGet.BuildType)
	{
		case "dotnetcore":
			RunTarget("Nuget-Package-DotNetCore");

			break;
		default: 
			RunTarget("Nuget-Package-CLI");
			
			break;
	}
	
});

Task("Nuget-Package-DotNetCore")
	.Description("Packages all projects in the solution using dotnetcore")
	.WithCriteria(settings.ExecutePackage)
	.Does(() =>
{
	var artifactsPath = Directory(settings.NuGet.ArtifactsPath);
		
	CreateDirectory(artifactsPath);

	var dotNetCoreBuildSettings = new DotNetCoreMSBuildSettings();
	if (!string.IsNullOrEmpty(versionInfo.ToVersionPrefix()))
		dotNetCoreBuildSettings.SetVersionPrefix(versionInfo.ToVersionPrefix());
	if (!string.IsNullOrEmpty(versionInfo.ToVersionSuffix()))
		dotNetCoreBuildSettings.SetVersionSuffix(versionInfo.ToVersionSuffix());			
	if (!string.IsNullOrEmpty(versionInfo.ToString()))
		dotNetCoreBuildSettings.SetFileVersion(versionInfo.ToString(true));			
						
	var opts = new DotNetCorePackSettings
	{
		Configuration = settings.Configuration,
		OutputDirectory = artifactsPath,
		NoBuild = true,
		NoRestore = true,
		MSBuildSettings = dotNetCoreBuildSettings
	};

	if (!string.IsNullOrEmpty(versionInfo.ToVersionSuffix()))
		opts.VersionSuffix = versionInfo.ToVersionSuffix();

	if (settings.NuGet.IncludeSymbols) {
		opts.ArgumentCustomization = args => args.Append("--include-symbols -p:SymbolPackageFormat=snupkg");
	}

	Information("Location of Artifacts: {0}", artifactsPath);

	foreach(var solution in solutions)
	{
		Information("Building Packages for {0}", solution);

		try {
			//DotNetCorePack("./src/**/*.csproj", opts);
			DotNetCorePack(solution.ToString(), opts);
		}
		catch (Exception ex)
		{
			Debug(ex.Message);
			Information("There was a problem with packing some of the projects in {0}", solution);
		}
	}
});

Task("Nuget-Package-CLI")
	.Description("Packages all projects in the solution using the nuget.exe cli")
	.WithCriteria(settings.ExecutePackage)
	.Does(() =>
{
	var artifactsPath = Directory(settings.NuGet.ArtifactsPath);
	var nugetProps = new Dictionary<string, string>() { {"Configuration", settings.Configuration} };
		
	CreateDirectory(artifactsPath);
	
	var nuspecFiles = GetFiles(settings.NuGet.NuSpecFileSpec);
	
	var opts = new NuGetPackSettings {
			Version = versionInfo.ToString(),
			ReleaseNotes = versionInfo.ReleaseNotes,
			Properties = nugetProps,
			OutputDirectory = artifactsPath,
			Symbols = settings.NuGet.IncludeSymbols
		};
		
	//if (settings.NuGet.IncludeSymbols) {
	//	opts.ArgumentCustomization = args => args.Append("-Symbols -SymbolPackageFormat snupkg");
	//}
	
	foreach(var nsf in nuspecFiles)
	{
		Information("Packaging {0}", nsf);
		
		if (settings.NuGet.UpdateVersion) {
			VersionUtils.UpdateNuSpecVersion(Context, settings, versionInfo, nsf.ToString());	
		}
		
		if (settings.NuGet.UpdateLibraryDependencies) {
			VersionUtils.UpdateNuSpecVersionDependency(Context, settings, versionInfo, nsf.ToString());
		}
		
		NuGetPack(nsf, opts);
	}
});

Task("Nuget-Publish")
	.Description("Publishes all of the nupkg packages to the nuget server. ")
	.IsDependentOn("Nuget-Package")
	.Does(() =>
{
	var artifactsPath = Directory(settings.NuGet.ArtifactsPath);
		
	CreateDirectory(artifactsPath);

	switch (settings.NuGet.FeedApiKey.ToUpper())
	{
		case "LOCAL":
				settings.NuGet.FeedUrl = Directory(settings.NuGet.FeedUrl).Path.FullPath;
				//Information("Using Local repository: {0}", settings.NuGet.FeedUrl);
			break;
		case "NUGETAPIKEY":
				if (!System.IO.File.Exists("nugetapi.key"))
				{
					Error("Could not load nugetapi.key");
					return;
				}
				
				settings.NuGet.FeedApiKey = System.IO.File.ReadAllText("nugetapi.key");
			break;
		case "AzureDevOps":
		case "VSTS":
				if (BuildSystem.IsRunningOnAzurePipelinesHosted)
				{
					//settings.NuGet.FeedApiKey = EnvironmentVariable("SYSTEM_ACCESSTOKEN");
					settings.NuGet.FeedApiKey = "AzureDevOps";
				}
			break;
	}
		
	if (string.IsNullOrEmpty(settings.NuGet.NuGetConfig)) settings.NuGet.NuGetConfig = null;

	switch (settings.NuGet.BuildType)
	{
		case "dotnetcore":
			RunTarget("Nuget-Publish-DotNetCore");

			break;
		default: 
			RunTarget("Nuget-Publish-CLI");
			
			break;
	}
	
});

Task("Nuget-Publish-DotNetCore")
	.Description("Publishes all of the nupkg packages to the nuget server. ")
	.Does(() =>
{
	var authError = false;
	
	Information("Publishing Packages from {0} to {1} for version {2}", settings.NuGet.ArtifactsPath, settings.NuGet.FeedUrl, versionInfo.ToString());

	// Lets get the list of packages (we can skip anything that is not part of the current version being built)
	var nupkgFiles = GetFiles(settings.NuGet.NuGetPackagesSpec).Where(x => x.ToString().Contains(versionInfo.ToString())).ToList();

	Information("\t{0}", string.Join("\n\t", nupkgFiles.Select(x => x.GetFilename().ToString()).ToList()));
	
	var opts = new DotNetCoreNuGetPushSettings
					{
						Source = settings.NuGet.FeedUrl,
						ApiKey = settings.NuGet.FeedApiKey
						// ,Verbosity = DotNetCoreVerbosity.Detailed
					};
		
	//if (settings.NuGet.IncludeSymbols) {
	//	opts.ArgumentCustomization = args => args.Append("-Symbols -SymbolPackageFormat snupkg");
	//}		
		
	foreach (var n in nupkgFiles)
	{
		Information("Pushing Package: {0}", n);

		try {
			DotNetCoreNuGetPush(n.ToString(), opts);
		}
		catch (Exception ex)
		{
			if (ex.Message.Contains("403") || ex.Message.Contains("401")) { 
				Information("\tUnable to Authenticate to Nuget Feed. Publish of {0} failed", n.ToString());

				authError = true; 
			}
			else {
				Warning("\tFailed to published: {0}", ex.Message);			
			}
		}
	}

	if (authError && settings.NuGet.FeedApiKey == "VSTS")
	{
		Warning("\tYou may need to Configuration Your Credentials.\r\n\t\tCredentialProvider.VSS.exe -Uri {0}", settings.NuGet.FeedUrl);
	}
});

Task("Nuget-Publish-CLI")
	.Description("Publishes all of the nupkg packages to the nuget server. ")
	.Does(() =>
{
	var authError = false;
	
	Information("Publishing Packages from {0} to {1} for version {2}", settings.NuGet.ArtifactsPath, settings.NuGet.FeedUrl, versionInfo.ToString());

	// Lets get the list of packages (we can skip anything that is not part of the current version being built)
	var nupkgFiles = GetFiles(settings.NuGet.NuGetPackagesSpec).Where(x => x.ToString().Contains(versionInfo.ToString())).ToList();

	Information("\t{0}", string.Join("\n\t", nupkgFiles.Select(x => x.GetFilename().ToString()).ToList()));
	
	// if (BuildSystem.IsRunningOnAzurePipelinesHosted)
	// {
	// 	settings.NuGet.FeedApiKey = EnvironmentVariable("SYSTEM_ACCESSTOKEN");
	// }

	var opts = new NuGetPushSettings {
				Source = settings.NuGet.FeedUrl,
				ApiKey = settings.NuGet.FeedApiKey,
				Verbosity = NuGetVerbosity.Detailed
			};
			
	foreach (var n in nupkgFiles)
	{
		Information("Pushing Package: {0}", n);

		try
		{		
			NuGetPush(n, opts);
		}
		catch (Exception ex)
		{
			if (ex.Message.Contains("403") || ex.Message.Contains("401")) { 
				Information("\tUnable to Authenticate to Nuget Feed. Publish of {0} failed", n.ToString());

				authError = true; 
			}
			else if (ex.Message.Contains("409")) {
				Warning("\tUnable to publish package: {0}", ex.Message);			
			}
			else {
				Error("\tFailed to published: {0}", ex.Message);			
			}
		}
	}
	
	if (authError && settings.NuGet.FeedApiKey == "VSTS")
	{
		Warning("\tYou may need to Configuration Your Credentials.\r\n\t\tCredentialProvider.VSS.exe -Uri {0}", settings.NuGet.FeedUrl);
	}
});

Task("Nuget-UnPublish")
	.Description("UnPublishes all of the current nupkg packages from the nuget server. Issue: versionToDelete must use : instead of . due to bug in cake")
	.Does(() =>
{
	var v = Argument<string>("versionToDelete", versionInfo.ToString()).Replace(":",".");
	
	var nuspecFiles = GetFiles(settings.NuGet.NuSpecFileSpec);
	foreach(var f in nuspecFiles)
	{
		Information("UnPublishing {0}", f.GetFilenameWithoutExtension());

		var args = string.Format("delete {0} {1} -Source {2} -NonInteractive", 
								f.GetFilenameWithoutExtension(),
								v,
								settings.NuGet.FeedUrl
								);
	
		//if (settings.NuGet.FeedApiKey != "VSTS" ) {
			args = args + string.Format(" -ApiKey {0}", settings.NuGet.FeedApiKey);
		//}
				
		if (!string.IsNullOrEmpty(settings.NuGet.NuGetConfig)) {
			args = args + string.Format(" -Config {0}", settings.NuGet.NuGetConfig);
		}
		
		Information("NuGet Command Line: {0}", args);
		using (var process = StartAndReturnProcess("tools\\nuget.exe", new ProcessSettings {
			Arguments = args
		}))
		{
			process.WaitForExit();
			Information("nuget delete exit code: {0}", process.GetExitCode());
		}
	}
});

Task("UpdateVersion")
	.Description("Updates the version number in the necessary files")
	.Does(() =>
{
	Information("Updating Version to {0}", versionInfo.ToString());
	
	VersionUtils.UpdateVersion(Context, settings, versionInfo);
});

Task("IncrementVersion")
	.Description("Increments the version number and then updates it in the necessary files")
	.Does(() =>
{
	var oldVer = versionInfo.ToString();
	if (versionInfo.IsPreRelease) versionInfo.PreRelease++; else versionInfo.Build++;
	
	Information("Incrementing Version {0} to {1}", oldVer, versionInfo.ToString());
	
	RunTarget("UpdateVersion");	
});

Task("BuildNewVersion")
	.Description("Increments and Builds a new version")
	.IsDependentOn("IncrementVersion")
	.IsDependentOn("Build")
	.Does(() =>
{
});

Task("PublishNewVersion")
	.Description("Increments, Builds, and publishes a new version")
	.IsDependentOn("BuildNewVersion")
	.IsDependentOn("Publish")
	.Does(() =>
{
});

Task("DisplaySettings")
	.Description("Displays All Settings.")
	.Does(() =>
{
	// Settings will be displayed as they are part of the Setup task
});

Task("DisplayHelp")
	.Description("Displays All Settings.")
	.Does(() =>
{
	// Settings will be displayed as they are part of the Setup task
	SettingsUtils.DisplayHelp(Context);
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
	.Description("This is the default task which will be ran if no specific target is passed in.")
	.IsDependentOn("Build");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(settings.Target);