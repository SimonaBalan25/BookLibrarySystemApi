﻿

using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookLibrarySystem.Logic.Tests.Factories
{
    // Provides a way to create mock DbSets
    public static class MockDbSetFactory
    {
        // Creates a mock DbSet from the specified data.
        public static Mock<DbSet<T>> Create<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mock;
        }
    }
}
