using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NISA.DSSelfAPI.WebApi.Models
{
    public class DocuRequest
    {
        public string ProviderName; // = "eToken Base Cryptographic Provider";
        public string PinCode; 
        public string timestampServer; 
        public byte[] BasePdf;
        public byte[] SignImage;
        public DocuRequest()
        {

        }
    }
}
