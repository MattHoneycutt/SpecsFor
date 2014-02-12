using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml;

namespace SpecsFor.Mvc.IIS
{
    internal class IISExpressProcess
    {
        private readonly string _pathToSite;
        private Process _iisProcess;
        private readonly string _applicationHostConfigurationFile;

        /// <summary>
        /// The name of the website to configure for this instance of IIS Express.
        /// </summary>
        private readonly string _webSiteName;

        /// <summary>
        /// Gets the port number in use by this instance of IIS Express.
        /// </summary>
        public int PortNumber { get; private set; }

        #region Constructors

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="pathToSite">The full path to the website to host.</param>
        public IISExpressProcess(string pathToSite)
        {
            _pathToSite = pathToSite;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="pathToSite">The full path to the website to host.</param>
        /// <param name="applicationHostConfigurationFile">The full path to the IIS Express application host configuration file.</param>
        /// <param name="webSiteName">The name of the site in the application host configuration file to configure for this IIS Express process.</param>
        public IISExpressProcess(string pathToSite, string applicationHostConfigurationFile, string webSiteName)
            : this(pathToSite)
        {
            _applicationHostConfigurationFile = applicationHostConfigurationFile;
            _webSiteName = webSiteName;
        }

        #endregion Constructors

        /// <summary>
        /// Starts an IIS Express process that will host the web application.
        /// </summary>
        /// <remarks>
        /// This method finds an open port to configure the site to use and if an application host configuration file
        /// is being used it also configures the site specified when this instance was created with the correct port
        /// and path to the web application.
        /// </remarks>
        public void Start()
        {
            var startInfo = new ProcessStartInfo
                                {
                                    ErrorDialog = false,
                                    CreateNoWindow = true,
                                    UseShellExecute = false
                                };

            // Get an open port number on the local system.
            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                sock.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                PortNumber = ((IPEndPoint)sock.LocalEndPoint).Port;
            }

            // If a configuration file was not provided use the simple IIS Express command line configuration.
            if (string.IsNullOrEmpty(_applicationHostConfigurationFile))
            {
                startInfo.Arguments = string.Format("/path:\"{0}\" /port:{1}", _pathToSite, PortNumber);
            }
            else  // When an IIS Express configuration file is used, configure the sites node with information about the current site being tested.
            {
                var xDoc = new XmlDocument();

                // Read the application host configuration file into an XML document.
                using (var hostConfigReader = new StreamReader(_applicationHostConfigurationFile))
                {
                    xDoc.Load(hostConfigReader);
                }

                // Determine if a site node with the given site name in the config.
                XmlNode site = xDoc.SelectSingleNode("/configuration/system.applicationHost/sites/site[@name=\"" + _webSiteName + "\"]");
                
                if (site != null)
                {
                    XmlNode virtualDirectory = site.SelectSingleNode("application/virtualDirectory");

                    if (virtualDirectory != null)
                    {
                        virtualDirectory.Attributes["physicalPath"].Value = _pathToSite;
                    }

                    XmlNode binding = site.SelectSingleNode("bindings/binding");

                    if (binding != null)
                    {
                        binding.Attributes["bindingInformation"].Value = string.Format(":{0}:localhost", PortNumber);
                    }
                }

                xDoc.SelectSingleNode("/configuration/system.applicationHost/sites").ReplaceChild(site, xDoc.SelectSingleNode("/configuration/system.applicationHost/sites/site[@name=\"" + _webSiteName + "\"]"));

                xDoc.Save(_applicationHostConfigurationFile);

                startInfo.Arguments += string.Format(" /config:\"{0}\"", _applicationHostConfigurationFile);
            }

            var programfiles = !string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles(x86)"])
                                ? startInfo.EnvironmentVariables["programfiles(x86)"]
                                : startInfo.EnvironmentVariables["programfiles"];

            var iisExpress = programfiles + "\\IIS Express\\iisexpress.exe";

            if (!File.Exists(iisExpress))
            {
                throw new FileNotFoundException(string.Format("Did not find iisexpress.exe at {0}. Ensure that IIS Express is installed to the default location.", iisExpress));
            }

            startInfo.FileName = iisExpress;

            _iisProcess = new Process { StartInfo = startInfo };
            _iisProcess.Start();
        }

        public void Stop()
        {
            if (_iisProcess != null && !_iisProcess.HasExited)
            {
                _iisProcess.CloseMainWindow();
                _iisProcess.Kill();
                _iisProcess.Dispose();
                _iisProcess = null;
            }
        }
    }
}