﻿
namespace Acando.AspNet.NodeServices.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using HostingModels;
    using log4net;

    /// <summary>
    /// Describes options used to configure an <see cref="INodeServices"/> instance.
    /// </summary>
    public class NodeServicesOptions
    {
        internal const string TimeoutConfigPropertyName = nameof(InvocationTimeoutMilliseconds);
        private const int DefaultInvocationTimeoutMilliseconds = 60 * 1000;
        private const string LogCategoryName = "Acando.AspNet.NodeServices";
        private static readonly string[] DefaultWatchFileExtensions = { ".js", ".jsx", ".ts", ".tsx", ".json", ".html" };

        private static readonly ILog Logger = LogManager.GetLogger(LogCategoryName);

        /// <summary>
        /// Creates a new instance of <see cref="NodeServicesOptions"/>.
        /// </summary>
        /// <param name="projectPath">Path to the project</param>
        public NodeServicesOptions(string projectPath)
        {
            int settingInvocation = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["NodeDefaultInvocationTimeoutMilliseconds"], out settingInvocation) == false)
            {
                settingInvocation = DefaultInvocationTimeoutMilliseconds;
            }

            EnvironmentVariables = new Dictionary<string, string>();
            InvocationTimeoutMilliseconds = settingInvocation;
            WatchFileExtensions = (string[])DefaultWatchFileExtensions.Clone();
            
            ProjectPath = projectPath;

            EnvironmentVariables["NODE_ENV"] = ConfigurationManager.AppSettings["node_env"] ?? "development"; // De-facto standard values for Node
            EnvironmentVariables["BABEL_CACHE_PATH"] = ConfigurationManager.AppSettings["BABEL_CACHE_PATH"] ?? projectPath + "../babel.json";
            EnvironmentVariables["WEBSITE_NODE_DEFAULT_VERSION"] = ConfigurationManager.AppSettings["WEBSITE_NODE_DEFAULT_VERSION"] ?? "8.1.1";
            
            NodeInstanceOutputLogger = Logger;

            // By default, we use this package's built-in out-of-process-via-HTTP hosting/transport
            this.UseHttpHosting();
        }

        /// <summary>
        /// Specifies how to construct Node.js instances. An <see cref="INodeInstance"/> encapsulates all details about
        /// how Node.js instances are launched and communicated with. A new <see cref="INodeInstance"/> will be created
        /// automatically if the previous instance has terminated (e.g., because a source file changed).
        /// </summary>
        public Func<INodeInstance> NodeInstanceFactory { get; set; }

        /// <summary>
        /// If set, overrides the path to the root of your application. This path is used when locating Node.js modules relative to your project.
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// If set, the Node.js instance should restart when any matching file on disk within your project changes.
        /// </summary>
        public string[] WatchFileExtensions { get; set; }

        /// <summary>
        /// The Node.js instance's stdout/stderr will be redirected to this <see cref="ILog"/>.
        /// </summary>
        public ILog NodeInstanceOutputLogger { get; set; }

        /// <summary>
        /// If true, the Node.js instance will accept incoming V8 debugger connections (e.g., from node-inspector).
        /// </summary>
        public bool LaunchWithDebugging { get; set; }

        /// <summary>
        /// If <see cref="LaunchWithDebugging"/> is true, the Node.js instance will listen for V8 debugger connections on this port.
        /// </summary>
        public int DebuggingPort { get; set; }

        /// <summary>
        /// If set, starts the Node.js instance with the specified environment variables.
        /// </summary>
        public IDictionary<string, string> EnvironmentVariables { get; set; }

        /// <summary>
        /// Specifies the maximum duration, in milliseconds, that your .NET code should wait for Node.js RPC calls to return.
        /// </summary>
        public int InvocationTimeoutMilliseconds { get; set; }
    }
}