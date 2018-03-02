using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace Bauland.Grove
{
    /// <summary>
    /// Object to store information of query
    /// </summary>
    public class QueryResponse
    {
        /// <summary>
        /// Result of operation, contains OK if successfull.
        /// </summary>
        public string Result;
        /// <summary>
        /// Type of information, depending of query
        /// </summary>
        public string Type;
        /// <summary>
        /// Information get by query
        /// </summary>
        public string Response;
    }
}
