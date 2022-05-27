using System.Linq.Expressions;
using System.Reflection;

namespace AssemblerInterpreter.Common
{
  internal static class DelegateBuilder
  {
    public static T BuildDelegate<I, T>(I instance, MethodInfo method, params object[] missingParamValues)
    {
      var queueMissingParams = new Queue<object>(missingParamValues);

      var invokeMethodInfo = typeof(T).GetMethod("Invoke");

      if (invokeMethodInfo == null)
      {
        var typeName = typeof(T).FullName;
        throw new TypeInitializationException(typeName, new InvalidOperationException($"Type {typeName} hasnt 'Invoke' method."));
      }

      var methodReturnType = invokeMethodInfo.ReturnType;
      var methodParameters = invokeMethodInfo.GetParameters();

      var paramsOfDelegate = methodParameters
          .Select(tp => Expression.Parameter(tp.ParameterType, tp.Name))
          .ToArray();

      var methodParams = method.GetParameters();

      Expression<T> expression;
      Expression[]? parametersToPass;

      if (method.DeclaringType == null)
      {
        throw new InvalidOperationException($"Method '{method.Name}' hasnt declaring type.");
      }

      var parameterInstance = Expression.Constant(instance);

      parametersToPass = methodParams
          .Select((parameter, index) => CreateParam(paramsOfDelegate, index, parameter, queueMissingParams))
          .ToArray();

      expression = Expression.Lambda<T>
      (
        Expression.Call(parameterInstance, method, parametersToPass),
        paramsOfDelegate
      );

      return expression.Compile();
    }

    public static T BuildDelegate<T>(MethodInfo method, params object[] missingParamValues)
    {
      var queueMissingParams = new Queue<object>(missingParamValues);

      var invokeMethodInfo = typeof(T).GetMethod("Invoke");

      if (invokeMethodInfo == null)
      {
        var typeName = typeof(T).FullName;
        throw new TypeInitializationException(typeName, new InvalidOperationException($"Type {typeName} hasnt 'Invoke' method."));
      }

      var methodReturnType = invokeMethodInfo.ReturnType;
      var methodParameters = invokeMethodInfo.GetParameters();

      var paramsOfDelegate = methodParameters
          .Select(tp => Expression.Parameter(tp.ParameterType, tp.Name))
          .ToArray();

      var methodParams = method.GetParameters();

      Expression<T> expression;
      Expression[]? parametersToPass;

      if (!method.IsStatic)
      {
        throw new InvalidOperationException($"Method '{method.Name}' is not static.");
      }

      parametersToPass = methodParams
          .Select((p, i) => CreateParam(paramsOfDelegate, i, p, queueMissingParams))
          .ToArray();

      expression = Expression.Lambda<T>
      (
        Expression.Call(method, parametersToPass),
        paramsOfDelegate
      );

      return expression.Compile();
    }

    private static Expression CreateParam(ParameterExpression[] paramsOfDelegate, int i, ParameterInfo callParamType, Queue<object> queueMissingParams)
    {
      if (i < paramsOfDelegate.Length)
        return Expression.Convert(paramsOfDelegate[i], callParamType.ParameterType);

      if (queueMissingParams.Count > 0)
        return Expression.Constant(queueMissingParams.Dequeue());

      if (callParamType.ParameterType.IsValueType)
        return Expression.Constant(Activator.CreateInstance(callParamType.ParameterType));

      return Expression.Constant(null);
    }
  }
}
