using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.WiFi;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;

namespace WiFiCommunication.Common
{
    public static class WiFiCommunicationHelper
    {
        public const string DefaultSsid = "Dawid-WiFi";
        public const string DefaultPassword = "P@ssw0rD";

        public const string Rpi2HostName = "Dawid-RPi2";
        public const int DefaultPort = 9090;

        public static async Task<WiFiConnectionStatus> ConnectToWiFiNetwork(string ssid = DefaultSsid, string password = DefaultPassword)
        {
            var connectionStatus = WiFiConnectionStatus.NetworkNotAvailable;

            // SSID 및 비밀번호 유효성 확인
            if (!string.IsNullOrEmpty(ssid) && !string.IsNullOrEmpty(password))
            {
                // 앱이 와이파이 기능에 액세스할 수 있는지 확인
                var hasAccess = await WiFiAdapter.RequestAccessAsync();

                // 액세스 가능하면, 첫 번째 와이파이 어댑터를 사용해 사용 가능한 네트워크 검색
                if (hasAccess == WiFiAccessStatus.Allowed)
                {
                    // 사용 가능한 첫 번째 와이파이 어댑터 획득
                    var wiFiAdapters = await WiFiAdapter.FindAllAdaptersAsync();
                    var firstWiFiAdapterAvailable = wiFiAdapters.FirstOrDefault();

                    if (firstWiFiAdapterAvailable != null)
                    {
                        // 네트워크 검색
                        await firstWiFiAdapterAvailable.ScanAsync();

                        // 사용 가능한 네트워크 목록을 SSID로 필터링
                        var wiFiNetwork = firstWiFiAdapterAvailable.NetworkReport.
                            AvailableNetworks.Where(network => network.Ssid == ssid).FirstOrDefault();

                        if (wiFiNetwork != null)
                        {
                            // 제공된 암호를 사용해 네트워크에 연결 시도
                            var passwordCredential = new PasswordCredential()
                            {
                                Password = password
                            };

                            var connectionResult = await firstWiFiAdapterAvailable.
                                ConnectAsync(wiFiNetwork, 
                                WiFiReconnectionKind.Automatic, passwordCredential);

                            // 연결 상태 반환
                            connectionStatus = connectionResult.ConnectionStatus;
                        }
                    }
                }
            }

            return connectionStatus;
        }

public static async Task<StreamSocket> ConnectToHost(string hostName = Rpi2HostName, int port = DefaultPort)
{
    var socket = new StreamSocket();

    await socket.ConnectAsync(new HostName(hostName), port.ToString());

    return socket;
}
    }
}
