// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.Internal
{
    public interface IProjectFile
    {
        void AddDocument(string filePath);
        void RemoveDocument(string filePath);
        void Save();
    }
}
