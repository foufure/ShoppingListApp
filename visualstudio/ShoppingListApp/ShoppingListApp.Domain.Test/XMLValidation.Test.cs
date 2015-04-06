using System.Xml.Schema;
using NUnit.Framework;
using ShoppingListApp.Domain.Concrete;

namespace ShoppingListApp.Domain.Test
{
    [TestFixture]
    public static class XmlValidationTest
    {
        [Test]
        public static void ReturnsFalse_WhenXmlSchemaIsInvalid()
        {
            // Arrange
            string xsdMarkup = @"<xsd:schema xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
                   <xsd:element name='Items'>
                    <xsd:complexType>
                        <xsd:sequence>
                            <xsd:element name='Item' minOccurs='0' maxOccurs='1'>
                             <xsd:complexType>
                              <xsd:sequence>
                               <xsd:element name='ItemId' minOccurs='0'/>
                               <xsd:element name='ItemName' minOccurs='0'/>
                               <xsd:element name='ItemCategory' minOccurs='0'/>
                              </xsd:sequence>
                             </xsd:complexType>
                            </xsd:element>
                        </xsd:sequence>
                    </xsd:complexType>
                   </xsd:element>
                  </xsd:schema>";

            // Act

            // Assert
            Assert.IsFalse(XmlRepositoryValidationExtensions.XmlRepositoryValidation(xsdMarkup, @"ItemRepository.example.xml"));  
        }

        [Test]
        public static void InvalidXmlRepository_WhenRepositoryNameIsInvalid()
        {
            // Arrange
            
            // Act

            // Assert
            Assert.IsFalse(XmlRepositoryValidationExtensions.XmlRepositoryValidation("XsdSchema", string.Empty));
        }
    }
}
