using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PizzaNetCommon.Requests
{
    [DataContract]
    public class PizzaServiceFault
    {
        public const string PASSWORD_EMPTY = "Password must not be empty";
        public const string EMAIL_EMPTY = "Email must not be empty";
        public const string ADDRESS_EMPTY = "Address must not be empty";
        public const string EMAIL_ALREADY_REGISTERED_FORMAT = "Email {0} already registered";

        public PizzaServiceFault(string reason)
        {
            Reason = reason;
        }

        [DataMember]
        public string Reason { get; set; }

        public static FaultException<PizzaServiceFault> Create(string reason)
        {
            return new FaultException<PizzaServiceFault>(new PizzaServiceFault(reason));
        }
    }
}
