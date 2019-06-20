using DevExtreme.AspNet.Data.RemoteGrouping;
using DevExtreme.AspNet.Data.Types;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder<T> {
        DataSourceLoadContext _context;
        bool _guardNulls;
        AnonTypeNewTweaks _anonTypeNewTweaks;

        public DataSourceExpressionBuilder(DataSourceLoadContext context, bool guardNulls = false, AnonTypeNewTweaks anonTypeNewTweaks = null) {
            _context = context;
            _guardNulls = guardNulls;
            _anonTypeNewTweaks = anonTypeNewTweaks;
        }

        public Expression BuildLoadExpr(Expression source, bool paginate = true, bool isDistinctQuery = false) {
            return BuildCore(source, paginate: paginate, isDistinctQuery: isDistinctQuery);
        }

        public Expression BuildCountExpr(Expression source, bool isDistinctQuery = false) {
            return BuildCore(source, isCountQuery: true, isDistinctQuery: isDistinctQuery);
        }

        public Expression BuildLoadGroupsExpr(Expression source, bool isDistinctQuery = false) {
            return BuildCore(source, isDistinctQuery: isDistinctQuery, remoteGrouping: true);
        }

        Expression BuildCore(Expression expr, bool paginate = false, bool isCountQuery = false, bool isDistinctQuery = false, bool remoteGrouping = false) {
            var queryableType = typeof(Queryable);
            var genericTypeArguments = new[] { typeof(T) };

            if(_context.HasFilter)
                expr = Expression.Call(queryableType, "Where", genericTypeArguments, expr, Expression.Quote(new FilterExpressionCompiler<T>(_guardNulls, _context.UseStringToLower).Compile(_context.Filter)));

            if(!isCountQuery) {
                if(!remoteGrouping) {
                    if(_context.HasAnySort)
                        expr = new SortExpressionCompiler<T>(_guardNulls).Compile(expr, _context.GetFullSort());
                    if(_context.HasAnySelect && _context.UseRemoteSelect) {
                        expr = new SelectExpressionCompiler<T>(_guardNulls).Compile(expr, _context.FullSelect);
                        genericTypeArguments = expr.Type.GetGenericArguments();
                    }
                } else {
                    expr = new RemoteGroupExpressionCompiler<T>(_guardNulls, _anonTypeNewTweaks, _context.Group, _context.TotalSummary, _context.GroupSummary).Compile(expr);
                }

                if(isDistinctQuery)
                    expr = Expression.Call(queryableType, "Distinct", genericTypeArguments, expr);

                if(paginate) {
                    if(_context.Skip > 0)
                        expr = Expression.Call(queryableType, "Skip", genericTypeArguments, expr, Expression.Constant(_context.Skip));

                    if(_context.Take > 0)
                        expr = Expression.Call(queryableType, "Take", genericTypeArguments, expr, Expression.Constant(_context.Take));
                }
            }

            if(isCountQuery) {
                if(isDistinctQuery)
                    expr = Expression.Call(queryableType, "Distinct", genericTypeArguments, expr);

                expr = Expression.Call(queryableType, "Count", genericTypeArguments, expr);
            }

            return expr;
        }
    }

}
