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
		public int? PortNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use HTTPS].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use HTTPS]; otherwise, <c>false</c>.
        /// </value>
        public bool UseHttps { get; set; }

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

            if (PortNumber == null)
            {
	            if (UseHttps) throw new ArgumentException("In order to use https you must specify a port that has already been configured for https.");

	            CaptureAvailablePortNumber();
            }
            
			// If a configuration file was not provided use the simple IIS Express command line configuration.
			if (string.IsNullOrEmpty(_applicationHostConfigurationFile))
			{
                var xDoc = new XmlDocument();
                var iisConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\iisexpress\config\applicationhost.config";

				// Read the application host configuration file into an XML document.
				using (var hostConfigReader = new StreamReader(iisConfigPath)) xDoc.Load(hostConfigReader);

				var sites = xDoc.SelectSingleNode("/configuration/system.applicationHost/sites");

				if (sites == null)
					throw new InvalidOperationException("Unable to locate sites element in your applicationhost.config file.");

                sites.RemoveAll();

                var site = xDoc.CreateElement("site");
                site.SetAttribute("name", _webSiteName);
                site.SetAttribute("id", "1");

                var application = xDoc.CreateElement("application");
                application.SetAttribute("path", "/");
                application.SetAttribute("applicationPool", "Clr4IntegratedAppPool");

                var virtualDirectory = xDoc.CreateElement("virtualDirectory");
                virtualDirectory.SetAttribute("physicalPath", _pathToSite);
                virtualDirectory.SetAttribute("path", "/");
                application.AppendChild(virtualDirectory);
                site.AppendChild(application);


                var bindings = xDoc.CreateElement("bindings");
                var binding = xDoc.CreateElement("binding");
                                
                if (UseHttps){
                    binding.SetAttribute("protocol", "https");
                    binding.SetAttribute("bindingInformation", $"*:{PortNumber}:localhost");
                }
                else
                {
                    binding.SetAttribute("protocol", "http");
                    binding.SetAttribute("bindingInformation", $":{PortNumber}:localhost");
                }

                bindings.AppendChild(binding);
                site.AppendChild(bindings);

                sites.AppendChild(site);

                xDoc.Save("specsFor.config");

                startInfo.Arguments = " /config:\"specsFor.config\"";
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
				var site = xDoc.SelectSingleNode("/configuration/system.applicationHost/sites/site[@name=\"" + _webSiteName + "\"]");
				
				if (site != null)
				{
					var virtualDirectory = site.SelectSingleNode("application/virtualDirectory");

					if (virtualDirectory != null)
					{
						virtualDirectory.Attributes["physicalPath"].Value = _pathToSite;
					}

					var binding = site.SelectSingleNode("bindings/binding");

					if (binding != null)
					{
						binding.Attributes["bindingInformation"].Value = $":{PortNumber}:localhost";
					}
				}

				xDoc.SelectSingleNode("/configuration/system.applicationHost/sites").ReplaceChild(site, xDoc.SelectSingleNode("/configuration/system.applicationHost/sites/site[@name=\"" + _webSiteName + "\"]"));

				xDoc.Save(_applicationHostConfigurationFile);

				startInfo.Arguments += $" /config:\"{_applicationHostConfigurationFile}\"";
			}

			var programfiles = !string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles(x86)"])
								? startInfo.EnvironmentVariables["programfiles(x86)"]
								: startInfo.EnvironmentVariables["programfiles"];

			var iisExpress = programfiles + "\\IIS Express\\iisexpress.exe";

			if (!File.Exists(iisExpress))
			{
				throw new FileNotFoundException(
					$"Did not find iisexpress.exe at {iisExpress}. Ensure that IIS Express is installed to the default location.");
			}

			startInfo.FileName = iisExpress;

			_iisProcess = new Process { StartInfo = startInfo };
			_iisProcess.Start();
		}

		private void CaptureAvailablePortNumber()
		{
			// Get an open port number on the local system.
			using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				sock.Bind(new IPEndPoint(IPAddress.Loopback, 0));

				PortNumber = ((IPEndPoint)sock.LocalEndPoint).Port;
			}
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