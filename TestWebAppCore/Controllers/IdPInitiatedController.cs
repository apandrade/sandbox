using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TestWebAppCore.Controllers
{
    [AllowAnonymous]
    [Route("IdPInitiated")]
    public class IdPInitiatedController : Controller
    {
        public IActionResult Initiate()
        {
            var serviceProviderRealm = "https://localhost:44306";

            var binding = new Saml2PostBinding();
            binding.RelayState = $"RPID={Uri.EscapeDataString(serviceProviderRealm)}";

            var config = new Saml2Configuration();

            config.Issuer = "sandbox.local";
            config.SingleSignOnDestination = new Uri("https://localhost:44306/Auth/Login");
            config.SigningCertificate = CertificateUtil.Load(Startup.AppEnvironment.MapToPhysicalFilePath("sandboxkey.pfx"), "1234");
            config.SignatureAlgorithm = Saml2SecurityAlgorithms.RsaSha256Signature;

            var appliesToAddress = "https://localhost:44306";

            var response = new Saml2AuthnResponse(config);
            response.Status = Saml2StatusCodes.Success;    
   
            var claimsIdentity = new ClaimsIdentity(CreateClaims());
            response.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
            response.ClaimsIdentity = claimsIdentity;
            var token = response.CreateSecurityToken(appliesToAddress);

            return binding.Bind(response).ToActionResult();
        }

        private IEnumerable<Claim> CreateClaims()
        {
            yield return new Claim(ClaimTypes.NameIdentifier, "some-user-identity");
            yield return new Claim(ClaimTypes.Email, "some-user@domain.com");
        }
    }
}
