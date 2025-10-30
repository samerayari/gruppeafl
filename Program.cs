using System.Net.Sockets;
using System.Text;

const string program = @"
def f():
  global RPC = rpc_factory(""xmlrpc"", ""http://localhost:41414"")

  global TOOL_INDEX = 0
  

  def rg_is_busy():
    return RPC.rg_get_busy(TOOL_INDEX)
  end

 
  def rg_grip(width, force = 10):
    RPC.rg_grip(TOOL_INDEX, width + .0, force + .0)
  

    sleep(0.01) 
    while (rg_is_busy()):
    end
  end

  p1 = p[.3, -.3, .1, 0, -3.1415, 0]
  p2 = p[.2, -.3, .1, 0, -3.1415, 0]
  times = 0
  while (times < 3):
    movej(get_inverse_kin(p1))
    movej(get_inverse_kin(p2))
    times = times + 1
  end

  rg_grip(0)
  rg_grip(50)
  rg_grip(0)
end
";

const int urscriptPort = 30002, dashboardPort = 29999;
const string IpAddress = "172.20.254.205";

void SendString(string host, int port, string message)
{
    using var client = new TcpClient(host, port);
    using var stream = client.GetStream();
    stream.Write(Encoding.ASCII.GetBytes(message));
}

SendString(IpAddress, dashboardPort, "brake release\n");
SendString(IpAddress, urscriptPort, program);
