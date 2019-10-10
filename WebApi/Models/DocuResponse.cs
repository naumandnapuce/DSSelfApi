using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NISA.DSSelfAPI.WebApi.Models
{
    public class DocuResponse
    {
        public bool HasError;
        public string ErrorMessage;
        public string StackTrace;
        public byte[] SignedPdf;
        public DocuResponse()
        {
            HasError = false;
            ErrorMessage = "";
            StackTrace = "";
        }
    }
}
