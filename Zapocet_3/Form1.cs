using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Zapocet_3
{
    public partial class Form1 : Form
    {
        private Session session;
        private Dictionary<string, TextBox> variableTextBoxes;
        private Dictionary<string, NodeId> variableNodes;
        private ApplicationConfiguration configuration;
        private HashSet<string> editingVariables = new HashSet<string>();

        public Form1()
        {
            InitializeComponent();
            variableTextBoxes = new Dictionary<string, TextBox>();
            variableNodes = new Dictionary<string, NodeId>();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            configuration = new ApplicationConfiguration
            {
                ApplicationName = "PLC Monitor",
                ApplicationUri = $"urn:{System.Net.Dns.GetHostName()}:PLCMonitor",
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

            btnConnect.Click += async (s, e) => await ConnectToPLC(txtIP.Text);
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateVariableValues;
        }

        private async Task ConnectToPLC(string ipAddress)
        {
            try
            {
                btnConnect.Enabled = false;
                statusLabel.Text = "Connecting...";

                string serverUrl = $"opc.tcp://{ipAddress}:4840";
                var endpointDescription = CoreClientUtils.SelectEndpoint(serverUrl, false);
                var endpointConfiguration = EndpointConfiguration.Create(configuration);
                var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                session = await Session.Create(
                    configuration,
                    endpoint,
                    false,
                    "PLC Monitor Session",
                    60000,
                    new UserIdentity(new AnonymousIdentityToken()),
                    null);

                await BrowseAndCreateControls();
                updateTimer.Start();
                statusLabel.Text = "Connected";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Connection failed";
            }
            finally
            {
                btnConnect.Enabled = true;
            }
        }

        private async Task BrowseAndCreateControls()
        {
            try
            {
                panelVariables.Controls.Clear();
                variableTextBoxes.Clear();
                variableNodes.Clear();
                editingVariables.Clear();

                statusLabel.Text = "Browsing for variables in Program node...";

                var nodePath = new[]
                {
                    ObjectIds.ObjectsFolder,        
                    ObjectIds.Server,               
                    new NodeId(20000, 4),           
                    new NodeId(1000, 6),            
                    new NodeId("::", 6),            
                    new NodeId("::Program", 6)      
                };

                NodeId currentNodeId = nodePath[0];
                foreach (var targetNodeId in nodePath.Skip(2))
                {
                    currentNodeId = await FindNodeInChildren(currentNodeId, targetNodeId);
                    if (currentNodeId == null)
                    {
                        MessageBox.Show($"Failed to find node {targetNodeId} in path");
                        return;
                    }
                }

                await BrowseVariablesNode(currentNodeId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error browsing variables: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<NodeId> FindNodeInChildren(NodeId parentNodeId, NodeId targetNodeId)
        {
            var browseDescription = new BrowseDescription
            {
                NodeId = parentNodeId,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable),
                ResultMask = (uint)BrowseResultMask.All
            };

            BrowseResultCollection results = null;
            DiagnosticInfoCollection diagnostics = null;

            await Task.Run(() => {
                session.Browse(null, null, 0, new BrowseDescriptionCollection { browseDescription },out results, out diagnostics);
            });

            if (results?[0]?.References != null)
            {
                foreach (var reference in results[0].References)
                {
                    var nodeId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);
                    if (nodeId.Equals(targetNodeId) || (nodeId.NamespaceIndex == targetNodeId.NamespaceIndex && nodeId.Identifier.ToString() == targetNodeId.Identifier.ToString()))
                    {
                        return nodeId;
                    }
                }
            }

            return null;
        }

        private async Task WriteArrayElementToPLC(string arrayName, int index, string value)
        {
            try
            {
                if (variableNodes.TryGetValue(arrayName, out NodeId nodeId))
                {
                    var currentValue = await Task.Run(() => session.ReadValue(nodeId));
                    if (!(currentValue.Value is Array arrayValue))
                    {
                        throw new Exception("Variable is not an array");
                    }

                    Type elementType = arrayValue.GetType().GetElementType();

                    object convertedValue = Convert.ChangeType(value, elementType);

                    Array newArray = (Array)arrayValue.Clone();
                    newArray.SetValue(convertedValue, index);

                    var writeValue = new WriteValue
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(newArray))
                    };

                    WriteValueCollection writeValues = new WriteValueCollection { writeValue };
                    StatusCodeCollection results = null;
                    DiagnosticInfoCollection diagnosticInfos = null;

                    await Task.Run(() => {
                        session.Write(null, writeValues, out results, out diagnosticInfos);
                    });

                    if (results[0] != StatusCodes.Good)
                    {
                        throw new Exception($"Failed to write value. Status: {results[0]}");
                    }
                }
            }
            catch (Exception ex)
            {
                await Task.Run(() => MessageBox.Show($"Error writing array element: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task BrowseVariablesNode(NodeId programNodeId)
        {
            var browseDescription = new BrowseDescription
            {
                NodeId = programNodeId,
                BrowseDirection = BrowseDirection.Forward,
                ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                IncludeSubtypes = true,
                NodeClassMask = (uint)NodeClass.Variable,
                ResultMask = (uint)BrowseResultMask.All
            };

            BrowseResultCollection results = null;
            DiagnosticInfoCollection diagnostics = null;

            await Task.Run(() => {
                session.Browse(null, null, 0, new BrowseDescriptionCollection { browseDescription },
                             out results, out diagnostics);
            });

            if (results?[0]?.References != null)
            {
                int yPosition = 10;
                bool foundVariables = false;

                foreach (var reference in results[0].References)
                {
                    foundVariables = true;
                    var nodeId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);
                    string variableName = reference.DisplayName.Text;
                    var value = await Task.Run(() => session.ReadValue(nodeId));

                    await Task.Run(() => {
                        this.Invoke((MethodInvoker)delegate
                        {
                            Label label = new Label
                            {
                                Text = variableName + ":",
                                Location = new System.Drawing.Point(10, yPosition),
                                AutoSize = true
                            };
                            panelVariables.Controls.Add(label);

                            if (value.Value is Array arrayValue)
                            {
                                for (int i = 0; i < arrayValue.Length; i++)
                                {
                                    Label elementLabel = new Label
                                    {
                                        Text = $"[{i}]:",
                                        Location = new System.Drawing.Point(30, yPosition + ((i + 1) * 30)),
                                        AutoSize = true
                                    };
                                    panelVariables.Controls.Add(elementLabel);

                                    TextBox textBox = new TextBox
                                    {
                                        Location = new System.Drawing.Point(150, yPosition + ((i + 1) * 30)),
                                        Width = 150,
                                        Name = $"{variableName}[{i}]"
                                    };

                                    textBox.Enter += (s, e) =>
                                    {
                                        editingVariables.Add(textBox.Name);
                                    };

                                    textBox.Leave += (s, e) =>
                                    {
                                        editingVariables.Remove(textBox.Name);
                                    };

                                    textBox.KeyDown += async (s, e) =>
                                    {
                                        if (e.KeyCode == Keys.Enter)
                                        {
                                            await WriteArrayElementToPLC(variableName, i, textBox.Text);
                                            editingVariables.Remove(textBox.Name);
                                            this.ActiveControl = null;
                                        }
                                    };

                                    panelVariables.Controls.Add(textBox);
                                    variableTextBoxes.Add(textBox.Name, textBox);
                                }
                                variableNodes.Add(variableName, nodeId);
                                yPosition += (arrayValue.Length + 1) * 30;
                            }
                            else
                            {
                                TextBox textBox = new TextBox
                                {
                                    Location = new System.Drawing.Point(150, yPosition),
                                    Width = 150,
                                    Name = variableName
                                };

                                textBox.Enter += (s, e) =>
                                {
                                    editingVariables.Add(variableName);
                                };

                                textBox.Leave += (s, e) =>
                                {
                                    editingVariables.Remove(variableName);
                                };

                                textBox.KeyDown += async (s, e) =>
                                {
                                    if (e.KeyCode == Keys.Enter)
                                    {
                                        await WriteValueToPLC(variableName, textBox.Text);
                                        editingVariables.Remove(variableName);
                                        this.ActiveControl = null;
                                    }
                                };

                                panelVariables.Controls.Add(textBox);
                                variableTextBoxes.Add(variableName, textBox);
                                variableNodes.Add(variableName, nodeId);
                                yPosition += 30;
                            }
                        });
                    });
                }

                if (!foundVariables)
                {
                    MessageBox.Show("No variables found in Program node. Please verify the variable path.");
                }
                else
                {
                    statusLabel.Text = $"Connected - Found {variableNodes.Count} variables";
                }
            }
        }

        private async void UpdateVariableValues(object sender, EventArgs e)
        {
            if (session == null || !session.Connected)
            {
                updateTimer.Stop();
                statusLabel.Text = "Disconnected";
                return;
            }

            try
            {
                foreach (var entry in variableNodes)
                {
                    try
                    {
                        var value = await Task.Run(() => session.ReadValue(entry.Value));

                        if (value.Value is Array arrayValue)
                        {
                            for (int i = 0; i < arrayValue.Length; i++)
                            {
                                string elementName = $"{entry.Key}[{i}]";

                                if (editingVariables.Contains(elementName))
                                    continue;

                                if (variableTextBoxes.TryGetValue(elementName, out TextBox textBox))
                                {
                                    textBox.Text = arrayValue.GetValue(i)?.ToString() ?? "";
                                }
                            }
                        }
                        else
                        {
                            if (editingVariables.Contains(entry.Key))
                                continue;

                            if (variableTextBoxes.TryGetValue(entry.Key, out TextBox textBox))
                            {
                                textBox.Text = value.Value?.ToString() ?? "";
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                updateTimer.Stop();
                statusLabel.Text = "Error updating values";
                await Task.Run(() => MessageBox.Show($"Error updating values: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task WriteValueToPLC(string variableName, string value)
        {
            try
            {
                if (variableNodes.TryGetValue(variableName, out NodeId nodeId))
                {
                    var currentValue = await Task.Run(() => session.ReadValue(nodeId));
                    var valueType = currentValue.Value.GetType();

                    object convertedValue = Convert.ChangeType(value, valueType);

                    var writeValue = new WriteValue
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(convertedValue))
                    };

                    WriteValueCollection writeValues = new WriteValueCollection { writeValue };
                    StatusCodeCollection results = null;
                    DiagnosticInfoCollection diagnosticInfos = null;

                    await Task.Run(() => {
                        session.Write(null, writeValues, out results, out diagnosticInfos);
                    });

                    if (results[0] != StatusCodes.Good)
                    {
                        throw new Exception($"Failed to write value. Status: {results[0]}");
                    }
                }
            }
            catch (Exception ex)
            {
                await Task.Run(() => MessageBox.Show($"Error writing value: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            updateTimer.Stop();
            session?.Close();
            base.OnFormClosing(e);
        }
    }
}