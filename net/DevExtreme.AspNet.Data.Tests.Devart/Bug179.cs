using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Devart {
    public class Bug179 {

        [Fact]
        public async Task ScenarioNullable() {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder() {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = "|DataDirectory|Database.mdf",
                IntegratedSecurity = true,
            };

            SqlConnection connection = new SqlConnection(connectionStringBuilder.ToString());

            SqlDataContext dataContext = new SqlDataContext(connection);

            string query = @"
SELECT	'A'	AS GroupColumn,	1		AS ValueColumn
UNION ALL
SELECT	'A'	AS GroupColumn,	3		AS ValueColumn
UNION ALL
SELECT	'A'	AS GroupColumn,	NULL	AS ValueColumn
UNION ALL
SELECT	'B'	AS GroupColumn,	5		AS ValueColumn
UNION ALL
SELECT	'B'	AS GroupColumn,	NULL	AS ValueColumn
";

            var queryable = dataContext.Query<QueryTypeNullable>(query);

            var loadResult = DataSourceLoader.Load(queryable, new SampleLoadOptions {
                Group = new[] {
                        new GroupingInfo { Selector = "GroupColumn", IsExpanded = false }
                    },
                GroupSummary = new[] {
                        new SummaryInfo { SummaryType = "avg", Selector = "ValueColumn" },
                        new SummaryInfo { SummaryType = "count" },
                        new SummaryInfo { SummaryType = "sum", Selector = "ValueColumn" }
                    },
                TotalSummary = new[] {
                        new SummaryInfo { SummaryType = "sum", Selector = "ValueColumn" },
                        new SummaryInfo { SummaryType = "count" },
                        new SummaryInfo { SummaryType = "avg", Selector = "ValueColumn" }
                    }
            });

            var loadResultGroups = loadResult.data.Cast<ResponseModel.Group>().ToArray();

            var referenceGroups = queryable
                .GroupBy(i => i.GroupColumn)
                .OrderBy(g => g.Key)
                .Select(g => new {
                    Avg = g.Average(i => i.ValueColumn),
                    Count = g.Count(),
                    Sum = g.Sum(i => i.ValueColumn)
                })
                .ToArray();

            for(var g = 0; g < 2; g++) {
                Assert.Equal((decimal)referenceGroups[g].Avg, loadResultGroups[g].summary[0]);
                Assert.Equal(referenceGroups[g].Count, loadResultGroups[g].summary[1]);
                Assert.Equal((decimal)referenceGroups[g].Sum, loadResultGroups[g].summary[2]);
            }

            Assert.Equal((decimal)queryable.Sum(i => i.ValueColumn), loadResult.summary[0]);
            Assert.Equal(queryable.Count(), loadResult.summary[1]);
            Assert.Equal((decimal)queryable.Average(i => i.ValueColumn), loadResult.summary[2]);
        }

        [Fact]
        public async Task ScenarioNotNullable() {
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder() {
                DataSource = @"(LocalDB)\MSSQLLocalDB",
                AttachDBFilename = "|DataDirectory|Database.mdf",
                IntegratedSecurity = true,
            };

            SqlConnection connection = new SqlConnection(connectionStringBuilder.ToString());

            SqlDataContext dataContext = new SqlDataContext(connection);

            string query = @"
SELECT	'A'	AS GroupColumn,	2		AS ValueColumn
UNION ALL
SELECT	'A'	AS GroupColumn,	6		AS ValueColumn
UNION ALL
SELECT	'A'	AS GroupColumn,	4	AS ValueColumn
UNION ALL
SELECT	'B'	AS GroupColumn,	10		AS ValueColumn
UNION ALL
SELECT	'B'	AS GroupColumn,	8	AS ValueColumn
";

            var queryable = dataContext.Query<QueryTypeNotNullable>(query);

            var loadResult = DataSourceLoader.Load(queryable, new SampleLoadOptions {
                Group = new[] {
                        new GroupingInfo { Selector = "GroupColumn", IsExpanded = false }
                    },
                GroupSummary = new[] {
                        new SummaryInfo { SummaryType = "avg", Selector = "ValueColumn" },
                        new SummaryInfo { SummaryType = "count" },
                        new SummaryInfo { SummaryType = "sum", Selector = "ValueColumn" }
                    },
                TotalSummary = new[] {
                        new SummaryInfo { SummaryType = "sum", Selector = "ValueColumn" },
                        new SummaryInfo { SummaryType = "count" },
                        new SummaryInfo { SummaryType = "avg", Selector = "ValueColumn" }
                    }
            });

            var loadResultGroups = loadResult.data.Cast<ResponseModel.Group>().ToArray();

            var referenceGroups = queryable
                .GroupBy(i => i.GroupColumn)
                .OrderBy(g => g.Key)
                .Select(g => new {
                    Avg = g.Average(i => i.ValueColumn),
                    Count = g.Count(),
                    Sum = g.Sum(i => i.ValueColumn)
                })
                .ToArray();

            for(var g = 0; g < 2; g++) {
                Assert.Equal((decimal)referenceGroups[g].Avg, loadResultGroups[g].summary[0]);
                Assert.Equal(referenceGroups[g].Count, loadResultGroups[g].summary[1]);
                Assert.Equal((decimal)referenceGroups[g].Sum, loadResultGroups[g].summary[2]);
            }

            Assert.Equal((decimal)queryable.Sum(i => i.ValueColumn), loadResult.summary[0]);
            Assert.Equal(queryable.Count(), loadResult.summary[1]);
            Assert.Equal((decimal)queryable.Average(i => i.ValueColumn), loadResult.summary[2]);
        }

    }


}
