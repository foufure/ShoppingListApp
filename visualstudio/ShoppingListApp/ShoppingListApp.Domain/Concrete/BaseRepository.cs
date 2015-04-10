using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ShoppingListApp.Domain.Concrete
{
    public abstract class BaseRepository
    {
        private string repositoryName;

        protected BaseRepository(IRepositoryNameProvider repositoryNameProvider)
        {
            if (repositoryNameProvider != null)
            {
                if (!string.IsNullOrEmpty(repositoryNameProvider.RepositoryName))
                {
                    this.repositoryName = repositoryNameProvider.RepositoryName;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Internal error: Items Repository could not be initialized because the repository name is null or an empty string", (Exception)null);
                }
            }
            else
            {
                throw new ArgumentNullException("Internal error: Items Repository could not be initialized because the repository name provider is null", (Exception)null);
            }
        }

        public string RepositoryName { get { return repositoryName; } }

        protected void InitializeXmlPersistentStorage(string repositoryType, string xsdRepositoryType)
        {
            if (!File.Exists(repositoryName) || !XmlRepositoryIsValid(xsdRepositoryType))
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(repositoryType));
                newRepository.Save(repositoryName);
            }
        }

        protected bool XmlRepositoryIsValid(string xsdRepositoryType)
        {
            bool xmlRepositoryValidationResult = false;

            xmlRepositoryValidationResult = true;

            XmlSchemaSet xmlRepositoryXsdSchemas = new XmlSchemaSet();

            using (StringReader markupReader = new StringReader(xsdRepositoryType))
            {
                xmlRepositoryXsdSchemas.Add(string.Empty, XmlReader.Create(markupReader));

                try
                {
                    XDocument.Load(repositoryName).Validate(xmlRepositoryXsdSchemas, null);
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

            return xmlRepositoryValidationResult;
        }
    }
}
