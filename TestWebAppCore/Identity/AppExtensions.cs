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

            yield return new XElement(samlNamespaceX + "SPType", "public");

            yield return new XElement(samlNamespaceX + "RequestedAttributes",
                GetRequestedAttribute("urn:oid:0.9.2342.19200300.100.1.1", isRequired: true, friendlyName: "Username"), // username
                GetRequestedAttribute("urn:oid:0.9.2342.19200300.100.1.3", isRequired: true, friendlyName: "Email"),    // email
                GetRequestedAttribute("urn:oid:2.5.4.42", isRequired: true, friendlyName: "First Name"),               // givenName
                GetRequestedAttribute("urn:oid:2.5.4.4", isRequired: true, friendlyName: "Last Name")                 // sn (Surname)
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
