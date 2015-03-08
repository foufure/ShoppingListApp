using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Concrete
{
    public class ItemXmlRepository : IItemsRepository
    {
        private List<Item> itemRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ItemXmlRepository(IRepositoryNameProvider repositoryNameProvider)
        {
            if (repositoryNameProvider.RepositoryNameIsValid())
            {
                this.repositoryNameProvider = repositoryNameProvider;
                this.InitializeXmlPersistentStorage();
                this.LoadFromXmlPersistentStorage();
            } 
        }

        public IEnumerable<Entities.Item> Repository
        { 
            get
            {
                return itemRepository;
            }
        }

        public void Add(string itemName)
        {
            uint itemId = itemRepository.OrderByDescending(item => item.ItemId).Select(item => item.ItemId).FirstOrDefault() + 1;
            Item itemToAdd = new Item() { ItemId = itemId, ItemName = itemName };
            itemRepository.Add(itemToAdd);
        }

        public void Modify(Item item)
        {
            itemRepository.Where(repositoryItem => repositoryItem.ItemId == item.ItemId).FirstOrDefault().ItemName = item.ItemName;
        }

        public void Remove(uint itemId)
        {
            itemRepository.Remove(itemRepository.Where(repositoryItem => repositoryItem.ItemId == itemId).FirstOrDefault());
        }

        public void Save()
        {
            XElement elements = new XElement("Items");
            foreach (Item item in itemRepository)
            {
                elements.Add(new XElement("Item", new XElement("ItemId") { Value = item.ItemId.ToString(CultureInfo.InvariantCulture) }, new XElement("ItemName") { Value = item.ItemName }));
            }

            elements.Save(repositoryNameProvider.RepositoryName);
        }

        private void LoadFromXmlPersistentStorage()
        {
            XDocument parsedFile = XDocument.Load(repositoryNameProvider.RepositoryName);

            itemRepository = new List<Item>();
            foreach (XElement element in parsedFile.Elements("Items").Elements("Item"))
            {
                itemRepository.Add(new Item() { ItemId = Convert.ToUInt32(element.Element("ItemId").Value, CultureInfo.InvariantCulture), ItemName = element.Element("ItemName").Value });
            }
        }

        private void InitializeXmlPersistentStorage()
        {
            if (!File.Exists(repositoryNameProvider.RepositoryName) || !XmlRepositoryIsValid())
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Items"));
                newRepository.Save(repositoryNameProvider.RepositoryName);
            }
        }

        private bool XmlRepositoryIsValid()
        {
            bool xmlRepositoryValidationResult = true;

            ////W3C XML Schema (XSD) Validation online: http://www.utilities-online.info/xsdvalidation/#.VPpACeHp6i8
            string xmlRepositoryXsdMarkup =
                @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                   <xsd:element name='Items'>
                    <xsd:complexType>
                        <xsd:sequence>
                            <xsd:element name='Item' minOccurs='0' maxOccurs='unbounded'>
                             <xsd:complexType>
                              <xsd:sequence>
                               <xsd:element name='ItemId' minOccurs='0'/>
                               <xsd:element name='ItemName' minOccurs='0'/>
                              </xsd:sequence>
                             </xsd:complexType>
                            </xsd:element>
                        </xsd:sequence>
                    </xsd:complexType>
                   </xsd:element>
                  </xsd:schema>";
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

            return xmlRepositoryValidationResult;
        }
    }
}
