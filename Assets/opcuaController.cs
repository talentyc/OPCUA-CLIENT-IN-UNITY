using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

public class opcuaController : MonoBehaviour
{
    public opcuaclient _eventSender;
    private string tagofTheOPCUAReceiver="opcuaclient";
    //public MonitoredItem MonitoredItem;

    public string inching;
    public string microspeed;
    public string ropeanglefeaturesbypass;
    public string swaycontrol;
    public string swaycontrol_slinglength;
    public string hoist_position;
    public string bridge_position;
    public string trolley_position;
    public string test;


    // Start is called before the first frame update
    void Start()
    {
        _eventSender = GameObject.FindGameObjectsWithTag(tagofTheOPCUAReceiver)[0].gameObject.GetComponent<opcuaclient>();
        _eventSender.OnMessageArrived += OnMessageArrivedHandler;
        

    }
    private void OnMessageArrivedHandler(Dictionary<string, string> newMsg)
    {
        //Debug.Log("Event Fired. The message, from Object " + newMsg);
        inching = newMsg["inching"];
        microspeed= newMsg["microspeed"];
        ropeanglefeaturesbypass= newMsg["ropeanglefeaturesbypass"];
        swaycontrol= newMsg["swaycontrol"];
        swaycontrol_slinglength= newMsg["swaycontrol_slinglength"];
        hoist_position= newMsg["hoist_position"];
        bridge_position= newMsg["bridge_position"];
        trolley_position= newMsg["trolley_position"];
        test = newMsg["test"];

        //foreach (KeyValuePair<string, string> pair in newMsg)
        //{
        //    Debug.Log(pair.Key+":"+pair.Value);
        //}
    }
    //private void monitoredItem_Notification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    //{
    //    foreach (var value in item.DequeueValues())
    //    {
    //        Debug.LogFormat("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        
    }
}
