using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace Gauniv.Client.Services
{
    internal partial class NetworkService : ObservableObject
    {

        public static NetworkService Instance { get; private set; } = new NetworkService();
        [ObservableProperty]
        private string token;
        public HttpClient httpClient;

        public NetworkService() {
            httpClient = new HttpClient();
            Token = null;
        }
        partial void OnTokenChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", value);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }


        public event Action OnConnected;

    }
}
