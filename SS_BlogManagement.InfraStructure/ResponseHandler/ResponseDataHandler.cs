using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SS_BlogManagement.InfraStructure.ResponseHandler
{
    public class ResponseData
    {
        public string code { get; set; }
        public string message { get; set; }
    }


    public interface IResponseHandler
    {
        ResponseData GetResponse<T>(Enum enumValue) where T : class;
    }
    public class ResponseDataHandler : IResponseHandler
    {

        private readonly IDictionary<string, ResponseData> jsonListstring;

        public ResponseDataHandler()
        {
            var folderDetail = Path.Combine(Directory.GetCurrentDirectory(), $"{"ResponseConfig.json"}");
            var json = System.IO.File.ReadAllText(folderDetail);
            jsonListstring = JsonConvert.DeserializeObject<IDictionary<string, ResponseData>>(json);

        }
        public ResponseData GetResponse<T>(Enum enumValue) where T : class
        {
            ResponseData responseCode = (ResponseData)jsonListstring[enumValue.ToString()];
            return responseCode;
        }

    }
}
