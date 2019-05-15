using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using OrlemSoftware.Wpf.Core.Threading;

namespace OrlemSoftware.Wpf.Mvvm.Core.Implementation.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private readonly IUiDispatcherService _dispatcherService;
        protected BaseViewModel(IUiDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (field?.Equals(value) == true)
                return;

            field = value;
            onPropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            onPropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExps = getMemberExpressions(propertyExpression);
            if (!memberExps.Any())
                return;

            var exp = memberExps.First();
            onPropertyChanged(exp.Member.Name);
        }

        protected void SetProperty<T>(Expression<Func<T>> propertyExpression, T value)
        {
            var memberExps = getMemberExpressions(propertyExpression);
            if (!memberExps.Any())
                return;

            var exp = memberExps.First();
            var memberName = exp.Member.Name;
            var propInfo = GetType().GetProperty(memberName);
            if (propInfo == null)
                return;

            if (!propInfo.CanWrite)
                return;

            var currentValue = propInfo.GetValue(this);
            if (currentValue?.Equals(value) == true)
                return;

            propInfo.SetValue(this, value);
            onPropertyChanged(memberName);
        }

        private static IReadOnlyCollection<MemberExpression> getMemberExpressions(Expression expression)
        {
            var retv = new List<MemberExpression>();

            if (expression is MemberExpression memberExpression)
            {
                var inner = getMemberExpressions(memberExpression.Expression);
                retv.AddRange(inner);
                retv.Add(memberExpression);
            }

            if (expression is BlockExpression blockExpression)
            {
                retv.AddRange(blockExpression.Expressions.SelectMany(getMemberExpressions));
            }

            if (expression is BinaryExpression binaryExpression)
            {
                var leftExps = getMemberExpressions(binaryExpression.Left);
                var rightExps = getMemberExpressions(binaryExpression.Right);
                retv.AddRange(leftExps);
                retv.AddRange(rightExps);
            }

            if (expression is UnaryExpression unaryExpression)
            {
                var innerExps = getMemberExpressions(unaryExpression.Operand);
                retv.AddRange(innerExps);
            }

            if (expression is LambdaExpression lambdaExpression)
            {
                var innerExps = getMemberExpressions(lambdaExpression.Body);
                var paramExps = lambdaExpression.Parameters.SelectMany(getMemberExpressions);

                retv.AddRange(paramExps);
                retv.AddRange(innerExps);
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                var argsExps = methodCallExpression.Arguments.SelectMany(getMemberExpressions);
                var innerExps = getMemberExpressions(methodCallExpression.Object);
                retv.AddRange(argsExps);
                retv.AddRange(innerExps);
            }

            return retv;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void onPropertyChanged(string propertyName = null)
        {
            if (checkInvokeRequired())
                _dispatcherService.UiDispatcher.InvokeAsync(() =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                });
            else
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool checkInvokeRequired()
        {
            return _dispatcherService.UiDispatcher.Thread.ManagedThreadId
                   != Thread.CurrentThread.ManagedThreadId;
        }
    }
}