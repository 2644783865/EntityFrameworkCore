﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.ProjectModel;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.Internal
{
    public class EfConsoleCommandResolver
    {
        private readonly string _basePath = AppContext.BaseDirectory;
        protected virtual string NetCoreToolDir
            => Path.Combine(_basePath, "tools", "netcoreapp1.0");
        protected virtual string DesktopToolDir
            => Path.Combine(_basePath, "tools", "net451");


        public virtual CommandSpec Resolve(ResolverArguments arguments)
            => arguments.IsDesktop
                ? CreateDesktopCommandSpec(arguments)
                : CreateNetCoreCommandSpec(arguments);

        public string FindEfCoreLibrary()
            => Path.Combine(NetCoreToolDir, "ef" + FileNameSuffixes.DotNet.DynamicLib);

        private CommandSpec CreateNetCoreCommandSpec(ResolverArguments arguments)
        {
            Check.NotEmpty(arguments.RuntimeConfigJson, "RuntimeConfigJson");
            Check.NotEmpty(arguments.DepsJsonFile, "DepsJsonFile");

            var args = new List<string>();

            args.Add("exec");

            args.Add("--runtimeconfig");
            args.Add(arguments.RuntimeConfigJson);

            args.Add("--depsfile");
            args.Add(arguments.DepsJsonFile);

            if (!string.IsNullOrEmpty(arguments.NuGetPackageRoot))
            {
                args.Add("--additionalprobingpath");
                args.Add(arguments.NuGetPackageRoot);
            }

            var commandPath = FindEfCoreLibrary();
            args.Add(commandPath);
            args.AddRange(arguments.CommandArguments);

            var muxer = new Muxer();
            var escapedArgs = ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(args.OrEmptyIfNull());

            return new CommandSpec(muxer.MuxerPath, escapedArgs, CommandResolutionStrategy.ProjectToolsPackage);
        }

        private CommandSpec CreateDesktopCommandSpec(ResolverArguments arguments)
        {
            var exeName = RuntimeInformation.OSArchitecture == Architecture.X86
                ? "ef.x86" + FileNameSuffixes.Windows.Exe
                : "ef" + FileNameSuffixes.Windows.Exe;
            var path = Path.Combine(DesktopToolDir, exeName);

            return new CommandSpec(path,
                ArgumentEscaper.EscapeAndConcatenateArgArrayForProcessStart(arguments.CommandArguments.OrEmptyIfNull()),
                CommandResolutionStrategy.ProjectToolsPackage);
        }
    }
}
