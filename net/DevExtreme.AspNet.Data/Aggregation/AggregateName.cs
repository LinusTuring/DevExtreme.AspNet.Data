using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Aggregation {

    static class AggregateName {
        public const string
            MIN = "min",
            MAX = "max",
            SUM = "sum",
            AVG = "avg",
            COUNT = "count";

        public const string
            SUM_NOT_NULL = "snn",
            REMOTE_COUNT = "remoteCount",
            REMOTE_AVG = "remoteAvg";
    }

}
