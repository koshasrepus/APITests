using NUnit.Framework;
using RestSharp;
using System.Net;
using Newtonsoft.Json.Linq;

namespace RestSharpExamples.Tests
{
    [TestFixture]
    public class BasicTests
    {
        #region
        /// <summary>
        /// Sends a request to receive breadcrumbs for the product.
        /// </summary>
        /// <param name="item_id">The product id.</param>
        #endregion
        IRestResponse GetResponse(string item_id)
        {
            string direct = $"navi/v1/category/item-crumbs/{item_id}";
            RestClient client = new RestClient("http://www.ozon.ru"); //т.к хоста не знаю, поставил любой
            RestRequest request = new RestRequest(direct, Method.GET);
            return client.Execute(request);
        }
        #region
        /// <summary>
        /// Returns the field value from the server response.
        /// </summary>
        /// <param name="response">The server response.</param>
        /// <param name="field">The JSON field.</param>
        #endregion

        string ParseJason(IRestResponse response, string field)
        {
            JObject jResponse = JObject.Parse(response.Content);
            return (string)jResponse.SelectToken(field); ;
        }

        [Test]
        public void StatusCodeTest()
        {
            string item_id = "1"; //id либо хардкодим в тесте, либо отдельным методом извлекаем из базы           
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void ContentTypeTest()
        {
            string item_id = "1";
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            Assert.That(response.ContentType, Is.EqualTo("application/json"));
        }
        [Test]
        public void NameIsCorrect()
        {
            string item_id = "1";
            string namePattern = "productName";            
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            string nameFromResponse = ParseJason(response, "main[0].name");
            Assert.That(nameFromResponse, Is.EqualTo(namePattern));
        }
        [Test]
        public void UrlIsCorrect()
        {
            string item_id = "1";
            string namePattern = "productName";
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            string nameFromResponse = ParseJason(response, "main[0].url");
            Assert.That(nameFromResponse, Is.EqualTo(namePattern));
        }
        [Test]
        public void OnlyOnePairOfValuesReturned()
        {
            string item_id = "1";
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            JObject jResponse = JObject.Parse(response.Content);
            JArray signInNames = (JArray)jResponse.SelectToken("main");            
            Assert.That(signInNames.Count, Is.EqualTo(1));
        }
        [Test]
        public void ResponseToNonexistentIdIsCorrect()
        {
            string item_id = "0";
            #region
            /*id либо хардкодим в тесте, либо отдельным методом извлекаем из базы. 
              В данном случае заодно проверяем
              граничное значение*/
            #endregion
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        public void BoundaryValueIsHandledCorrectly()
        {
            string item_id = "18446744073709551615";
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        public void IdOutOfRangeIsHandledCorrectly()
        {
            string item_id = "18446744073709551616";
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
        public void NegativeIdIsHandledCorrectly()
        {
            string item_id = "-1";
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        public void NullIdIsHandledCorrectly()
        {
            string item_id = "NULL";
            // act
            IRestResponse response = GetResponse(item_id);
            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

    }
}
