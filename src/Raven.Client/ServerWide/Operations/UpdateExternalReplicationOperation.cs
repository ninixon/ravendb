﻿using System;
using System.Net.Http;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Operations;
using Raven.Client.Http;
using Raven.Client.Json;
using Raven.Client.Json.Converters;
using Sparrow.Json;
using Sparrow.Json.Parsing;

namespace Raven.Client.ServerWide.Operations
{
    public class UpdateExternalReplicationOperation : IMaintenanceOperation<ModifyOngoingTaskResult>
    {
        private readonly ExternalReplication _newWatcher;

        public UpdateExternalReplicationOperation(ExternalReplication newWatcher)
        {
            _newWatcher = newWatcher;
        }

        public RavenCommand<ModifyOngoingTaskResult> GetCommand(DocumentConventions conventions, JsonOperationContext ctx)
        {
            return new UpdateExternalReplication(_newWatcher);
        }

        private class UpdateExternalReplication : RavenCommand<ModifyOngoingTaskResult>
        {
            private readonly ExternalReplication _newWatcher;

            public UpdateExternalReplication(ExternalReplication newWatcher)
            {
                _newWatcher = newWatcher;
            }

            public override HttpRequestMessage CreateRequest(JsonOperationContext ctx, ServerNode node, out string url)
            {
                url = $"{node.Url}/databases/{node.Database}/admin/tasks/external-replication";

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new BlittableJsonContent(stream =>
                    {
                        var json = new DynamicJsonValue
                        {
                            ["Watcher"] = _newWatcher.ToJson()
                        };

                        ctx.Write(stream, ctx.ReadObject(json, "update-replication"));
                    })
                };

                return request;
            }

            public override void SetResponse(JsonOperationContext context, BlittableJsonReaderObject response, bool fromCache)
            {
                if (response == null)
                    ThrowInvalidResponse();

                Result = JsonDeserializationClient.ModifyOngoingTaskResult(response);
            }

            public override bool IsReadRequest => false;
        }
    }

}