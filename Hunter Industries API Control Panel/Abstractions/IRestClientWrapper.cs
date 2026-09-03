// Copyright © - Unpublished - Toby Hunter
using RestSharp;

namespace HunterIndustriesAPIControlPanel.Abstractions
{
    public interface IRestClientWrapper
    {
        Task<RestResponse> ExecuteAsync(string url, RestRequest request);
    }
}
