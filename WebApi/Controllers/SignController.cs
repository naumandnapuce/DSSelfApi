using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using NISA.DSSelfAPI.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Http;
using System.Windows.Forms;

namespace NISA.DSSelfAPI.WebApi.Controllers
{
    public class SignController : ApiController
    {

        private MemoryStream signedPdf_mem;
        private PdfStamper pdfStamper;

        [HttpPost]
        public HttpResponseMessage Doc([FromBody] DocuRequest doc)
        {
            DocuResponse DR = new DocuResponse();
            try
            {
               

                if (string.IsNullOrWhiteSpace(doc.PinCode))
                {
                    DR = SignDocumentWithoutPin(doc);

                }
                else
                {
                    DR = SignDocumentWithPin(doc);

                }

                return Request.CreateResponse(HttpStatusCode.OK, DR);

            }
            catch (Exception ex)
            {
               
                DR.HasError = true;
                DR.ErrorMessage = ex.Message;
                DR.StackTrace = ex.StackTrace;
                return Request.CreateResponse(HttpStatusCode.BadRequest, DR);
            }


        }

        private DocuResponse SignDocumentWithPin(DocuRequest doc)
        {
            //Sign from SmartCard
            //note : ProviderName and KeyContainerName can be found with the dos command : CertUtil -ScInfo
            DocuResponse respo = new DocuResponse();

           
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2 cert = null;
            if (doc.ProviderName == "")
            {
                respo.HasError = true;
                respo.ErrorMessage = "Per te firmosur me certifikaten tuaj duhet te jepni Provider Name";
                return respo;
            }
            foreach (X509Certificate2 cert2 in store.Certificates)
            {
                if (cert2.HasPrivateKey)
                {
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
                    if (rsa == null) continue; // not smart card cert again
                    if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                    {
                        if (rsa.CspKeyContainerInfo.ProviderName == doc.ProviderName)
                        {
                            //we found it
                            cert = cert2;
                            break;
                        }
                    }
                }
            }
            if (cert == null)
            {
                respo.HasError = true;
                respo.ErrorMessage = "Certifikata nuk u gjet!";
                return respo;
            }

            if (doc.PinCode != "")
            {
                //if pin code is set then no windows form will popup to ask it
                RSACryptoServiceProvider rsaForKeyContainer = (RSACryptoServiceProvider)cert.PrivateKey;
                SecureString pwd = GetSecurePin(doc.PinCode);
                CspParameters csp = new CspParameters(1,
                                                        doc.ProviderName,
                                                        rsaForKeyContainer.CspKeyContainerInfo.KeyContainerName,
                                                        new System.Security.AccessControl.CryptoKeySecurity(),
                                                        pwd);
                try
                {
                    RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider(csp);

                }
                catch (Exception ex)
                {
                    respo.HasError = true;
                    respo.ErrorMessage = "Crypto error: " + ex.Message;
                    respo.StackTrace = ex.StackTrace;
                    return respo;
                }
            }

            //sign


            Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
            Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] {
            cp.ReadCertificate(cert.RawData)};

            IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA-1");

            PdfReader pdfReader;
            using (pdfReader = new PdfReader(doc.BasePdf))
            {
                using (signedPdf_mem = new MemoryStream())
                {
                    pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf_mem, '\0');
                    PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                    if (doc.SignImage != null && doc.SignImage.Length!=0)
                    {
                        signatureAppearance.SignatureGraphic = iTextSharp.text.Image.GetInstance(doc.SignImage);
                        signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
                    }
                    else
                    {
                        signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION;
                    }
                    signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 250, 150), pdfReader.NumberOfPages, "NisaDigiSign");
                    signatureAppearance.Reason = "Certifikuar elektronikisht nga sistemi NISA";

                    ITSAClient TsaClient = new TSAClientBouncyCastle(doc.timestampServer);

                    MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, TsaClient, 0, CryptoStandard.CMS);

                    respo.SignedPdf = signedPdf_mem.ToArray();
                }
            }

            return respo;

        }

        private DocuResponse SignDocumentWithoutPin(DocuRequest doc)
        {
            //Sign from SmartCard
            //note : ProviderName and KeyContainerName can be found with the dos command : CertUtil -ScInfo
            DocuResponse respo = new DocuResponse();


            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2 cert = null;
            if (doc.ProviderName == "")
            {
                respo.HasError = true;
                respo.ErrorMessage = "Per te firmosur me certifikaten tuaj duhet te jepni Provider Name";
                return respo;
            }
            foreach (X509Certificate2 cert2 in store.Certificates)
            {
                if (cert2.HasPrivateKey)
                {
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
                    if (rsa == null) continue; // not smart card cert again
                    if (rsa.CspKeyContainerInfo.HardwareDevice) // sure - smartcard
                    {
                        if ((rsa.CspKeyContainerInfo.ProviderName == doc.ProviderName))
                        {
                            //we found it
                            cert = cert2;
                            break;
                        }
                    }
                }
            }
            if (cert == null)
            {
                respo.HasError = true;
                respo.ErrorMessage = "Certifikata nuk u gjet!";
                return respo;
            }

            //sign


            Org.BouncyCastle.X509.X509CertificateParser cp = new Org.BouncyCastle.X509.X509CertificateParser();
            Org.BouncyCastle.X509.X509Certificate[] chain = new Org.BouncyCastle.X509.X509Certificate[] {
            cp.ReadCertificate(cert.RawData)};

            IExternalSignature externalSignature = new X509Certificate2Signature(cert, "SHA-1");

            PdfReader pdfReader;
            using (pdfReader = new PdfReader(doc.BasePdf))
            {
                using (signedPdf_mem = new MemoryStream())
                {
                    pdfStamper = PdfStamper.CreateSignature(pdfReader, signedPdf_mem, '\0');
                    PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                    if (doc.SignImage != null && doc.SignImage.Length!=0)
                    {
                        signatureAppearance.SignatureGraphic = iTextSharp.text.Image.GetInstance(doc.SignImage);
                        signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.GRAPHIC_AND_DESCRIPTION;
                    }
                    else
                    {
                        signatureAppearance.SignatureRenderingMode = PdfSignatureAppearance.RenderingMode.NAME_AND_DESCRIPTION;
                    }
                    signatureAppearance.SetVisibleSignature(new iTextSharp.text.Rectangle(100, 100, 250, 150), pdfReader.NumberOfPages, "NisaDigiSign");
                    signatureAppearance.Reason = "Certifikuar elektronikisht nga sistemi NISA";

                    ITSAClient TsaClient = new TSAClientBouncyCastle(doc.timestampServer);

                    MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, TsaClient, 0, CryptoStandard.CMS);

                    respo.SignedPdf = signedPdf_mem.ToArray();
                }
            }

            return respo;

        }

        private SecureString GetSecurePin(string PinCode)
        {
            SecureString pwd = new SecureString();
            foreach (var c in PinCode.ToCharArray()) pwd.AppendChar(c);
            return pwd;
        }

    }
}
