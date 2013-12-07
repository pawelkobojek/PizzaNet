using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon
{
    public abstract class ServiceProxyBase<T> : IDisposable where T : class
    {
        private readonly string endPointAddress;
        private readonly object _syncRoot = new object();
        private IChannelFactory<T> channelFactory;
        private T channel;
        private bool disposed = false;

        protected ServiceProxyBase(string address)
        {
            endPointAddress = address;
        }

        protected T Channel
        {
            get
            {
                Initialize();
                return channel;
            }
        }

        protected void CloseChannel()
        {
            if (channel != null)
            {
                ((ICommunicationObject)channel).Close();
            }
        }

        private void Initialize()
        {
            lock (_syncRoot)
            {
                if (channel != null)
                    return;

                var endPoint = new EndpointAddress(endPointAddress);
                channelFactory = new ChannelFactory<T>(new BasicHttpBinding());
                channel = channelFactory.CreateChannel(endPoint);
            }
        }

        ~ServiceProxyBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposeManaged)
        {
            if (disposed) return;

            if (disposeManaged)
            {
                lock (_syncRoot)
                {
                    CloseChannel();

                    if (channelFactory != null)
                    {
                        ((IDisposable)channelFactory).Dispose();
                    }
                    channelFactory = null;
                    channel = null;
                }
            }
            disposed = true;
        }
    }
}
