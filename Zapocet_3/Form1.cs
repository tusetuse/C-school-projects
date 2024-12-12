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

                // First verify the server exists
                var endpointDescription = CoreClientUtils.SelectEndpoint(serverUrl, false);
                var endpointConfiguration = EndpointConfiguration.Create(configuration);
                var endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // Create session
                session = await Session.Create(
                    configuration,
                    endpoint,
                    false,
                    "PLC Monitor Session",
                    60000,
                    new UserIdentity(new AnonymousIdentityToken()),
                    null);

                // Once connected, browse for variables
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
                // Clear existing controls
                panelVariables.Controls.Clear();
                variableTextBoxes.Clear();
                variableNodes.Clear();
                editingVariables.Clear();

                statusLabel.Text = "Browsing for variables in Program node...";

                // Define the path to Program variables
                var nodePath = new[]
                {
                    ObjectIds.ObjectsFolder,          // i=84
                    ObjectIds.Server,                 // i=85
                    new NodeId(20000, 4),            // ns=4;i=20000
                    new NodeId(1000, 6),             // ns=6;i=1000
                    new NodeId("::", 6),             // ns=6;s=::
                    new NodeId("::Program", 6)       // ns=6;s=::Program
                };

                // Navigate through the path
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

                // Now browse the Program node for variables
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
                session.Browse(null, null, 0, new BrowseDescriptionCollection { browseDescription },
                             out results, out diagnostics);
            });

            if (results?[0]?.References != null)
            {
                foreach (var reference in results[0].References)
                {
                    var nodeId = ExpandedNodeId.ToNodeId(reference.NodeId, session.NamespaceUris);
                    if (nodeId.Equals(targetNodeId) ||
                        (nodeId.NamespaceIndex == targetNodeId.NamespaceIndex &&
                         nodeId.Identifier.ToString() == targetNodeId.Identifier.ToString()))
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
                    // Read current array to get its type and values
                    var currentValue = await Task.Run(() => session.ReadValue(nodeId));
                    if (!(currentValue.Value is Array arrayValue))
                    {
                        throw new Exception("Variable is not an array");
                    }

                    // Get the element type
                    Type elementType = arrayValue.GetType().GetElementType();

                    // Convert the input value to the correct type
                    object convertedValue = Convert.ChangeType(value, elementType);

                    // Create a copy of the array and update the specified element
                    Array newArray = (Array)arrayValue.Clone();
                    newArray.SetValue(convertedValue, index);

                    // Create write value for the entire array
                    var writeValue = new WriteValue
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(newArray))
                    };

                    // Write value
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
                await Task.Run(() => MessageBox.Show($"Error writing array element: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
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

                    // Read variable to check if it's an array
                    var value = await Task.Run(() => session.ReadValue(nodeId));

                    await Task.Run(() => {
                        this.Invoke((MethodInvoker)delegate
                        {
                            // Create label
                            Label label = new Label
                            {
                                Text = variableName + ":",
                                Location = new System.Drawing.Point(10, yPosition),
                                AutoSize = true
                            };
                            panelVariables.Controls.Add(label);

                            // Check if the value is an array
                            if (value.Value is Array arrayValue)
                            {
                                // Create textbox for each array element
                                for (int i = 0; i < arrayValue.Length; i++)
                                {
                                    // Create element label
                                    Label elementLabel = new Label
                                    {
                                        Text = $"[{i}]:",
                                        Location = new System.Drawing.Point(30, yPosition + ((i + 1) * 30)),
                                        AutoSize = true
                                    };
                                    panelVariables.Controls.Add(elementLabel);

                                    // Create textbox for array element
                                    TextBox textBox = new TextBox
                                    {
                                        Location = new System.Drawing.Point(150, yPosition + ((i + 1) * 30)),
                                        Width = 150,
                                        Name = $"{variableName}[{i}]"
                                    };

                                    // Add handler for when user starts editing
                                    textBox.Enter += (s, e) =>
                                    {
                                        editingVariables.Add(textBox.Name);
                                    };

                                    // Add handler for when editing is complete
                                    textBox.Leave += (s, e) =>
                                    {
                                        editingVariables.Remove(textBox.Name);
                                    };

                                    // Add KeyDown handler for writing values
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
                                // Store the NodeId once for the entire array
                                variableNodes.Add(variableName, nodeId);
                                yPosition += (arrayValue.Length + 1) * 30;
                            }
                            else
                            {
                                // Original code for non-array variables
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
                            // Update each array element
                            for (int i = 0; i < arrayValue.Length; i++)
                            {
                                string elementName = $"{entry.Key}[{i}]";

                                // Skip if the element is being edited
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
                            // Handle non-array variables as before
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
                await Task.Run(() => MessageBox.Show($"Error updating values: {ex.Message}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
            }
        }

        private async Task WriteValueToPLC(string variableName, string value)
        {
            try
            {
                if (variableNodes.TryGetValue(variableName, out NodeId nodeId))
                {
                    // Read current value to get its type
                    var currentValue = await Task.Run(() => session.ReadValue(nodeId));
                    var valueType = currentValue.Value.GetType();

                    // Convert the input value to the correct type
                    object convertedValue = Convert.ChangeType(value, valueType);

                    // Create write value
                    var writeValue = new WriteValue
                    {
                        NodeId = nodeId,
                        AttributeId = Attributes.Value,
                        Value = new DataValue(new Variant(convertedValue))
                    };

                    // Write value
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
                await Task.Run(() => MessageBox.Show($"Error writing value: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error));
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