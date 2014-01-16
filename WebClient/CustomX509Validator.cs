using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaWebClient
{
    public class CustomX509Validator : X509CertificateValidator
    {
        public override void Validate(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate)
        {
            //if (certificate == null || certificate.SubjectName.Name != "CN=MyServerCert")
            //{
            //    throw new SecurityTokenValidationException("Certificate validation error");
            //}
        }
    }
}
