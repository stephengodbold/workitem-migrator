using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WorkItemMigrator.Migration.Models;

namespace WorkItemMigrator.Migration
{
    public class SearchHub
    {
        private readonly IEnumerable<IRepository> repositories;

        public SearchHub(IEnumerable<IRepository> repositories)
        {
            this.repositories = repositories;
        }

        public IEnumerable<WorkItem> Search(string criteria)
        {
            int identifier;

            var tasks = int.TryParse(criteria, out identifier) ? Fetch(criteria) : Query(criteria);
            var taskArray = tasks.ToArray();

            try
            {
                Task.WaitAll(taskArray);
            }
            catch (AggregateException) { } //not ideal, but we'd like to return results for all repositories that complete

            var results = taskArray.Aggregate(new List<WorkItem>(),
                                          (list, task) =>
                                          {
                                              if (task.Status == TaskStatus.RanToCompletion)
                                              {
                                                  list.AddRange(task.Result.Where(result => result != null));
                                              }

                                              return list;
                                          });

            return results;
        }

        private IEnumerable<Task<WorkItem[]>> Query(string criteria)
        {
            return repositories.Select(provider => Task.Factory.StartNew(() => provider.Search(criteria).ToArray()));
        }

        private IEnumerable<Task<WorkItem[]>> Fetch(string identifier)
        {
            return repositories.Select(provider => Task.Factory.StartNew(() =>
                {
                    var result = provider.Fetch(identifier);
                    return new[] { result };
                }));
        }
    }
}
