using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkItemMigrator.Migration;
using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Tests.SearchTests
{
    // ReSharper disable InconsistentNaming
    public static class SearchHubTests
    {
        [TestClass]
        public class When_There_Is_One_Repository
        {
            [TestMethod]
            public void Search_Should_Be_Called()
            {
                const string searchText = "Search Text";
                var repository = new StubRepository
                    {
                        SearchCallback = s =>
                            {
                                Assert.AreEqual(searchText, s);
                                return new[] { new WorkItem() };
                            }
                    };

                var searchHub = new SearchHub(new[] { repository });
                var result = searchHub.Search(searchText);

                Assert.IsTrue(repository.SearchCalled);
                Assert.IsNotNull(result);
            }
        }

        [TestClass]
        public class When_There_Is_More_Than_One_Repository
        {
            [TestMethod]
            public void Results_Are_Aggregated()
            {
                const string searchText = "Search Text";
                var repository = new StubRepository
                {
                    SearchCallback = s =>
                    {
                        Assert.AreEqual(searchText, s);
                        return new[] { new WorkItem { Id = 1 } };
                    }
                };

                var secondRepository = new StubRepository
                {
                    SearchCallback = s =>
                    {
                        Assert.AreEqual(searchText, s);
                        return new[] { new WorkItem { Id = 2 } };
                    }
                };

                var searchHub = new SearchHub(new[] { repository, secondRepository });
                var result = searchHub.Search(searchText);

                Assert.IsTrue(repository.SearchCalled);
                Assert.IsNotNull(result);
                Assert.AreEqual(2, result.Count());
            }

            [TestMethod]
            public void Exceptions_Do_Not_Fail_Entire_Action()
            {
                const string searchText = "Search Text";
                var repository = new StubRepository
                {
                    SearchCallback = s =>
                    {
                        Assert.AreEqual(searchText, s);
                        return new[] { new WorkItem { Id = 1 } };
                    }
                };

                var secondRepository = new StubRepository
                {
                    SearchCallback = s =>
                    {
                        throw new ArgumentOutOfRangeException("s", "Testing exceptions");
                    }
                };

                var searchHub = new SearchHub(new[] { repository, secondRepository });
                var result = searchHub.Search(searchText);

                Assert.IsTrue(repository.SearchCalled);
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count());
            }

            [TestMethod]
            public void Nulls_Do_Not_End_Up_In_Results()
            {
                const string searchText = "Search Text";
                var repository = new StubRepository
                {
                    SearchCallback = s =>
                    {
                        Assert.AreEqual(searchText, s);
                        return new[] { new WorkItem { Id = 1 } };
                    }
                };

                var secondRepository = new StubRepository
                {
                    SearchCallback = s => new WorkItem[] { null }
                };

                var searchHub = new SearchHub(new[] { repository, secondRepository });
                var result = searchHub.Search(searchText).ToArray();

                Assert.IsTrue(repository.SearchCalled);
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(result.First());
            }
        }
    }
    // ReSharper restore InconsistentNaming

    public class StubRepository : IRepository
    {
        public string FriendlyName { get { return "Stub"; } }

        public void ResetFlags()
        {
            FetchCalled = false;
            SearchCalled = false;
            MigrateFromCalled = false;
            MigrateToCalled = false;
        }

        public bool FetchCalled { get; private set; }
        public Func<string, WorkItem> FetchCallback { get; set; }

        public WorkItem Fetch(string itemId)
        {
            FetchCalled = true;
            return FetchCallback(itemId);
        }

        public bool SearchCalled { get; private set; }
        public Func<string, IEnumerable<WorkItem>> SearchCallback { get; set; }

        public IEnumerable<WorkItem> Search(string query)
        {
            SearchCalled = true;
            return SearchCallback(query);
        }

        public bool MigrateFromCalled { get; private set; }
        public Action<string, string, bool, string> MigrateFromCallback { get; set; }

        public void MigrateFrom(string itemId, string migratedId, bool close, string comment)
        {
            MigrateFromCalled = true;
            MigrateFromCallback(itemId, migratedId, close, comment);
        }

        public bool MigrateToCalled { get; private set; }
        public Func<WorkItem, string> MigrateToCallback { get; set; }

        public string MigrateTo(WorkItem item)
        {
            MigrateToCalled = true;
            return MigrateToCallback(item);
        }

    }
}
