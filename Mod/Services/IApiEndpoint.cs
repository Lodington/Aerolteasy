using System.Net;

namespace RoR2DevTool.Services
{
    public interface IApiEndpoint
    {
        string Path { get; }
        void HandleRequest(HttpListenerRequest request, HttpListenerResponse response);
    }
}
