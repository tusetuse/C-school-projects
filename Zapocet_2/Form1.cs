using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Zapocet_2
{



    public partial class Form1 : Form
    {
        private Session _session;
        private ApplicationConfiguration _configuration;
        private const int OPC_UA_PORT = 4840;

        public Form1()
        {
            InitializeComponent();
            InitializeApplication();
            treeViewNodes.BeforeExpand += treeViewNodes_BeforeExpand;
        }

        private void InitializeApplication()
        {
            _configuration = CreateApplicationConfiguration();
            treeViewNodes.NodeMouseDoubleClick += TreeViewNodes_NodeMouseDoubleClick;
            PopulateDataTypeComboBox();
        }

        private void PopulateDataTypeComboBox()
        {
            cmbDataType.Items.AddRange(new string[]
            {
                "System.Boolean",
                "System.Int16",
                "System.Int32",
                "System.Int64",
                "System.Float",
                "System.Double",
                "System.String"
            });
            cmbDataType.SelectedIndex = 0;
        }

        private ApplicationConfiguration CreateApplicationConfiguration()
        {
            return new ApplicationConfiguration
            {
                ApplicationName = "OPC UA Browser",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:OPCUABrowser",
                ApplicationType = ApplicationType.Client,
                ClientConfiguration = new ClientConfiguration
                {
                    DefaultSessionTimeout = 60000,
                    MinSubscriptionLifetime = 10000
                },
                TransportQuotas = new TransportQuotas
                {
                    OperationTimeout = 60000,
                    MaxStringLength = 67108864,
                    MaxByteStringLength = 67108864,
                    MaxArrayLength = 65535
                },
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier(),
                    TrustedIssuerCertificates = new CertificateTrustList(),
                    TrustedPeerCertificates = new CertificateTrustList(),
                    RejectedCertificateStore = new CertificateTrustList()
                }
            };
        }

        private async void btnDiscoverServers_Click(object sender, EventArgs e)
         {
             listBoxServers.Items.Clear();
             toolStripStatusLabel.Text = "Looking for server...";
       
             string serverUrl = "opc.tcp://192.168.100.124:4840";
       
             try
             {
                 // Run the discovery in a background task
                 await Task.Run(() =>
                 {
                     // Try to connect to check if server exists
                     var config = EndpointConfiguration.Create();
                     var endpoints = CoreClientUtils.SelectEndpoint(serverUrl, false);
       
                     // Update UI on the main thread
                     this.Invoke(new Action(() =>
                     {
                         if (endpoints != null)
                         {
                             listBoxServers.Items.Add(serverUrl);
                             toolStripStatusLabel.Text = "Server found!";
                         }
                     }));
                 });
             }
             catch (Exception ex)
             {
                 toolStripStatusLabel.Text = "Server not found";
                 MessageBox.Show($"Error discovering server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
             }
         }

        private async void listBoxServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxServers.SelectedItem == null) return;

            string endpoint = listBoxServers.SelectedItem.ToString();
            try
            {
                var endpointDescription = CoreClientUtils.SelectEndpoint(endpoint, false);
                var endpointConfiguration = EndpointConfiguration.Create(_configuration);
                var configuredEndpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                _session = await Session.Create(
                    _configuration,
                    configuredEndpoint,
                    false,
                    "OPC UA Browser Session",
                    60000,
                    new UserIdentity(new AnonymousIdentityToken()),
                    null);

                await BrowseServer(); // Make sure this is called after connection
                toolStripStatusLabel.Text = "Connected and browsing server";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BrowseServer()
        {
            try
            {
                treeViewNodes.Nodes.Clear();

                // Start from root (NodeId 84 is typically the Objects folder)
                var rootNodeId = new NodeId(84, 0);  // namespace 0, identifier 84
                var rootNode = new TreeNode("Root") { Tag = rootNodeId };
                treeViewNodes.Nodes.Add(rootNode);

                // Browse children of root
                await BrowseChildren(rootNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Browse Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task BrowseChildren(TreeNode parentNode)
        {
            try
            {
                var nodeId = (NodeId)parentNode.Tag;

                // Create browse description
                var browseDescription = new BrowseDescription
                {
                    NodeId = nodeId,
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                    IncludeSubtypes = true,
                    NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method | NodeClass.ObjectType | NodeClass.VariableType),
                    ResultMask = (uint)BrowseResultMask.All
                };

                // Browse
                var browseCollection = new BrowseDescriptionCollection { browseDescription };
                BrowseResultCollection results;
                DiagnosticInfoCollection diagnostics;

                _session.Browse(null, null, 0, browseCollection, out results, out diagnostics);

                if (results?[0]?.References != null)
                {
                    foreach (var reference in results[0].References)
                    {
                        var childNodeId = ExpandedNodeId.ToNodeId(reference.NodeId, _session.NamespaceUris);
                        var displayName = $"{reference.DisplayName.Text} [{reference.NodeClass}]";

                        var childNode = new TreeNode(displayName) { Tag = childNodeId };
                        parentNode.Nodes.Add(childNode);

                        // Add a dummy node to enable the expand button
                        childNode.Nodes.Add(new TreeNode("Loading..."));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Browse Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void treeViewNodes_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var node = e.Node;

            // If this node only has the dummy "Loading..." node, browse for real children
            if (node.Nodes.Count == 1 && node.Nodes[0].Text == "Loading...")
            {
                node.Nodes.Clear();
                BrowseChildren(node);
            }
        }

        private void TreeViewNodes_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var nodeId = e.Node.Tag as NodeId;
            if (nodeId != null)
            {
                txtNodeId.Text = nodeId.ToString();
                ReadNodeValue(nodeId);
            }
        }

        private void ReadNodeValue(NodeId nodeId)
        {
            try
            {
                var value = _session.ReadValue(nodeId);
                txtReadValue.Text = value.Value?.ToString() ?? "null";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            try
            {
                var nodeId = NodeId.Parse(txtNodeId.Text);
                var valueToWrite = Convert.ChangeType(txtWriteValue.Text, Type.GetType(cmbDataType.SelectedItem.ToString()));

                // Create write value
                var writeValue = new WriteValue
                {
                    NodeId = nodeId,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(valueToWrite))
                };

                // Create write request
                var writeRequest = new WriteValueCollection { writeValue };

                // Write value
                StatusCodeCollection results;
                DiagnosticInfoCollection diagnosticInfos;

                ResponseHeader response = _session.Write(
                    null,                  // requestHeader
                    writeRequest,          // nodesToWrite
                    out results,          // results
                    out diagnosticInfos   // diagnosticInfos
                );

                if (results[0] == StatusCodes.Good)
                {
                    MessageBox.Show("Value written successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to write value. Status: {results[0]}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Write Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
