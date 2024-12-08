using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Windows.Forms;

namespace Zapocet_v1
{
    public partial class Form1 : Form
    {

        private Session _session;
        private ApplicationConfiguration _configuration;

        public Form1()
        {
            InitializeComponent();
            _configuration = CreateApplicationConfiguration();
        }

        private ApplicationConfiguration CreateApplicationConfiguration()
        {
            // Create a complete application configuration
            var configuration = new ApplicationConfiguration
            {
                ApplicationName = "PLCCommunicationApp",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:PLCCommunicationApp",
                ApplicationType = ApplicationType.Client,

                // Add ClientConfiguration
                ClientConfiguration = new ClientConfiguration
                {
                    DefaultSessionTimeout = 60000, // 60 seconds
                    MinSubscriptionLifetime = 10000 // 10 seconds
                },

                // Add Transport Quotas
                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = 60000, // 60 seconds
                    MaxStringLength = 67108864, // 64 MB
                    MaxByteStringLength = 67108864, // 64 MB
                    MaxArrayLength = 65535
                },

                // Minimal Security Configuration
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier(),
                    TrustedIssuerCertificates = new CertificateTrustList(),
                    TrustedPeerCertificates = new CertificateTrustList(),
                    RejectedCertificateStore = new CertificateTrustList()
                }
            };

            return configuration;
        }


        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string endpointUrl = txtEndpointUrl.Text;

                // Select endpoint
                var endpointDescription = CoreClientUtils.SelectEndpoint(endpointUrl, false);

                // Create endpoint configuration
                var endpointConfiguration = EndpointConfiguration.Create(_configuration);

                // Create configured endpoint
                var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // Create session with anonymous identity
                _session = await Session.Create(
                    _configuration,
                    endpoint,
                    false,
                    $"PLCCommunicationApp Session for {endpointUrl}",
                    60000,
                    new UserIdentity(new AnonymousIdentityToken()),
                    null
                );

                MessageBox.Show("Successfully connected to PLC!", "Connection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnReadValue.Enabled = true;
                btnWriteValue.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReadValue_Click(object sender, EventArgs e)
        {
            try
            {
                string nodeIdString = txtNodeId.Text;
                NodeId nodeId = NodeId.Parse(nodeIdString);

                // Read value from specified node
                var nodeToRead = new ReadValueId
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value
                };

                var readRequest = new ReadRequest
                {
                    NodesToRead = new ReadValueIdCollection { nodeToRead }
                };

                var response = _session.Read(null, 0, TimestampsToReturn.Both, readRequest.NodesToRead, out DataValueCollection results, out DiagnosticInfoCollection diagnosticInfos);

                if (results.Count > 0 && results[0].StatusCode == StatusCodes.Good)
                {
                    txtReadValue.Text = results[0].Value.ToString();
                }
                else
                {
                    MessageBox.Show("Failed to read value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWriteValue_Click(object sender, EventArgs e)
        {
            try
            {
                string nodeIdString = txtNodeId.Text;
                NodeId nodeId = NodeId.Parse(nodeIdString);

                object valueToWrite = Convert.ChangeType(txtWriteValue.Text, Type.GetType(cmbDataType.SelectedItem.ToString()));

                var writeValue = new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(valueToWrite))
                };

                var writeRequest = new WriteRequest
                {
                    NodesToWrite = new WriteValueCollection { writeValue }
                };

                var response = _session.Write(null, writeRequest.NodesToWrite, out StatusCodeCollection results, out DiagnosticInfoCollection diagnosticInfos);

                if (results.Count > 0 && results[0] == StatusCodes.Good)
                {
                    MessageBox.Show("Value written successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to write value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Write Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                _session?.Close();
                MessageBox.Show("Disconnected from PLC", "Disconnection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnReadValue.Enabled = false;
                btnWriteValue.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Disconnection Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
