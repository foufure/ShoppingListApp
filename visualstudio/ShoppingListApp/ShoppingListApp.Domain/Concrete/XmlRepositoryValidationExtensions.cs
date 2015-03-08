using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public static class XmlRepositoryValidationExtensions
    {
        public static bool XmlRepositoryValidation(string xmlRepositoryXsdMarkup, IRepositoryNameProvider repositoryNameProvider)
        {
            bool xmlRepositoryValidationResult = false;

            if (repositoryNameProvider.RepositoryNameIsValid())
            {
                xmlRepositoryValidationResult = true;

                XmlSchemaSet xmlRepositoryXsdSchemas = new XmlSchemaSet();

                using (StringReader markupReader = new StringReader(xmlRepositoryXsdMarkup))
                {
                    xmlRepositoryXsdSchemas.Add(string.Empty, XmlReader.Create(markupReader));

                    try
                    {
                        XDocument.Load(repositoryNameProvider.RepositoryName).Validate(xmlRepositoryXsdSchemas, null);
                    }
                    catch (XmlSchemaValidationException)
                    {
                        xmlRepositoryValidationResult = false;
                    }
                    catch (XmlException)
                    {
                        xmlRepositoryValidationResult = false;
                    }
                }
            }

            return xmlRepositoryValidationResult;
        }
    }
}
