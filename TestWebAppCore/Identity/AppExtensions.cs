using Schemas = ITfoxtec.Identity.Saml2.Schemas;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TestWebAppCore.Identity
{
    public class AppExtensions : Schemas.Extensions
    {
        static readonly XName samlNamespaceNameX = XNamespace.Xmlns + "saml";
        static readonly Uri samlNamespace = new Uri("urn:oasis:names:tc:SAML:2.0:assertion");
        static readonly XNamespace samlNamespaceX = XNamespace.Get(samlNamespace.OriginalString);

        public AppExtensions()
        {
            Element.Add(GetXContent());
        }

        protected IEnumerable<XObject> GetXContent()
        {
            yield return new XAttribute(samlNamespaceNameX, samlNamespace.OriginalString);

            yield return new XElement(samlNamespaceX + "Sandbox", "public");

            yield return new XElement(samlNamespaceX + "RequestedAttributes",
                GetRequestedAttribute("urn:oid:1.2.840.113549.1.9.1", isRequired: true, friendlyName: "email"),
                GetRequestedAttribute("urn:oid:2.5.4.42", isRequired: true, friendlyName: "givenName"),
                GetRequestedAttribute("urn:oid:2.5.4.4", isRequired: true, friendlyName: "surname")
            );
        }

        private static XElement GetRequestedAttribute(string name, bool isRequired = false, string value = null, string friendlyName = null)
        {
            var element = new XElement(samlNamespaceX + "RequestedAttribute",
                                 new XAttribute("Name", name),
                                 new XAttribute("NameFormat", "urn:oasis:names:tc:SAML:2.0:attrname-format:uri"),
                                 new XAttribute("isRequired", isRequired));

            if (!string.IsNullOrWhiteSpace(friendlyName))
            {
                element.Add(new XAttribute("FriendlyName", friendlyName));
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                element.Add(new XElement(samlNamespaceX + "AttributeValue", value));
            }

            return element;
        }
    }
}
