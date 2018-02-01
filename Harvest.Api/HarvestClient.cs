﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harvest.Api
{
    public class HarvestClient : IDisposable
    {
        #region Members
        private HttpClient _client;
        private RequestBuilder _requestBuilder = new RequestBuilder();
        #endregion

        #region Properties
        public long? DefaultAccountId { get; set; }
        public string Token { get; }
        public string UserAgent { get; set; }
        #endregion

        #region Constructor
        public HarvestClient(string token, string tokenType = "Bearer", HttpClientHandler httpClientHandler = null)
        {
            this.Token = token ?? throw new ArgumentNullException(nameof(token));

            _client = new HttpClient(httpClientHandler ?? new HttpClientHandler());
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(RequestBuilder.JsonMimeType));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, token);
        }
        #endregion

        #region API methods
        public Task<AccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin("https://id.getharvest.com/api/v1/accounts")
                .SendAsync<AccountsResponse>(_client, cancellationToken);
        }

        public Task<TimeEntriesResponse> GetTimeEntriesAsync(long? userId = null, long? clientId = null, long? projectId = null, bool? isBilled = null,
            DateTime? updatedSince = null, DateTime? fromDate = null, DateTime? toDate = null, int? page = null, int? perPage = null, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin("https://api.harvestapp.com/v2/time_entries")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .Query("user_id", userId)
                .Query("client_id", clientId)
                .Query("project_id", projectId)
                .Query("is_billed", isBilled)
                .Query("updated_since", updatedSince)
                .Query("from", fromDate)
                .Query("to", toDate)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<TimeEntriesResponse>(_client, cancellationToken);
        }

        public Task<TimeEntry> GetTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin($"https://api.harvestapp.com/v2/time_entries/{entryId}")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .SendAsync<TimeEntry>(_client, cancellationToken);
        }

        public Task<TimeEntry> RestartTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin(RequestBuilder.PatchMethod, $"https://api.harvestapp.com/v2/time_entries/{entryId}/restart")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .SendAsync<TimeEntry>(_client, cancellationToken);
        }

        public Task<TimeEntry> StopTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin(RequestBuilder.PatchMethod, $"https://api.harvestapp.com/v2/time_entries/{entryId}/stop")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .SendAsync<TimeEntry>(_client, cancellationToken);
        }

        public System.Threading.Tasks.Task DeleteTimeEntryAsync(long entryId, long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin(HttpMethod.Delete, $"https://api.harvestapp.com/v2/time_entries/{entryId}")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .SendAsync(_client, cancellationToken);
        }

        public Task<TimeEntry> CreateTimeEntryAsync(long projectId, long taskId, DateTime spentDate,
            TimeSpan? startedTime = null, TimeSpan? endedTime = null, decimal? hours = null, string notes = null, ExternalReference externalReference = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin(HttpMethod.Post, $"https://api.harvestapp.com/v2/time_entries")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .Form("project_id", projectId)
                .Form("task_id", taskId)
                .Form("spent_date", spentDate)
                .Form("started_time", startedTime)
                .Form("ended_time", endedTime)
                .Form("hours", hours)
                .Form("notes", notes)
                .Form("external_reference", externalReference)
                .SendAsync<TimeEntry>(_client, cancellationToken);
        }

        public Task<TimeEntry> UpdateTimeEntryAsync(long entryId,
            long? projectId = null, long? taskId = null, DateTime? spentDate = null, TimeSpan? startedTime = null, TimeSpan? endedTime = null,
            decimal? hours = null, string notes = null, ExternalReference externalReference = null,
            long? accountId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _requestBuilder
                .Begin(RequestBuilder.PatchMethod, $"https://api.harvestapp.com/v2/time_entries/{entryId}")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .Form("project_id", projectId)
                .Form("task_id", taskId)
                .Form("spent_date", spentDate)
                .Form("started_time", startedTime)
                .Form("ended_time", endedTime)
                .Form("hours", hours)
                .Form("notes", notes)
                .Form("external_reference", externalReference)
                .SendAsync<TimeEntry>(_client, cancellationToken);
        }

        public Task<ProjectAssignmentsResponse> GetProjectAssignmentsAsync(long? userId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null, long? accountId = null)
        {
            var userIdOrMe = userId.HasValue ? userId.ToString() : "me";

            return _requestBuilder
                .Begin($"https://api.harvestapp.com/v2/users/{userIdOrMe}/project_assignments")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .Query("updated_since", updatedSince)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<ProjectAssignmentsResponse>(_client, CancellationToken.None);
        }

        public Task<ProjectsResponse> GetProjectsAsync(long? clientId = null, DateTime? updatedSince = null, int? page = null, int? perPage = null, long? accountId = null)
        {
            return _requestBuilder
                .Begin("https://api.harvestapp.com/v2/projects")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .Query("client_id", clientId)
                .Query("updated_since", updatedSince)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<ProjectsResponse>(_client, CancellationToken.None);
        }

        public Task<TasksResponse> GetTasksAsync(DateTime? updatedSince = null, int? page = null, int? perPage = null, long? accountId = null)
        {
            return _requestBuilder
                .Begin("https://api.harvestapp.com/v2/tasks")
                .AccountId(accountId ?? this.DefaultAccountId)
                .UserAgent(this.UserAgent)
                .Query("updated_since", updatedSince)
                .Query("page", page)
                .Query("per_page", perPage)
                .SendAsync<TasksResponse>(_client, CancellationToken.None);
        }
        #endregion

        #region Implementation
        public void Dispose()
        {
            _client.Dispose();
        }
        #endregion
    }
}