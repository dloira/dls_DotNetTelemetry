using System.Xml;
using Microsoft.Extensions.Options;
using Telemetry_Receiver.Diagnostics;
using Telemetry_Receiver.Options;

namespace Telemetry_Receiver.Infraestructure.QueryReader
{
    internal class XmlQueryProviderService
        : IQueryProviderService
    {
        private readonly IOptionsMonitor<TelemetryReceiverOptions> _options;
        private readonly TelemetryReceiverDiagnostics _diagnostics;
        private readonly Dictionary<string, string> _projectQueries;

        public XmlQueryProviderService(IOptionsMonitor<TelemetryReceiverOptions> options, TelemetryReceiverDiagnostics diagnostics)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
            _projectQueries = new Dictionary<string, string>();
            InitializeQueries();
        }

        private void InitializeQueries() 
        {
            _projectQueries.Clear();

            string queryXmlFile = _options.CurrentValue.Database.QueryXmlFilePath;
            if (queryXmlFile == null)
            {
                _diagnostics.QueriesFileNameNotSet();
                throw new Exception($"QueryXmlFilePath not found in appsettings file");
            }

            using var reader = XmlReader.Create(queryXmlFile, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse });
            while (reader.ReadToFollowing("query"))
            {
                var queryName = reader.GetAttribute("name");
                if (!string.IsNullOrEmpty(queryName))
                {
                    if (_projectQueries.ContainsKey(queryName))
                    {
                        _diagnostics.QueryNotUnique(queryName, queryXmlFile);
                        throw new Exception($"There are more than one query with name '{queryName}' on XML file '{queryXmlFile}'. Query name must be unique.");
                    }

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.CDATA)
                        {
                            _projectQueries.Add(queryName, reader.Value);
                            break;
                        }

                        if (reader.Name == queryName && reader.NodeType == XmlNodeType.EndElement)
                        {
                            _diagnostics.CDataNotFoundForQuery(queryName, queryXmlFile);
                            throw new Exception($"Can't find CDATA with query string for query name '{queryName}' on xml file '{queryXmlFile}'");
                        }
                    }
                }
            }
        }

        public string? GetQuery(string queryName)
        {
            _diagnostics.RequestedQuery(queryName);

            string? query = _projectQueries.GetValueOrDefault(queryName);

            if (query == null)
            {
                _diagnostics.QueryNotFound(queryName);
            }
            else
            {
                _diagnostics.QueryFound(queryName);
            }

            return query;
        }
    }

}
