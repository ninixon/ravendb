// -----------------------------------------------------------------------
//  <copyright file="RavenDB_3237.cs" company="Hibernating Rhinos LTD">
//      Copyright (c) Hibernating Rhinos LTD. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FastTests;
using Raven.Client.Documents.Operations;
using Raven.Server.Documents.Indexes.Static;
using Xunit;

namespace SlowTests.Issues
{
    public class RavenDB_3237 : RavenTestBase
    {
        [Fact]
        public void CaseOne()
        {
            using (var store = GetDocumentStore())
            {
                using (var commands = store.Commands())
                {
                    commands
                        .Put("keys/1", null, new { Test = new[] { 7 } });

                    store
                        .Operations
                        .Send(new PatchOperation("keys/1", null, new PatchRequest { Script = "var a = 1;" }));

                    dynamic doc = commands.Get("keys/1");

                    Assert.NotNull(doc);

                    dynamic test = (DynamicArray)doc.Test;

                    Assert.Equal(1, test.Length);
                    Assert.Equal(7, test[0]);
                }
            }
        }

        [Fact]
        public void CaseTwo()
        {
            using (var store = GetDocumentStore())
            {
                using (var commands = store.Commands())
                {
                    commands
                        .Put("keys/1", null, new { Test = new[] { 3 }, Test2 = new[] { 7 } });

                    store
                        .Operations
                        .Send(new PatchOperation("keys/1", null, new PatchRequest { Script = "this.Test.push(4);" }));

                    dynamic doc = commands.Get("keys/1");

                    Assert.NotNull(doc);

                    dynamic test = (DynamicArray)doc.Test;

                    Assert.Equal(2, test.Length);
                    Assert.Equal(3, test[0]);
                    Assert.Equal(4, test[1]);

                    dynamic test2 = (DynamicArray)doc.Test2;

                    Assert.Equal(1, test2.Length);
                    Assert.Equal(7, test2[0]);
                }
            }
        }

        [Fact]
        public void CaseThree()
        {
            using (var store = GetDocumentStore())
            {
                using (var commands = store.Commands())
                {
                    commands
                        .Put("keys/1", null, new { Test = new[] { 3 }, Test2 = new[] { "7" } });

                    store
                        .Operations
                        .Send(new PatchOperation("keys/1", null, new PatchRequest { Script = "this.Test.push(4);" }));

                    dynamic doc = commands.Get("keys/1");

                    Assert.NotNull(doc);

                    dynamic test = (DynamicArray)doc.Test;

                    Assert.Equal(2, test.Length);
                    Assert.Equal(3, test[0]);
                    Assert.Equal(4, test[1]);

                    dynamic test2 = (DynamicArray)doc.Test2;

                    Assert.Equal(1, test2.Length);
                    Assert.Equal("7", test2[0]);
                }
            }
        }
    }
}
