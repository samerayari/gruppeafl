using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem;

public class Robot(string ipAddress = "localhost", int dashboardPort = 29999, int urscriptPort = 30002)
{
    private readonly TcpClient _clientDashboard = new();
    private readonly TcpClient _clientUrscript = new();
    private Stream _streamDashboard;
    private StreamReader _streamReaderDashboard;
    private Stream _streamUrscript;

    public bool ProgramRunning
    {
        get
        {
            if (_clientDashboard.Connected)
            {
                SendDashboard("running\n");
                return ReadLineDashboard() == "Program running: true";
            }

            return false;
        }
    }

    public bool Connected => _clientDashboard.Connected && _clientUrscript.Connected;

    public string RobotMode
    {
        get
        {
            SendDashboard("robotmode\n");
            return ReadLineDashboard();
        }
    }

    public void Connect()
    {
        _clientDashboard.Connect(ipAddress, dashboardPort);
        _streamDashboard = _clientDashboard.GetStream();
        _streamReaderDashboard = new StreamReader(_streamDashboard, Encoding.ASCII);

        
        ReadLineDashboard();

        _clientUrscript.Connect(ipAddress, urscriptPort);
        _streamUrscript = _clientUrscript.GetStream();
    }


    public async void PowerOn()
    {
        SendDashboard("power on\n");
        ReadLineDashboard(); 
        while (RobotMode != "Robotmode: IDLE") await Task.Delay(1000);
    }

    public async void BrakeRelease()
    {
        SendDashboard("brake release\n");
        ReadLineDashboard(); 
        while (RobotMode != "Robotmode: RUNNING") await Task.Delay(1000);
    }


    public void Disconnect()
    {
        _clientDashboard.Close();
        _clientUrscript.Close();
    }


    public void SendDashboard(string command)
    {
        _streamDashboard.Write(Encoding.ASCII.GetBytes(command));
    }

    public void SendUrscript(string program)
    {
        _streamUrscript.Write(Encoding.ASCII.GetBytes(program));
    }

    public void SendUrscriptFile(string path)
    {
        var program = File.ReadAllText(path) + Environment.NewLine;
        SendUrscript(program);
    }

    public string ReadLineDashboard()
    {
        return _streamReaderDashboard.ReadLine();
    }
}