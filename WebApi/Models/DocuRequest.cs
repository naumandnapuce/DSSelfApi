using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NISA.DSSelfAPI.WebApi.Models
{
    public class DocuRequest
    {
        public string ProviderName; // = "eToken Base Cryptographic Provider";
        public string PinCode; //= "Akshi@123";
        public string timestampServer; //=http://134.0.36.2:8081/
        public byte[] BasePdf;
        public byte[] SignImage;
        public DocuRequest()
        {

        }
    }
}
