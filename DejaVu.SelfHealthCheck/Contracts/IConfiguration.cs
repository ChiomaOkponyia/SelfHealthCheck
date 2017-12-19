using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{

    /// <summary>
    /// Contract for holding App-Specific self-health check Configuration details
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Gets or sets the app ID.
        /// </summary>
        /// <value>
        /// The app ID.
        /// </value>
        string AppID { get; set; }

        /// <summary>
        /// Gets or sets the interval for running the checks.
        /// </summary>
        /// <value>
        /// The interval in milli-seconds.
        /// </value>
        double CheckInterval { get; set; }

        /// <summary>
        /// Gets or sets the IP Address for the configured app.
        /// </summary>
        /// <value>
        /// IP Address.
        /// </value>
        string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the next Check Time for the configured app.
        /// </summary>
        /// <value>
        /// IP Address.
        /// </value>
        DateTime NextCheckTime { get; set; }
        /// <summary>
        /// Gets or sets the different checks (database, network, url, etc).
        /// </summary>
        /// <value>
        /// The list of checks.
        /// </value>
        IList<ICheckConfiguration> Checks { get; set; }
    }
}
