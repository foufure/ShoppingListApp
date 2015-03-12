using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingListApp.Domain.Concrete
{
    public static class RepositoriesXsd
    {
        ////W3C XML Schema (XSD) Validation online: http://www.utilities-online.info/xsdvalidation/#.VPpACeHp6i8

        public static string Items() 
        {  
                return @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
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
        }

        public static string ShoppingLists()
        {
                return @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
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
            }
    }
}
