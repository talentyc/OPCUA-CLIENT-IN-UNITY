from random import randint
import time
import datetime
from random import randint
from opcua import Server,ua
if __name__ == "__main__":

    def add_variable(object, node, node_name, node_value):
        n = object.add_variable(node, node_name, node_value)
        n.set_writable()

    server = Server()
    url="opc.tcp://127.0.0.1:48484/OPCUA/SimulationServer"
    #url="opc.tcp://localhost:48484/"
    server.set_endpoint(url)
    server.set_server_name("OPC UA Server")
    server.set_security_policy([
                ua.SecurityPolicyType.NoSecurity,
                ua.SecurityPolicyType.Basic256Sha256_SignAndEncrypt,
                ua.SecurityPolicyType.Basic256Sha256_Sign])
    policyIDs =   [
                    "Anonymous", "Basic256Sha256", "Username"
                ]
    server.set_security_IDs(policyIDs)
    # server.load_certificate(r"C:\Users\Administrator\Desktop\OPCUA+MQTT\OPCUA\certificate-3072-example.der")
    # server.load_private_key(r"C:\Users\Administrator\Desktop\OPCUA+MQTT\OPCUA\private-key-3072-example.pem")
    # server.private_key=None
    # server.certificate=None
    objs = server.get_objects_node()

    name="ns=7;s=SCF"
    addspace=server.register_namespace(name)

    obj = objs.add_object(addspace, "Crane")  #ns=7;s=SCF address space
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Watchdog", "Watchdog", int(0))
    # add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.AccessCode", "AccessCode", int(0))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Hoist.Up", "Hoist.Up", bool(False))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Hoist.Down", "Hoist.Down", bool(False))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Hoist.Speed", "Hoist.Speed", float(0.0))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Trolley.Forward", "Trolley.Forward", bool(False))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Trolley.Backward", "Trolley.Backward", bool(False))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Trolley.Speed", "Trolley.Speed", float(0.0))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Bridge.Forward", "Bridge.Forward", bool(False))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Bridge.Backward", "Bridge.Backward", bool(False))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Controls.Bridge.Speed", "Bridge.Speed", float(0.0))

    #obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Controls.TargetPositioning.SelectionInUse", "SelectionInUse", bool(False))
    #obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Controls.TargetPositioning.DriveToTarget", "DriveToTarget", bool(False))
    #obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Controls.TargetPositioning.DriveToHome", "DriveToHome", bool(False))
    #obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Controls.TargetPositioning.Target", "Target", int(0))
    #obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Controls.TargetPositioning.Home", "Home", int(0))

    #obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Status.ReadingControls", "ReadingControls", bool(False))
    #obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Status.WatchDogFault", "WatchDogFault", bool(False))

    obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.Inching", "Inching", bool(False))
    obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.MicroSpeed", "MicroSpeed", bool(False))
    obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.RopeAngleFeaturesBypass", "RopeAngleFeaturesBypass", bool(False))
    obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.SwayControl", "SwayControl", bool(False))
    obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.RadioSelection.SwayControl_SlingLength_mm", "SwayControl_SlingLength_mm", int(0))


    Diagnostic=obj.add_variable("ns=7;s=SCF.PLC.DX_Custom_V.Status.Hoist.Diagnostics.Ok", "Diagnostics.Ok", bool(False))
    #Lisää loput


    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Status.Hoist.Position.Position_m", "HoistPosition", float(0.0))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Status.Bridge.Position.Position_m", "BridgePosition", float(0.0))
    add_variable(obj, "ns=7;s=SCF.PLC.DX_Custom_V.Status.Trolley.Position.Position_m", "TrolleyPosition", float(0.0))
    

    #Test value
    test=obj.add_variable(addspace,"test",0)
    test.set_writable()
    
    server.start()
    print("Server started at {}".format(url))
    
    
    index=0
    while True:
        index=index+1
        test_data= 10  #randint(10,50)
        TIME=datetime.datetime.now().isoformat()
        print(index,TIME,test_data)
        test.set_value(test_data)
        time.sleep(1)
    #     if index==20:
    #         break
    # server.stop()