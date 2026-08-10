// Copyright © Unpublished - Toby Hunter
using HunterIndustriesAPIControlPanel.Models.Responses;
using HunterIndustriesAPIControlPanel.Services;

namespace HunterIndustriesAPIControlPanel.Functions
{
    public static class UserFunction
    {
        /// <summary>
        /// Returns all users from the API.
        /// </summary>
        public static async Task<List<UserModel>> GetUsers(APIService apiService)
        {
            List<UserModel> users = [];

            bool nextPage = true;
            int pageNumber = 1;

            while (nextPage)
            {
                PagedAPIResponseModel<UserModel>? pagedUsers = await apiService.GetUsers(
                    true,
                    200,
                    pageNumber);

                if (pagedUsers != null && pagedUsers.EntryCount > 0)
                {
                    users.AddRange(pagedUsers.Entries);

                    if (pageNumber < pagedUsers.TotalPageCount)
                    {
                        pageNumber++;
                    }

                    else
                    {
                        nextPage = false;
                    }
                }

                else
                {
                    nextPage = false;
                }
            }

            return users;
        }
    }
}
