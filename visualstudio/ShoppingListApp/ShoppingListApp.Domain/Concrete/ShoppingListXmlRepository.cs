﻿using System;
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
    public class ShoppingListXmlRepository : IShoppingListRepository
    {
        private List<ShoppingList> shoppinglistRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ShoppingListXmlRepository(IRepositoryNameProvider repositoryNameProvider)
        {
            if (repositoryNameProvider.RepositoryNameIsValid())
            {
                this.repositoryNameProvider = repositoryNameProvider;
                this.InitializeXmlPersistentStorage();
                this.LoadFromXmlPersistentStorage();
            } 
        }

        public IEnumerable<ShoppingList> Repository
        {
            get
            {
                return shoppinglistRepository;
            }
        }

        public void Add(ShoppingList newShoppingList)
        {
            if (newShoppingList == null)
            {
                throw new ArgumentNullException("Internal Error: the shopping list to add is empty or null. Please enter a valid shopping list", (Exception)null);
            }

            newShoppingList.ShoppingListId = shoppinglistRepository.OrderByDescending(shoppinglist => shoppinglist.ShoppingListId).Select(shoppinglist => shoppinglist.ShoppingListId).FirstOrDefault() + 1;
            shoppinglistRepository.Add(newShoppingList);
        }

        public void Remove(uint shoppingListId)
        {
            if (!shoppinglistRepository.Remove(shoppinglistRepository.Where(repositoryShoppingList => repositoryShoppingList.ShoppingListId == shoppingListId).FirstOrDefault()))
            {
                throw new ArgumentOutOfRangeException("Internal Error: the item to be deleted does not exist. Please enter a valid Shopping List Id", (Exception)null);
            }
        }

        public void Modify(ShoppingList shoppingList)
        {
            int shoppingListNotFound = 0;

            if (shoppingList == null)
            {
                throw new ArgumentNullException("Internal Error: the shopping list to modify is empty or null. Please enter a valid shopping list", (Exception)null);
            }

            if (shoppinglistRepository.RemoveAll(item => item.ShoppingListId == shoppingList.ShoppingListId) == shoppingListNotFound)
            {
                throw new ArgumentOutOfRangeException("Internal Error: the shopping list to modify does not exist in the repository. Please enter a valid shopping list", (Exception)null);
            }

            shoppinglistRepository.Add(shoppingList);
        }

        public void Save()
        {
            XElement elements = new XElement("ShoppingLists");
            foreach (ShoppingList shoppinglist in shoppinglistRepository.OrderBy(list => list.ShoppingListId))
            {
                List<XElement> lines = new List<XElement>();
                
                foreach (ShoppingListLine line in shoppinglist.ShoppingListContent)
                {
                    lines.Add(new XElement(
                                            "ShoppingListLine",
                                            new XElement("ItemId") { Value = line.ItemToBuy.ItemId.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("ItemName") { Value = line.ItemToBuy.ItemName.ToString() },
                                            new XElement("ItemQuantity") { Value = line.QuantityToBuy.ToString(CultureInfo.InvariantCulture) }));
                }
                
                elements.Add(new XElement(
                                            "ShoppingList",
                                            new XElement("ShoppingListId") { Value = shoppinglist.ShoppingListId.ToString(CultureInfo.InvariantCulture) },
                                            new XElement("ShoppingListName") { Value = shoppinglist.ShoppingListName },
                                            new XElement("ShoppingListDueDate") { Value = shoppinglist.ShoppingListDueDate.Date.ToString("u", CultureInfo.InvariantCulture).Split(' ')[0] }, // Universal Date without Time
                                            lines));
            }

            elements.Save(repositoryNameProvider.RepositoryName);
        }

        public void LoadFromXmlPersistentStorage()
        {
            XDocument parsedFile = XDocument.Load(repositoryNameProvider.RepositoryName);

            shoppinglistRepository = new List<ShoppingList>();
            foreach (XElement element in parsedFile.Elements("ShoppingLists").Elements("ShoppingList"))
            {
                ShoppingList newShoppingList = new ShoppingList()
                {
                    ShoppingListId = Convert.ToUInt32(element.Element("ShoppingListId").Value, CultureInfo.InvariantCulture),
                    ShoppingListName = element.Element("ShoppingListName").Value,
                    ShoppingListDueDate = Convert.ToDateTime(element.Element("ShoppingListDueDate").Value, CultureInfo.InvariantCulture)
                };
                    
                foreach (XElement itemElement in element.Elements("ShoppingListLine"))
                {
                    newShoppingList.ShoppingListContent.Add(new ShoppingListLine() { ItemToBuy = new Item() { ItemId = Convert.ToUInt32(itemElement.Element("ItemId").Value, CultureInfo.InvariantCulture), ItemName = itemElement.Element("ItemName").Value }, QuantityToBuy = Convert.ToInt32(itemElement.Element("ItemQuantity").Value, CultureInfo.InvariantCulture) });
                }

                shoppinglistRepository.Add(newShoppingList);
            }
        }

        private void InitializeXmlPersistentStorage()
        {
            if (!File.Exists(repositoryNameProvider.RepositoryName) || !XmlRepositoryIsValid())
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("ShoppingLists"));
                newRepository.Save(repositoryNameProvider.RepositoryName);
            }
        }

        private bool XmlRepositoryIsValid()
        {
            ////W3C XML Schema (XSD) Validation online: http://www.utilities-online.info/xsdvalidation/#.VPpACeHp6i8
            string xmlRepositoryXsdMarkup =
                @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                   <xsd:element name='ShoppingLists'>
                    <xsd:complexType>
                        <xsd:sequence>
                            <xsd:element name='ShoppingList' minOccurs='0' maxOccurs='unbounded'>
                             <xsd:complexType>
                              <xsd:sequence>
                               <xsd:element name='ShoppingListId' minOccurs='0'/>
                               <xsd:element name='ShoppingListName' minOccurs='0'/>
                               <xsd:element name='ShoppingListDueDate' minOccurs='0'/>
                               <xsd:element name='ShoppingListLine' minOccurs='0' maxOccurs='unbounded'>
                                <xsd:complexType>
                                    <xsd:sequence>
                                        <xsd:element name='ItemId' minOccurs='0'/>
                                        <xsd:element name='ItemName' minOccurs='0'/>
                                        <xsd:element name='ItemQuantity' minOccurs='0'/>
                                    </xsd:sequence>
                                </xsd:complexType>
                               </xsd:element>
                              </xsd:sequence>
                             </xsd:complexType>
                            </xsd:element>
                        </xsd:sequence>
                    </xsd:complexType>
                   </xsd:element>
                  </xsd:schema>";

            return XmlRepositoryValidationExtensions.XmlRepositoryValidation(xmlRepositoryXsdMarkup, repositoryNameProvider);
        }
    }
}
