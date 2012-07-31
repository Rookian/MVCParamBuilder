using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace MVCParamBuilder
{
    public class ParamBuilder<TModel> : Dictionary<string, object>
    {
        private readonly TModel _model;

        public ParamBuilder(TModel model)
        {
            _model = model;
        }

        public RouteValueDictionary For(params Expression<Func<TModel, object>>[] expressions)
        {
            var parameters = new RouteValueDictionary();
            foreach (var expression in expressions)
            {
                var propertyNames = Reflector.GetPropertyName(expression);
                var value = GetPropertyValueFromPropertyNames(_model, propertyNames);

                if (!parameters.ContainsKey(propertyNames))
                {
                    parameters.Add(propertyNames.Split(new string[]{"."}, StringSplitOptions.None).LastOrDefault(), value);
                }
            }
            return parameters;
        }

        private object GetPropertyValueFromPropertyNames(object instance, string propertyNames)
        {
            object propertyValue = null;
            foreach (var propertyName in propertyNames.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (propertyValue == null)
                {
                    propertyValue = GetPropertyFromPropertyName(instance, propertyName);
                }
                else
                {
                    propertyValue = GetPropertyFromPropertyName(propertyValue, propertyName);
                }
            }

            return propertyValue;
        }

        private object GetPropertyFromPropertyName(object instance, string propertyName)
        {
            return instance.GetType().GetProperty(propertyName).GetValue(instance, null);
        }
    }

    public abstract class ViewPageBase<TModel> : WebViewPage<TModel>
    {
        public ParamBuilder<TModel> Param { get { return new ParamBuilder<TModel>(Model); } }
    }

    public class Reflector
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            var lambdaEx = expression as LambdaExpression;
            if (lambdaEx == null) throw new ArgumentNullException("expression");

            MemberExpression memberExpression = null;

            if (lambdaEx.Body.NodeType == ExpressionType.Convert)
            {
                var unaryExpression = lambdaEx.Body as UnaryExpression;
                if (unaryExpression == null) throw new ArgumentNullException("expression");

                if (unaryExpression.Operand.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
            }
            else if (lambdaEx.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = lambdaEx.Body as MemberExpression;
            }
            else if (lambdaEx.Body.NodeType == ExpressionType.Call)
            {
                var methodCallExpression = lambdaEx.Body as MethodCallExpression;
                if (methodCallExpression != null) return methodCallExpression.Method.Name;
            }

            if (memberExpression == null) throw new ArgumentNullException("expression");
            var lastPropertyName = memberExpression.Member.Name;

            var propertyNames = new List<string>();
            
            if (memberExpression.Expression != null)
            {
                var expr = memberExpression.Expression as MemberExpression;
                propertyNames.Add(expr.Member.Name);
            }
            propertyNames.Add(lastPropertyName);
            return string.Join(".", propertyNames);
        }
    }
}