using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Demo.Observers
{
    public abstract class DiagnosticObserverBase : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        protected abstract bool IsMatch(string name);

        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
        {
            if (IsMatch(diagnosticListener.Name))
            {
                _subscriptions.Add(
                    diagnosticListener.Subscribe(this));

                _subscriptions.Add(
                    diagnosticListener.SubscribeWithAdapter(this));
            }
        }

        void IObserver<DiagnosticListener>.OnError(Exception error)
        { }

        void IObserver<DiagnosticListener>.OnCompleted()
        {
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }

        void IObserver<KeyValuePair<string, object>>.OnNext(KeyValuePair<string, object> pair)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine(pair.Key + ":");

            var valueType = pair.Value.GetType();
            var valueProperties = valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in valueProperties)
            {
                var propertyValue = property.GetValue(pair.Value);
                sb.AppendLine($"\t{property.Name}: {propertyValue}");
            }

            sb.AppendLine();

            Console.WriteLine(sb.ToString());
        }

        void IObserver<KeyValuePair<string, object>>.OnError(Exception error)
        { }

        void IObserver<KeyValuePair<string, object>>.OnCompleted()
        { }
    }
}
