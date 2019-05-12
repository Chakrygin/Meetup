using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Demo.Observers
{
    public abstract class DiagnosticObserverBase : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
    {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        protected abstract bool IsMatch(string name);

        #region IObserver<DiagnosticListener>

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

        #endregion

        #region IObserver<KeyValuePair<string, object>>

        public void OnNext(KeyValuePair<string, object> value)
        {
            Console.WriteLine($"{value.Key}: {value.Value}");
        }

        public void OnError(Exception error)
        { }

        public void OnCompleted()
        { }

        #endregion
    }
}
