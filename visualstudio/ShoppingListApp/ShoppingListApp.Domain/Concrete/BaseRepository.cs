using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.I18N.Utils;

namespace ShoppingListApp.Domain.Concrete
{
    public abstract class BaseRepository
    {
        private string repositoryName;
        private string dataPath;

        protected BaseRepository(IRepositoryNameProvider repositoryNameProvider, IDataPathProvider dataPathProvider)
        {
            if (repositoryNameProvider != null && dataPathProvider != null)
            {
                if (!string.IsNullOrEmpty(repositoryNameProvider.RepositoryName) && !string.IsNullOrEmpty(dataPathProvider.DataPath))
                {
                    this.repositoryName = repositoryNameProvider.RepositoryName;
                    this.dataPath = dataPathProvider.DataPath;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Internal error: Repository could not be initialized because the repository or datapath name is null or an empty string", (Exception)null);
                }
            }
            else
            {
                throw new ArgumentNullException("Internal error: Repository could not be initialized because the repository name or datapath provider is null", (Exception)null);
            }
        }

        public string RepositoryName
        {
            get { return repositoryName; }
        }

        protected void SetDefaultRepositoryAccordingToCurrentUICulture(string defaultRepositoryType)
        {
            string languageSuffix = string.Empty;
            if (CurrentCultureConfiguration.GetCurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                languageSuffix = "_" + CurrentCultureConfiguration.GetCurrentUICulture.TwoLetterISOLanguageName;
            }

            System.IO.File.Copy(dataPath + @"\Default" + defaultRepositoryType + @"Repository" + languageSuffix + @".xml", repositoryName, true);
        }

        protected void ResetToEmptyRepository(string repositoryType)
        {
            XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement(repositoryType));
            newRepository.Save(repositoryName);
        }

        protected void InitializeXmlPersistentStorage(string repositoryType, string xsdRepositoryType)
        {
            if (!File.Exists(repositoryName) || !XmlRepositoryIsValid(xsdRepositoryType))
            {
                SetDefaultRepositoryAccordingToCurrentUICulture(repositoryType);
            }
        }

        protected bool XmlRepositoryIsValid(string xsdRepositoryType)
        {
            bool xmlRepositoryValidationResult = true;

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
