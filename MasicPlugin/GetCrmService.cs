using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Text;

namespace GetExcsettingWF
{
    public class GetCRMServiceClass
    {

        public static IOrganizationService GetCRMService(string ServerURL, string Organization, string UserName, string Password, string DomainName, string UserGuid)
        {
            // CRMService.OrganizationServiceClient client = new OrganizationServiceClient();
            //  return client;
            ClientCredentials credentials = new ClientCredentials();
            credentials.Windows.ClientCredential = new System.Net.NetworkCredential(UserName, Password, DomainName);
            Uri organizationUri = new Uri(ServerURL + "/" + Organization + "/XRMServices/2011/Organization.svc");
            Uri homeRealmUri = null;
            OrganizationServiceProxy orgService = new OrganizationServiceProxy(organizationUri, homeRealmUri, credentials, null);
            orgService.EnableProxyTypes();
            if (!string.IsNullOrEmpty(UserGuid))
            {
                //orgService.CallerId = new Guid(UserGuid);
            }


            IOrganizationService _service = (IOrganizationService)orgService;
            return _service;
        }
        public static IOrganizationService GetCRMService(string ServerURL, string Organization, string UserName, string Password, string DomainName)
        {
            return GetCRMService(ServerURL, Organization, UserName, Password, DomainName, "");
        }
        public static IOrganizationService Service
        {
            get
            {

                // UserGuid="7C92A4B5-7FF8-E411-80C2-0050568B1DBC';
                // if (context.Session[serviceSessionId] == null)
                {
                    string CRMServerURL = "https://axmth.almutahidah.com:5555/";
                    string CRMOrganiza = "test";
                    string CRMUserName = "masic.crm";
                    string CRMPassword = "M@$ic@exc2019";
                    //CRMPassword = DecryptText(CRMPassword, "Ahmed");
                    string CRMDomain = "masic";


                    //context.Session[serviceSessionId] =

                    return GetCRMService(CRMServerURL, CRMOrganiza, CRMUserName, CRMPassword, CRMDomain);

                }
                //return (Microsoft.Xrm.Sdk.IOrganizationService)context.Session[serviceSessionId];
            }
        }

        public static string DecryptText(string input, string password)
        {
            // Get the bytes of the string
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string result = Encoding.UTF8.GetString(bytesDecrypted);

            return result;
        }
        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }


    }
}