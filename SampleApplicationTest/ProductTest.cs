using Moq;
using SampleWebApp;
using SampleWebApp.Controllers;
using SampleWebApp.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SampleApplicationTest
{
    public class ProductTest
    {
        #region Property  
        public Mock<IProductService> mock = new Mock<IProductService>();
        #endregion

        [Fact]
        public void GetProducts_ProductService_Called()
        {
            mock.Setup(p => p.GetProducts());
            ProductController productController = new ProductController(mock.Object);
            productController.Get();
            mock.Verify(s => s.GetProducts(), Times.Once);
        }

        [Fact]
        public void GetProducts_Count_NotZero()
        {
            List<Product> dummyList = new List<Product>() { new Product() { ProductID = 1, ProductName = "Sample Product ", Description = "Sample Description ", Price = 100 } };
                
            mock.Setup(p => p.GetProducts()).Returns(dummyList);
            ProductController productController = new ProductController(mock.Object);
            IEnumerable<Product> result = productController.Get();
            Assert.True(result.Count() > 0);
        }

        [Fact]
        public void GetProducts_Valid_ProductId()
        {
            List<Product> dummyList = new List<Product>() { new Product() { ProductID = 1, ProductName = "Sample Product ", Description = "Sample Description ", Price = 100 } };

            mock.Setup(p => p.GetProducts()).Returns(dummyList);
            ProductController productController = new ProductController(mock.Object);
            IEnumerable<Product> result = productController.Get();
            Assert.True(result.First().ProductID == 1);
        }

        [Fact]
        public void InsertProduct_ProductService_Called()
        {
            mock.Setup(p => p.InsertProduct());
            ProductController productController = new ProductController(mock.Object);
            productController.Post();
            mock.Verify(s => s.InsertProduct(), Times.Once);
        }
    }
}
