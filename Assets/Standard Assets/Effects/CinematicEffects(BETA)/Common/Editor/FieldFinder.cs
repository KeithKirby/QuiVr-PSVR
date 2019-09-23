using System;
using System.Linq.Expressions;
using System.Reflection;

namespace UnityStandardAssets.CinematicEffects
{
    public static class FieldFinder<T>
    {
        public static FieldInfo GetField<TValue>(System.Linq.Expressions.Expression<Func<T, TValue>> selector)
        {
            System.Linq.Expressions.Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (FieldInfo)((MemberExpression)body).Member;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
