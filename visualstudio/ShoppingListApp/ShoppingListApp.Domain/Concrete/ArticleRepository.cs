using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingListApp.Domain.Abstract;
using System.Xml.Linq;
using ShoppingListApp.Domain.Entities;
using System.IO;

namespace ShoppingListApp.Domain.Concrete
{
    public class ArticleRepository : IArticleRepository
    {
        private List<Article> articleRepository = null;
        private IRepositoryNameProvider repositoryNameProvider;

        public ArticleRepository(IRepositoryNameProvider repositoryNameProviderParam)
        {
            repositoryNameProvider = repositoryNameProviderParam;

            if (!File.Exists(repositoryNameProvider.repositoryName))
            {
                XDocument newRepository = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Articles"));
                newRepository.Save(repositoryNameProvider.repositoryName);
            }

            XDocument parsedFile = XDocument.Load(repositoryNameProvider.repositoryName);
            
            
            articleRepository = new List<Article>();
            foreach (XElement element in parsedFile.Elements("Articles").Elements("Article"))
            {
                articleRepository.Add(new Article() { ArticleID = Convert.ToUInt32(element.Element("ArticleID").Value), ArticleName = element.Element("ArticleName").Value });
            }
        }

        public IEnumerable<Entities.Article> repository
        { 
            get
            {
                return articleRepository;
            }
        }

        public void Add(string articleName)
        {
            uint articleID = articleRepository.OrderByDescending(article => article.ArticleID).Select(article => article.ArticleID).FirstOrDefault() + 1;
            Article articleToAdd = new Article() { ArticleID = articleID, ArticleName = articleName };
            articleRepository.Add(articleToAdd);
        }

        public void Modify(Article article)
        {
            articleRepository.Where(repositoryArticle => repositoryArticle.ArticleID == article.ArticleID).FirstOrDefault().ArticleName = article.ArticleName;
        }

        public void Remove(uint articleID)
        {
            articleRepository.Remove(articleRepository.Where(repositoryArticle => repositoryArticle.ArticleID == articleID).FirstOrDefault());
        }

        public void Save()
        {
            XElement elements = new XElement("Articles");
            foreach (Article article in articleRepository)
            {
                elements.Add(new XElement("Article", new XElement("ArticleID") { Value = article.ArticleID.ToString() }, new XElement("ArticleName") { Value = article.ArticleName }));
            }
            elements.Save(repositoryNameProvider.repositoryName);
        }
    }
}
