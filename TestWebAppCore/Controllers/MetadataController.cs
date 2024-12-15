using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace TestWebApp.Controllers
{
    [AllowAnonymous]
    [Route("Metadata")]
    public class MetadataController : Controller
    {
        private readonly Saml2Configuration config;

        public MetadataController(Saml2Configuration config)
        {
            this.config = config;
        }

        public IActionResult Index()
        {
            var defaultSite = new Uri($"{Request.Scheme}://{Request.Host.ToUriComponent()}/");

            var entityDescriptor = new EntityDescriptor(config);
            entityDescriptor.ValidUntil = 365;
            entityDescriptor.SPSsoDescriptor = new SPSsoDescriptor
            {
                AuthnRequestsSigned = config.SignAuthnRequest,
                WantAssertionsSigned = true,
                SigningCertificates =
                [
                    config.SigningCertificate
                ],
                //EncryptionCertificates = config.DecryptionCertificates,
                SingleLogoutServices =
                [
                    new SingleLogoutService { Binding = ProtocolBindings.HttpPost, Location = new Uri(defaultSite, "Auth/SingleLogout"), ResponseLocation = new Uri(defaultSite, "Auth/LoggedOut") }
                ],
                NameIDFormats = [NameIdentifierFormats.X509SubjectName],
                AssertionConsumerServices =
                [
                    new AssertionConsumerService { Binding = ProtocolBindings.HttpPost, Location = new Uri(defaultSite, "Auth/AssertionConsumerService") },                    
                ],
                AttributeConsumingServices =
                [
                    new AttributeConsumingService { ServiceNames = [new LocalizedNameType("Sandbox", "en")], RequestedAttributes = CreateRequestedAttributes() }
                ],
            };

            var organization = new Organization(
                [new LocalizedNameType("Arke", "en")],
                [new LocalizedNameType("Arke Systems", "en")],
                [new LocalizedUriType(new Uri("https://arke.com/"), "en")]);

            entityDescriptor.Organization = organization;
            entityDescriptor.ContactPersons = 
            [
                new ContactPerson(ContactTypes.Administrative)
                {
                    Company = "Arke",
                    GivenName = "Andre",
                    SurName = "Andrade",
                    EmailAddress = "aandrade@arke.com",
                    TelephoneNumber = "11111111",
                },
                new ContactPerson(ContactTypes.Technical)
                {
                    Company = "Arke",
                    GivenName = "Andre",
                    SurName = "Andrade",
                    EmailAddress = "aandrade@arke.com",
                    TelephoneNumber = "11111111",
                }
            ];
            return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
        }

        private IEnumerable<RequestedAttribute> CreateRequestedAttributes()
        {
            yield return new RequestedAttribute("urn:oid:2.5.4.4");
            yield return new RequestedAttribute("urn:oid:2.5.4.3", false);
            yield return new RequestedAttribute("urn:xxx", "test-value");
            yield return new RequestedAttribute("urn:yyy", "123") { AttributeValueType = "xs:integer" };
        }
    }
}