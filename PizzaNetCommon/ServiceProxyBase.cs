using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using PizzaNetCommon.Services;

namespace PizzaNetCommon
{
    public abstract class ServiceProxyBase<T> : IDisposable where T : class
    {
        private readonly object _syncRoot = new object();
        private ChannelFactory<T> channelFactory;
        private T channel;
        private bool disposed = false;

        protected ServiceProxyBase()
        {
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

                EndpointIdentity identity = EndpointIdentity.CreateDnsIdentity("MyServerCert");
                //var endPoint = new EndpointAddress(new Uri(this.endPointAddress), identity);
                channelFactory = new ChannelFactory<T>("PizzaServiceSecure");
                channelFactory.Credentials.UserName.UserName = "UserName";
                channelFactory.Credentials.UserName.Password = "Password";

                channel = channelFactory.CreateChannel();
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
                    var c = channel as IChannel;
                    try
                    {
                        if (c != null)
                        {
                            if (c.State != CommunicationState.Faulted)
                            {
                                c.Close();
                            }
                            else
                            {
                                c.Abort();
                            }
                        }
                    }
                    catch (CommunicationException)
                    {
                        c.Abort();
                    }
                    catch (TimeoutException)
                    {
                        c.Abort();
                    }
                    catch (Exception)
                    {
                        c.Abort();
                        throw;
                    }
                    finally
                    {
                        channel = null;
                    }
                    /*CloseChannel();

                    if (channelFactory != null)
                    {
                        ((IDisposable)channelFactory).Dispose();
                    }
                    channelFactory = null;
                    channel = null;*/
                }
            }
            disposed = true;
        }
    }
}
