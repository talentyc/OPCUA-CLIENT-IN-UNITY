using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Opc.Ua;
using Opc.Ua.Client;
using System.Threading.Tasks;
public class OPCUA : MonoBehaviour
{
    private ApplicationConfiguration config;
    public double dataFromServer;
    public Session session;
    //public string url = "opc.tcp://localhost:48484/";


    // Start is called before the first frame update
    void Start()
    {
        Display();
    }
    public async void Init()
    {
        config = new ApplicationConfiguration()
        {
            ApplicationName = "Test-Client",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration { ApplicationCertificate = new CertificateIdentifier() },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        };

        await config.Validate(ApplicationType.Client);
        if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
        {
            config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
        }
    }
    public async Task<ServerNode> Fetchdata(string nodeId)
    {
        Init();
        using (session = await Session.Create(config, new ConfiguredEndpoint(null, new EndpointDescription("opc.tcp://127.0.0.1:48484/OPCUA/SimulationServer")), true, "", 6000, null, null))
        {
            var val = session.ReadValue(NodeId.Parse("ns=2;i=2"));              //"ns=2;i=1" convert to NodeId, read info from that NodeId
            double value = Math.Round(Convert.ToDouble(val.ToString()), 3);     //handle with the read value
            string name = Convert.ToString(session.ReadNode(NodeId.Parse(nodeId)));
            ServerNode serverNode = new ServerNode(name, value);
            return serverNode;
            //return referenceDescriptions;
        }
    }
    public class ServerNode
    {
        private string nodename;
        private double nodeValue;

        public ServerNode(string name, double value)
        {
            this.nodename = name;
            this.nodeValue = value;
        }
        public string Nodename
        {
            get { return nodename; }
        }

        public double Nodevalue
        {
            get { return nodeValue; }
        }
    }
    async void Display()
    {
        string nodeId = "ns=2;i=2";
        ServerNode node = await Fetchdata(nodeId);
        Debug.Log(node.Nodename.ToString() + ": " + node.Nodevalue.ToString());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
