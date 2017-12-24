// <copyright file="ExceptionMessagesResourceManager.cs" company="David Eiwen">
// Copyright (c) David Eiwen. All rights reserved.
// </copyright>

namespace IndiePortable.Communication.Core.Resources
{
    using System.Reflection;
    using System.Resources;

    internal static class ExceptionMessagesResourceManager
    {
        private static ResourceManager Manager { get; } = new ResourceManager("ExceptionMessages", typeof(ExceptionMessagesResourceManager).GetTypeInfo().Assembly);

        public static string GetString(string key) => Manager.GetString(key);
    }
}
