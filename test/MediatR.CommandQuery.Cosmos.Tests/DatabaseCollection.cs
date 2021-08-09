﻿using Xunit;

namespace MediatR.CommandQuery.Cosmos.Tests
{
    [CollectionDefinition(DatabaseCollection.CollectionName)]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        public const string CollectionName = "DatabaseCollection";
    }
}
