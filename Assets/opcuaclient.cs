using System.Collections.Generic;
using UnityEngine;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;


public class opcuaclient: MonoBehaviour
{
    private ApplicationConfiguration app_configuration = new ApplicationConfiguration();
    //private Subscription m_subscription;
    //private MonitoredItem monitoredItem;
    private Session m_session;
    public string server_address = @"opc.tcp://127.0.0.1:48484/OPCUA/SimulationServer";
    //public string val;
    private Dictionary<string, string> msg_dic;
    //prameter from server
    private string inching;
    private string microspeed;
    private string ropeanglefeaturesbypass;
    private string swaycontrol;
    private string swaycontrol_slinglength;
    private string hoist_position;
    private string bridge_position;
    private string trolley_position;
    private string test;

    public delegate void OnMessageArrivedDelegate(Dictionary<string, string> newMsg);
    public event OnMessageArrivedDelegate OnMessageArrived;
    private Dictionary<string, string> m_msg;
    public Dictionary<string, string> msg
    {
        get
        {
            return m_msg;
        }
        set
        {
            foreach (string key in value.Keys)
            {
                if (m_msg[key] != value[key])
                {
                    m_msg = value;
                    break;
                }
            }

            //m_msg = value;
            if (OnMessageArrived != null)
            {
                OnMessageArrived(m_msg);
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {   

        upcua_read();
        inching = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.Inching")).ToString();
        microspeed = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.MicroSpeed")).ToString();
        ropeanglefeaturesbypass = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.RopeAngleFeaturesBypass")).ToString();
        swaycontrol = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.SwayControl")).ToString();
        swaycontrol_slinglength = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.SwayControl_SlingLength_mm")).ToString();
        hoist_position = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.Status.Hoist.Position.Position_m")).ToString();
        bridge_position = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.Status.Bridge.Position.Position_m")).ToString();
        trolley_position = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.Status.Trolley.Position.Position_m")).ToString();
        test = m_session.ReadValue(NodeId.Parse("ns=2;i=2")).ToString();
        msg_dic = new Dictionary<string, string>();
        m_msg= new Dictionary<string, string>();
        m_msg.Add("inching", inching);
        m_msg.Add("microspeed", microspeed);
        m_msg.Add("ropeanglefeaturesbypass", ropeanglefeaturesbypass);
        m_msg.Add("swaycontrol", swaycontrol);
        m_msg.Add("swaycontrol_slinglength", swaycontrol_slinglength);
        m_msg.Add("hoist_position", hoist_position);
        m_msg.Add("bridge_position", bridge_position);
        m_msg.Add("trolley_position", trolley_position);
        m_msg.Add("test", test);

    }

    ApplicationConfiguration OpcUa_Client_Configuration()
    {
        // Configuration OPCUa Client {W/R -> Data}
        var config = new ApplicationConfiguration()
        {
            // Initialization (Name, Uri, etc.)
            ApplicationName = "OPCUa_AS", // OPCUa AS (Automation Studio B&R)
            ApplicationUri = @"opc.tcp://127.0.0.1:48484/OPCUA/SimulationServer",
            // Type -> Client
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                // Security Configuration - Certificate
                ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "OPCUa_AS", System.Net.Dns.GetHostName()) },
                TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                AutoAcceptUntrustedCertificates = true,
                AddAppCertToTrustedStore = true
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 10000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 50000 },
            TraceConfiguration = new TraceConfiguration()
        };
        config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
        if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
        {
            config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
        }

        var application = new ApplicationInstance
        {
            ApplicationName = "OPCUa_AS",
            ApplicationType = ApplicationType.Client,
            ApplicationConfiguration = config
        };
        //application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();

        return config;
    }

    Session OpcUa_Create_Session(ApplicationConfiguration client_configuration, EndpointDescription client_end_point)
    {
        return Session.Create(client_configuration, new ConfiguredEndpoint(null, client_end_point, EndpointConfiguration.Create(client_configuration)), false, "", 10000, null, null).GetAwaiter().GetResult();
    }

    void upcua_read() 
    {
        try
        {
            app_configuration = OpcUa_Client_Configuration();
            EndpointDescription end_point = CoreClientUtils.SelectEndpoint(server_address, useSecurity: false);
            m_session = OpcUa_Create_Session(app_configuration, end_point);
            //var val = client_session.ReadValue(NodeId.Parse("ns=2;i=2"));
            //NodeId m_NodeId = new NodeId("ns=2;i=2");
            //if (m_subscription == null)
            //{
            //    m_subscription = new Subscription(m_session.DefaultSubscription);
            //    m_subscription.PublishingEnabled = true;
            //    m_subscription.PublishingInterval = 1000;
            //    m_session.AddSubscription(m_subscription);
            //    m_subscription.Create();
            //    m_subscription.ApplyChanges();
            //}
           
            //if (monitoredItem == null)
            //{
            //    monitoredItem = new MonitoredItem(m_subscription.DefaultItem);
            //    monitoredItem.StartNodeId = m_NodeId;
            //    monitoredItem.AttributeId = Attributes.Value;
            //    monitoredItem.MonitoringMode = MonitoringMode.Reporting;
            //    monitoredItem.SamplingInterval = 1000;
            //    monitoredItem.QueueSize = 0;
            //    monitoredItem.DiscardOldest = true;
            //    // define event handler for this item, and then add to subscription
            //    //monitoredItem.Notification += new MonitoredItemNotificationEventHandler(OnNotification);
            //    m_subscription.AddItem(monitoredItem);
            //}
        }
        catch (Exception e)
        {
            Console.WriteLine("Communication Problem: {0}", e);
        }
    }
    void FixedUpdate()
    {
        //msg = m_session.ReadValue(NodeId.Parse("ns=2;i=2")).ToString();
        inching= m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.Inching")).ToString();
        microspeed = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.MicroSpeed")).ToString();
        ropeanglefeaturesbypass = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.RopeAngleFeaturesBypass")).ToString();
        swaycontrol = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.SwayControl")).ToString();
        swaycontrol_slinglength = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.SwayControl_SlingLength_mm")).ToString();
        hoist_position = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.Status.Hoist.Position.Position_m")).ToString();
        bridge_position = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.Status.Bridge.Position.Position_m")).ToString();
        trolley_position = m_session.ReadValue(NodeId.Parse("ns=7;s=SCF.PLC.DX_Custom_V.Status.Trolley.Position.Position_m")).ToString();
        test = m_session.ReadValue(NodeId.Parse("ns=2;i=2")).ToString();
        
        msg_dic["inching"] = inching;
        msg_dic["microspeed"] = microspeed;
        msg_dic["ropeanglefeaturesbypass"] = ropeanglefeaturesbypass;
        msg_dic["swaycontrol"] = swaycontrol;
        msg_dic["swaycontrol_slinglength"] = swaycontrol_slinglength;
        msg_dic["hoist_position"] = hoist_position;
        msg_dic["bridge_position"] = bridge_position;
        msg_dic["trolley_position"] = trolley_position;
        msg_dic["test"] = test;
        msg = msg_dic;
        //print(msg["test"]);
        //msg_dic.Add("inching", inching);
        //msg_dic.Add("microspeed", microspeed);
        //msg_dic.Add("ropeanglefeaturesbypass", ropeanglefeaturesbypass);
        //msg_dic.Add("swaycontrol", swaycontrol);
        //msg_dic.Add("swaycontrol_slinglength", swaycontrol_slinglength);
        //msg_dic.Add("hoist_position", hoist_position);
        //msg_dic.Add("bridge_position", bridge_position);
        //msg_dic.Add("trolley_position", trolley_position);
        //msg_dic.Add("test", test);


    }


    //private void monitoredItem_Notification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
    //{
    //    print(5);
    //    //if (InvokeRequired)
    //    //{
    //    //    BeginInvoke(new MonitoredItemNotificationEventHandler(monitoredItem_Notification), monitoredItem, e);
    //    //    return;
    //    //}
    //    MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;
    //    if (notification == null)
    //    {
    //        return;
    //    }
    //    Debug.Log("value: " + Utils.Format("{0}", notification.Value.WrappedValue.ToString()) +
    //      ";\nStatusCode: " + Utils.Format("{0}", notification.Value.StatusCode.ToString()) +
    //      ";\nSource timestamp: " + notification.Value.SourceTimestamp.ToString() +
    //      ";\nServer timestamp: " + notification.Value.ServerTimestamp.ToString());
    //}
    //public void Init()
    //{
    //    Console.WriteLine("Step 1 - Create application configuration and certificate.");
    //    var config = new ApplicationConfiguration()
    //    {
    //        ApplicationName = "OPCUa_AS",
    //        ApplicationUri = @"opc.tcp://127.0.0.1:48484/OPCUA/SimulationServer",/*Utils.Format(@"urn:{0}:OPCUa_AS", System.Net.Dns.GetHostName()),*/
    //        ApplicationType = ApplicationType.Client,
    //        SecurityConfiguration = new SecurityConfiguration
    //        {
    //            ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "OPCUa_AS", System.Net.Dns.GetHostName()) },
    //            TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },            //@: the string will be handled with as a interity
    //            TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
    //            RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
    //            AutoAcceptUntrustedCertificates = true
    //        },
    //        TransportConfigurations = new TransportConfigurationCollection(),
    //        TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
    //        ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
    //        TraceConfiguration = new TraceConfiguration()
    //    };
    //    config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
    //    if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
    //    {
    //        config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
    //    }

    //    var application = new ApplicationInstance
    //    {
    //        ApplicationName = "Test-Client",
    //        ApplicationType = ApplicationType.Client,
    //        ApplicationConfiguration = config
    //    };
    //    //application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();

    //    var selectedEndpoint = CoreClientUtils.SelectEndpoint(@"opc.tcp://127.0.0.1:48484/OPCUA/SimulationServer", useSecurity: true, discoverTimeout: 15000);

    //    //Console.WriteLine($"Step 2 - Create a session with your server: {selectedEndpoint.EndpointUrl} ");
    //    using (var session = Session.Create(config, new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(config)), false, "", 60000, null, null).GetAwaiter().GetResult())
    //    {
    //        Console.WriteLine("Step 3 - Browse the server namespace.");
    //        ReferenceDescriptionCollection refs;
    //        Byte[] cp;
    //        session.Browse(null, null, ObjectIds.ObjectsFolder, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out cp, out refs);
    //        Console.WriteLine("DisplayName: BrowseName, NodeClass");
    //        foreach (var rd in refs)
    //        {
    //            Console.WriteLine("{0}: {1}, {2}", rd.DisplayName, rd.BrowseName, rd.NodeClass);
    //            ReferenceDescriptionCollection nextRefs;
    //            byte[] nextCp;
    //            session.Browse(null, null, ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris), 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out nextCp, out nextRefs);
    //            foreach (var nextRd in nextRefs)
    //            {
    //                Console.WriteLine("+ {0}: {1}, {2}, {3}", nextRd.DisplayName, nextRd.BrowseName, nextRd.NodeClass, nextRd.NodeId);
    //            }
    //        }

    //        Console.WriteLine("Step 4 - Create a subscription. Set a faster publishing interval if you wish.");
    //        //var val = session.ReadValue(NodeId.Parse("ns=2;i=2"));
    //        //print(val);
    //        var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 1000 };
    //        Console.WriteLine("Step 5 - Add a list of items you wish to monitor to the subscription.");
    //        NodeId nodeId = new NodeId("ns=2;i=2");
    //        //var list = new List<MonitoredItem> { new MonitoredItem(subscription.DefaultItem) { DisplayName = @"crane", StartNodeId = /*NodeId.Parse(*/@"ns=2;i=2" } };
    //        var test = new MonitoredItem(subscription.DefaultItem) {DisplayName = "test"};
    //        //list.ForEach(i => i.Notification += OnNotification);
    //        test.Notification += OnNotification;
    //        subscription.AddItem(test);
    //        Console.WriteLine("Step 6 - Add the subscription to the session.");
    //        session.AddSubscription(subscription);
    //        subscription.Create();
    //        subscription.ApplyChanges();
    //        //Console.WriteLine("Press any key to remove subscription...");
    //        //Console.ReadKey(true);

    //    }

    //    //Console.WriteLine("Press any key to exit...");
    //    //Console.ReadKey(true);
    //}
    //private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    //{
    //    foreach (var value in item.DequeueValues())
    //    {
    //        Debug.LogFormat("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
    //    }
    //}
    //Update is called once per frame

}
