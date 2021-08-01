using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleWebApp.Service
{
    public class ProductService : IProductService
    {

        public static List<Product> productList = new List<Product>();
        public IEnumerable<Product> GetProducts()
        {
            for (int i = 1; i < 5; i++)
            {
                Product product = new Product();
                product.ProductID = i;
                product.ProductName = "Sample Product " + i;
                product.Description = "Sample Description " + i;
                product.Price = 100 + i;
                if (productList != null && productList.All(x => x.ProductID != i))
                    productList.Add(product);
            }
            return productList;
        }

        public Product InsertProduct()
        {
            int id = new Random().Next();
            Product product = new Product();
            product.ProductID = id;
            product.ProductName = "Sample Product " + id;
            product.Description = "Sample Description " + id;
            product.Price = 100 + id;
            productList.Add(product);

            return product;
        }
    }
}

