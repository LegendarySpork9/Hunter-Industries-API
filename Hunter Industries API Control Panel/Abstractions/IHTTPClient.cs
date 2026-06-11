// Copyright © 11/06/2026 Toby Hunter
namespace HunterIndustriesAPIControlPanel.Abstractions
{
    /// <summary>
    /// Interface for the HTTP Client.
    /// </summary>
    public interface IHTTPClient
    {
        Task<HttpResponseMessage?> Send(HttpRequestMessage request);
    }
}
