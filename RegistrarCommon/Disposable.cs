using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarCommon
{
    public static class Disposable
    {
        private class AnonymousDisposable : IDisposable
        {
            private readonly Action _dispose;
            public AnonymousDisposable(Action dispose)
            {
                _dispose = dispose;
            }

            public void Dispose()
            {
                _dispose?.Invoke();
            }
        }
        public static IDisposable Create(Action dispose)
        {
            return new AnonymousDisposable(dispose);
        }
    }
}
